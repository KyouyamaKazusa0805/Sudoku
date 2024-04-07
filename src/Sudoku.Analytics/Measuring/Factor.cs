namespace Sudoku.Measuring;

/// <summary>
/// Represents a factor that describes a rule for calculating difficulty rating for a step in one factor.
/// </summary>
/// <param name="options">Indiates the options instance to be used for passing diffculty rating scale value.</param>
public abstract class Factor(StepSearcherOptions options) : ICultureFormattable
{
	/// <summary>
	/// Indicates the scale value to be set. The value will be used by scaling formula. By default, the value will be 0.1.
	/// </summary>
	public decimal Scale { get; } = options.DifficultyRatingScale;

	/// <summary>
	/// Try to get the scale unit length from the <see cref="Scale"/> value.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the scale value is negative.</exception>
	/// <seealso cref="Scale"/>
	public int ScaleUnitLength
		=> Scale switch
		{
			< 0 => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("ScaleValueCannotBeNegative")),
			0 => 0, // Special case: return 0 if scale is 0.
			_ when Scale.ToString() is var str && str.IndexOf('.') is var posOfPeriod => posOfPeriod switch
			{
				-1 => 0, // This is an integer (no period token is found). Return 0.
				_ => str.Length - posOfPeriod - 1
			}
		};

	/// <summary>
	/// Indicates the name of the factor that can be used by telling with multple <see cref="Factor"/>
	/// instances with different types.
	/// </summary>
	public string DistinctKey => GetType().Name;

	/// <summary>
	/// Represents the string representation of formula.
	/// </summary>
	/// <remarks>
	/// The text can contain placeholders such like <c>{0}</c>, <c>{1}</c>, <c>{2}</c> and so on. The values will be replaced
	/// with the fact parameter name from property <see cref="Parameters"/> in runtime.
	/// </remarks>
	/// <seealso cref="Parameters"/>
	[StringSyntax(StringSyntaxAttribute.CompositeFormat)]
	public abstract string FormulaString { get; }

	/// <summary>
	/// Try to get the scale unit from the <see cref="Scale"/> value.
	/// </summary>
	/// <seealso cref="Scale"/>
	public string? ScaleUnitFormatString => ScaleUnitLength == 0 ? null : $"#.{new('0', ScaleUnitLength)}";

	/// <summary>
	/// Indicates a list of <see cref="string"/> instances that binds with real instance properties
	/// stored inside a <see cref="Step"/> instance, representing the target step type is compatible
	/// with the current factor and can be calculated its rating.
	/// </summary>
	public abstract string[] ParameterNames { get; }

	/// <summary>
	/// Indicates the relied <see cref="Type"/> instance.
	/// </summary>
	public abstract Type ReflectedStepType { get; }

	/// <summary>
	/// Indicates a <see cref="PropertyInfo"/> instance that creates from property <see cref="ParameterNames"/>.
	/// </summary>
	/// <seealso cref="ParameterNames"/>
	/// <seealso cref="PropertyInfo"/>
	public PropertyInfo[] Parameters => from parameterName in ParameterNames select ReflectedStepType.GetProperty(parameterName)!;

	/// <summary>
	/// Provides with a formula that calculates for the result, unscaled.
	/// </summary>
	public abstract Func<Step, int?> Formula { get; }


	/// <summary>
	/// Try to fetch the name stored in resource dictionary.
	/// </summary>
	/// <param name="culture">The culture.</param>
	/// <returns>The name of the factor.</returns>
	public string GetName(CultureInfo? culture = null) => ResourceDictionary.Get($"Factor_{DistinctKey}", culture);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => ToString(null);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CultureInfo? culture = null) => $"{GetName(culture)}:{Environment.NewLine}{FormulaString}";

	/// <summary>
	/// Calculates the rating from the specified <see cref="Step"/> instance, and return the string representation
	/// of the rating text.
	/// </summary>
	/// <param name="step">The step to be calculated.</param>
	/// <param name="culture">The culture to be used.</param>
	/// <returns>The string representation of final rating text.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(Step step, CultureInfo? culture = null)
		=> Formula(step) switch
		{
			{ } result => $"{GetName(culture)}: {(result * Scale).ToString(ScaleUnitFormatString)}",
			_ => string.Empty // Failed to calculate because the invalid step data.
		};
}
