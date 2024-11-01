namespace System.Runtime.Versioning;

/// <summary>
/// Represents an attribute type that describes the version that begins this API.
/// </summary>
/// <param name="major"><inheritdoc cref="Version.Major" path="/summary"/></param>
/// <param name="minor"><inheritdoc cref="Version.Minor" path="/summary"/></param>
/// <param name="build"><inheritdoc cref="Version.Build" path="/summary"/></param>
/// <param name="revision"><inheritdoc cref="Version.Revision" path="/summary"/></param>
[AttributeUsage(
	AttributeTargets.Assembly | AttributeTargets.Module
		| AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface
		| AttributeTargets.Enum | AttributeTargets.Delegate
		| AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field
		| AttributeTargets.Event,
	Inherited = false)]
public sealed class IntroducedSinceAttribute(int major, int minor, int build = -1, int revision = -1) :
	VersioningAttribute(new(major, minor, build, revision));
