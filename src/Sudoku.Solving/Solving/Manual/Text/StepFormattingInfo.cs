namespace Sudoku.Solving.Manual.Text;

/// <summary>
/// Provides with a tuple that holds the formatting information that is used for formatting a technique step.
/// </summary>
/// <typeparam name="TStep">Indicates the real type of the <see cref="Step"/> instance.</typeparam>
[AutoEquality(nameof(Lcid), nameof(_innerType))]
[AutoGetHashCode(nameof(Lcid), nameof(_innerType))]
public sealed partial class StepFormattingInfo<TStep> : IEquatable<StepFormattingInfo<TStep>> where TStep : Step
{
	/// <summary>
	/// Indicates the result property values used.
	/// </summary>
	private readonly object?[] _propertyValues;

	/// <summary>
	/// Indicates the inner <see cref="Type"/> instance that binds with the <typeparamref name="TStep"/>.
	/// </summary>
	private readonly Type _innerType;


	/// <summary>
	/// Initializes an instance via those arguments.
	/// </summary>
	/// <exception cref="TypeInitializationException">
	/// Throws when at least one property name in <paramref name="propertyNames"/>
	/// can't be found in the instance <paramref name="step"/>.
	/// The exception type is <see cref="TypeInitializationException"/>,
	/// whose inner exception is <see cref="FormatException"/>.
	/// </exception>
	public StepFormattingInfo(int lcid, string format, string[] propertyNames, TStep step)
	{
		Lcid = lcid;
		Format = format;
		Step = step;
		PropertyNames = propertyNames;
		_innerType = typeof(TStep);
		_propertyValues = f(step, propertyNames);


		static object?[] f(TStep step, string[] propertyNames)
		{
			var type = typeof(TStep);
			string? invalidPropertyName = null;
			object?[] propertyValues = new object?[propertyNames.Length];
			for (int i = 0; i < propertyNames.Length; i++)
			{
				string propertyName = propertyNames[i];
				if (type.GetProperty(propertyName) is { } propertyInfo)
				{
					propertyValues[i] = propertyInfo.GetValue(step);
				}
				else
				{
					invalidPropertyName = propertyName;
					break;
				}
			}
			if (invalidPropertyName is not null)
			{
				throw new TypeInitializationException(
					typeof(StepFormattingInfo<TStep>).FullName,
					new FormatException($"The given property name is invalid: {invalidPropertyName}.")
				);
			}

			return propertyValues;
		}
	}


	/// <summary>
	/// Indicates the current LCID used.
	/// </summary>
	public int Lcid { get; }

	/// <summary>
	/// Indicates the format string.
	/// </summary>
	public string Format { get; }

	/// <summary>
	/// Indicates the display name for the localized culture.
	/// </summary>
	public string LocalizedCultureName => new CultureInfo(Lcid).Name;

	/// <summary>
	/// Indicates the property names used.
	/// </summary>
	public string[] PropertyNames { get; }

	/// <summary>
	/// Indicates the step instance used.
	/// </summary>
	public TStep Step { get; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => string.Format(Format, _propertyValues);
}
