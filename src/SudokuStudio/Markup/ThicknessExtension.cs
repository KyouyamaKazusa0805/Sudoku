namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that can create a new <see cref="Thickness"/> instance via base one.
/// </summary>
[ContentProperty(Name = nameof(Base))]
[MarkupExtensionReturnType(ReturnType = typeof(Thickness))]
public sealed class ThicknessExtension : MarkupExtension
{
	/// <summary>
	/// Indicates the base <see cref="Thickness"/> instance.
	/// </summary>
	public Thickness Base { get; set; }

	/// <inheritdoc cref="Thickness.Top"/>
	public double Top { get; set; } = -1;

	/// <inheritdoc cref="Thickness.Bottom"/>
	public double Bottom { get; set; } = -1;

	/// <inheritdoc cref="Thickness.Left"/>
	public double Left { get; set; } = -1;

	/// <inheritdoc cref="Thickness.Right"/>
	public double Right { get; set; } = -1;


	/// <inheritdoc/>
	protected override object ProvideValue()
		=> Base with
		{
			Top = Top < 0 ? Base.Top : Top,
			Bottom = Bottom < 0 ? Base.Bottom : Bottom,
			Left = Left < 0 ? Base.Left : Left,
			Right = Right < 0 ? Base.Right : Right
		};
}
