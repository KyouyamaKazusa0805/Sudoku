namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents an integer box.
/// </summary>
public sealed class IntegerBox : NumberBox
{
	/// <summary>
	/// Initializes an <see cref="IntegerBox"/>.
	/// </summary>
	public IntegerBox() => DefaultStyleKey = typeof(NumberBox);


	/// <inheritdoc cref="NumberBox.Value"/>
	public new int Value
	{
		get => (int)(double)GetValue(ValueProperty);

		set => SetValue(ValueProperty, (double)value);
	}

	/// <inheritdoc cref="NumberBox.Minimum"/>
	public new int Minimum
	{
		get => (int)(double)GetValue(MinimumProperty);

		set => SetValue(MinimumProperty, (double)value);
	}

	/// <inheritdoc cref="NumberBox.Maximum"/>
	public new int Maximum
	{
		get => (int)(double)GetValue(MaximumProperty);

		set => SetValue(MaximumProperty, (double)value);
	}

	/// <inheritdoc cref="NumberBox.SmallChange"/>
	public new int SmallChange
	{
		get => (int)(double)GetValue(SmallChangeProperty);

		set => SetValue(SmallChangeProperty, (double)value);
	}

	/// <inheritdoc cref="NumberBox.LargeChange"/>
	public new int LargeChange
	{
		get => (int)(double)GetValue(LargeChangeProperty);

		set => SetValue(LargeChangeProperty, (double)value);
	}
}
