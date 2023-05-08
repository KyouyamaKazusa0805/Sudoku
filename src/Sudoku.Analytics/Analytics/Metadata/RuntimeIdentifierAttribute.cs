namespace Sudoku.Analytics.Metadata;

/// <summary>
/// Represents an attribute that can be applied to a property in a <see cref="StepSearcher"/>,
/// indicating the runtime identifier. This property will be used for checking and replacing values in runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed partial class RuntimeIdentifierAttribute([PrimaryConstructorParameter] string runtimeIdentifier) : Attribute;
