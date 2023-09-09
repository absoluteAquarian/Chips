using AsmResolver.DotNet;
using AsmResolver.PE.DotNet.Metadata.Tables;
using Chips.Compiler.Utility;
using System;
using System.IO;

namespace Chips.Utility {
	partial class Extensions {
		public static void Write(this BinaryWriter writer, TypeDefinition? type, TypeResolver resolver) {
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(resolver);

			var assembly = type.Module?.Assembly ?? throw new ArgumentException("Type must be defined in an assembly.", nameof(type));
			int moduleIndex = assembly.Modules.IndexOf(type.Module);

			resolver.AddAssembly(assembly);

			writer.Write((ushort)resolver.FindAssemblyIndex(assembly));
			writer.Write((ushort)moduleIndex);
			writer.Write(type.MetadataToken.ToUInt32());
		}

		public static TypeDefinition ReadTypeDefinition(this BinaryReader reader, TypeResolver resolver) {
			int assemblyIndex = reader.ReadUInt16();
			int moduleIndex = reader.ReadUInt16();
			MetadataToken token = reader.ReadUInt32();

			if (resolver.GetAssembly(assemblyIndex) is not AssemblyDefinition assembly)
				throw new InvalidDataException("Assembly index was invalid or referred to a null assembly");

			if (moduleIndex >= assembly.Modules.Count || assembly.Modules[moduleIndex] is not ModuleDefinition module)
				throw new InvalidDataException($"Module index {moduleIndex} was invalid or referred to a null module");

			if (!module.TryLookupMember(token, out var member))
				throw new InvalidDataException($"Could not resolve type definition token {token} from assembly \"{assembly.Name}\"");

			if (member is not TypeDefinition type)
				throw new InvalidDataException($"Token {token} did not refer to a type definition in assembly \"{assembly.Name}\"");

			return type;
		}

		public static void Write(this BinaryWriter writer, FieldDefinition field, TypeResolver resolver) {
			ArgumentNullException.ThrowIfNull(field);
			ArgumentNullException.ThrowIfNull(resolver);

			var assembly = field.Module?.Assembly ?? throw new ArgumentException("Field must be defined in an assembly.", nameof(field));
			int moduleIndex = assembly.Modules.IndexOf(field.Module);

			resolver.AddAssembly(assembly);

			writer.Write((ushort)resolver.FindAssemblyIndex(assembly));
			writer.Write((ushort)moduleIndex);
			writer.Write(field.MetadataToken.ToUInt32());
		}

		public static FieldDefinition? ReadFieldDefinition(this BinaryReader reader, TypeResolver resolver) {
			int assemblyIndex = reader.ReadUInt16();
			int moduleIndex = reader.ReadUInt16();
			MetadataToken token = reader.ReadUInt32();

			if (resolver.GetAssembly(assemblyIndex) is not AssemblyDefinition assembly)
				throw new InvalidDataException("Assembly index was invalid or referred to a null assembly");

			if (moduleIndex >= assembly.Modules.Count || assembly.Modules[moduleIndex] is not ModuleDefinition module)
				throw new InvalidDataException($"Module index {moduleIndex} was invalid or referred to a null module");

			if (!module.TryLookupMember(token, out var member))
				throw new InvalidDataException($"Could not resolve field definition token {token} from assembly \"{assembly.Name}\"");

			if (member is not FieldDefinition field)
				throw new InvalidDataException($"Token {token} did not refer to a field definition in assembly \"{assembly.Name}\"");

			return field;
		}
	}
}
