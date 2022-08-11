namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that displays for a digit that is the cell-leveled digit.
/// </summary>
#if DEBUG
[DebuggerDisplay($$"""{{{nameof(DebuggerDisplayView)}},nq}""")]
#endif
internal sealed class CellDigit : DrawingElement
{
	/// <summary>
	/// Indicates whether displaying mask ellipse.
	/// </summary>
	private bool _isMaskMode;

	/// <summary>
	/// Indicates the cell status.
	/// </summary>
	private bool? _isGiven;

	/// <summary>
	/// The digit.
	/// </summary>
	private byte _digit;

	/// <summary>
	/// The inner text block.
	/// </summary>
	private readonly TextBlock? _textBlock;

	/// <summary>
	/// The mask ellipse.
	/// </summary>
	private readonly Ellipse? _maskEllipse;

	/// <summary>
	/// The user preference.
	/// </summary>
	private readonly IDrawingPreference _preference = null!;


	/// <summary>
	/// Indicates whether the current cell is the given cell.
	/// </summary>
	/// <returns>
	/// The return value is <see cref="bool"/>?, which means the return value contains three possible cases:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The cell is the given.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The cell is modifiable.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The cell is modifiable, but the value is wrong.</description>
	/// </item>
	/// </list>
	/// </returns>
	public required bool? IsGiven
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isGiven;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			(_isGiven, _textBlock.Foreground) = (value, new SolidColorBrush(GetColor(value, _preference)));
		}
	}

	/// <summary>
	/// Indicates whether the digit is displayed at present.
	/// </summary>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <see langword="value"/> holds <see langword="true"/> value.
	/// </exception>
	public bool ShowDigit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Digit != 255;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => Digit = value ? throw new ArgumentException("The value must be false.", nameof(value)) : byte.MaxValue;
	}

	/// <summary>
	/// Indicates whether displaying mask ellipse.
	/// </summary>
	public required bool IsMaskMode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _isMaskMode;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (_isMaskMode == value)
			{
				return;
			}

			if (_isMaskMode && _isGiven is null)
			{
				// If the current cell is an empty cell, we should skip the mask operation.
				return;
			}

			ArgumentNullException.ThrowIfNull(_maskEllipse);
			ArgumentNullException.ThrowIfNull(_textBlock);

			_isMaskMode = value;
			SetMaskEllipseVisibility(value && _isGiven is true);
			SetTextBlockVisibility(!value);
		}
	}

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public required byte Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			return _digit;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_digit = value;

			ArgumentNullException.ThrowIfNull(_textBlock);

			_textBlock.Text = value == byte.MaxValue ? string.Empty : (value + 1).ToString();
		}
	}

	/// <summary>
	/// The font size.
	/// </summary>
	public double FontSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			return _textBlock.FontSize;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			_textBlock.FontSize = value;
		}
	}

	/// <summary>
	/// The font name.
	/// </summary>
	public string FontName
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ValueFont.FontName;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			_preference.ValueFont = _preference.ValueFont with { FontName = value };
			_textBlock.FontFamily = new(value);
		}
	}

	/// <summary>
	/// The given color.
	/// </summary>
	public Color GivenColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.GivenColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			_preference.GivenColor = value;
			if (_isGiven is true)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

	/// <summary>
	/// The modifiable color.
	/// </summary>
	public Color ModifiableColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.ModifiableColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			_preference.ModifiableColor = value;
			if (_isGiven is false)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

	/// <summary>
	/// Indicates the cell delta color.
	/// </summary>
	public Color CellDeltaColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference.CellDeltaColor;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			ArgumentNullException.ThrowIfNull(_textBlock);

			_preference.CellDeltaColor = value;
			if (_isGiven is null)
			{
				_textBlock.Foreground = new SolidColorBrush(value);
			}
		}
	}

	/// <summary>
	/// Indicates the user preference.
	/// </summary>
	public required IDrawingPreference UserPreference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _preference;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[MemberNotNull(nameof(_textBlock), nameof(_maskEllipse))]
		init
		{
			_preference = value;

			_textBlock ??= new TextBlock()
				.WithText(_digit == byte.MaxValue ? string.Empty : (_digit + 1).ToString())
				.WithFontSize(value.RenderingCellSize * value.ValueFont.FontScale)
				.WithFontFamily(value.ValueFont.FontName)
				.WithTextAlignment(TextAlignment.Center)
				.WithHorizontalTextAlignment(TextAlignment.Center)
				.WithHorizontalAlignment(HorizontalAlignment.Center)
				.WithVerticalAlignment(VerticalAlignment.Center)
				.WithForeground(GetColor(_isGiven, value))
				.WithOpacity(1)
				.WithOpacityTransition(TimeSpan.FromMilliseconds(500));
			_maskEllipse ??= new Ellipse()
				.WithFill(value.MaskEllipseColor)
				.WithWidth(value.RenderingCellSize * value.ValueFont.FontScale)
				.WithHeight(value.RenderingCellSize * value.ValueFont.FontScale)
				.WithHorizontalAlignment(HorizontalAlignment.Center)
				.WithVerticalAlignment(VerticalAlignment.Center)
				.WithOpacity(_isMaskMode ? 1 : 0)
				.WithOpacityTransition(TimeSpan.FromMilliseconds(500));
		}
	}

	/// <inheritdoc/>
	protected override string TypeIdentifier => nameof(CellDigit);

#if DEBUG
	/// <summary>
	/// Defines the debugger view.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	private string DebuggerDisplayView
	{
		get
		{
			string status = (_isGiven, _isMaskMode) switch
			{
				(_, true) => "Masked",
				(true, _) => "Given",
				(false, _) => "Modifiable",
				_ => "Empty"
			};
			string digit = (_isMaskMode, _textBlock) switch
			{
				(true, _) => "<Masked>",
				(false, { Text: var s and not "" }) => s,
				(_, null) => "<_textBlock: Null>",
				_ => "<Empty>"
			};

			return $$"""{{nameof(CellDigit)}} { Status = {{status}}, Digit = {{digit}} }""";
		}
	}
#endif


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals([NotNullWhen(true)] DrawingElement? other)
		=> other is CellDigit comparer && _isGiven == comparer._isGiven && Digit == comparer.Digit
		&& _isMaskMode == comparer._isMaskMode;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(TypeIdentifier, _isGiven, Digit);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override TextBlock GetControl()
		=> _textBlock ?? throw new InvalidOperationException("The value cannot be null.");

	/// <summary>
	/// Gets the control of the mask ellipse.
	/// </summary>
	/// <returns>The mask ellipse control.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Ellipse GetMaskEllipseControl()
		=> _maskEllipse ?? throw new InvalidOperationException("The value cannot be null.");

	/// <summary>
	/// Sets the specified visibility.
	/// </summary>
	/// <param name="isVisible">The visibility.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetTextBlockVisibility(bool isVisible)
	{
		if (_textBlock is null)
		{
			return;
		}

		_textBlock.Opacity = isVisible ? 1 : 0;
	}

	/// <summary>
	/// Sets the specified visibility.
	/// </summary>
	/// <param name="isVisible">The visibility.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetMaskEllipseVisibility(bool isVisible)
	{
		if (_maskEllipse is null)
		{
			return;
		}

		_maskEllipse.Opacity = isVisible ? 1 : 0;
	}


	/// <summary>
	/// Gets the color of the status.
	/// </summary>
	/// <param name="status">The status.</param>
	/// <param name="preference">The preference as the source.</param>
	/// <returns>The <see cref="Color"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Color GetColor(bool? status, IDrawingPreference preference)
		=> status switch
		{
			true => preference.GivenColor,
			false => preference.ModifiableColor,
			_ => preference.CellDeltaColor
		};
}
