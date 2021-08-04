using System;
using System.Drawing;
using Emgu.CV.Structure;

namespace Sudoku.Recognition.Extensions
{
	/// <summary>
	/// Provides the extensions methods for point array <see cref="PointF"/>[].
	/// </summary>
	/// <seealso cref="PointF"/>
	public static class PointArrayEx
	{
		/// <summary>
		/// Get true if contour is rectangle with angles within <c>[lowAngle, upAngle]</c> degree.
		/// The default case is <c>[75, 105]</c> given by <paramref name="lowerAngle"/> and
		/// <paramref name="upperAngle"/>.
		/// </summary>
		/// <param name="contour">The contour.</param>
		/// <param name="lowerAngle">The lower angle. The default value is <c>75</c>.</param>
		/// <param name="upperAngle">The upper angle. The default value is <c>105</c>.</param>
		/// <param name="ratio">The ratio. The default value is <c>.35</c>.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		public static bool IsRectangle(
			this PointF[] contour, int lowerAngle = 75, int upperAngle = 105, double ratio = .35)
		{
			if (contour.Length > 4)
			{
				return false;
			}

			var sides = new LineSegment2DF[]
			{
				new(contour[0], contour[1]),
				new(contour[1], contour[3]),
				new(contour[2], contour[3]),
				new(contour[0], contour[2])
			};

			// Check angles between common sides.
			for (int j = 0; j < 4; j++)
			{
				double angle = Math.Abs(sides[(j + 1) % sides.Length].GetExteriorAngleDegree(sides[j]));
				if (angle < lowerAngle || angle > upperAngle)
				{
					return false;
				}
			}

			// Check ratio between all sides, it can't be more than allowed.
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					if (sides[i].Length / sides[j].Length < ratio || sides[i].Length / sides[j].Length > 1 + ratio)
					{
						return false;
					}
				}
			}

			return true;
		}
	}
}
