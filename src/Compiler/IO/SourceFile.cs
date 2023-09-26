using Chips.Compiler.Parsing.States;
using Chips.Utility;
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
			ChipsCompiler.CompilingSourceLine = 0;

			context.heap.Clear();
			context.resolver.Clear();
			context.SetMethod(null!);

			using StreamReader reader = new StreamReader(File.OpenRead(file));

			BytecodeFile code = new BytecodeFile(codeFilePath);

			BaseState state = new FileScope();

			while (!reader.EndOfStream) {
				ChipsCompiler.CompilingSourceLineOverride = null;

				try {
					if (state.ParseNext(reader, context, code, out BaseState? next)) {
						if (next is null) {
							next = new FileScope();
							throw ChipsCompiler.ErrorAndThrow(new ParsingException($"State {state.GetType().Name} produced a null next state, defaulting to FileScope"));
						}

						next.Previous ??= state;  // Some states manually set their previous state, so don't overwrite it

						state = next;
					} else {
						// State failed
						goto fail;
					}
				} catch {
					// Exceptions should already be reported to the error list, so just swallow them
					goto fail;
				}

				continue;
				fail:

				// State failed, skip to next line
				ChipsCompiler.Error($"State \"{state.GetType().Name}\" failed to parse the text, skipping to next line and applying previous state");
				reader.ReadUntilNewline();
				reader.Read();
				state = state?.Previous ?? new FileScope();
			}

			// Update the byte arrays representing the bytecode file
			code.Serialize(context, lastWriteTime);

			return code;
		}
	}
}
