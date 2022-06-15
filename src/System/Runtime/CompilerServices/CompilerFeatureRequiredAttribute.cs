#pragma warning disable CS1591

namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.All)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
	public CompilerFeatureRequiredAttribute(string featureName) => FeatureName = featureName;


	public string FeatureName { get; }

	public bool IsOptional { get; set; }
}
