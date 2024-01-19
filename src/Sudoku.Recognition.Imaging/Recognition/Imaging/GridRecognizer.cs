namespace Sudoku.Recognition.Imaging;

/// <summary>
/// Provides a grid field recognizer. If you want to know what is a <b>field</b>,
/// please see the 'remark' part of <see cref="InternalServiceProvider"/>.
/// </summary>
/// <param name="photo">Indicates the photo to be assigned.</param>
/// <seealso cref="InternalServiceProvider"/>
internal sealed class GridRecognizer(Bitmap photo) : IDisposable
{
	/// <summary>
	/// Indicates the L2Gradient.
	/// </summary>
	private const bool L2Gradient = true;

	/// <summary>
	/// Indicates the maximum size.
	/// </summary>
	private const int MaxSize = 2000;

	/// <summary>
	/// Indicates the R-size.
	/// </summary>
	private const int RSize = 360;

	/// <summary>
	/// Indicates the minimum threshold.
	/// </summary>
	private const int ThresholdMin = 50;

	/// <summary>
	/// Indicates the maximum threshold.
	/// </summary>
	private const int ThresholdMax = 100;

	/// <summary>
	/// Indicates the font size.
	/// </summary>
	private const int FontSize = 3;

	/// <summary>
	/// Indicates the font size pr.
	/// </summary>
	private const int FontSizePr = 1;

	/// <summary>
	/// Indicates the font face.
	/// </summary>
	private const FontFace Font = FontFace.HersheyPlain;

	/// <summary>
	/// Indicates the behavior that specifies and executes the chain approximation algorithm.
	/// </summary>
	private const ChainApproxMethod ChainApproximation = ChainApproxMethod.ChainApproxNone;


	/// <summary>
	/// The image.
	/// </summary>
	private Image<Bgr, byte> _image = photo.CorrectOrientation().ToImage<Bgr, byte>();


	/// <inheritdoc/>
	public void Dispose() => _image.Dispose();

	/// <summary>
	/// Recognize.
	/// </summary>
	/// <returns>The result.</returns>
	public Image<Bgr, byte> Recognize()
	{
		using var edges = PrepareImage();
		return CutField(FindField(edges));
	}

	/// <summary>
	/// Find the field.
	/// </summary>
	/// <param name="edges">The edges.</param>
	/// <returns>The points.</returns>
	private PointF[] FindField(UMat edges)
	{
		var maxRectArea = 0D;
		var biggestRectangle = new PointF[4];
		using var contours = new VectorOfVectorOfPoint();

		// Finding contours and choosing needed.
		CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApproximation);

		for (var i = 0; i < contours.Size; i++)
		{
			if (contours[i].Size < 4)
			{
				continue;
			}

			var shape = GetFourCornerPoints(contours[i].ToArray());
			if (shape.IsRectangle())
			{
				var (width, height) = CvInvoke.MinAreaRect(shape).Size;
				var area = width * height;
				if (area > maxRectArea)
				{
					maxRectArea = area;
					biggestRectangle = shape;
				}
			}
		}

		return biggestRectangle;
	}

	/// <summary>
	/// Getting four corner points from contour points.
	/// </summary>
	/// <param name="points">The points.</param>
	/// <returns>The points.</returns>
	private PointF[] GetFourCornerPoints(Point[] points)
	{
		// Order:
		// 1--2
		// |  |
		// 3--4

		var (corners, maxSum, maxDiff, minSum, minDiff) = (new PointF[4], 0, 0, -1, 0);
		foreach (var point in points)
		{
			var sum = point.X + point.Y;
			var diff = point.X - point.Y;

			// Get bottom-right point.
			if (sum > maxSum)
			{
				corners[3] = point;
				maxSum = sum;
			}

			// Get top-left point.
			if (sum < minSum || minSum == -1)
			{
				corners[0] = point;
				minSum = sum;
			}

			// Get top-right point.
			if (diff > maxDiff)
			{
				corners[1] = point;
				maxDiff = diff;
			}

			// Get bottom-left point.
			if (diff < minDiff)
			{
				corners[2] = point;
				minDiff = diff;
			}
		}

		return corners;
	}

	/// <summary>
	/// Prepare the image.
	/// </summary>
	/// <returns>The <see cref="UMat"/> instance.</returns>
	private UMat PrepareImage()
	{
		// Resize image.
		if (_image.Width > MaxSize && _image.Height > MaxSize)
		{
			_image = _image.Resize(MaxSize, MaxSize * _image.Width / _image.Height, Inter.Linear, true);
		}

		// Convert the image to gray-scale and filter out the noise.
		using var uImage = new UMat();
		CvInvoke.CvtColor(_image, uImage, ColorConversion.Bgr2Gray);

		// Use image pyramid to remove noise.
		using var pyrDown = new UMat();
		CvInvoke.PyrDown(uImage, pyrDown);
		CvInvoke.PyrUp(pyrDown, uImage);

		var cannyEdges = new UMat();

		CvInvoke.Canny(uImage, cannyEdges, ThresholdMin, ThresholdMax, l2Gradient: L2Gradient);

		return cannyEdges;
	}

	/// <summary>
	/// To cut the field.
	/// </summary>
	/// <param name="field">The field.</param>
	/// <returns>The image.</returns>
	private Image<Bgr, byte> CutField(PointF[] field)
	{
		// Size for output image, recommendation: multiples of 9 and 6.
		var resultField = new Image<Bgr, byte>(RSize, RSize);

		// Transformation sudoku field to rectangle size and aligning the sides.
		CvInvoke.WarpPerspective(
			_image,
			resultField,
			CvInvoke.GetPerspectiveTransform(field, [new(0, 0), new(RSize, 0), new(0, RSize), new(RSize, RSize)]),
			new(RSize, RSize)
		);

		return resultField;
	}
}
