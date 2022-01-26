namespace Sudoku.UI.ViewModels;

/// <summary>
/// Defines a base object type that holds and handles the action on notifications.
/// </summary>
public abstract class NotificationObject : INotifyPropertyChanged, INotifyPropertyChanging
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;

	/// <inheritdoc/>
	public event PropertyChangingEventHandler? PropertyChanging;


	/// <summary>
	/// To trigger the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <param name="propertyName">
	/// Indicates the property name whose corresponding property status is changed.
	/// </param>
	/// <seealso cref="PropertyChanged"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void RaisePropertyChanged(string propertyName) =>
		PropertyChanged?.Invoke(this, new(propertyName));

	/// <summary>
	/// To trigger the event <see cref="PropertyChanging"/>.
	/// </summary>
	/// <param name="propertyName">
	/// Indicates the property name whose corresponding property status is changed.
	/// </param>
	/// <seealso cref="PropertyChanging"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected void RaisePropertyChanging(string propertyName) =>
		PropertyChanging?.Invoke(this, new(propertyName));
}
