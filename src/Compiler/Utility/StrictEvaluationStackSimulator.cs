using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System;
using AsmResolver.DotNet;
using System.Linq;
using Chips.Runtime.Utility;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// An object representing a simulation of the evaluation stack, used to provide context for CIL instructions
	/// </summary>
	public sealed class StrictEvaluationStackSimulator {
		[DebuggerDisplay($"{{{nameof(DebuggerDisplay)},nq}}")]
		private readonly struct State : IEquatable<State> {
			public readonly ITypeDefOrRef?[] stack;

			public State(StrictEvaluationStackSimulator simulator) {
				stack = simulator._stack.ToArray();
			}

			public bool Equals(State other) {
				ReadOnlySpan<ITypeDefOrRef?> a = stack;
				ReadOnlySpan<ITypeDefOrRef?> b = other.stack;
				return a.SequenceEqual(b, TypeTokenComparer.Instance);
			}

			private class TypeTokenComparer : IEqualityComparer<ITypeDefOrRef?> {
				public static TypeTokenComparer Instance { get; } = new();

				public bool Equals(ITypeDefOrRef? x, ITypeDefOrRef? y) => x?.MetadataToken == y?.MetadataToken;

				public int GetHashCode(ITypeDefOrRef? obj) => obj?.MetadataToken.GetHashCode() ?? 0;
			}

			public override bool Equals(object? obj) => obj is State state && Equals(state);

			public override int GetHashCode() => stack.GetHashCode();

			public override string? ToString() => stack.Length == 0 ? "[ ]" : $"[ {string.Join(", ", stack.Select(static t => t?.Name ?? "null"))} ]";

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator ==(State left, State right) => left.Equals(right);

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static bool operator !=(State left, State right) => !(left == right);

			private string DebuggerDisplay => ToString()!;
		}

		private readonly Stack<ITypeDefOrRef?> _stack = new();

		private readonly Dictionary<int, State> _knownStates = new();
		private int _numStatesFromStart;

		public void Push(ITypeDefOrRef? obj) {
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

		public void PushBranch(ITypeDefOrRef? objIfBranchNotMet, int branchTarget) {
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

		public ITypeDefOrRef? Pop() {
			if (_stack.Count == 0)
				throw new InvalidOperationException("Cannot pop from an empty stack");

			_knownStates.Add(_numStatesFromStart, new State(this));
			_numStatesFromStart++;
			return _stack.Pop();
		}

		public ITypeDefOrRef? Peek() {
			if (_stack.Count == 0)
				throw new InvalidOperationException("Cannot peek from an empty stack");

			return _stack.Peek();
		}

		public ITypeDefOrRef? PeekSkip(int amount) {
			if (_stack.Count == 0)
				throw new InvalidOperationException("Cannot peek from an empty stack");

			int size = _stack.Count - 1;

			if (size < amount)
				throw new InvalidOperationException($"Cannot peek more than the stack size ({size + 1} <= {amount})");

			var underlyingArray = typeof(Stack<ITypeDefOrRef?>).RetrieveField<ITypeDefOrRef?[]>("_array", _stack);

			if (underlyingArray is null)
				throw new InvalidOperationException("Could not retrieve the underlying array of the stack");

			return underlyingArray[size - amount];
		}

		public void Clear() {
			_stack.Clear();
			_knownStates.Clear();
			_numStatesFromStart = 0;
		}
	}
}
