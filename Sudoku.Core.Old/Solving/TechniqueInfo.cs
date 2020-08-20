#pragma warning disable CS8767

using System;
using System.Collections.Generic;
using Sudoku.Data.Meta;
using Sudoku.Drawing;
using static Sudoku.Solving.DifficultyLevel;

namespace Sudoku.Solving
{
	public abstract class TechniqueInfo : IComparable<TechniqueInfo>, IEquatable<TechniqueInfo>
	{
		protected TechniqueInfo(Conclusion conclusion, ICollection<View> views) =>
			(Conclusion, Views) = (conclusion, views);


		public bool IsSSTS => DifficultyLevel == Easy || DifficultyLevel == Moderate;

		public abstract decimal Difficulty { get; }

		public abstract string Name { get; }

		public abstract DifficultyLevel DifficultyLevel { get; }

		public Conclusion Conclusion { get; }

		public ICollection<View> Views { get; }


		public void ApplyTo(Grid grid)
		{
			// Note that the operation is un-undo-able...
			// If you want to impliment undo-and-redos, please
			// design a data structure (may be a `Stack<Step>`)
			// to store these assignment and elimination steps.
			if (Conclusion.ConclusionType == ConclusionType.Assignment)
			{
				foreach (var (cell, digit) in Conclusion)
				{
					ref var info = ref grid[cell];
					if (!info.IsValueCell)
					{
						info.CellType = CellType.Modifiable;
						info.Value = digit;
					}
				}
			}
			else // Eliminations.
			{
				foreach (var (cell, digit) in Conclusion)
				{
					grid[cell][digit] = false;
				}
			}
		}

		public sealed override bool Equals(object? obj) =>
			obj is TechniqueInfo comparer && Equals(comparer);

		public virtual bool Equals(TechniqueInfo other) =>
			Difficulty == other.Difficulty;

		public virtual int CompareTo(TechniqueInfo other) =>
			Difficulty.CompareTo(other.Difficulty);

		public override int GetHashCode()
		{
			int result = 0xfacade;
			foreach (var view in Views)
			{
				result ^= view.GetHashCode();
			}

			return result;
		}

		public abstract override string ToString();


		public static bool operator ==(TechniqueInfo left, TechniqueInfo right) => left.Equals(right);

		public static bool operator !=(TechniqueInfo left, TechniqueInfo right) => !(left == right);

		public static bool operator >(TechniqueInfo left, TechniqueInfo right) => left.CompareTo(right) > 0;

		public static bool operator <(TechniqueInfo left, TechniqueInfo right) => left.CompareTo(right) < 0;

		public static bool operator >=(TechniqueInfo left, TechniqueInfo right) => left.CompareTo(right) >= 0;

		public static bool operator <=(TechniqueInfo left, TechniqueInfo right) => left.CompareTo(right) <= 0;
	}
}
