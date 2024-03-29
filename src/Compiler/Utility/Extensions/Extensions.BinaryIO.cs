﻿using AsmResolver.DotNet;
using System.IO;
using System;

namespace Chips.Compiler.Utility {
	partial class Extensions {
		public static void Write(this BinaryWriter writer, ITypeDefOrRef? type, StringHeap heap) {
			ArgumentNullException.ThrowIfNull(type);
			ArgumentNullException.ThrowIfNull(heap);

			if (type.Module?.Assembly is null)
				throw new ArgumentException("Type must be defined in an assembly", nameof(type));

			type.WriteFullName(writer, heap);
		}

		public static DelayedTypeResolver ReadTypeDefinition(this BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			string fullName = heap.ReadString(reader);

			return new DelayedTypeResolver(resolver, fullName, false);
		}

		public static void Write(this BinaryWriter writer, IFieldDescriptor field, StringHeap heap) {
			ArgumentNullException.ThrowIfNull(field);
			ArgumentNullException.ThrowIfNull(heap);
			
			field.WriteFullName(writer, heap);
		}

		public static DelayedFieldResolver ReadFieldDefinition(this BinaryReader reader, TypeResolver resolver, StringHeap heap) {
			string typeName = heap.ReadString(reader);
			string name = heap.ReadString(reader);

			return new DelayedFieldResolver(resolver, $"{typeName}::{name}");
		}
	}
}
