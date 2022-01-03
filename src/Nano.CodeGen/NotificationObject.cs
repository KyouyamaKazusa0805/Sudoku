namespace Nano.Diagnostics.CodeGen;

/// <summary>
/// Defines an abstraction on view models.
/// The type will be used to provide with inheritance on all view models, including the members
/// about executing and raising events on property changed.
/// </summary>
public abstract class NotificationObject : INotifyPropertyChanged
{
	/// <summary>
	/// Indicates the object that is only used on synchronization.
	/// </summary>
	private static readonly object SyncRoot = new();


	/// <summary>
	/// Indicates the back field for the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <seealso cref="PropertyChanged"/>
	private PropertyChangedEventHandler? _propertyChangedHandler;


	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged
	{
		add
		{
			lock (SyncRoot)
			{
				_propertyChangedHandler += value;
			}
		}

		remove
		{
			lock (SyncRoot)
			{
				_propertyChangedHandler -= value;
			}
		}
	}


	/// <summary>
	/// To raise the event <see cref="PropertyChanged"/> during the notification.
	/// </summary>
	/// <param name="propertyName">Indicates the property name.</param>
	/// <seealso cref="PropertyChanged"/>
	public void RaiseNotification(string propertyName) =>
		_propertyChangedHandler?.Invoke(this, new(propertyName));
}
