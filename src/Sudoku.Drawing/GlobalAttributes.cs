[assembly: SupportedOSPlatform("windows")]
[assembly: AutoExtensionDeconstruction(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]
[assembly: AutoExtensionDeconstruction(typeof(Point), nameof(Point.X), nameof(Point.Y))]
[assembly: AutoExtensionDeconstruction(typeof(PointF), nameof(PointF.X), nameof(PointF.Y))]
[assembly: AutoExtensionDeconstruction(typeof(Size), nameof(Size.Width), nameof(Size.Height))]
[assembly: AutoExtensionDeconstruction(typeof(SizeF), nameof(SizeF.Width), nameof(SizeF.Height))]
[assembly: AutoExtensionDeconstruction(
	typeof(RectangleF), nameof(RectangleF.X), nameof(RectangleF.Y),
	nameof(RectangleF.Width), nameof(RectangleF.Height))]