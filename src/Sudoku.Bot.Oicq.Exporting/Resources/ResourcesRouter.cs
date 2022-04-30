#nullable enable

using ResourceBackingType = Resources.Resources;

namespace Sudoku.Bot.Oicq.Resources;

/// <summary>
/// Defines a resource router that can get the specified resource via the key.
/// </summary>
internal sealed class ResourcesRouter
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
	public static readonly ResourcesRouter R = new();


	/// <summary>
	/// Gets the value via the specified string key.
	/// </summary>
	/// <param name="key">The resource key.</param>
	/// <returns>The value. If none found, <see langword="null"/>.</returns>
	public string? this[string key] => ResourceBackingType.ResourceManager.GetString(key);
}
