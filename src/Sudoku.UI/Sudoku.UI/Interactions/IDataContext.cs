namespace Sudoku.UI.Interactions;

/// <summary>
/// Defines a basic data context type that can be used in the <see cref="UserControl"/> and <see cref="Page"/>
/// as its <c>DataContext</c> property value.
/// </summary>
/// <typeparam name="TSelf">The type of the view model itself.</typeparam>
/// <seealso cref="UserControl"/>
/// <seealso cref="Page"/>
[RequiresPreviewFeatures]
public interface IDataContext<TSelf> where TSelf : IDataContext<TSelf>, new()
{
	/// <summary>
	/// Creates a new instance of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <returns>The instance.</returns>
	static abstract TSelf CreateInstance();
}
