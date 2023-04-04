using Chips.Compilation;
using Chips.Runtime.Types;
using System;
using System.IO;
using System.Numerics;

namespace Chips.Utility {
	internal sealed class BytecodeWriter : BinaryWriter {
		public BytecodeWriter(Stream output) : base(output) { }

		private void WriteConstantID(OperandType type)
			=> Write((byte)type);

		public void WriteConstant(object value) {
			switch (value) {
				case int i32:
					WriteConstantID(OperandType.I32);
					Write(i32);
					break;
				case sbyte i8:
					WriteConstantID(OperandType.I8);
					Write(i8);
					break;
				case short i16:
					WriteConstantID(OperandType.I16);
					Write(i16);
					break;
				case long i64:
					WriteConstantID(OperandType.I64);
					Write(i64);
					break;
				case uint u32:
					WriteConstantID(OperandType.U32);
					Write(u32);
					break;
				case byte u8:
					WriteConstantID(OperandType.U8);
					Write(u8);
					break;
				case ushort u16:
					WriteConstantID(OperandType.U16);
					Write(u16);
					break;
				case ulong u64:
					WriteConstantID(OperandType.U64);
					Write(u64);
					break;
				case BigInteger big:
					WriteConstantID(OperandType.BigInt);
					var bigArray = big.ToByteArray();
					Write7BitEncodedInt(bigArray.Length);
					Write(bigArray);
					break;
				case float f32:
					WriteConstantID(OperandType.F32);
					Write(f32);
					break;
				case double f64:
					WriteConstantID(OperandType.F64);
					Write(f64);
					break;
				case decimal f128:
					WriteConstantID(OperandType.F128);
					Write(f128);
					break;
				case Half f16:
					WriteConstantID(OperandType.F16);
					Write(f16);
					break;
				case char c:
					WriteConstantID(OperandType.Char);
					Write(c);
					break;
				case string s:
					WriteConstantID(OperandType.String);
					this.WriteCPDBString(s);
					break;
				case Indexer idx:
					WriteConstantID(OperandType.Indexer);
					Write(idx.value);
					break;
				case bool b:
					WriteConstantID(OperandType.Bool);
					Write(b);
					break;
				case Complex cmp:
					WriteConstantID(OperandType.Complex);
					Write(cmp.Real);
					Write(cmp.Imaginary);
					break;
				default:
					throw new ArgumentException($"Unsupported constant type: {value.GetType().FullName}");
			}
		}

		public void WriteLocalAccess(int index)
			=> WriteVariableAccess(0b00, index);

		public void WriteGlobalAccess(int index)
			=> WriteVariableAccess(0b01, index);

		public void WriteRegisterAccess(string registerName) {
			int index = registerName switch {
				"A" => 0,
				"E" => 1,
				"X" => 2,
				"Y" => 3,
				"SP" => 4,
				"S" => 5,
				_ => throw new ArgumentException("Unknown register: " + registerName)
			};
			
			WriteVariableAccess(0x10, index);
		}

		public void WriteFlagAccess(string flagName) {
			int index = flagName switch {
				"Carry" => 0,
				"C" => 0,
				"Conversion" => 1,
				"N" => 1,
				"Comparison" => 2,
				"O" => 2,
				"PropertyAccess" => 3,
				"P" => 3,
				"RegexSuccess" => 4,
				"R" => 4,
				"Zero" => 5,
				"Z" => 5,
				_ => throw new ArgumentException("Unknown flag: " + flagName)
			};
			
			WriteVariableAccess(0x10, index);
		}

		private void WriteVariableAccess(byte type, int index) {
			if (index >= int.MaxValue >> 2)
				throw new Exception("Variable index was too large");
			
			int data = type | (index << 2);
			Write7BitEncodedInt(data);
		}

		public void WriteType(string chipsType) {
			this.WriteCPDBString(chipsType);
		}
	}
}
