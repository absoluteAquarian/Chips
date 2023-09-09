using AsmResolver.DotNet;
using AsmResolver.DotNet.Signatures.Types;
using System;
using System.Linq;

namespace Chips.Utility {
	partial class Extensions {
		public static string? GetSimplifiedGenericTypeName(this Type type) {
			//Handle all invalid cases here:
			if (type.FullName is null)
				return type.Name;

			if (!type.IsGenericType)
				return type.FullName;

			string parent = type.GetGenericTypeDefinition().FullName!;

			//Include all but the "`X" part
			parent = parent[..parent.IndexOf('`')];

			//Construct the child types
			return $"{parent}<{string.Join(", ", type.GetGenericArguments().Select(GetSimplifiedGenericTypeName))}>";
		}

		public static TypeDefinition AdjustTypeBasedOnSuffix(this TypeDefinition type, string suffix) {
			var factory = type.Module?.CorLibTypeFactory ?? throw new InvalidOperationException("Type is not in a module.");

			bool nullable = false;

			foreach (string modifier in suffix.Split(',', StringSplitOptions.RemoveEmptyEntries)) {
				if (modifier.StartsWith("[]d")) {
					// []dN -> array of N dimensions
					int dimensions = int.Parse(suffix[3..]);

					ArrayBaseTypeSignature signature = dimensions switch {
						1 => type.MakeSzArrayType(),
						_ => type.MakeArrayType(dimensions)
					};

					type = signature.Resolve()!;

					// Nullable modifier was consumed by the array signature
					nullable = false;
					continue;
				} else if (modifier == "?") {
					// Nullable type
					if (nullable)
						throw new ArgumentException("Multiple nullable modifiers were specified or the type is already Nullable<T>.");

					type = factory.CorLibScope
						.CreateTypeReference("System", "Nullable`1")
						.MakeGenericInstanceType(type.ToTypeSignature())
						.Resolve()!;
					nullable = true;
					continue;
				}

				throw new ArgumentException($"Invalid type suffix: {suffix}");
			}

			return type;
		}
	}
}
