using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Solving.Singles;

namespace Sudoku.Solving
{
	public sealed class AnalysisInfo
	{
		public AnalysisInfo(
			string solverName, IEnumerable<TechniqueInfo>? solvingSteps, TimeSpan elapsedTime, bool isSolved) =>
			(SolverName, SolvingSteps, ElapsedTime, IsSolved) = (solverName, solvingSteps, elapsedTime, isSolved);


		public bool IsSolved { get; }

		public decimal? PuzzleDifficulty =>
			IsSolved ? (SolvingSteps?.Max(info => info.Difficulty) ?? null) : 20m;

		public decimal? PearlDifficulty =>
			 IsSolved ? (SolvingSteps?.First().Difficulty) ?? null : 20m;

		public decimal? DiamondDifficulty
		{
			get
			{
				if (IsSolved)
				{
					if (SolvingSteps is null)
					{
						return null;
					}
					else
					{
						var list = new List<TechniqueInfo>(SolvingSteps);
						for (int i = 1; i < list.Count; i++)
						{
							if (list[i] is SingleInfo)
							{
								return list[i - 1].Difficulty;
							}
						}
					}
				}

				// Not solved.
				return 20m;
			}
		}

		public decimal? Total => SolvingSteps?.Sum(info => info.Difficulty);

		public int SolvingStepCount => SolvingSteps.Count();

		public string SolverName { get; }

		public TimeSpan ElapsedTime { get; }

		public IEnumerable<TechniqueInfo>? SolvingSteps { get; }


		public override string ToString()
		{
			var sb = new StringBuilder($"Solving tool: {SolverName}{Environment.NewLine}");
			if (SolvingSteps is null)
			{
				sb.AppendLine($"Puzzle has{(IsSolved ? " " : " not ")}been solved.");
				sb.AppendLine($"Using time: {ElapsedTime:dd':'hh':'mm':'ss':'fff}");
			}
			else
			{
				foreach (var info in SolvingSteps)
				{
					sb.AppendLine($"({info.Difficulty}) {info}");
				}

				sb.AppendLine($"Puzzle has{(IsSolved ? " " : " not ")}been solved.");
				sb.AppendLine($"Time elapsed: {ElapsedTime:dd':'hh':'mm':'ss':'fff}");
				sb.AppendLine($"Solving steps count: {SolvingStepCount}");
				sb.AppendLine(
					$"Puzzle rating: {PuzzleDifficulty:0.0}/{PearlDifficulty:0.0}/{DiamondDifficulty:0.0}");
			}

			return sb.ToString();
		}
	}
}
