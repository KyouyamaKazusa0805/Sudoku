namespace Nano.ViewModels;

/// <summary>
/// Indicates the view model that binds with <see cref="DemoPage"/>.
/// </summary>
/// <seealso cref="DemoPage"/>
public sealed partial class DemoPageViewModel : NotificationObject
{
	/// <summary>
	/// Initializes a <see cref="DemoPageViewModel"/> instance.
	/// </summary>
	public DemoPageViewModel() => AddCommand = new() { CanExecuteCommand = null, ExecuteCommand = Add };


	[PropertyAutoNotify]
	private double _first;

	[PropertyAutoNotify]
	private double _second;

	[PropertyAutoNotify]
	private double _result;


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
