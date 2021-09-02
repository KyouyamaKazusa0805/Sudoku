namespace Sudoku.UI;

/// <summary>
/// Provides the methods that handles with resource dictionary.
/// </summary>
internal sealed class Ui : DynamicObject
{
	/// <summary>
	/// Gets the default handler that interacts with resource dictionary.
	/// </summary>
	public static readonly dynamic Current = new Ui();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool TryGetMember(GetMemberBinder binder, out dynamic? result) =>
		Application.Current.Resources.TryGetValue(binder.Name, out result);
}
