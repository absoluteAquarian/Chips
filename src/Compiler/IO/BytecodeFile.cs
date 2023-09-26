using System;
using System.Collections.Generic;
using System.IO;
using Chips.Compiler.Compilation;
using Chips.Compiler.IO.PDB;

namespace Chips.Compiler.IO {
	internal class BytecodeFile {
		private CPDBFile? cpdbInfo;

		public readonly string sourceFile;

		private bool _initialized;
		internal byte[] _rawData;
		internal byte[]? _rawDataCPDB;

		private readonly List<BytecodeTopLevelSegment> segments = new();

		public BytecodeFile(string sourceFile, byte[] code, byte[]? cpdbCode) {
			this.sourceFile = sourceFile;
			_rawData = code;
			_rawDataCPDB = cpdbCode;
		}

		public void CompileToCIL(CompilationContext context) {
			if (_rawData is null)
				throw new InvalidDataException("Bytecode file was not serialized");

			ChipsCompiler.CompilingSourceFile = sourceFile;
			ChipsCompiler.CompilingSourceLine = -1;

			Initialize(context);


		}

		private void Initialize(CompilationContext context) {
			if (_initialized)
				return;

			using MemoryStream data = new MemoryStream(_rawData);
			using BinaryReader reader = new BinaryReader(data);

			// First bit of data is the last write time, which isn't relevant in this context
			reader.ReadInt64();

			while (reader.BaseStream.Position < reader.BaseStream.Length) {
				BytecodeMember member = BytecodeFileSegment.ReadSegmentIdentifier(reader);

				BytecodeTopLevelSegment segment = member switch {
					BytecodeMember.Namespace => BytecodeNamespaceSegment.ReadMember(context, reader, null),
					BytecodeMember.ExternNamespace => BytecodeNamespaceImportSegment.ReadMember(context, reader),
					BytecodeMember.TypeAlias => BytecodeTypeAliasSegment.ReadMember(context, reader),
					_ => throw new InvalidDataException($"Invalid top-level segment identifier {member}; expected {BytecodeMember.Namespace}, {BytecodeMember.ExternNamespace} or {BytecodeMember.TypeAlias}")
				};

				segments.Add(segment);
			}

			if (_rawDataCPDB is not null)
				cpdbInfo = CPDBFile.FromStream(new MemoryStream(_rawDataCPDB));
		}

		// Compiler-implemented logic, not intended to be used elsewhere
		internal BytecodeFile(string sourceFile) {
			this.sourceFile = sourceFile;
			_rawData = null!;
			_rawDataCPDB = null;
			cpdbInfo = new CPDBFile();
		}

		internal CPDBFileMemberCollection CPDBMembers => cpdbInfo!.Information;

		internal void AddSegment(BytecodeTopLevelSegment segment) {
			segments.Add(segment);
		}

		internal void Serialize(CompilationContext context, DateTime lastWriteTime) {
			using MemoryStream ms = new();
			using BinaryWriter writer = new(ms);

			writer.Write(lastWriteTime.ToBinary());

			foreach (BytecodeFileSegment segment in segments)
				segment.WriteMember(context, writer);

			_rawData = ms.ToArray();

			if (!ChipsCompiler.IncludeSourceInformation)
				return;

			using MemoryStream ms2 = new();
			cpdbInfo!.WriteToStream(ms2);

			_rawDataCPDB = ms2.ToArray();
		}
	}
}
