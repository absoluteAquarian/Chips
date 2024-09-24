using System.Runtime.CompilerServices;

namespace Chips.Runtime.Meta {
	interface ICreateObject<T> where T : struct, ICreateObject<T> {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		abstract static T Create<U>(in U value);
	}
}
