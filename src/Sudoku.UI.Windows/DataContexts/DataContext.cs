namespace Sudoku.UI.Windows.DataContexts;

/// <summary>
/// Defines a base type for data context.
/// The type will be used to provide with inheritance on all data contexts, including the members
/// about executing and raising events on property changed.
/// </summary>
public abstract class DataContext : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// To raise the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <param name="propertyName">Indicates the name of the property that triggers the event.</param>
	/// <seealso cref="PropertyChanged"/>
	public void RaiseNotification(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));
}