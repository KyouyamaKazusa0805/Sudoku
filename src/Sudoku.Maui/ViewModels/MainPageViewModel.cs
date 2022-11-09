namespace Sudoku.Maui.ViewModels;

/// <summary>
/// Defines a view model that handles the interaction logic for <see cref="MainPage"/>.
/// </summary>
/// <seealso cref="MainPage"/>
internal sealed class MainPageViewModel : ViewModelBase
{
	/// <summary>
	/// The button text.
	/// </summary>
	private string _buttonText;

	/// <summary>
	/// The backing field of property <see cref="ClickedCount"/>.
	/// </summary>
	/// <seealso cref="ClickedCount"/>
	private int _clickedCount;


	/// <summary>
	/// Initializes a <see cref="MainPageViewModel"/> instance.
	/// </summary>
	public MainPageViewModel()
	{
		ClickedCount = 0;
		ButtonCommand = new Command(() => ClickedCount++);
	}


	/// <summary>
	/// The button text.
	/// </summary>
	public string ButtonText
	{
		get => _buttonText;

		[MemberNotNull(nameof(_buttonText))]
		private set
		{
			_buttonText = value;

			OnPropertyChanged();
		}
	}

	/// <summary>
	/// Indicates the number of times that the button has been clicked.
	/// </summary>
	public int ClickedCount
	{
		get => _clickedCount;

		[MemberNotNull(nameof(_buttonText))]
		set
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value));
			}

			_clickedCount = value;
			ButtonText = value switch { 0 => "Click me!", 1 => $"Clicked {value} time", _ => $"Clicked {value} times" };

			OnPropertyChanged();
		}
	}

	/// <summary>
	/// The button command.
	/// </summary>
	public ICommand ButtonCommand { get; }
}
