using System.Globalization;
using Sudoku.Techniques;

namespace Sudoku.Resources;

/// <summary>
/// Defines a merged resource dictionary.
/// </summary>
internal sealed class MergedResources
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
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public string? this[string key]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
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
	public void ChangeLanguage(int lcid) =>
		Resources.Culture = lcid == NeutralLanguageLcid ? Neutral : CultureInfo.GetCultureInfo(lcid);

	/// <summary>
	/// Changes the language to the specified language.
	/// </summary>
	/// <param name="name">The culture string, such as <c>"en-US"</c> or <c>"zh-CN"</c>.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeLanguage(string name) =>
		Resources.Culture = name.Equals(NeutralLanguageName, StringComparison.OrdinalIgnoreCase)
			? Neutral
			: CultureInfo.GetCultureInfo(name);

	/// <summary>
	/// Emits a string value represented as the specified punctuation mark.
	/// </summary>
	/// <param name="punctuation">The punctuation mark.</param>
	/// <returns>The string that represents the punctuation mark.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string EmitPunctuation(Punctuation punctuation) => R[punctuation.ToString()]!;

	/// <summary>
	/// Emits a string value represented as the technique name.
	/// </summary>
	/// <param name="technique">The technique name.</param>
	/// <returns>The string that represents the technique name.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string EmitTechniqueName(Technique technique) => R[technique.ToString()]!;
}
