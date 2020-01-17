using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.Diagnostics.CodeAnalysis.Nullability;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	public sealed class AnalysisResult
	{
		public AnalysisResult(
			Grid initialGrid, string solverName, bool hasSolved, Grid? solution,
			TimeSpan elapsedTime, IReadOnlyList<TechniqueInfo>? solvingList, string? additional)
		{
			(Puzzle, SolverName, HasSolved, Solution, SolvingSteps, ElapsedTime, Additional) =
				(initialGrid, solverName, hasSolved, solution, solvingList, elapsedTime, additional);
		}


		public bool HasSolved { get; }

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		public decimal MaxDifficulty =>
			SolvingSteps is null || !SolvingSteps.Any() ? 20 : SolvingSteps.Max(info => info.Difficulty);

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		public decimal? TotalDifficulty => SolvingSteps?.Sum(info => info.Difficulty);

		public decimal? PearlDifficulty =>
			 HasSolved ? (SolvingSteps?.First().Difficulty) ?? null : 20m;

		public decimal? DiamondDifficulty
		{
			get
			{
				if (HasSolved)
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
							if (list[i] is SingleTechniqueInfo)
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

		public int SolvingStepsCount => SolvingSteps?.Count ?? 1;

		public string SolverName { get; }

		public string? Additional { get; }

		public TimeSpan ElapsedTime { get; }

		public DifficultyLevels DifficultyLevel
		{
			get
			{
				var maxLevel = DifficultyLevels.Unknown;
				if (HasSolved && !(SolvingSteps is null))
				{
					foreach (var step in SolvingSteps)
					{
						if (step.DifficultyLevel > maxLevel)
						{
							maxLevel = step.DifficultyLevel;
						}
					}
				}

				return maxLevel;
			}
		}

		public Grid Puzzle { get; }

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		public Grid? Solution { get; }

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		public IReadOnlyList<TechniqueInfo>? SolvingSteps { get; }

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		private IEnumerable<IGrouping<string, TechniqueInfo>>? SolvingStepsGrouped
		{
			get
			{
				return SolvingSteps is null
					? null
					: from solvingStep in SolvingSteps
					  orderby solvingStep.Difficulty
					  group solvingStep by solvingStep.Name;
			}
		}


		[OnDeconstruction]
		public void Deconstruct(
			out int? solvingStepsCount, out IReadOnlyList<TechniqueInfo>? solvingSteps) =>
			(solvingStepsCount, solvingSteps) = (SolvingStepsCount == 0 ? (int?)null : SolvingStepsCount, SolvingSteps);

		[OnDeconstruction]
		public void Deconstruct(
			out string solverName, out bool hasSolved, out string? additional) =>
			(solverName, hasSolved, additional) = (SolverName, HasSolved, Additional);

		[OnDeconstruction]
		public void Deconstruct(
			out decimal? total, out decimal? max,
			out decimal? pearl, out decimal? diamond) =>
			(total, max, pearl, diamond) = (TotalDifficulty, MaxDifficulty == 20 ? (decimal?)null : MaxDifficulty, PearlDifficulty, DiamondDifficulty);

		[OnDeconstruction]
		public void Deconstruct(
			out Grid puzzle, out bool hasSolved, out TimeSpan elapsedTime,
			out Grid? solution, out DifficultyLevels difficultyLevel) =>
			(puzzle, hasSolved, elapsedTime, solution, difficultyLevel) = (Puzzle, HasSolved, ElapsedTime, Solution, DifficultyLevel);

		public override string ToString()
		{
			var sb = new StringBuilder($"Puzzle: {Puzzle:#}{Environment.NewLine}");
			sb.AppendLine($"Solving tool: {SolverName}");
			if (SolvingSteps is null)
			{
				sb.AppendLine($"Puzzle has {(HasSolved ? "" : "not ")}been solved.");
				if (!(Solution is null))
				{
					sb.AppendLine($"Puzzle solution: {Solution:!}");
				}
				sb.AppendLine($"Time elapsed: {ElapsedTime:hh':'mm'.'ss'.'fff}");
			}
			else
			{
				foreach (var info in SolvingSteps)
				{
					sb.AppendLine($"({info.Difficulty}) {info}");
				}

				sb.AppendLine($"Puzzle has {(HasSolved ? "" : "not ")}been solved.");
				if (!(Solution is null))
				{
					sb.AppendLine($"Puzzle solution: {Solution:!}");
				}
				sb.AppendLine($"Time elapsed: {ElapsedTime:hh':'mm'.'ss'.'fff}");
				sb.AppendLine("Technique used:");
				if (!(SolvingStepsGrouped is null))
				{
					foreach (var solvingStepsGroup in SolvingStepsGrouped)
					{
						sb.AppendLine($"{solvingStepsGroup.Count()} * {solvingStepsGroup.Key}");
					}
				}
				sb.AppendLine($"Total solving steps count: {SolvingStepsCount}");
				sb.AppendLine($"Difficulty total: {TotalDifficulty}");
				sb.AppendLine($"Puzzle rating: {MaxDifficulty:0.0}/{PearlDifficulty:0.0}/{DiamondDifficulty:0.0}");
			}

			if (!(Additional is null))
			{
				sb.AppendLine(new string('-', 10));
				sb.AppendLine(Additional);
			}

			return sb.ToString();
		}
	}
}
