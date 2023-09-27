using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Chips.Compiler.IO.Project {
	internal class SourceResolver {
		private class DirectoryNode {
			public readonly string name;
			public DirectoryNode? parent;
			public readonly List<DirectoryNode> children = new();
			public readonly List<FileNode> files = new();

			public DirectoryInfo info;

			public bool included = true;

			// Nesting level relative to the resolver's root directory
			private int absoluteNestingLevel;

			public DirectoryNode(string name, DirectoryNode? parent) {
				this.name = name;
				this.parent = parent;
			}

			public DirectoryNode? GetChild(string name) {
				foreach (DirectoryNode child in children) {
					if (child.name == name)
						return child;
				}

				return null;
			}

			public DirectoryNode GetOrAddChild(string name) {
				if (string.IsNullOrWhiteSpace(name))
					throw new ArgumentException("Directory name cannot be null, empty or whitespace", nameof(name));

				if (name == ".")
					return this;

				if (name == "..") {
					if (parent is not null)
						return parent;

					// Build the parent directory and assign it as the parent for this directory
					DirectoryInfo parentInfo = info.Parent
						?? throw new DirectoryNotFoundException($"Could not find parent directory of \"{info.FullName}\"");

					parent = new(parentInfo.Name, null) { info = parentInfo };
					parent.absoluteNestingLevel = absoluteNestingLevel + 1;

					return parent;
				}

				DirectoryNode? child = GetChild(name);

				if (child is null) {
					child = new(name, this) {
						info = info.GetDirectories().FirstOrDefault(d => d.Name == name)
							?? throw new DirectoryNotFoundException($"Could not find directory \"{name}\" in \"{info.FullName}\"")
					};
					children.Add(child);
				}

				return child;
			}

			public DirectoryNode GetOrAddChild(DirectoryInfo info) {
				DirectoryNode? child = GetChild(info.Name);

				if (child is null) {
					child = new(info.Name, this) { info = info };
					children.Add(child);
				}

				return child;
			}

			public FileNode? GetFile(string file) {
				foreach (FileNode node in files) {
					if (node.name == file)
						return node;
				}

				return null;
			}

			public FileNode GetOrAddFile(string file) {
				FileNode? node = GetFile(file);

				if (node is null) {
					node = new(file, this) {
						info = info.GetFiles().FirstOrDefault(f => f.Name == file)
							?? throw new FileNotFoundException($"Could not find file \"{file}\" in \"{info.FullName}\"")
					};

					if (Path.GetExtension(node.info.FullName) != ".chp")
						throw new ArgumentException($"File \"{node.GetPath()}\" was not a Chips source file");

					files.Add(node);
				}

				return node;
			}

			public FileNode GetOrAddFile(FileInfo info) {
				FileNode? node = GetFile(info.Name);

				if (node is null) {
					node = new(info.Name, this) { info = info };

					if (Path.GetExtension(info.FullName) != ".chp")
						throw new ArgumentException($"File \"{node.GetPath()}\" was not a Chips source file");

					files.Add(node);
				}

				return node;
			}


			public string GetPath() {
				if (parent is not null)
					return Path.Combine(parent.GetPath(), name);

				string pathPrefix = string.Empty;
				if (absoluteNestingLevel > 0) {
					StringBuilder sb = new();
					for (int i = 0; i < absoluteNestingLevel; i++) {
						sb.Append("..");
						sb.Append(Path.DirectorySeparatorChar);
					}
					pathPrefix = sb.ToString();
				}

				return pathPrefix + name;
			}

			public IEnumerable<ProjectSource> EnumerateIncludedFiles() {
				if (!included)
					yield break;

				foreach (FileNode file in files) {
					if (file.included)
						yield return file.GetProjectSource();
				}

				foreach (DirectoryNode child in children) {
					if (child.included) {
						foreach (ProjectSource file in child.EnumerateIncludedFiles())
							yield return file;
					}
				}
			}
		}

		private class FileNode {
			public readonly string name;
			public readonly DirectoryNode parent;

			public FileInfo info;

			public bool included = true;

			public FileNode(string name, DirectoryNode parent) {
				this.name = name;
				this.parent = parent;
			}

			public string GetPath() {
				return Path.Combine(parent.GetPath(), name);
			}

			public ProjectSource GetProjectSource() {
				return new(name, parent.info.FullName, info);
			}
		}

		private readonly DirectoryNode root;

		public SourceResolver(string rootDirectory) {
			root = new DirectoryNode(rootDirectory, null) { info = new DirectoryInfo(rootDirectory) };
		}

		public void AddFiles(string searchPattern, bool include) {
			// If the search pattern is only "*", then include all files in the current directory and subdirectories
			if (searchPattern == "*") {
				Stack<DirectoryNode> directoryQueue = new();
				directoryQueue.Push(root);

				while (directoryQueue.TryPop(out DirectoryNode? current)) {
					// Add all files in the current directory
					StepDirectory(searchPattern, include, current);

					// Push all subdirectories onto the queue in reverse order so that the first subdirectory is processed first
					foreach (DirectoryInfo dir in current.info.GetDirectories().Reverse())
						directoryQueue.Push(current.GetOrAddChild(dir));
				}
			} else {
				// Start at the current directory and process the search pattern
				StepDirectory(searchPattern, include, root);
			}
		}

		private void StepDirectory(ReadOnlySpan<char> searchPattern, bool include, DirectoryNode current) {
			int separatorIndex = searchPattern.IndexOf(Path.DirectorySeparatorChar);
			if (separatorIndex < 0) {
				// The directory path is now a file specifier.  Add the files in the directory
				foreach (FileInfo file in current.info.GetFiles(searchPattern.ToString())) {
					// Ignore files without the ".chp" extension
					if (Path.GetExtension(file.Name) != ".chp")
						continue;

					FileNode fileNode = current.GetOrAddFile(file);
					fileNode.included = include;
				}

				return;
			}

			// Get the directory name
			ReadOnlySpan<char> directoryName = searchPattern[..separatorIndex];

			// Get the next directory
			DirectoryNode next = current.GetOrAddChild(directoryName.ToString());
			next.included = include;

			// Step into the next directory
			StepDirectory(searchPattern[(separatorIndex + 1)..], include, next);
		}

		public IEnumerable<ProjectSource> EnumerateFiles() => root.EnumerateIncludedFiles();
	}

	public readonly struct ProjectSource {
		public readonly string file;
		public readonly string directory;
		public readonly FileInfo fileInfo;

		public ProjectSource(string file, string directory, FileInfo fileInfo) {
			this.file = file;
			this.directory = directory;
			this.fileInfo = fileInfo;
		}
	}
}
