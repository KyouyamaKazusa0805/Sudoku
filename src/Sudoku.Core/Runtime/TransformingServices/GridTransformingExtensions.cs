namespace Sudoku.Runtime.TransformingServices;

using unsafe GridRandomizedSufflerFuncPtr = delegate*<Random, ref Grid, void>;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridTransformingExtensions
{
	/// <summary>
	/// Transforms the grid by the specified type.
	/// </summary>
	/// <param name="grid">The grid to be transformed.</param>
	/// <param name="transformTypes">
	/// The transform types can be applied. You can use <c><see cref="TransformType"/>.<see langword="operator"/> |</c> to combine flags.
	/// </param>
	public static unsafe void Transform(this ref Grid grid, TransformType transformTypes)
	{
		if (transformTypes == TransformType.None)
		{
			return;
		}

		var rng = Random.Shared;
		foreach (var kind in transformTypes)
		{
			(
				kind switch
				{
					TransformType.DigitSwap => &swapDigits,
					TransformType.RowSwap => &swapRow,
					TransformType.ColumnSwap => &swapColumn,
					TransformType.BandSwap => &swapBand,
					TransformType.TowerSwap => &swapTower,
					TransformType.MirrorLeftRight => &mirrorLeftRight,
					TransformType.MirrorTopBottom => &mirrorTopBottom,
					TransformType.MirrorDiagonal => &mirrorDiagonal,
					TransformType.MirrorAntidiagonal => &mirrorAntidiagonal,
					TransformType.RotateClockwise => &rotateClockwise,
					TransformType.RotateCounterclockwise => &rotateCounterclockwise,
					_ => default(GridRandomizedSufflerFuncPtr)
				}
			)(rng, ref grid);
		}


		static void swapDigits(Random random, ref Grid grid)
		{
			for (var i = 0; i < 10; i++)
			{
				var d1 = random.NextDigit();
				int d2;
				do
				{
					d2 = random.NextDigit();
				} while (d1 == d2);
				grid.SwapDigit(d1, d2);
			}
		}

		static void swapRow(Random random, ref Grid grid)
		{
			for (var i = 0; i < 10; i++)
			{
				var l1 = random.Next(9, 18);
				var l1p = l1 / 3;
				int l2;
				do
				{
					l2 = l1p * 3 + random.Next(0, 3);
				} while (l1 == l2);
				grid.SwapHouse(l1, l2);
			}
		}

		static void swapColumn(Random random, ref Grid grid)
		{
			for (var i = 0; i < 10; i++)
			{
				var l1 = random.Next(18, 27);
				var l1p = l1 / 3;
				int l2;
				do
				{
					l2 = l1p * 3 + random.Next(0, 3);
				} while (l1 == l2);
				grid.SwapHouse(l1, l2);
			}
		}

		static void swapBand(Random random, ref Grid grid)
		{
			for (var i = 0; i < 2; i++)
			{
				var c1 = random.Next(0, 3);
				int c2;
				do
				{
					c2 = random.Next(0, 3);
				} while (c1 == c2);
				grid.SwapChute(c1, c2);
			}
		}

		static void swapTower(Random random, ref Grid grid)
		{
			for (var i = 0; i < 2; i++)
			{
				var c1 = random.Next(3, 6);
				int c2;
				do
				{
					c2 = random.Next(3, 6);
				} while (c1 == c2);
				grid.SwapChute(c1, c2);
			}
		}

		static void mirrorLeftRight(Random _, ref Grid grid) => grid.MirrorLeftRight();

		static void mirrorTopBottom(Random _, ref Grid grid) => grid.MirrorTopBottom();

		static void mirrorDiagonal(Random _, ref Grid grid) => grid.MirrorDiagonal();

		static void mirrorAntidiagonal(Random _, ref Grid grid) => grid.MirrorAntidiagonal();

		static void rotateClockwise(Random random, ref Grid grid)
		{
			var times = random.Next(0, 4);
			for (var i = 0; i < times; i++)
			{
				grid.RotateClockwise();
			}
		}

		static void rotateCounterclockwise(Random random, ref Grid grid)
		{
			var times = random.Next(0, 4);
			for (var i = 0; i < times; i++)
			{
				grid.RotateCounterclockwise();
			}
		}
	}
}
