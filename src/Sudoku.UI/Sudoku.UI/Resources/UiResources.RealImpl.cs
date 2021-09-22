using System.Text.RegularExpressions;

namespace Sudoku.UI.Resources;

/// <summary>
/// Provides the methods that handles with resource dictionary.
/// </summary>
internal sealed class UiResources : DynamicObject
{
	/// <summary>
	/// Gets the default handler that interacts with resource dictionary.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This is a <see langword="dynamic"/> instance, which means you can get anything you want
	/// using the following code style to get the items in this class:
	/// <list type="bullet">
	/// <item><c>Current.PropertyName</c> (Property invokes)</item>
	/// <item><c>Current[PropertyIndex]</c> (Indexer invokes)</item>
	/// </list>
	/// </para>
	/// <para>
	/// For example,
	/// if you want to get the <see cref="string"/> value from the key <c>"Bug"</c>, now you may
	/// write <c>Current.Bug</c> or <c>Current["Bug"]</c> to get that value.
	/// </para>
	/// </remarks>
	public static readonly dynamic Current = new UiResources();

	/// <summary>
	/// Indicates the regular expression that matches a dictionary file name (a <see cref="Uri"/> string).
	/// </summary>
	private static readonly Regex DictionaryNameRegex = new(
		@"Dic_\d{4,5}\.xaml",
		RegexOptions.IgnoreCase,
		TimeSpan.FromSeconds(5)
	);


	/// <summary>
	/// Indicates the current UI project uses the light mode or the dark mode.
	/// </summary>
	public static ApplicationTheme LightOrDarkMode => Application.Current.RequestedTheme;

	/// <summary>
	/// A simplified way to get the resource dictonaries.
	/// </summary>
	public static IEnumerable<ResourceDictionary> Dictionaries =>
		from mergedDictionary in Application.Current.Resources.MergedDictionaries
		where DictionaryNameRegex.IsMatch(mergedDictionary.Source.ToString())
		select mergedDictionary;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool TryGetMember(GetMemberBinder binder, out dynamic? result) =>
		Application.Current.Resources.TryGetValue(binder.Name, out result);
	
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
	{
		if (indexes is not { Length: 1 } || indexes[0] is not string index)
		{
			result = null;
			return false;
		}

		return Application.Current.Resources.TryGetValue(index, out result);
	}
}
