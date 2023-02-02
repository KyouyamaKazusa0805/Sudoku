namespace SudokuStudio.Configuration;

/// <summary>
/// Represents with preference items that is used by <see cref="LogicalSolver"/>.
/// </summary>
/// <seealso cref="LogicalSolver"/>
public sealed class LogicalAnalysisPreference : PreferenceGroup
{
	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_EnableFullHouse"/>
	public bool EnableFullHouse
	{
		get => Solver.SingleStepSearcher_EnableFullHouse;

		set
		{
			if (EnableFullHouse == value)
			{
				return;
			}

			Solver.SingleStepSearcher_EnableFullHouse = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_EnableLastDigit"/>
	public bool EnableLastDigit
	{
		get => Solver.SingleStepSearcher_EnableLastDigit;

		set
		{
			if (EnableLastDigit == value)
			{
				return;
			}

			Solver.SingleStepSearcher_EnableLastDigit = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.SingleStepSearcher_HiddenSinglesInBlockFirst"/>
	public bool HiddenSinglesInBlockFirst
	{
		get => Solver.SingleStepSearcher_HiddenSinglesInBlockFirst;

		set
		{
			if (HiddenSinglesInBlockFirst == value)
			{
				return;
			}

			Solver.SingleStepSearcher_HiddenSinglesInBlockFirst = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles"/>
	public bool AllowIncompleteUniqueRectangles
	{
		get => Solver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles;

		set
		{
			if (AllowIncompleteUniqueRectangles == value)
			{
				return;
			}

			Solver.UniqueRectangleStepSearcher_AllowIncompleteUniqueRectangles = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles"/>
	public bool SearchForExtendedUniqueRectangles
	{
		get => Solver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles;

		set
		{
			if (SearchForExtendedUniqueRectangles == value)
			{
				return;
			}

			Solver.UniqueRectangleStepSearcher_SearchForExtendedUniqueRectangles = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes"/>
	public bool SearchExtendedBivalueUniversalGraveTypes
	{
		get => Solver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes;

		set
		{
			if (SearchExtendedBivalueUniversalGraveTypes == value)
			{
				return;
			}

			Solver.BivalueUniversalGraveStepSearcher_SearchExtendedTypes = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnAlmostLockedSetXzRule
	{
		get => Solver.AlmostLockedSetsXzStepSearcher_AllowCollision;

		set
		{
			if (AllowCollisionOnAlmostLockedSetXzRule == value)
			{
				return;
			}

			Solver.AlmostLockedSetsXzStepSearcher_AllowCollision = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns"/>
	public bool AllowLoopedPatternsOnAlmostLockedSetXzRule
	{
		get => Solver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns;

		set
		{
			if (AllowLoopedPatternsOnAlmostLockedSetXzRule == value)
			{
				return;
			}

			Solver.AlmostLockedSetsXzStepSearcher_AllowLoopedPatterns = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.AlmostLockedSetsXyWingStepSearcher_AllowCollision"/>
	public bool AllowCollisionOnAlmostLockedSetXyWing
	{
		get => Solver.AlmostLockedSetsXyWingStepSearcher_AllowCollision;

		set
		{
			if (AllowCollisionOnAlmostLockedSetXyWing == value)
			{
				return;
			}

			Solver.AlmostLockedSetsXyWingStepSearcher_AllowCollision = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.IsFullApplying"/>
	public bool LogicalSolverIsFullApplying
	{
		get => Solver.IsFullApplying;

		set
		{
			if (LogicalSolverIsFullApplying == value)
			{
				return;
			}

			Solver.IsFullApplying = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.IgnoreSlowAlgorithms"/>
	public bool LogicalSolverIgnoresSlowAlgorithms
	{
		get => Solver.IgnoreSlowAlgorithms;

		set
		{
			if (LogicalSolverIgnoresSlowAlgorithms == value)
			{
				return;
			}

			Solver.IgnoreSlowAlgorithms = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.IgnoreHighAllocationAlgorithms"/>
	public bool LogicalSolverIgnoresHighAllocationAlgorithms
	{
		get => Solver.IgnoreHighAllocationAlgorithms;

		set
		{
			if (LogicalSolverIgnoresHighAllocationAlgorithms == value)
			{
				return;
			}

			Solver.IgnoreHighAllocationAlgorithms = value;
		}
	}

	/// <inheritdoc cref="StepsGatherer.OnlyShowSameLevelTechniquesInFindAllSteps"/>
	public bool StepGathererOnlySearchSameLevelTechniquesInFindAllSteps
	{
		get => Gatherer.OnlyShowSameLevelTechniquesInFindAllSteps;

		set
		{
			if (StepGathererOnlySearchSameLevelTechniquesInFindAllSteps == value)
			{
				return;
			}

			Gatherer.OnlyShowSameLevelTechniquesInFindAllSteps = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.RegularWingStepSearcher_MaxSize"/>
	public int MaxSizeOfRegularWing
	{
		get => Solver.RegularWingStepSearcher_MaxSize;

		set
		{
			if (MaxSizeOfRegularWing == value)
			{
				return;
			}

			Solver.RegularWingStepSearcher_MaxSize = value;
		}
	}

	/// <inheritdoc cref="LogicalSolver.ComplexFishStepSearcher_MaxSize"/>
	public int MaxSizeOfComplexFish
	{
		get => Solver.ComplexFishStepSearcher_MaxSize;

		set
		{
			if (MaxSizeOfComplexFish == value)
			{
				return;
			}

			Solver.ComplexFishStepSearcher_MaxSize = value;
		}
	}

	/// <inheritdoc cref="StepsGatherer.MaxStepsGathered"/>
	public int StepGathererMaxStepsGathered
	{
		get => Gatherer.MaxStepsGathered;

		set
		{
			if (StepGathererMaxStepsGathered == value)
			{
				return;
			}

			Gatherer.MaxStepsGathered = value;
		}
	}

	/// <summary>
	/// The faster entry of project-scoped solver instance.
	/// </summary>
	[JsonIgnore]
	private LogicalSolver Solver => ((App)Application.Current).EnvironmentVariables.Solver;

	/// <summary>
	/// The faster entry of project-scoped step gatherer instance.
	/// </summary>
	[JsonIgnore]
	private StepsGatherer Gatherer => ((App)Application.Current).EnvironmentVariables.Gatherer;
}
