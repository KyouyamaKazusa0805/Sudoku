namespace Sudoku.Runtime.MeasuringServices;

/// <summary>
/// Represents a factor that describes a rule for calculating difficulty rating for a step in one factor.
/// </summary>
public abstract class Factor
{
	/// <summary>
	/// Default property flags, including properties that are explicitly interface implemented.
	/// </summary>
	private const BindingFlags PropertyFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;


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
	/// <exception cref="AmbiguousMatchException">Throws when property names is invalid.</exception>
	/// <seealso cref="ParameterNames"/>
	/// <seealso cref="PropertyInfo"/>
	public ReadOnlySpan<PropertyInfo> Parameters
	{
		get
		{
			var result = (
				from propertyInfo in ReflectedStepType.GetProperties(PropertyFlags)
				let indexOfMatch = Array.FindIndex(ParameterNames, name => compareString(propertyInfo.Name, name))
				where indexOfMatch != -1
				orderby indexOfMatch
				select propertyInfo
			).ToArray();
			return result.Length == ParameterNames.Length ? result : throw new AmbiguousMatchException();


			// Here a property may be explicitly implemented, the name may starts with interface name.
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool compareString(string a, string b) => a == b || a.Contains('.') && a[(a.LastIndexOf('.') + 1)..] == b;
		}
	}

	/// <summary>
	/// Provides with a formula that calculates for the result, unscaled.
	/// </summary>
	public abstract Func<ReadOnlySpan<object?>, int> Formula { get; }


	/// <summary>
	/// Try to fetch the name stored in resource dictionary.
	/// </summary>
	/// <param name="formatProvider">The culture information.</param>
	/// <returns>The name of the factor.</returns>
	public string GetName(IFormatProvider? formatProvider) => SR.Get($"Factor_{GetType().Name}", formatProvider as CultureInfo);
}
