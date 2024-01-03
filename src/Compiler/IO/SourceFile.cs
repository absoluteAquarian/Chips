using Chips.Compiler.Parsing;
using Chips.Compiler.Parsing.States;
using System;
using System.IO;

namespace Chips.Compiler.IO {
	internal class SourceFile {
		public readonly string file;

		public SourceFile(string file) {
			this.file = file;
		}

		public BytecodeFile CompileToBytecode(CompilationContext context, DateTime lastWriteTime, string codeFilePath) {
			ChipsCompiler.CompilingSourceFile = file;
			ChipsCompiler.CompilingSourceLine = 1;

			context.heap.Clear();
			context.resolver.Clear();
			context.SetCILMethod(null!);

			using SourceReader reader = new SourceReader(new StreamReader(File.OpenRead(file)));

			BytecodeFile code = new BytecodeFile(codeFilePath);

			BaseState state = new FileScope();

			while (!reader.BaseReader.EndOfStream) {
				ChipsCompiler.CompilingSourceLineOverride = null;

				try {
					if (state.ParseNext(reader, context, code, out BaseState? next)) {
						if (next is null) {
							next = new FileScope();
							throw new ParsingException($"State {state.GetType().Name} produced a null next state, defaulting to FileScope");
						}

						if (!object.ReferenceEquals(state, next)) {
							state.Success();
							next.Previous ??= state;  // Some states manually set their previous state, so don't overwrite it
						}

						state = next;
					} else {
						// State failed
						goto fail;
					}
				} catch (Exception ex) {
					// Exceptions should already be reported to the error list, so just swallow them
					ChipsCompiler.ErrorAndThrow(ex);
					goto fail;
				}

				continue;
				fail:

				// State failed, skip to next line
				reader.ReadUntilNewline();
				reader.BaseReader.Read();
			}

			// Update the byte arrays representing the bytecode file
			code.Serialize(context, lastWriteTime);

			return code;
		}
	}
}
