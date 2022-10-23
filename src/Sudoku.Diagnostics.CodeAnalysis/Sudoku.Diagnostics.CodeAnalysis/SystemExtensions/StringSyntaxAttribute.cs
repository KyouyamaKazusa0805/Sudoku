#pragma warning disable IDE0060

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
internal sealed class StringSyntaxAttribute : Attribute
{
	public const string Regex = nameof(Regex);

	public StringSyntaxAttribute(string _)
	{
	}

	public StringSyntaxAttribute(string _, params object?[] __)
	{
	}
}
