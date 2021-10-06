namespace System.Drawing;

internal sealed class RectangleFDeconstructionArgumentProvider
{
	private RectangleFDeconstructionArgumentProvider() { }


	[DeconstructArgumentProvider]
	internal static PointF Point(RectangleF @this) => new(@this.X, @this.Y);

	[DeconstructArgumentProvider]
	internal static SizeF Size(RectangleF @this) => new(@this.Size);
}
