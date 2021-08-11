namespace Sudoku.Solving.Manual.Uniqueness.Square
{
	partial class UsStepSearcher
	{
		static UsStepSearcher()
		{
			int[,] ChuteIterator =
			{
				{ 0, 3, 6 }, { 0, 3, 7 }, { 0, 3, 8 }, { 0, 4, 6 }, { 0, 4, 7 }, { 0, 4, 8 },
				{ 0, 5, 6 }, { 0, 5, 7 }, { 0, 5, 8 },
				{ 1, 3, 6 }, { 1, 3, 7 }, { 1, 3, 8 }, { 1, 4, 6 }, { 1, 4, 7 }, { 1, 4, 8 },
				{ 1, 5, 6 }, { 1, 5, 7 }, { 1, 5, 8 },
				{ 2, 3, 6 }, { 2, 3, 7 }, { 2, 3, 8 }, { 2, 4, 6 }, { 2, 4, 7 }, { 2, 4, 8 },
				{ 2, 5, 6 }, { 2, 5, 7 }, { 2, 5, 8 }
			};

			int length = ChuteIterator.Length / 3, n = 0;
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < length; j++)
				{
					int a = ChuteIterator[j, 0] + i * 27;
					int b = ChuteIterator[j, 1] + i * 27;
					int c = ChuteIterator[j, 2] + i * 27;
					Patterns[n++] = new() { a, b, c, a + 9, b + 9, c + 9, a + 18, b + 18, c + 18 };
				}
			}

			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < length; j++)
				{
					int a = ChuteIterator[j, 0] * 9;
					int b = ChuteIterator[j, 1] * 9;
					int c = ChuteIterator[j, 2] * 9;
					Patterns[n++] = new()
					{
						a + 3 * i,
						b + 3 * i,
						c + 3 * i,
						a + 1 + 3 * i,
						b + 1 + 3 * i,
						c + 1 + 3 * i,
						a + 2 + 3 * i,
						b + 2 + 3 * i,
						c + 2 + 3 * i
					};
				}
			}
		}
	}
}
