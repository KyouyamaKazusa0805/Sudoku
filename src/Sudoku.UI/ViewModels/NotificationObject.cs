using System.ComponentModel;

namespace Sudoku.UI.ViewModels;

/// <summary>
/// Defines a base object type that holds and handles the action on notifications.
/// </summary>
public abstract class NotificationObject : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// The internal method that will raise the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <param name="propertyName">
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp5/feature[@name="caller-member-name"]/target[@name="argument"]' />
	/// </param>
	/// <seealso cref="PropertyChanged"/>
	/// <seealso cref="CallerMemberNameAttribute"/>
	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
		PropertyChanged?.Invoke(this, new(propertyName));

	/// <summary>
	/// Try to set the property with the new value, and calls <see cref="OnPropertyChanged(string?)"/>
	/// to raise the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <typeparam name="TNotNull">The type of the property.</typeparam>
	/// <param name="originalValue">The original value of the property.</param>
	/// <param name="newValue">The new value to set.</param>
	/// <param name="propertyName">
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp5/feature[@name="caller-member-name"]/target[@name="argument"]' />
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the assignment is successful.
	/// If the argument <paramref name="originalValue"/> is same as <paramref name="newValue"/>,
	/// the return value will be <see langword="false"/>; otherwise, <see langword="true"/>.
	/// </returns>
	/// <seealso cref="OnPropertyChanged(string?)"/>
	/// <seealso cref="PropertyChanged"/>
	protected bool SetProperty<TNotNull>(
		ref TNotNull? originalValue, in TNotNull? newValue, [CallerMemberName] string? propertyName = null)
		where TNotNull : notnull
	{
		// Compare two values.
		// If two values are same, the assignment won't be successful. Therefore, return false.
		if (Equals(originalValue, newValue))
		{
			return false;
		}

		// Assign the value.
		originalValue = newValue;

		// Raise the event, and return true as the successful operation.
		OnPropertyChanged(propertyName);
		return true;
	}
}
