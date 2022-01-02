namespace Nano.ViewModels;

/// <summary>
/// Indicates the view model that binds with <see cref="DemoPage"/>.
/// </summary>
/// <seealso cref="DemoPage"/>
public sealed class DemoPageViewModel : NotificationObject
{
	/// <summary>
	/// Initializes a <see cref="DemoPageViewModel"/> instance.
	/// </summary>
	public DemoPageViewModel() => AddCommand = new() { CanExecuteCommand = null, ExecuteCommand = Add };


	/// <summary>
	/// The back field for the property <see cref="First"/>.
	/// </summary>
	/// <seealso cref="First"/>
	private double _first;

	/// <summary>
	/// The back field for the property <see cref="Second"/>.
	/// </summary>
	/// <seealso cref="Second"/>
	private double _second;

	/// <summary>
	/// The back field for the property <see cref="Result"/>.
	/// </summary>
	/// <seealso cref="Result"/>
	private double _result;


	/// <summary>
	/// Indicates the first number.
	/// </summary>
	public double First
	{
		get => _first;

		set
		{
			_first = value;
			RaiseNotification(nameof(First));
		}
	}

	/// <summary>
	/// Indicates the second number.
	/// </summary>
	public double Second
	{
		get => _second;

		set
		{
			_second = value;
			RaiseNotification(nameof(Second));
		}
	}

	/// <summary>
	/// Indicates the result number.
	/// </summary>
	public double Result
	{
		get => _result;

		set
		{
			_result = value;
			RaiseNotification(nameof(Result));
		}
	}

	/// <summary>
	/// Indicates the command that handles the adding operation.
	/// </summary>
	public DelegateCommand AddCommand { get; set; }


	/// <summary>
	/// The back method binds with the property <see cref="AddCommand"/>.
	/// </summary>
	/// <param name="parameter"></param>
	/// <seealso cref="AddCommand"/>
	private void Add([IsDiscard] object? parameter) => Result = First + Second;
}
