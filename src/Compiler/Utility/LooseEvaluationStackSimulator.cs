using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// An object representing a simulation of the evaluation stack, used to provide context for Chips instructions.<br/>
	/// For a more precise evaluation, use <see cref="StrictEvaluationStackSimulator"/>
	/// </summary>
	public sealed class LooseEvaluationStackSimulator {
		[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
		private readonly struct State : IEquatable<State> {
			public readonly StackObject[] stack;

			public State(LooseEvaluationStackSimulator simulator) {
				stack = simulator._stack.ToArray();
			}

			public bool Equals(State other) {
				ReadOnlySpan<StackObject> a = stack;
				ReadOnlySpan<StackObject> b = other.stack;
				return a.SequenceEqual(b);
			}

			public override bool Equals(object? obj) => obj is State state && Equals(state);

			public override int GetHashCode() => stack.GetHashCode();

			public override string? ToString() => stack.Length == 0 ? "[ ]" : $"[ {string.Join(", ", stack)} ]";

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator ==(State left, State right) => left.Equals(right);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator !=(State left, State right) => !(left == right);

			private string DebuggerDisplay => ToString()!;
		}

		private readonly Stack<StackObject> _stack = new();

		private readonly Dictionary<int, State> _knownStates = new();
		private int _numStatesFromStart;

		public void Push(StackObject obj) {
			// State refers to the state of the stack BEFORE this instruction is executed
			State state = new State(this);

			if (_knownStates.TryGetValue(_numStatesFromStart, out State existingState)) {
				// If the state is already known, check if it matches the current state
				if (existingState != state)
					throw new InvalidOperationException("Evaluation stack was imbalanced");

				_numStatesFromStart++;
				_stack.Push(obj);
			} else {
				// If the state is not known, add it to the known states
				_knownStates.Add(_numStatesFromStart, state);
			}

			_numStatesFromStart++;
			_stack.Push(obj);
		}

		public void PushBranch(StackObject objIfBranchNotMet, int branchTarget) {
			// State refers to the state of the stack BEFORE this instruction is executed
			State state = new State(this);

			if (!_knownStates.TryGetValue(branchTarget, out State existingState)) {
				// If the state is not known, add it to the known states
				_knownStates.Add(branchTarget, state);
			} else {
				// If the state is already known, check if it matches the current state
				if (existingState != state)
					throw new InvalidOperationException("Evaluation stack was imbalanced");
			}

			// Update the stack in either case
			_numStatesFromStart++;
			_stack.Push(objIfBranchNotMet);
		}

		public void SetBranch(int branchTarget) {
			// State refers to the state of the stack BEFORE this instruction is executed
			State state = new State(this);

			if (!_knownStates.TryGetValue(branchTarget, out State existingState)) {
				// If the state is not known, add it to the known states
				_knownStates.Add(branchTarget, state);
			} else {
				// If the state is already known, check if it matches the current state
				if (existingState != state)
					throw new InvalidOperationException("Evaluation stack was imbalanced");
			}
		}

		public StackObject Pop() {
			if (_stack.Count == 0)
				throw new InvalidOperationException("Cannot pop from an empty stack");

			// State refers to the state of the stack BEFORE this instruction is executed
			_knownStates.Add(_numStatesFromStart, new State(this));
			_numStatesFromStart++;
			return _stack.Pop();
		}

		public StackObject Peek() {
			if (_stack.Count == 0)
				throw new InvalidOperationException("Cannot peek from an empty stack");

			return _stack.Peek();
		}

		public void Clear() {
			_stack.Clear();
			_knownStates.Clear();
			_numStatesFromStart = 0;
		}
	}

	public enum StackObject {
		/// <summary>
		/// A value of an undetermined type
		/// </summary>
		Object,
		/// <summary>
		/// Any integer value
		/// </summary>
		Integer,
		/// <summary>
		/// Any floating-point value
		/// </summary>
		Float,
		/// <summary>
		/// Any boolean value
		/// </summary>
		Boolean,
		/// <summary>
		/// A native integer for a memory address
		/// </summary>
		Address,
		/// <summary>
		/// A string value
		/// </summary>
		String
	}
}
