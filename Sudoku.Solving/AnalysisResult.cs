using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Data.Meta;
using Sudoku.Diagnostics.CodeAnalysis.Nullability;
using Sudoku.Solving.Manual.Singles;

namespace Sudoku.Solving
{
	public sealed class AnalysisResult
	{
		public AnalysisResult(
			Grid initialGrid, string solverName, bool hasSolved, Grid? solution,
			TimeSpan elapsedTime, IList<TechniqueInfo>? solvingList, string? additional)
		{
			(InitialGrid, SolverName, HasSolved, Solution, SolvingSteps, ElapsedTime, Additional) =
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

				#region Deprecated code
				//return HasSolved
				//	? MaxDifficulty switch
				//	{
				//		_ when MaxDifficulty >= 1.0m && MaxDifficulty <= 1.2m => DifficultyLevels.VeryEasy,
				//		_ when MaxDifficulty > 1.2m && MaxDifficulty <= 1.5m => DifficultyLevels.Easy,
				//		_ when MaxDifficulty > 1.5m && MaxDifficulty <= 2.3m => DifficultyLevels.Moderate,
				//		_ when MaxDifficulty > 2.3m && MaxDifficulty <= 2.8m => DifficultyLevels.Advanced,
				//		_ when MaxDifficulty > 2.8m && MaxDifficulty <= 3.4m => DifficultyLevels.Hard,
				//		_ when MaxDifficulty > 3.4m && MaxDifficulty <= 4.4m => DifficultyLevels.VeryHard,
				//		_ when MaxDifficulty > 4.5m && MaxDifficulty <= 6.2m => DifficultyLevels.Fiendish,
				//		_ when MaxDifficulty > 6.2m && MaxDifficulty <= 7.6m => DifficultyLevels.Diabolical,
				//		_ when MaxDifficulty > 7.6m && MaxDifficulty <= 8.9m => DifficultyLevels.Crazy,
				//		_ when MaxDifficulty > 8.9m && MaxDifficulty <= 10.0m => DifficultyLevels.Nightmare,
				//		_ when MaxDifficulty > 10.0m && MaxDifficulty <= 12.0m => DifficultyLevels.BeyondNightmare,
				//		_ => DifficultyLevels.Unknown
				//	}
				//	: DifficultyLevels.Unknown;
				#endregion
			}
		}

		public Grid InitialGrid { get; }

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		public Grid? Solution { get; }

		[PropertyNotNullWhen(nameof(HasSolved), true)]
		public IList<TechniqueInfo>? SolvingSteps { get; }

		public IEnumerable<IGrouping<string, TechniqueInfo>>? SolvingStepsGrouped
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


		public override string ToString()
		{
			var sb = new StringBuilder($"Initial grid: {InitialGrid:#}{Environment.NewLine}");
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
