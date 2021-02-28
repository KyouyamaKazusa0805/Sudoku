#if SUDOKU_RECOGNITION

using System;
using System.Drawing;
using System.Extensions;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Sudoku.Drawing.Extensions;
using Sudoku.Recognition.Extensions;
using static Sudoku.Recognition.Constants;
using Cv = Emgu.CV.CvInvoke;
using Field = Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>;

namespace Sudoku.Recognition
{
	/// <summary>
	/// Provides a grid field recognizer. If you want to know what is a <b>field</b>,
	/// please see the 'remark' part of <see cref="InternalServiceProvider"/>.
	/// </summary>
	/// <seealso cref="InternalServiceProvider"/>
	internal sealed class GridRecognizer : IDisposable
	{
		/// <summary>
		/// The image.
		/// </summary>
		private Field _image;


		/// <summary>
		/// Initializes an instance with the specified photo.
		/// </summary>
		/// <param name="photo">The photo.</param>
		public GridRecognizer(Bitmap photo)
		{
			photo.CorrectOrientation();
			_image = photo.ToImage<Bgr, byte>();
		}


		/// <inheritdoc/>
		public void Dispose() => _image.Dispose();

		/// <summary>
		/// Recognize.
		/// </summary>
		/// <returns>The result.</returns>
		public Field Recognize()
		{
			using var edges = PrepareImage();
			return CutField(FindField(edges));
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
			using var uimage = new UMat();
			Cv.CvtColor(_image, uimage, ColorConversion.Bgr2Gray);

			// Use image pyramid to remove noise.
			using var pyrDown = new UMat();
			Cv.PyrDown(uimage, pyrDown);
			Cv.PyrUp(pyrDown, uimage);

			var cannyEdges = new UMat();

			Cv.Canny(uimage, cannyEdges, ThresholdMin, ThresholdMax, l2Gradient: L2Gradient);

			return cannyEdges;
		}

		/// <summary>
		/// Find the field.
		/// </summary>
		/// <param name="edges">The edges.</param>
		/// <returns>The points.</returns>
		private PointF[] FindField(UMat edges)
		{
			double maxRectArea = 0;
			var biggestRectangle = new PointF[4];
			using var contours = new VectorOfVectorOfPoint();

			// Finding contours and choosing needed.
			Cv.FindContours(edges, contours, null, RetrType.List, ChainApprox);

			for (int i = 0; i < contours.Size; i++)
			{
				if (contours[i].Size < 4)
				{
					continue;
				}

				var shape = GetFourCornerPoints(contours[i].ToArray());
				if (shape.IsRectangle())
				{
					var (_, (width, height)) = Cv.MinAreaRect(shape);
					float area = width * height;

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
		private Field CutField(PointF[] field)
		{
			// Size for output image, recommendation: multiples of 9 and 6.
			var resultField = new Field(RSize, RSize);

			// Transformation sudoku field to rectangle size and aligning the sides.
			Cv.WarpPerspective(
				_image,
				resultField,
				Cv.GetPerspectiveTransform(
					field,
					new PointF[] { new(0, 0), new(RSize, 0), new(0, RSize), new(RSize, RSize) }),
				new(RSize, RSize));

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
			int maxSum = 0, maxDiff = 0, minSum = -1, minDiff = 0;
			foreach (var point in points)
			{
				int sum = point.X + point.Y;
				int diff = point.X - point.Y;

				// Get right-bottom point.
				if (sum > maxSum)
				{
					corners[3] = point;
					maxSum = sum;
				}

				// Get left-top point.
				if (sum < minSum || minSum == -1)
				{
					corners[0] = point;
					minSum = sum;
				}

				// Get right-top point.
				if (diff > maxDiff)
				{
					corners[1] = point;
					maxDiff = diff;
				}

				// Get left-bottom point.
				if (diff < minDiff)
				{
					corners[2] = point;
					minDiff = diff;
				}
			}

			return corners;
		}
	}
}
#endif