namespace Sudoku.UI.DataTypes;

/// <summary>
/// Defines a base type that is the notification object.
/// </summary>
public abstract class NotificationObject : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// To raise the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <param name="propertyName">The name of the property whose value has been changed.</param>
	/// <seealso cref="PropertyChanged"/>
	protected void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
}
