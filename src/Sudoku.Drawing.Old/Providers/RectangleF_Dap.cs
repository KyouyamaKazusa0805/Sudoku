namespace System.Drawing.Providers;

internal sealed class RectangleF_Dap
{
	[DeconstructArgumentProvider]
	internal static PointF Point(RectangleF @this) => new(@this.X, @this.Y);

	[DeconstructArgumentProvider]
	internal static SizeF Size(RectangleF @this) => new(@this.Size);
}
