using AsmResolver.DotNet;
using Chips.Compiler.ErrorHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Chips.Compiler.Utility {
	/// <summary>
	/// A type containing information about referenced assemblies, their namespaces and resolving type identifiers
	/// </summary>
	public sealed class TypeResolver {
		private readonly struct ResolvedType(AssemblyDefinition assembly, string declaringNamespace, string type, TypeDefinition definition) {
			public readonly AssemblyDefinition assembly = assembly;
			public readonly string declaringNamespace = declaringNamespace;
			public readonly string type = type;
			public readonly TypeDefinition definition = definition;

			public string FullName => declaringNamespace + "." + type;
		}

		private readonly struct ResolvedAssembly {
			public readonly AssemblyDefinition assembly;
			private readonly HashSet<string> _resolvedNamespaces;
			public readonly List<ResolvedType> resolvedTypes;

			public ResolvedAssembly(AssemblyDefinition assembly) {
				ArgumentNullException.ThrowIfNull(assembly);

				this.assembly = assembly;
				_resolvedNamespaces = [];
				resolvedTypes = [];

				foreach (var type in assembly.Modules.SelectMany(static m => m.GetAllTypes())) {
					var declaringNamespace = type.Namespace ?? "";
					var typeName = type.Name ?? throw new NullReferenceException("Type name must not be null");

					var resolvedType = new ResolvedType(assembly, declaringNamespace, typeName, type);

					_resolvedNamespaces.Add(declaringNamespace);
					resolvedTypes.Add(resolvedType);
				}
			}

			public bool HasNamespace(string ns) => _resolvedNamespaces.Contains(ns);

			public TypeResolveAttempt TryResolveType(string fullTypeName, [NotNullWhen(true)] out TypeDefinition? definition, IEnumerable<string> availableNamespaces) {
				definition = null;

				// No available namespaces, type resolving would fail
				if (!availableNamespaces.Any())
					return TypeResolveAttempt.Fail_NoSources(fullTypeName);

				// Split namespace and type
				int lastDotIndex = fullTypeName.LastIndexOf('.');

				string? typeNamespace = lastDotIndex >= 0 ? fullTypeName[..lastDotIndex] : null;
				string typeName = lastDotIndex >= 0 ? fullTypeName[(lastDotIndex + 1)..] : fullTypeName;

				foreach (var ns in availableNamespaces) {
					if (!HasNamespace(ns))
						continue;

					foreach (var resolvedType in resolvedTypes) {
						// If a namespace was provided
						if ((typeNamespace is null || resolvedType.declaringNamespace == typeNamespace || resolvedType.declaringNamespace.EndsWith(typeNamespace)) && resolvedType.type == typeName) {
							// If the definition already exists, the resolving failed
							if (definition is not null) {
								definition = null;
								return TypeResolveAttempt.Fail_MultipleInOneAssembly(fullTypeName, resolvedType.assembly.FullName);
							}

							definition = resolvedType.definition;
						}
					}
				}

				if (definition is null)
					return TypeResolveAttempt.Fail_NotFound(fullTypeName);

				return TypeResolveAttempt.Success();
			}
		}

		private readonly Dictionary<string, int> _assemblyNameToIndex = [];
		private readonly List<ResolvedAssembly> _referencedAssemblies = [];

		private readonly Dictionary<string, string> _userDefinedTypeAliases = [];
		private readonly List<string> _availableNamespaces = [];

		public string ActiveNamespaceScope { get; private set; }

		private ISourceFileInfoProvider _sourceProvider;

		public TypeResolver(ISourceFileInfoProvider sourceProvider) {
			_sourceProvider = sourceProvider;

			AddAssembly(ChipsCompiler.DotNetAssemblyDefinition);
			AddAssembly(ChipsCompiler.buildingAssembly);
		}

		public void SetSourceProvider(ISourceFileInfoProvider sourceProvider) => _sourceProvider = sourceProvider;

		public TypeResolverSnapshot GetSnapshot() => new TypeResolverSnapshot([.. _availableNamespaces], ActiveNamespaceScope, _userDefinedTypeAliases);

		public void RestoreSnapshot(TypeResolverSnapshot snapshot) {
			_availableNamespaces.Clear();
			_availableNamespaces.AddRange(snapshot.availableNamespaces);

			ActiveNamespaceScope = snapshot.activeNamespace;

			_userDefinedTypeAliases.Clear();
			foreach (var (alias, type) in snapshot.EnumerateAliases())
				_userDefinedTypeAliases.Add(alias, type);
		}

		public bool AddAssembly(AssemblyDefinition? assembly) {
			ArgumentNullException.ThrowIfNull(assembly);
			if (assembly.Name is null) {
				throw new ArgumentException("Assembly must have a fully quantifiable name", nameof(assembly));
			}

			if (_assemblyNameToIndex.ContainsKey(assembly.Name))
				return false;

			int index = _referencedAssemblies.Count;
			_assemblyNameToIndex.Add(assembly.Name, index);
			_referencedAssemblies.Add(new ResolvedAssembly(assembly));

			return true;
		}

		public int FindAssemblyIndex(AssemblyDefinition? assembly) {
			ArgumentNullException.ThrowIfNull(assembly);
			if (assembly.FullName is null)
				throw new ArgumentException("Assembly must have a fully quantifiable name", nameof(assembly));

			return FindAssemblyIndex(assembly.Name);
		}

		public int FindAssemblyIndex(string? assemblyName) {
			ArgumentNullException.ThrowIfNull(assemblyName);

			return _assemblyNameToIndex[assemblyName];
		}

		public AssemblyDefinition? GetAssembly(int index) => index < 0 || index >= _referencedAssemblies.Count ? null : _referencedAssemblies[index].assembly;

		public bool AddTypeAlias(string alias, string fullTypeName, bool muteErrors = false) {
			if (_userDefinedTypeAliases.ContainsKey(alias)) {
				if (!muteErrors)
					ChipsCompiler.Results.Error(_sourceProvider.SourceFile, _sourceProvider.LineNumber, ErrorID.TypeAliasAlreadyDefined, alias);

				return false;
			}

			_userDefinedTypeAliases.Add(alias, fullTypeName);
			return true;
		}

		public bool AddNamespaceImport(string ns, bool muteErrors = false) {
			if (_availableNamespaces.Contains(ns)) {
				if (!muteErrors)
					ChipsCompiler.Results.Error(_sourceProvider.SourceFile, _sourceProvider.LineNumber, ErrorID.NamespaceAlreadyImported, ns);

				return false;
			}

			_availableNamespaces.Add(ns);
			return true;
		}

		public void EnterNamespaceScope(string ns) {
			ActiveNamespaceScope = ns;
		}

		public void Clear(bool clearAssemblies = false) {
			if (clearAssemblies) {
				_referencedAssemblies.Clear();
				_assemblyNameToIndex.Clear();

				// Always ensure that a reference to the .NET lib and the compiling assembly are used
				AddAssembly(ChipsCompiler.DotNetAssemblyDefinition);
				AddAssembly(ChipsCompiler.buildingAssembly);
			}

			_userDefinedTypeAliases.Clear();
			_availableNamespaces.Clear();
		}

		public bool Resolve(string fullTypeName, [NotNullWhen(true)] out AssemblyDefinition? sourceAssembly, [NotNullWhen(true)] out TypeDefinition? type, bool muteErrors = false) {
			sourceAssembly = null;
			type = null;

			if (_referencedAssemblies.Count == 0) {
				if (!muteErrors)
					TypeResolveAttempt.Fail_NoSources(fullTypeName).CheckError(_sourceProvider);

				return false;
			}

			// Resolve any aliases
			fullTypeName = ResolveTypeAlias(fullTypeName);

			if (AttemptCoreTypeResolve(fullTypeName, out sourceAssembly, out type))
				return true;

			foreach (var assembly in _referencedAssemblies) {
				if (!CheckAssembly(fullTypeName, assembly, ref sourceAssembly, ref type, muteErrors)) {
					sourceAssembly = null;
					type = null;
					return false;
				}
			}

			if (type is null || sourceAssembly is null) {
				// Type could not be resolved at all
				if (!muteErrors)
					TypeResolveAttempt.Fail_NotFound(fullTypeName).CheckError(_sourceProvider);

				sourceAssembly = null;
				type = null;
				return false;
			}

			// Type was resolved successfully
			return true;
		}

		public bool Resolve(string fullTypeName, int assemblyIndex, [NotNullWhen(true)] out TypeDefinition? type, bool muteErrors = false) {
			// Resolve any aliases
			fullTypeName = ResolveTypeAlias(fullTypeName);

			if (AttemptCoreTypeResolve(fullTypeName, out _, out type))
				return true;

			AssemblyDefinition? sourceAssembly = null;
			if (!CheckAssembly(fullTypeName, _referencedAssemblies[assemblyIndex], ref sourceAssembly, ref type, muteErrors)) {
				type = null;
				return false;
			}

			if (type is null || sourceAssembly is null) {
				// Type could not be resolved at all
				if (!muteErrors) {
					TypeResolveAttempt.Fail_NotFound(fullTypeName).CheckError(_sourceProvider);
				}

				sourceAssembly = null;
				type = null;
				return false;
			}

			// Type was resolved successfully
			return true;
		}

		private bool CheckAssembly(string fullTypeName, ResolvedAssembly assembly, ref AssemblyDefinition? sourceAssembly, ref TypeDefinition? type, bool muteErrors) {
			var attempt = assembly.TryResolveType(fullTypeName, out TypeDefinition? typeDef, _availableNamespaces.Prepend(ActiveNamespaceScope));

			// Type wasn't in the assembly; just ignore it
			if (attempt.result == TypeResolveResult.Fail_NotFound)
				return true;

			switch (attempt.result) {
				case TypeResolveResult.Success:
					if (type is not null) {
						if (!muteErrors)
							TypeResolveAttempt.Fail_MultipleInMultipleAssemblies(fullTypeName, sourceAssembly!.FullName, assembly.assembly.FullName).CheckError(_sourceProvider);

						sourceAssembly = null;
						type = null;
						return false;
					}

					sourceAssembly = assembly.assembly;
					type = typeDef;
					break;
				case TypeResolveResult.Fail_NotFound:
					// Ignore the resolve result
					return true;
				case TypeResolveResult.Fail_NoPossibleSources:
					// Type could not be resolved in any assembly
					attempt.CheckError(_sourceProvider);
					sourceAssembly = null;
					type = null;
					return false;
				case TypeResolveResult.Fail_MultipleInOneAssembly:
					// Forward the fail and then return
					attempt.CheckError(_sourceProvider);
					sourceAssembly = null;
					type = null;
					return false;
			}

			return true;
		}

		private bool AttemptCoreTypeResolve(string fullTypeName, [NotNullWhen(true)] out AssemblyDefinition? sourceAssembly, [NotNullWhen(true)] out TypeDefinition? type) {
			var factory = ChipsCompiler.ManifestModule.CorLibTypeFactory;

			var signature = fullTypeName switch {
				"System.SByte" => factory.SByte,
				"System.Byte" => factory.Byte,
				"System.Int16" => factory.Int16,
				"System.UInt16" => factory.UInt16,
				"System.Int32" => factory.Int32,
				"System.UInt32" => factory.UInt32,
				"System.Int64" => factory.Int64,
				"System.UInt64" => factory.UInt64,
				"System.IntPtr" => factory.IntPtr,
				"System.UIntPtr" => factory.UIntPtr,
				"System.Single" => factory.Single,
				"System.Double" => factory.Double,
				"System.Boolean" => factory.Boolean,
				"System.Char" => factory.Char,
				"System.String" => factory.String,
				"System.Object" => factory.Object,
				"System.Void" => factory.Void,
				_ => null
			};

			if (signature is not null) {
				type = signature.ImportWith(ChipsCompiler.ManifestModule.DefaultImporter).Resolve()!;
				sourceAssembly = ChipsCompiler.DotNetAssemblyDefinition;
				return true;
			}

			// Attempt to resolve other types that don't have predefined signatures
			type = fullTypeName switch {
				"System.Decimal" => _referencedAssemblies[0].resolvedTypes.First(static t => t.FullName == "System.Decimal").definition,
				"System.Half" => _referencedAssemblies[0].resolvedTypes.First(static t => t.FullName == "System.Half").definition,
				_ => null,
			};

			// Type null?  Return false
			if (type is null) {
				sourceAssembly = null;
				return false;
			}

			sourceAssembly = ChipsCompiler.DotNetAssemblyDefinition;
			return true;
		}

		public string ResolveTypeAlias(string alias) {
			return alias switch {
				"sbyte" => "System.SByte",
				"byte" => "System.Byte",
				"short" => "System.Int16",
				"ushort" => "System.UInt16",
				"int" => "System.Int32",
				"uint" => "System.UInt32",
				"long" => "System.Int64",
				"ulong" => "System.UInt64",
				"nint" => "System.IntPtr",
				"nuint" => "System.UIntPtr",
				"float" => "System.Single",
				"double" => "System.Double",
				"decimal" => "System.Decimal",
				"bool" => "System.Boolean",
				"char" => "System.Char",
				"string" => "System.String",
				"object" => "System.Object",
				"void" => "System.Void",
				_ => _userDefinedTypeAliases.TryGetValue(alias, out string? realType) ? ResolveTypeAlias(realType) : alias,
			};
		}
	}

	public enum TypeResolveResult : byte {
		Success,
		Fail_NotFound,
		Fail_NoPossibleSources,
		Fail_MultipleInOneAssembly,
		Fail_MultipleInMultipleAssemblies
	}

	public sealed class TypeResolveAttempt {
		public readonly TypeResolveResult result;
		public readonly ErrorID error;
		private readonly object[] _arguments;

		private TypeResolveAttempt(TypeResolveResult result, ErrorID error, params object[] arguments) {
			this.result = result;
			this.error = error;
			_arguments = arguments;
		}

		public static TypeResolveAttempt Success() => new(TypeResolveResult.Success, default);

		public static TypeResolveAttempt Fail_NoSources(string fullTypeName) => new(TypeResolveResult.Fail_NoPossibleSources, ErrorID.CannotResolveType_NoAssemblies, fullTypeName);

		public static TypeResolveAttempt Fail_NotFound(string fullTypeName) => new(TypeResolveResult.Fail_NotFound, ErrorID.CannotResolveType_NotFound, fullTypeName);

		public static TypeResolveAttempt Fail_MultipleInOneAssembly(string fullTypeName, string assemblyName) => new(TypeResolveResult.Fail_MultipleInOneAssembly, ErrorID.CannotResolveType_Ambiguous, fullTypeName, assemblyName);

		public static TypeResolveAttempt Fail_MultipleInMultipleAssemblies(string fullTypeName, string assemblyName1, string assemblyName2) => new(TypeResolveResult.Fail_MultipleInMultipleAssemblies, ErrorID.CannotResolveType_Ambiguous_MultipleAssemblies, fullTypeName, assemblyName1, assemblyName2);

		public void CheckError(ISourceFileInfoProvider sourceProvider) {
			if (result == TypeResolveResult.Success)
				return;

			ChipsCompiler.Results.Error(sourceProvider.SourceFile, sourceProvider.LineNumber, error, _arguments);
		}
	}

	public readonly struct TypeResolverSnapshot {
		public readonly string[] availableNamespaces;
		public readonly string activeNamespace;
		private readonly Dictionary<string, string> _typeAliases;

		private readonly bool _isValidInstance;
		public bool IsValid => _isValidInstance;

		internal TypeResolverSnapshot(string[] availableNamespaces, string activeNamespace, Dictionary<string, string> typeAliases) {
			this.availableNamespaces = availableNamespaces;
			this.activeNamespace = activeNamespace;
			_typeAliases = new Dictionary<string, string>(typeAliases);
			_isValidInstance = true;
		}

		public IEnumerable<KeyValuePair<string, string>> EnumerateAliases() {
			foreach (var (alias, type) in _typeAliases)
				yield return new KeyValuePair<string, string>(alias, type);
		}
	}
}
