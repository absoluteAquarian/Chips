using Chips.Common.DataStructures;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace Chips.Runtime.Meta {
	partial class RegisterTable {
		public ref T Get<T>(Register register) {
			if (!_usedRegisters[(int)register])
				throw new UninitializedRegisterException(register);

			// JIT reduces typeof(T) to a constant, allowing for code blocks to be removed entirely
			if (typeof(T) == typeof(sbyte) || typeof(T) == typeof(byte) || typeof(T) == typeof(short) || typeof(T) == typeof(ushort)
			|| typeof(T) == typeof(int) || typeof(T) == typeof(uint) || typeof(T) == typeof(long) || typeof(T) == typeof(ulong)
			|| typeof(T) == typeof(char)) {
				// Register must be a direct integer register ($Zxx), or a variant register with an integer value
				_Integer type = TypeToInteger<T>();

				if (register >= Register.Z_0 && register <= Register.Z_1F) {
					ref _NumberInteger info = ref Get(ref _z, register - Register.Z_0);
					if (info.type != type)
						throw new RegisterMismatchException<T>(register);
					
					return ref Unsafe.As<_NumberInteger, T>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.Integer || (_Integer)info.subtype != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, T>(ref info);
				}
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T) == typeof(nint) || typeof(T) == typeof(nuint)) {
				// Register must be a direct address register ($PTRxx), or a variant register with an address value
				_NativeInt type = TypeToNativeInt<T>();

				if (register >= Register.PTR_0 && register <= Register.PTR_F) {
					ref _Address info = ref Get(ref _ptr, register - Register.PTR_0);
					if (info.type != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_Address, T>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.Address || (_NativeInt)info.subtype != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, T>(ref info);
				}
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T) == typeof(float) || typeof(T) == typeof(double) || typeof(T) == typeof(decimal)) {
				// Register must be a direct floating-point register ($FLxx), or a variant register with a floating-point value
				_Float type = TypeToFloat<T>();

				if (register >= Register.FL_0 && register <= Register.FL_1F) {
					ref _NumberFloat info = ref Get(ref _fl, register - Register.FL_0);
					if (info.type != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_NumberFloat, T>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.Float || (_Float)info.subtype != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, T>(ref info);
				}
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T) == typeof(string) || typeof(T) == typeof(StringBuilder)) {
				// Register must be a direct string register ($STRxx), or a variant register with a string value
				_String type = TypeToStringObject<T>();

				if (register >= Register.S_0 && register <= Register.S_F) {
					ref _StringObject info = ref Get(ref _s, register - Register.S_0);
					if (info.type != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_StringObject, T>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.String || (_String)info.subtype != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, T>(ref info);
				}
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T) == typeof(Vector2) || typeof(T) == typeof(Vector3) || typeof(T) == typeof(Vector4)) {
				// Register must be a direct vector register ($Vxx), or a variant register with a vector value
				_Vector type = TypeToVector<T>();

				if (register >= Register.V_0 && register <= Register.V_F) {
					ref _VectorObject info = ref Get(ref _v, register - Register.V_0);
					if (info.type != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VectorObject, T>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.Vector)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, T>(ref info);
				}
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T) == typeof(RuntimeTypeHandle) || typeof(T) == typeof(RuntimeMethodHandle) || typeof(T) == typeof(RuntimeFieldHandle)) {
				// Register must be a direct metadata token register ($Txx), or a variant register with a metadata token value
				_MetadataToken type = TypeToMetadataToken<T>();

				if (register >= Register.T_0 && register <= Register.T_2) {
					ref _MetadataTokenHandle info = ref Get(ref _t, register - Register.T_0);
					if (info.type != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_MetadataTokenHandle, T>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.Token || (_MetadataToken)info.subtype != type)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, T>(ref info);
				}
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T) == typeof(Status)) {
				if (register == Register.PS)
					return ref Unsafe.As<Status, T>(ref _ps);
				
				throw new InvalidRegisterException<T>(register);
			} else if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Vector<>)) {
				// A different method should be used instead
				throw new ArgumentException("Vector<T> is not a valid type for this method, use GetVector<T>() instead");
			} else if (typeof(Exception).IsAssignableFrom(typeof(T))) {
				if (register == Register.EX)
					return ref Unsafe.As<Exception, T>(ref _ex);
				
				throw new InvalidRegisterException<T>(register);
			} else {
				// Register must be a direct object register ($OBJxx), or a variant register
				if ((register >= Register.OBJ_0 && register <= Register.OBJ_3F) || (register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS)
					throw new InvalidRegisterMethodException(register, alternative: typeof(T).IsValueType ? "GetStruct<T>" : "GetObject<T>");
				else
					throw new InvalidRegisterException<T>(register);
			}
		}

		public ref Vector<T> GetVector<T>(Register register) where T : struct {
			if (!_usedRegisters[(int)register])
				throw new UninitializedRegisterException(register);

			if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Vector<>)) {
				// Register must be a direct vector register ($Vxx), or a variant register with a vector value
				if (register >= Register.V_0 && register <= Register.V_F) {
					ref _VectorObject info = ref Get(ref _v, register - Register.V_0);
					if (info.type != _Vector.VectorX)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VectorObject, Vector<T>>(ref info);
				} else if ((register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
					ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref Get(ref _r, register - Register.R_0));
					if (info.type != _Variant.Vector)
						throw new RegisterMismatchException<T>(register);

					return ref Unsafe.As<_VariantObject, Vector<T>>(ref info);
				}
					
				throw new InvalidRegisterException<T>(register);
			} else
				throw new ArgumentException("T must be a Vector<T> type", nameof(T));
		}

		public ref T GetObject<T>(Register register) where T : class {
			if (!_usedRegisters[(int)register])
				throw new UninitializedRegisterException(register);

			// Register must be a direct object register ($OBJxx), or a variant register
			if ((register >= Register.OBJ_0 && register <= Register.OBJ_3F) || (register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
				ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref register >= Register.R_0 ? ref Get(ref _r, register - Register.R_0) : ref Get(ref _obj, register - Register.OBJ_0));

				if (info.type != _Variant.Object || (_VariantObjectSubtype)info.subtype != _VariantObjectSubtype.Reference)
					throw new RegisterMismatchException<T>(register);

				ref object? obj = ref _VariantObject.GetObject<object>(ref info);
				if (obj is not (null or T))
					throw new RegisterMismatchException<T>(register);

				return ref obj is null ? ref Unsafe.NullRef<T>() : ref Unsafe.As<object, T>(ref obj);
			} else
				throw new InvalidRegisterException<T>(register);
		}

		public ref T GetStruct<T>(Register register) where T : struct {
			if (!_usedRegisters[(int)register])
				throw new UninitializedRegisterException(register);

			// Register must be a direct object register ($OBJxx), or a variant register
			if ((register >= Register.OBJ_0 && register <= Register.OBJ_3F) || (register >= Register.R_0 && register <= Register.R_2) || register == Register.ANS) {
				ref _VariantObject info = ref (register == Register.ANS ? ref _ans : ref register >= Register.R_0 ? ref Get(ref _r, register - Register.R_0) : ref Get(ref _obj, register - Register.OBJ_0));

				if (info.type != _Variant.Object || (_VariantObjectSubtype)info.subtype != _VariantObjectSubtype.Valuetype || _VariantObject.GetObject<object>(ref info) is not Ref<T> objRef)
					throw new RegisterMismatchException<T>(register);

				return ref objRef.value;
			} else
				throw new InvalidRegisterException<T>(register);
		}
	}
}
