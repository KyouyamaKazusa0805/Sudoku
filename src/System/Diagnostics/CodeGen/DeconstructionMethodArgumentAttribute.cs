namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that specifies a parameter used by a deconstruction method.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class DeconstructionMethodArgumentAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DeconstructionMethodArgumentAttribute"/> instance via the member name the parameter points to.
	/// </summary>
	/// <param name="referencedMemberName">The referenced member name.</param>
	public DeconstructionMethodArgumentAttribute([SuppressMessage("Style", IDE0060, Justification = Pending)] string referencedMemberName)
	{
	}
}
