namespace Sudoku.Resources;

/// <summary>
/// Defines a merged resource dictionary.
/// </summary>
public sealed class MergedResources
{
	/// <summary>
	/// Indicates the default LCID used.
	/// </summary>
	private const int NeutralLanguageLcid = 1033;

	/// <summary>
	/// Indicates the default culture name used.
	/// </summary>
	private const string NeutralLanguageName = "en-US";


	/// <summary>
	/// <para>
	/// Indicates the instance that has the ability to call the resource, and distinct with different
	/// natural languages.
	/// </para>
	/// <para>
	/// If you want to check the resource, please use the indexer <see cref="this[string]"/> to get them.
	/// The return value of the indexer is <c><see cref="string"/>?</c>:
	/// <code>
	/// using static Sudoku.Resources.MergedResources;
	/// 
	/// string? resourceValue = R["SuchKeyYouWantToSearchFor"];
	/// if (resourceValue is not null)
	/// {
	///     // Insert the arbitrary code using 'resourceValue'.
	/// }
	/// </code>
	/// </para>
	/// <para>
	/// In addition, the return value of that indexer can be <see langword="null"/>
	/// if the resource doesn't find such value via the specified key.
	/// </para>
	/// </summary>
	/// <seealso cref="this[string]"/>
	public static readonly MergedResources R = new();

	/// <summary>
	/// Indicates the default language culture used in the resource file.
	/// </summary>
	private static readonly CultureInfo Neutral = CultureInfo.GetCultureInfo(NeutralLanguageLcid);


	/// <summary>
	/// Indicates the extra value selectors. The value can be <see langword="null"/> if the field
	/// of all usages on the current instance only raises in the current project.
	/// </summary>
	private ICollection<(Assembly[] SupportedAssemblies, Func<string, string?> ValueSelector)>? _valueSelectors;


	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public string? this[string key]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (Assembly.GetCallingAssembly() is var callingAssembly && callingAssembly != GetType().Assembly)
			{
				// Uses external resource routing.
				return _valueSelectors?.FirstOrDefault(predicate) is var (_, valueSelector) ? valueSelector(key) : null;


				bool predicate((Assembly[] A, Func<string, string?>) p) => Array.Exists(p.A, a => a == callingAssembly);
			}

			var @default = Resources.ResourceManager;
			return @default.GetString(key) ?? @default.GetString(key, Neutral);
		}
	}


	/// <summary>
	/// Changes the language to the specified language.
	/// </summary>
	/// <param name="lcid">
	/// The LCID of the culture, such as <c>1033</c> for <c>en-US</c> or <c>2052</c> for <c>zh-CN</c>.
	/// </param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeLanguage(int lcid)
		=> Resources.Culture = lcid == NeutralLanguageLcid ? Neutral : CultureInfo.GetCultureInfo(lcid);

	/// <summary>
	/// Changes the language to the specified language.
	/// </summary>
	/// <param name="name">The culture string, such as <c>"en-US"</c> or <c>"zh-CN"</c>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeLanguage(string name)
		=> Resources.Culture = name.Equals(NeutralLanguageName, StringComparison.OrdinalIgnoreCase)
			? Neutral
			: CultureInfo.GetCultureInfo(name);

	/// <summary>
	/// Registers a new assembly that can use and fetch resource via field <see cref="R"/>.
	/// This method is used for fetching resources from external projects whose containing types are not stored in <c>Sudoku.Core</c>.
	/// </summary>
	/// <param name="assembly">The assembly.</param>
	/// <seealso cref="R"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RegisterAssembly(Assembly assembly) => AddExternalResourceFetecher(assembly, static key => R[key]);

	/// <summary>
	/// Adds an extra value selector into the current resource fetching instance.
	/// </summary>
	/// <param name="assembly">The assembly.</param>
	/// <param name="valueSelector">The value selector.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddExternalResourceFetecher(Assembly assembly, Func<string, string?> valueSelector)
		=> AddExternalResourceFetecher(new[] { assembly }, valueSelector);

	/// <summary>
	/// Adds an extra value selector into the current resource fetching instance.
	/// </summary>
	/// <param name="assemblies">The supported assemblies.</param>
	/// <param name="valueSelector">The value selector.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void AddExternalResourceFetecher(Assembly[] assemblies, Func<string, string?> valueSelector)
		=> (_valueSelectors ??= new List<(Assembly[], Func<string, string?>)>()).Add((assemblies, valueSelector));

	/// <summary>
	/// Emits a string value represented as the specified punctuation mark.
	/// </summary>
	/// <param name="punctuation">The punctuation mark.</param>
	/// <returns>The string that represents the punctuation mark.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string EmitPunctuation(Punctuation punctuation) => R[punctuation.ToString()]!;
}
