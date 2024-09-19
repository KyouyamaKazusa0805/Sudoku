namespace Sudoku.Measuring;

using Formula = Func<ReadOnlySpan<object?>, int>;

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
			var propertyInfoDictionary = new Dictionary<Type, PropertyInfo[]>();
			for (var type = ReflectedStepType; type?.IsAssignableTo(typeof(Step)) ?? false; type = type.BaseType)
			{
				propertyInfoDictionary.Add(type, type.GetProperties(PropertyFlags));
			}

			var matchPropertyInfoList = new List<PropertyInfo>();
			foreach (var parameterName in ParameterNames)
			{
				var found = false;
				foreach (var propertyInfoList in propertyInfoDictionary.Values)
				{
					switch (Array.FindAll(propertyInfoList, p => nameMatcher(p.Name, parameterName)))
					{
						case [var match]:
						{
							matchPropertyInfoList.Add(match);
							found = true;
							goto NextMatch;
						}
						case [var firstMatch, .. { Length: not 0 }] matches:
						{
							// If multiple values matched, we should select the best one.
							// The best-match property is a property without any prefixes
							// that can only produced in explicitly interface implementation.
							matchPropertyInfoList.Add(
								Array.FindIndex(matches, static match => !match.Name.Contains('.')) is var index and not -1
									? matches[index]
									: firstMatch // The arbitary one in matched set will be selected.
							);
							found = true;
							goto NextMatch;
						}
					}
				}

			NextMatch:
				if (!found)
				{
					throw new InvalidOperationException();
				}
			}

			return matchPropertyInfoList.ToArray();


			// Here a property may be explicitly implemented, the name may starts with interface name.
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool nameMatcher(string a, string b) => a == b || a.Contains('.') && a[(a.LastIndexOf('.') + 1)..] == b;
		}
	}

	/// <summary>
	/// Provides with a formula that calculates for the result.
	/// </summary>
	public abstract Formula Formula { get; }

	/// <summary>
	/// Indicates the factor resource key.
	/// </summary>
	protected virtual string FactorResourceKey => $"Factor_{GetType().Name}";


	/// <summary>
	/// Try to fetch the name stored in resource dictionary.
	/// </summary>
	/// <param name="formatProvider">The culture information.</param>
	/// <returns>The name of the factor.</returns>
	public string GetName(IFormatProvider? formatProvider) => SR.Get(FactorResourceKey, formatProvider as CultureInfo);


	/// <summary>
	/// Creates a <see cref="Factor"/> instance that assigns predefined values.
	/// </summary>
	/// <param name="resourceKey"><inheritdoc cref="FactorResourceKey" path="/summary"/></param>
	/// <param name="parameterNames"><inheritdoc cref="ParameterNames" path="/summary"/></param>
	/// <param name="reflectedStepType"><inheritdoc cref="ReflectedStepType" path="/summary"/></param>
	/// <param name="formula"><inheritdoc cref="Formula" path="/summary"/></param>
	/// <returns>A <see cref="Factor"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Factor Create(string resourceKey, string[] parameterNames, Type reflectedStepType, Formula formula)
		=> new AnonymousFactor(resourceKey, parameterNames, reflectedStepType, formula);
}

/// <summary>
/// Defines an anonymous factor.
/// </summary>
/// <param name="resourceKey"><inheritdoc cref="FactorResourceKey" path="/summary"/></param>
/// <param name="parameterNames"><inheritdoc cref="ParameterNames" path="/summary"/></param>
/// <param name="reflectedStepType"><inheritdoc cref="ReflectedStepType" path="/summary"/></param>
/// <param name="formula"><inheritdoc cref="Formula" path="/summary"/></param>
file sealed class AnonymousFactor(string resourceKey, string[] parameterNames, Type reflectedStepType, Formula formula) : Factor
{
	/// <inheritdoc/>
	public override string[] ParameterNames => parameterNames;

	/// <inheritdoc/>
	public override Formula Formula => formula;

	/// <inheritdoc/>
	public override Type ReflectedStepType => reflectedStepType;

	/// <inheritdoc/>
	protected override string FactorResourceKey => resourceKey;
}
