namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a tile that displays a getting-started link.
/// </summary>
public sealed partial class GettingStartedTile : UserControl
{
	/// <summary>
	/// Defines with the dependency property that binds with the property <see cref="Title"/>.
	/// </summary>
	/// <seealso cref="Title"/>
	public static readonly DependencyProperty TitleProperty =
		DependencyProperty.Register(nameof(Title), typeof(string), typeof(GettingStartedTile), new(null));

	/// <summary>
	/// Defines with the dependency property that binds with the property <see cref="Source"/>.
	/// </summary>
	/// <seealso cref="Source"/>
	public static readonly DependencyProperty SourceProperty =
		DependencyProperty.Register(nameof(Source), typeof(string), typeof(GettingStartedTile), new(null));

	/// <summary>
	/// Defines with the dependency property that binds with the property <see cref="Link"/>.
	/// </summary>
	/// <seealso cref="Link"/>
	public static readonly DependencyProperty LinkProperty =
		DependencyProperty.Register(nameof(Link), typeof(string), typeof(GettingStartedTile), new(null));

	/// <summary>
	/// Indicates the extracted read-only value that is used for the property <see cref="UIElement.CenterPoint"/>.
	/// </summary>
	/// <seealso cref="UIElement.CenterPoint"/>
	private static readonly Vector3 CenterPointValue = new(70, 40, 1);


	/// <summary>
	/// Indicates the compositor.
	/// </summary>
	private readonly Compositor _compositor = CompositionTarget.GetCompositorForCurrentThread();

	/// <summary>
	/// Indicates the spring animation.
	/// </summary>
	private SpringVector3NaturalMotionAnimation _springAnimation = null!;


	/// <summary>
	/// Initializes a <see cref="GettingStartedTile"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public GettingStartedTile() => InitializeComponent();


	/// <summary>
	/// Indicates the title to describe the link.
	/// </summary>
	public string Title
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(TitleProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(TitleProperty, value);
	}

	/// <summary>
	/// Indicates the file path that points to the image.
	/// </summary>
	public string Source
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(SourceProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(SourceProperty, value);
	}

	/// <summary>
	/// Indicates the link that navigates to.
	/// </summary>
	public string Link
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (string)GetValue(LinkProperty);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetValue(LinkProperty, value);
	}


	/// <summary>
	/// To create or update the spring animation, updating the field <see cref="_springAnimation"/>.
	/// </summary>
	/// <param name="finalValue">The final value.</param>
	/// <seealso cref="_springAnimation"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void CreateOrUpdateSpringAnimation(float finalValue)
	{
		if ((_springAnimation, _compositor) is (null, not null))
		{
			_springAnimation = _compositor.CreateSpringVector3Animation();
			_springAnimation.Target = nameof(Scale);
		}

		_springAnimation!.FinalValue = new(finalValue);
	}


	/// <summary>
	/// Triggers when the cursor is entered into the control.
	/// </summary>
	/// <param name="sender">The object having triggered this event.</param>
	/// <param name="e">The event arguments provided.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Element_PointerEntered(object sender, PointerRoutedEventArgs e)
	{
		if (sender is not UIElement uiElement)
		{
			return;
		}

		CreateOrUpdateSpringAnimation(1.1F);
		uiElement.CenterPoint = CenterPointValue;
		uiElement.StartAnimation(_springAnimation);
	}

	/// <summary>
	/// Triggers when the cursor is entered into the control.
	/// </summary>
	/// <param name="sender">The object having triggered this event.</param>
	/// <param name="e">The event arguments provided.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Element_PointerExited(object sender, PointerRoutedEventArgs e)
	{
		if (sender is not UIElement uiElement)
		{
			return;
		}

		CreateOrUpdateSpringAnimation(1);
		uiElement.CenterPoint = CenterPointValue;
		uiElement.StartAnimation(_springAnimation);
	}
}
