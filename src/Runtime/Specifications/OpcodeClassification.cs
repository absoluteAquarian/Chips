using System;

namespace Chips.Runtime.Specifications {
	[Flags]
	public enum OpcodeClassification {
		/// <summary>
		/// An instruction that does not have an operand (e.g. "neg")
		/// </summary>
		NoOperand = 0x1,
		/// <summary>
		/// An instruction that performs arithmetic (e.g. "add")
		/// </summary>
		Arithmetic = ArithmeticPure | ModifiesAccumulator,
		ArithmeticPure = 0x2,
		/// <summary>
		/// An instruction that performs arithmetic without using the accumulator (e.g. "dex")
		/// </summary>
		ArithmeticNoAccumulator = ArithmeticPure | ModifiesRegister,
		/// <summary>
		/// An instruction that moves a value from one location to another (e.g. "mov")
		/// </summary>
		ValueTransfer = 0x4,
		/// <summary>
		/// An instruction that compares two values (e.g. "ceq")
		/// </summary>
		Comparison = ComparisonPure | ModifiesFlag,
		ComparisonPure = 0x8,
		/// <summary>
		/// An instruction that creates a new object (e.g. "new ~rand")
		/// </summary>
		ObjectCreation = 0x10,
		/// <summary>
		/// An instruction that jumps execution to a target location (e.g. "btc")
		/// </summary>
		Branching = 0x20,
		/// <summary>
		/// An instruction that does not modify any registers nor flags (e.g. "ret")
		/// </summary>
		NoSpecification = 0x40,
		/// <summary>
		/// An instruction that performs logic on the console window (e.g. "cnrb")
		/// </summary>
		Console = 0x80,
		/// <summary>
		/// An instruction that modifies the value of a flag, either directly (e.g. "clc") or indirectly (e.g. "aems")
		/// </summary>
		ModifiesFlag = 0x100,
		/// <summary>
		/// An instruction that modifies a register
		/// </summary>
		ModifiesRegister = ModifiesRegisterPure | ModifiesFlag,
		ModifiesRegisterPure = 0x200,
		/// <summary>
		/// An instruction that modifies the accumulator
		/// </summary>
		ModifiesAccumulator = ModifiesAccumulatorPure | ModifiesRegister,
		ModifiesAccumulatorPure = 0x400,
		/// <summary>
		/// An instruction that performs I/O logic (e.g. "inc")
		/// </summary>
		InputOutput = 0x800,
		/// <summary>
		/// An instruction used by the try-catch clauses in Chips (e.g. "tryc")
		/// </summary>
		ExceptionHandling = 0x1000,
		/// <summary>
		/// An instruction that modifies the evaluation stack directly
		/// </summary>
		ModifiesEvaluationStack = 0x2000,
		/// <summary>
		/// An instruction that accesses a component of data (e.g. "dtai" and "dtd")
		/// </summary>
		ValueAccess = 0x4000,
		/// <summary>
		/// A <see cref="Opcode"/> instance which represents a family of opcodes shared under a common byte in the opcode table (e.g. <see cref="Opcodes.Io"/>)
		/// </summary>
		ExtendedOpcode = ExtendedOpcodePure | NoSpecification,
		ExtendedOpcodePure = 0x8000,
		/// <summary>
		/// An instruction that modifies the internal Chips stack structure (e.g. "dup")
		/// </summary>
		ModifiesStack = 0x10000
	}
}
