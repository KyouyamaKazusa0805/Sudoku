namespace System.Drawing.Providers;

[PrivatizeParameterlessConstructor]
internal sealed partial class RectangleF_Dap
{
	[DeconstructArgumentProvider]
	internal static PointF Point(RectangleF @this) => new(@this.X, @this.Y);

	[DeconstructArgumentProvider]
	internal static SizeF Size(RectangleF @this) => new(@this.Size);
}
