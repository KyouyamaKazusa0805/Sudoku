#if SUDOKU_RECOGNIZING
using System;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Sudoku.Drawing.Extensions;
using static Sudoku.InternalSettings;

namespace Sudoku.Recognizations
{
	/// <summary>
	/// Provides a grid recognizer.
	/// </summary>
	public sealed class GridRecognizer
	{
		/// <summary>
		/// Initializes an instance with the specified photo.
		/// </summary>
		/// <param name="photo">The photo.</param>
		public GridRecognizer(Bitmap photo)
		{
			photo.CorectOrientation();
			Image = photo.ToImage<Bgr, byte>();
		}


		/// <summary>
		/// The image.
		/// </summary>
		public Image<Bgr, byte> Image { get; private set; }


		/// <summary>
		/// Recognize.
		/// </summary>
		/// <returns>The result.</returns>
		public Image<Bgr, byte> Recognize() => CutField(FindField(PrepareImage()));

		/// <summary>
		/// Prepare the image.
		/// </summary>
		/// <returns>The <see cref="UMat"/> instance.</returns>
		public UMat PrepareImage()
		{
			// Resize image.
			if (Image.Width > MaxSize && Image.Height > MaxSize)
			{
				Image = Image.Resize(
					MaxSize, MaxSize * Image.Width / Image.Height, Inter.Linear, true);
			}

			// Convert the image to gray-scale and filter out the noise.
			var uimage = new UMat();
			CvInvoke.CvtColor(Image, uimage, ColorConversion.Bgr2Gray);

			// Use image pyramid to remove noise.
			var pyrDown = new UMat();
			CvInvoke.PyrDown(uimage, pyrDown);
			CvInvoke.PyrUp(pyrDown, uimage);

			var cannyEdges = new UMat();
			CvInvoke.Canny(uimage, cannyEdges, ThresholdMin, ThresholdMax, l2Gradient: L2Gradient);

			// Another methods to process image, but worse. Use only one!
			//CvInvoke.Threshold(uimage, cannyEdges, 50.0, 100.0, ThresholdType.Binary);
			//CvInvoke.AdaptiveThreshold(uimage, cannyEdges, 50, AdaptiveThresholdType.MeanC, ThresholdType.Binary, 7, 1);

			return cannyEdges;
		}

		/// <summary>
		/// Find the field.
		/// </summary>
		/// <param name="edges">The edges.</param>
		/// <returns>The points.</returns>
		public PointF[] FindField(UMat edges)
		{
			double maxRectArea = 0;
			var biggestRectangle = new PointF[4];
			using var contours = new VectorOfVectorOfPoint();

			// Finding contours and choosing needed
			CvInvoke.FindContours(edges, contours, null, RetrType.List, ChainApprox);

			for (int i = 0; i < contours.Size; i++)
			{
				if (contours[i].Size < 4)
				{
					continue;
				}

				var shape = GetFourCornerPoints(contours[i].ToArray());
				if (IsRectangle(shape))
				{
					var rect = CvInvoke.MinAreaRect(shape);
					float area = rect.Size.Height * rect.Size.Width;

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
		/// To cut the field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns>The image.</returns>
		public Image<Bgr, byte> CutField(PointF[] field)
		{
			// Size for output image, recommendation: multiples of 9 and 6.
			var resultField = new Image<Bgr, byte>(RSize, RSize);

			// Transformation sudoku field to rectangle size and aligning the sides
			CvInvoke.WarpPerspective(
				Image,
				resultField,
				CvInvoke.GetPerspectiveTransform(
					field,
					new[]
					{
						new PointF(0, 0), new PointF(RSize, 0),
						new PointF(0, RSize), new PointF(RSize, RSize)
					}),
				new Size(RSize, RSize));

			return resultField;
		}

		/// <summary>
		/// Getting four corner points from contour points.
		/// </summary>
		/// <param name="points">The points.</param>
		/// <returns>The points.</returns>
		private PointF[] GetFourCornerPoints(Point[] points)
		{
			// 1--2
			// |  |
			// 3--4

			var corners = new PointF[4];

			int maxSum = 0, maxDiff = 0;
			int minSum = -1, minDiff = 0;
			foreach (var point in points)
			{
				var sum = point.X + point.Y;
				var diff = point.X - point.Y;

				// get right-bottom point
				if (sum > maxSum)
				{
					corners[3] = point;
					maxSum = sum;
				}

				// get left-top point
				if (sum < minSum || minSum == -1)
				{
					corners[0] = point;
					minSum = sum;
				}

				// get right-top point
				if (diff > maxDiff)
				{
					corners[1] = point;
					maxDiff = diff;
				}

				// get left-bottom point
				if (diff < minDiff)
				{
					corners[2] = point;
					minDiff = diff;
				}
			}

			return corners;
		}

		/// <summary>
		/// Get true if contour is rectangle with angles within [lowAngle, upAngle] degree.
		/// Default: [75, 105].
		/// </summary>
		/// <param name="contour">The contour.</param>
		/// <param name="lowAngle">The lower angle. The default value is 75.</param>
		/// <param name="upAngle">The upper angle. The default value is 105.</param>
		/// <param name="ratio">The ratio. The default value is .35</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		private bool IsRectangle(PointF[] contour, int lowAngle = 75, int upAngle = 105, double ratio = .35)
		{
			if (contour.Length > 4)
			{
				return false;
			}

			var sides = new[]
			{
				new LineSegment2DF(contour[0], contour[1]),
				new LineSegment2DF(contour[1], contour[3]),
				new LineSegment2DF(contour[2], contour[3]),
				new LineSegment2DF(contour[0], contour[2])
			};

			// Check angles between common sides.
			for (int j = 0; j < 4; j++)
			{
				double angle = Math.Abs(sides[(j + 1) % sides.Length].GetExteriorAngleDegree(sides[j]));
				if (angle < lowAngle || angle > upAngle)
				{
					return false;
				}
			}

			// Check ratio between all sides, it cannot be more than allowed.
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (sides[i].Length / sides[j].Length < ratio || sides[i].Length / sides[j].Length > (1 + ratio))
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
#endif