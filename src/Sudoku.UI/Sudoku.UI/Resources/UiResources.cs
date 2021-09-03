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
}
