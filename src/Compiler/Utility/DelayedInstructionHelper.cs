using AsmResolver.DotNet;
using AsmResolver.DotNet.Code.Cil;
using AsmResolver.DotNet.Signatures;
using AsmResolver.PE.DotNet.Cil;
using System;

namespace Chips.Compiler.Utility {
	internal static partial class DelayedInstructionHelper {
		internal static readonly SignatureComparer _typeComparer = new SignatureComparer(SignatureComparisonFlags.AcceptNewerVersions);

		public static StrictEvaluationStackSimulator GetStackUpTo(this CilMethodBody body, int instructionIndexExclusive) {
			StrictEvaluationStackSimulator stack = new();
			stack.ModifyStack(body, 0, instructionIndexExclusive);
			return stack;
		}

		public static void ModifyStack(this StrictEvaluationStackSimulator stack, CilMethodBody body, int instructionIndexStart, int instructionIndexEndExclusive) {
			for (int i = instructionIndexStart; i < instructionIndexEndExclusive; i++)
				body.ModifyStackSimulation(i, stack);
		}

		public static void ModifyStackSimulation(this CilMethodBody body, int index, StrictEvaluationStackSimulator stack) {
			var instruction = body.Instructions[index];

			// Simulate the stack
			switch (instruction.OpCode.Code) {
				case CilCode.Nop:
				case CilCode.Break:
					break;
				case CilCode.Ldarg_0:
				case CilCode.Ldarg_1:
				case CilCode.Ldarg_2:
				case CilCode.Ldarg_3:
					stack.Push(instruction.GetArgument(body));
					break;
				case CilCode.Ldloc_0:
				case CilCode.Ldloc_1:
				case CilCode.Ldloc_2:
				case CilCode.Ldloc_3:
					stack.Push(instruction.GetLocal(body));
					break;
				case CilCode.Stloc_0:
				case CilCode.Stloc_1:
				case CilCode.Stloc_2:
				case CilCode.Stloc_3:
					stack.Pop();
					break;
				case CilCode.Ldarg_S:
					stack.Push(instruction.GetArgument(body));
					break;
				case CilCode.Ldarga_S:
					stack.Push(GetAddress());
					break;
				case CilCode.Starg_S:
					stack.Pop();
					break;
				case CilCode.Ldloc_S:
					stack.Push(instruction.GetLocal(body));
					break;
				case CilCode.Ldloca_S:
					stack.Push(GetAddress());
					break;
				case CilCode.Stloc_S:
					stack.Pop();
					break;
				case CilCode.Ldnull:
					stack.Push(GetNull());
					break;
				case CilCode.Ldc_I4_M1:
				case CilCode.Ldc_I4_0:
				case CilCode.Ldc_I4_1:
				case CilCode.Ldc_I4_2:
				case CilCode.Ldc_I4_3:
				case CilCode.Ldc_I4_4:
				case CilCode.Ldc_I4_5:
				case CilCode.Ldc_I4_6:
				case CilCode.Ldc_I4_7:
				case CilCode.Ldc_I4_8:
				case CilCode.Ldc_I4_S:
				case CilCode.Ldc_I4:
					stack.Push(GetInt32());
					break;
				case CilCode.Ldc_I8:
					stack.Push(GetInt64());
					break;
				case CilCode.Ldc_R4:
					stack.Push(GetSingle());
					break;
				case CilCode.Ldc_R8:
					stack.Push(GetDouble());
					break;
				case CilCode.Dup:
					stack.Push(stack.Peek());
					break;
				case CilCode.Pop:
					stack.Pop();
					break;
				case CilCode.Jmp:
					break;
				case CilCode.Call:
				case CilCode.Calli:
					instruction.PushAndPopMethodArgsAndReturn(stack);
					break;
				case CilCode.Ret:
					break;
				case CilCode.Br_S:
					stack.SetBranch(instruction.GetBranchTarget(body));
					break;
				case CilCode.Brfalse_S:
				case CilCode.Brtrue_S:
					stack.Pop();
					stack.SetBranch(instruction.GetBranchTarget(body));
					break;
				case CilCode.Beq_S:
				case CilCode.Bge_S:
				case CilCode.Bgt_S:
				case CilCode.Ble_S:
				case CilCode.Blt_S:
				case CilCode.Bne_Un_S:
				case CilCode.Bge_Un_S:
				case CilCode.Bgt_Un_S:
				case CilCode.Ble_Un_S:
				case CilCode.Blt_Un_S:
					stack.Pop();
					stack.Pop();
					stack.SetBranch(instruction.GetBranchTarget(body));
					break;
				case CilCode.Br:
					stack.SetBranch(instruction.GetBranchTarget(body));
					break;
				case CilCode.Brfalse:
				case CilCode.Brtrue:
					stack.Pop();
					stack.SetBranch(instruction.GetBranchTarget(body));
					break;
				case CilCode.Beq:
				case CilCode.Bge:
				case CilCode.Bgt:
				case CilCode.Ble:
				case CilCode.Blt:
				case CilCode.Bne_Un:
				case CilCode.Bge_Un:
				case CilCode.Bgt_Un:
				case CilCode.Ble_Un:
				case CilCode.Blt_Un:
					stack.Pop();
					stack.Pop();
					stack.SetBranch(instruction.GetBranchTarget(body));
					break;
				case CilCode.Switch:
					instruction.UpdateSwitchOffsets(index, stack);
					break;
				case CilCode.Ldind_I1:
				case CilCode.Ldind_U1:
				case CilCode.Ldind_I2:
				case CilCode.Ldind_U2:
				case CilCode.Ldind_I4:
				case CilCode.Ldind_U4:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Ldind_I8:
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Ldind_I:
					stack.Pop();
					stack.Push(GetNativeInt());
					break;
				case CilCode.Ldind_R4:
					stack.Pop();
					stack.Push(GetSingle());
					break;
				case CilCode.Ldind_R8:
					stack.Pop();
					stack.Push(GetDouble());
					break;
				case CilCode.Ldind_Ref:
					stack.Pop();
					stack.Push(GetObject());
					break;
				case CilCode.Stind_Ref:
				case CilCode.Stind_I1:
				case CilCode.Stind_I2:
				case CilCode.Stind_I4:
				case CilCode.Stind_I8:
				case CilCode.Stind_R4:
				case CilCode.Stind_R8:
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Add:
				case CilCode.Sub:
				case CilCode.Mul:
				case CilCode.Div:
				case CilCode.Div_Un:
				case CilCode.Rem:
				case CilCode.Rem_Un:
				case CilCode.And:
				case CilCode.Or:
				case CilCode.Xor:
				case CilCode.Shl:
				case CilCode.Shr:
				case CilCode.Shr_Un:
					PushAndPopArithmetic(stack);
					break;
				case CilCode.Neg:
				case CilCode.Not:
					stack.Push(stack.Pop());
					break;
				case CilCode.Conv_I1:
				case CilCode.Conv_I2:
				case CilCode.Conv_I4:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Conv_I8:
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Conv_R4:
					stack.Pop();
					stack.Push(GetSingle());
					break;
				case CilCode.Conv_R8:
					stack.Pop();
					stack.Push(GetDouble());
					break;
				case CilCode.Conv_U4:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Conv_U8:
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Callvirt:
					instruction.PushAndPopMethodArgsAndReturn(stack);
					break;
				case CilCode.Cpobj:
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Ldobj:
					stack.Pop();
					stack.Push(instruction.GetInlineTypeOperand());
					break;
				case CilCode.Ldstr:
					stack.Push(GetString());
					break;
				case CilCode.Newobj:
					instruction.PushAndPopMethodArgsAndReturn(stack);
					break;
				case CilCode.Castclass:
				case CilCode.Isinst:
					stack.Pop();
					stack.Push(instruction.GetInlineTypeOperand());
					break;
				case CilCode.Conv_R_Un:
					stack.Pop();
					stack.Push(GetSingle());
					break;
				case CilCode.Unbox:
					stack.Pop();
					stack.Push(GetAddress());
					break;
				case CilCode.Throw:
					stack.Pop();
					break;
				case CilCode.Ldfld:
					stack.Pop();
					stack.Push(instruction.GetFieldOperandType());
					break;
				case CilCode.Ldflda:
					stack.Pop();
					stack.Push(GetAddress());
					break;
				case CilCode.Stfld:
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Ldsfld:
					stack.Push(instruction.GetFieldOperandType());
					break;
				case CilCode.Ldsflda:
					stack.Push(GetAddress());
					break;
				case CilCode.Stsfld:
					stack.Pop();
					break;
				case CilCode.Stobj:
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Conv_Ovf_I1_Un:
				case CilCode.Conv_Ovf_I2_Un:
				case CilCode.Conv_Ovf_I4_Un:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Conv_Ovf_I8_Un:
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Conv_Ovf_U1_Un:
				case CilCode.Conv_Ovf_U2_Un:
				case CilCode.Conv_Ovf_U4_Un:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Conv_Ovf_U8_Un:
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Conv_Ovf_I_Un:
				case CilCode.Conv_Ovf_U_Un:
					stack.Pop();
					stack.Push(GetNativeInt());
					break;
				case CilCode.Box:
					var typeToBox = stack.Pop() ?? throw new InvalidOperationException("Object on stack was null.");
					stack.Push(new BoxedTypeDefOrRef(typeToBox));
					break;
				case CilCode.Newarr:
					stack.Pop();
					stack.Push(instruction.GetArrayOperandType());
					break;
				case CilCode.Ldlen:
					stack.Pop();
					stack.Push(GetNativeUnsignedInt());
					break;
				case CilCode.Ldelema:
					stack.Pop();
					stack.Pop();
					stack.Push(GetAddress());
					break;
				case CilCode.Ldelem_I1:
				case CilCode.Ldelem_U1:
				case CilCode.Ldelem_I2:
				case CilCode.Ldelem_U2:
				case CilCode.Ldelem_I4:
				case CilCode.Ldelem_U4:
					stack.Pop();
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Ldelem_I8:
					stack.Pop();
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Ldelem_I:
					stack.Pop();
					stack.Pop();
					stack.Push(GetNativeInt());
					break;
				case CilCode.Ldelem_R4:
					stack.Pop();
					stack.Pop();
					stack.Push(GetSingle());
					break;
				case CilCode.Ldelem_R8:
					stack.Pop();
					stack.Pop();
					stack.Push(GetDouble());
					break;
				case CilCode.Ldelem_Ref:
					stack.Pop();
					stack.Pop();
					stack.Push(GetObject());
					break;
				case CilCode.Stelem_I:
				case CilCode.Stelem_I1:
				case CilCode.Stelem_I2:
				case CilCode.Stelem_I4:
				case CilCode.Stelem_I8:
				case CilCode.Stelem_R4:
				case CilCode.Stelem_R8:
				case CilCode.Stelem_Ref:
					stack.Pop();
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Ldelem:
					stack.Pop();
					stack.Pop();
					stack.Push(instruction.GetInlineTypeOperand());
					break;
				case CilCode.Stelem:
					stack.Pop();
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Unbox_Any:
					stack.Pop();
					stack.Push(instruction.GetInlineTypeOperand());
					break;
				case CilCode.Conv_Ovf_I1:
				case CilCode.Conv_Ovf_U1:
				case CilCode.Conv_Ovf_I2:
				case CilCode.Conv_Ovf_U2:
				case CilCode.Conv_Ovf_I4:
				case CilCode.Conv_Ovf_U4:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Conv_Ovf_I8:
				case CilCode.Conv_Ovf_U8:
					stack.Pop();
					stack.Push(GetInt64());
					break;
				case CilCode.Refanyval:
					stack.Pop();
					stack.Push(GetAddress());
					break;
				case CilCode.Ckfinite:
					stack.Push(stack.Pop());
					break;
				case CilCode.Mkrefany:
					stack.Pop();
					stack.Push(GetTypedReference());
					break;
				case CilCode.Ldtoken:
					stack.Push(GetRuntimeHandle());
					break;
				case CilCode.Conv_U2:
				case CilCode.Conv_U1:
					stack.Pop();
					stack.Push(GetInt32());
					break;
				case CilCode.Conv_I:
				case CilCode.Conv_Ovf_I:
				case CilCode.Conv_Ovf_U:
					stack.Pop();
					stack.Push(GetNativeInt());
					break;
				case CilCode.Add_Ovf:
				case CilCode.Add_Ovf_Un:
				case CilCode.Mul_Ovf:
				case CilCode.Mul_Ovf_Un:
				case CilCode.Sub_Ovf:
				case CilCode.Sub_Ovf_Un:
					PushAndPopArithmetic(stack);
					break;
				case CilCode.Endfinally:
				case CilCode.Leave:
				case CilCode.Leave_S:
					break;
				case CilCode.Stind_I:
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Conv_U:
					stack.Pop();
					stack.Push(GetNativeInt());
					break;
				case CilCode.Prefix7:
				case CilCode.Prefix6:
				case CilCode.Prefix5:
				case CilCode.Prefix4:
				case CilCode.Prefix3:
				case CilCode.Prefix2:
				case CilCode.Prefix1:
				case CilCode.Prefixref:
					break;
				case CilCode.Arglist:
					stack.Push(GetNativeInt()?.MakePointerType().Resolve() ?? throw new ArgumentException("Could not resolve nint* type."));
					break;
				case CilCode.Ceq:
				case CilCode.Cgt:
				case CilCode.Cgt_Un:
				case CilCode.Clt:
				case CilCode.Clt_Un:
					PushAndPopComparison(stack);
					break;
				case CilCode.Ldftn:
					stack.Push(GetAddress());
					break;
				case CilCode.Ldvirtftn:
					stack.Pop();
					stack.Push(GetAddress());
					break;
				case CilCode.Ldarg:
					stack.Push(instruction.GetArgument(body));
					break;
				case CilCode.Ldarga:
					stack.Push(GetAddress());
					break;
				case CilCode.Starg:
					stack.Pop();
					break;
				case CilCode.Ldloc:
					stack.Push(instruction.GetLocal(body));
					break;
				case CilCode.Ldloca:
					stack.Push(GetAddress());
					break;
				case CilCode.Stloc:
					stack.Pop();
					break;
				case CilCode.Localloc:
					stack.Push(GetNativeInt());
					break;
				case CilCode.Endfilter:
					throw new InvalidOperationException("Opcode \"endfilter\" is not supported.");
				case CilCode.Unaligned:
					throw new InvalidOperationException("Opcode \"unaligned.\" is not supported.");
				case CilCode.Volatile:
					throw new InvalidOperationException("Opcode \"volatile.\" is not supported.");
				case CilCode.Tailcall:
					throw new InvalidOperationException("Opcode \"tail.\" is not supported.");
				case CilCode.Initobj:
					stack.Pop();
					break;
				case CilCode.Constrained:
					throw new InvalidOperationException("Opcode \"constrained.\" is not supported.");
				case CilCode.Cpblk:
				case CilCode.Initblk:
					stack.Pop();
					stack.Pop();
					stack.Pop();
					break;
				case CilCode.Rethrow:
					break;
				case CilCode.Sizeof:
					stack.Push(GetInt32());
					break;
				case CilCode.Refanytype:
					stack.Pop();
					stack.Push(GetTypeHandle());
					break;
				case CilCode.Readonly:
					throw new InvalidOperationException("Opcode \"readonly.\" is not supported.");
				default:
					throw new ArgumentOutOfRangeException("Unknown opcode: " + instruction.OpCode.Code);
			}
		}
	}
}
