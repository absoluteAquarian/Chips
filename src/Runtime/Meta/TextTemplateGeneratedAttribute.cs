using System;

namespace Chips.Runtime.Meta {
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	internal class TextTemplateGeneratedAttribute : Attribute { }
}
