namespace Sudoku.Rating;

using RarityKind = Rarity;

/// <summary>
/// Represents a rater instance to get the score of a puzzle on logically solved result.
/// </summary>
public sealed class Rater
{
	/// <summary>
	/// Initializes a <see cref="Rater"/> instance via the specified solver result.
	/// </summary>
	/// <param name="solverResult">The solver result.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Rater(LogicalSolverResult solverResult) => SolverResult = solverResult;


	/// <summary>
	/// Indicates the scoring of the exerciziability value. The value is in range [0, 100].
	/// </summary>
	public int Exerciziability
	{
		get
		{
			var techniqueUsedDic = new Dictionary<Technique, List<IStep>>();
			foreach (var step in SolverResult)
			{
				if (step is { TechniqueCode: var technique, DifficultyLevel: not DifficultyLevel.Easy }
					&& !techniqueUsedDic.TryAdd(technique, new()))
				{
					techniqueUsedDic[technique].Add(step);
				}
			}

			if (techniqueUsedDic.Count == 1 && techniqueUsedDic.First().Value.Count == 1)
			{
				return 100;
			}

			var total = 300;
			foreach (var (technique, steps) in techniqueUsedDic)
			{
				foreach (var step in steps)
				{
					total -= step.DifficultyLevel switch
					{
						DifficultyLevel.Moderate => 1,
						DifficultyLevel.Hard => 2,
						DifficultyLevel.Fiendish => 4,
						DifficultyLevel.Nightmare => 8,
						_ => 0
					};
				}
			}

			return (int)(Clamp(total, 0, 300) / 3D);
		}
	}

	/// <summary>
	/// Indicates the rarity of the puzzle. The value is in range [0, 100].
	/// </summary>
	public int Rarity
	{
		get
		{
			var total = 300;
			foreach (var step in SolverResult)
			{
				total -= step.Rarity switch
				{
					RarityKind.Always or RarityKind.ReplacedByOtherTechniques or RarityKind.OnlyForSpecialPuzzles => 0,
					RarityKind.HardlyEver => 1,
					RarityKind.Seldom => 2,
					RarityKind.Sometimes => 4,
					RarityKind.Often => 8,
					_ => 0
				};
			}

			return (int)(Clamp(total, 0, 300) / 3D);
		}
	}

	/// <summary>
	/// Indicates the directability of the puzzle. The value is in range [0, 100].
	/// </summary>
	public int Directability
	{
		get
		{
			var total = 300D;
			foreach (var step in SolverResult)
			{
				total -= step switch
				{
					{ TechniqueCode: Technique.NakedSingle } => .5,
					{ TechniqueCode: Technique.HiddenSingleColumn or Technique.HiddenSingleRow } => .25,
					{ DifficultyLevel: DifficultyLevel.Easy } => 0,
					{ DifficultyLevel: DifficultyLevel.Moderate } => 1,
					{ DifficultyLevel: DifficultyLevel.Hard } => 3,
					{ DifficultyLevel: DifficultyLevel.Fiendish } => 6,
					{ DifficultyLevel: DifficultyLevel.Nightmare } => 10,
					_ => 10
				};
			}

			return (int)(Clamp(total, 0, 300) / 3);
		}
	}

	/// <summary>
	/// The solver result instance.
	/// </summary>
	public LogicalSolverResult SolverResult { get; }
}
