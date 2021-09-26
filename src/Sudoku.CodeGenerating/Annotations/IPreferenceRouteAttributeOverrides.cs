namespace Sudoku.CodeGenerating;

/// <summary>
/// Defines a serial of members that requires the attribute <see cref="PreferenceRouteAttribute"/>
/// and its derived types to implement.
/// </summary>
/// <typeparam name="TSelf">
/// The type of the derived attriute type from <see cref="PreferenceRouteAttribute"/>.
/// </typeparam>
public interface IPreferenceRouteAttributeOverrides<in TSelf>
where TSelf : PreferenceRouteAttribute, IPreferenceRouteAttributeOverrides<TSelf>
{
	/// <summary>
	/// Indicates the bound control name.
	/// </summary>
	string ControlName { get; }

	/// <summary>
	/// Indicates the effect control name.
	/// </summary>
	string EffectControlName { get; }

	/// <summary>
	/// Indicates the method name that executes the code, to assign the result.
	/// </summary>
#if NETSTANDARD2_1_OR_GREATER
	[DisallowNull]
#endif
	string? PreferenceSetterMethodName { get; init; }
}
