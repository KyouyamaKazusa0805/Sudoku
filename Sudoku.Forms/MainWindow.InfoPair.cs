using Sudoku.Solving;

namespace Sudoku.Forms
{
	partial class MainWindow
	{
		private readonly struct InfoPair
		{
			public InfoPair(int index, TechniqueInfo techniqueInfo) =>
				(Index, TechniqueInfo) = (index, techniqueInfo);

			public InfoPair((int _index, TechniqueInfo _info) pair) =>
				(Index, TechniqueInfo) = (pair._index, pair._info);


			public int Index { get; }

			public TechniqueInfo TechniqueInfo { get; }


			public void Deconstruct(out int index, out TechniqueInfo techniqueInfo) =>
				(index, techniqueInfo) = (Index, TechniqueInfo);

			public override string ToString() => TechniqueInfo.ToString();
		}
	}
}
