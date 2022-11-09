namespace Sudoku.Maui.ViewModels;

/// <summary>
/// Defines a base view model type.
/// </summary>
internal abstract class ViewModelBase : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// The internal method to trigger event <see cref="PropertyChanged"/>.
	/// That event is commonly used for binding, especially two-way binding.
	/// </summary>
	/// <param name="propertyName">
	/// <para>
	/// The property name that triggers the event <see cref="PropertyChanged"/>.
	/// This parameter is an optional one, which means you can pass a value if necessary
	/// or use the default value without passing.
	/// </para>
	/// <para>
	/// By default, argument <paramref name="propertyName"/> will hold the caller property name,
	/// although the default value is an empty string <c>""</c>.
	/// The real value to be passed will be replaced with the property name, by compiler, automatically.
	/// </para>
	/// </param>
	/// <seealso cref="PropertyChanged"/>
	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "") => PropertyChanged?.Invoke(this, new(propertyName));
}
