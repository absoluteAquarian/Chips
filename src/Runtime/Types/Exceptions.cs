using Chips.Runtime.Specifications;
using System;

namespace Chips.Runtime.Types {
	internal static class ExceptionHelper {
		public static string GetContextString(Opcode.FunctionContext context)
			=> !string.IsNullOrEmpty(context.sourceFile) && context.sourceLine >= 0
				? $"\n  in \"{context.sourceFile}\" on line {context.sourceLine}"
				: "";
	}

	public class RegisterAssignmentException : Exception {
		public RegisterAssignmentException(string message, Opcode.FunctionContext context) : base(message + ExceptionHelper.GetContextString(context)) { }
	}

	public class InvalidRegisterTypeException : Exception {
		public InvalidRegisterTypeException(string message, Opcode.FunctionContext context) : base(message + ExceptionHelper.GetContextString(context)) { }
	}

	public class InvalidRegisterValueException : Exception {
		public InvalidRegisterValueException(string message, Opcode.FunctionContext context) : base(message + ExceptionHelper.GetContextString(context)) { }
	}

	public class InvalidOpcodeArgumentException : Exception {
		public InvalidOpcodeArgumentException(int argument, string reason, Opcode.FunctionContext context) : base($"Argument {argument} was invalid." +
			$"\nReason: {reason}" + ExceptionHelper.GetContextString(context)) { }
	}

	public class UnkownOpcodeException : Exception {
		public UnkownOpcodeException(byte code) : base($"Unknown opcode (0x{code:X2})") { }
	}
}
