using Resource = System.Resources.ResourceManager;

namespace Sudoku.Resources;

/// <summary>
/// Defines a merged resource dictionary.
/// </summary>
internal sealed class MergedResources
{
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
	/// The default manager. If the key cannot be found in the list <see cref="_managers"/>,
	/// this field will be looked up.
	/// </summary>
	private readonly Resource _default;

	/// <summary>
	/// The inner list.
	/// </summary>
	private readonly IDictionary<int, Resource> _managers;

	/// <summary>
	/// Indicates the current LCID.
	/// </summary>
	private int _lcid = 1033;


	/// <summary>
	/// Initializes a <see cref="MergedResources"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private MergedResources()
	{
		_default = Resources1033.ResourceManager;

		_managers = new Dictionary<int, Resource>
		{
			{ 2052, Resources2052.ResourceManager }
		};
	}


	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public string? this[string key]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _lcid != 1033 && _managers[_lcid].GetString(key) is { } r1 ? r1 : _default.GetString(key);
	}


	/// <summary>
	/// Change the language to the specified one that is represented as the language LCID value.
	/// </summary>
	/// <param name="lcid">The LCID value. The default LCID for the resource dictionary is 1033.</param>
	/// <exception cref="ArgumentException">
	/// Throws when the specified LCID cannot be found in the current resource dictionary.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ChangeLanguage(int lcid) =>
		_lcid = lcid != 1033 && _managers.ContainsKey(lcid)
			? lcid
			: throw new ArgumentException("Such LCID already exists in the current manager.", nameof(lcid));

	/// <summary>
	/// Emits a string value represented as the specified punctuation mark.
	/// </summary>
	/// <param name="punctuation">The punctuation mark.</param>
	/// <returns>The string that represents the punctuation mark.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string EmitPunctuation(Punctuation punctuation) => R[punctuation.ToString()]!;
}
