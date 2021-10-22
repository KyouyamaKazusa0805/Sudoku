namespace System.Drawing.Providers;

[PrivatizeParameterlessConstructor]
internal sealed partial class RectangleF_Dap
{
	[LambdaBody]
	internal static PointF Point(RectangleF @this) => new(@this.X, @this.Y);

	[LambdaBody]
	internal static SizeF Size(RectangleF @this) => new(@this.Size);
}
