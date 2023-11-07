using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Steps;

namespace Sudoku.Concepts;

using Pair = (Grid SteppingGrid, Step Step);

/// <summary>
/// Represents a solving path.
/// </summary>
[Equals(EqualsBehavior.ThrowNotSupportedException)]
[GetHashCode(GetHashCodeBehavior.ThrowNotSupportedException)]
public readonly ref partial struct SolvingPath
{
	/// <summary>
	/// The reference to the first of stepping grids.
	/// </summary>
	private readonly Grid[] _steppingGridFirst = [];

	/// <summary>
	/// The reference to the first of steps.
	/// </summary>
	private readonly Step[] _stepsFirst = [];


	/// <summary>
	/// Initializes a <see cref="SolvingPath"/> instance.
	/// </summary>
	/// <param name="steppingGrids">The stepping grids.</param>
	/// <param name="steps">The steps.</param>
	/// <exception cref="ArgumentOutOfRangeException">Throws when the length of arguments are not equal.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SolvingPath(Grid[] steppingGrids, Step[] steps)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(steppingGrids.Length, steps.Length);

		_steppingGridFirst = steppingGrids;
		_stepsFirst = steps;
		(Length, IsSolved) = (steppingGrids.Length, true);
	}


	/// <summary>
	/// Indicates whether the puzzle has been solved.
	/// </summary>
	public bool IsSolved { get; }

	/// <summary>
	/// Indicates the number of elements stored in this collection.
	/// </summary>
	public int Length { get; }

	/// <summary>
	/// Indicates the stepping grids.
	/// </summary>
	public ReadOnlySpan<Grid> SteppingGrids => _steppingGridFirst;

	/// <summary>
	/// Indicates the steps.
	/// </summary>
	public ReadOnlySpan<Step> Steps => _stepsFirst;

	/// <summary>
	/// Gets the bottleneck during the whole grid solving.
	/// Returns <see langword="null"/> if the property <see cref="Steps"/> is default case (i.e. empty).
	/// </summary>
	/// <seealso cref="Steps"/>
	public Step? Bottleneck
	{
		get
		{
			if (!IsSolved)
			{
				return null;
			}

			switch (Steps)
			{
				case [var firstStep, ..]:
				{
					foreach (var step in new ReverseEnumerator<Step>(Steps))
					{
						if (step is not SingleStep)
						{
							return step;
						}
					}

					// If code goes to here, all steps are more difficult than single techniques. Get the first one.
					return firstStep;
				}
				default:
				{
					return null;
				}
			}
		}
	}

	/// <summary>
	/// Indicates the internal pairs.
	/// </summary>
	internal ReadOnlySpan<Pair> Pairs
	{
		get
		{
			var result = new Pair[Length];
			for (var i = 0; i < SteppingGrids.Length; i++)
			{
				result[i] = (SteppingGrids[i], Steps[i]);
			}
			return result;
		}
	}


	/// <summary>
	/// Try to fetch the pair of stepping grid and step at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the pair of stepping grid and step.</returns>
	public ref readonly Pair this[int index] => ref Pairs[index];

	/// <summary>
	/// Gets the found <see cref="Step"/> instance whose corresponding candidates are same with the specified argument <paramref name="grid"/>.
	/// </summary>
	/// <param name="grid">The grid to be matched.</param>
	/// <returns>The found <see cref="Step"/> instance.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the puzzle is not solved (i.e. <see cref="IsSolved"/> property returns <see langword="false"/>).
	/// </exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the specified puzzle cannot correspond to a paired <see cref="Step"/> instance.
	/// </exception>
	public Step this[scoped ref readonly Grid grid]
	{
		get
		{
			if (!IsSolved)
			{
				throw new InvalidOperationException("The puzzle must have been solved before you use this indexer.");
			}

			foreach (var (g, s) in this)
			{
				if (g == grid)
				{
					return s;
				}
			}

			throw new ArgumentOutOfRangeException("The specified step is not found.");
		}
	}

	/// <summary>
	/// Gets the first found <see cref="Step"/> whose name is specified one, or nearly same as the specified one.
	/// </summary>
	/// <param name="techniqueName">Technique name.</param>
	/// <returns>The first found step.</returns>
	public Pair? this[string techniqueName]
	{
		get
		{
			if (!IsSolved)
			{
				return null;
			}

			foreach (var pair in this)
			{
				var (_, step) = pair;

				var name = step.Name;
				if (nameEquality(name))
				{
					return pair;
				}

				var aliases = step.Code.GetAliases();
				if (aliases is not null && Array.Exists(aliases, nameEquality))
				{
					return pair;
				}

				var abbr = step.Code.GetAbbreviation();
				if (abbr is not null && nameEquality(abbr))
				{
					return pair;
				}
			}
			return null;


			bool nameEquality(string name) => name == techniqueName || name.Contains(techniqueName, StringComparison.OrdinalIgnoreCase);
		}
	}

	/// <summary>
	/// Gets a list of <see cref="Step"/>s that has the same difficulty rating value as argument <paramref name="difficultyRating"/>. 
	/// </summary>
	/// <param name="difficultyRating">The specified difficulty rating value.</param>
	/// <returns>
	/// A list of <see cref="Step"/>s found. If the puzzle cannot be solved (i.e. <see cref="IsSolved"/> returns <see langword="false"/>),
	/// the return value will be <see langword="null"/>. If the puzzle is solved, but the specified value is not found,
	/// the return value will be an empty array, rather than <see langword="null"/>. The nullability of the return value
	/// only depends on property <see cref="IsSolved"/>.
	/// </returns>
	/// <seealso cref="IsSolved"/>
	public ReadOnlySpan<Step> this[decimal difficultyRating]
		=> IsSolved ? Steps.FindAll((ref readonly Step step) => step.Difficulty == difficultyRating) : [];

	/// <summary>
	/// Gets a list of <see cref="Step"/>s that matches the specified technique.
	/// </summary>
	/// <param name="code">The specified technique code.</param>
	/// <returns>
	/// <inheritdoc cref="this[decimal]" path="/returns"/>
	/// </returns>
	/// <seealso cref="IsSolved"/>
	public ReadOnlySpan<Step> this[Technique code] => IsSolved ? Steps.FindAll((ref readonly Step step) => step.Code == code) : [];

	/// <summary>
	/// Gets a list of <see cref="Step"/>s that has the same difficulty level as argument <paramref name="difficultyLevel"/>. 
	/// </summary>
	/// <param name="difficultyLevel">The specified difficulty level.</param>
	/// <returns>
	/// <inheritdoc cref="this[decimal]" path="/returns"/>
	/// </returns>
	/// <seealso cref="IsSolved"/>
	public ReadOnlySpan<Step> this[DifficultyLevel difficultyLevel]
		=> IsSolved ? Steps.FindAll((ref readonly Step step) => step.DifficultyLevel == difficultyLevel) : [];


	/// <summary>
	/// Determine whether the analyzer result instance contains any step with specified technique.
	/// </summary>
	/// <param name="technique">The technique you want to be checked.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has not been solved.</exception>
	public bool HasTechnique(Technique technique)
	{
		if (!IsSolved)
		{
			throw new InvalidOperationException("The puzzle must be solved before call this method.");
		}

		foreach (var step in Steps)
		{
			if (step.Code == technique)
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Try to fetch the stepping grid at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The reference to the stepping grid at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly Grid SteppingGridAt(int index) => ref SteppingGrids[index];

	/// <summary>
	/// Gets an enumerator instance to iterate on each pair of stepping grids and actual step.
	/// </summary>
	/// <returns>An enumerator instance that iterates on each pair of stepping grid and actual step.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Pair>.Enumerator GetEnumerator() => Pairs.GetEnumerator();

	/// <summary>
	/// Try to enumerate all steps.
	/// </summary>
	/// <returns>An enumerator instance that iterates on each stepping grid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Grid>.Enumerator EnumerateSteppingGrids() => SteppingGrids.GetEnumerator();

	/// <summary>
	/// Try to enumerate all steps.
	/// </summary>
	/// <returns>An enumerator instance that iterates on each steps.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ReadOnlySpan<Step>.Enumerator EnumerateSteps() => Steps.GetEnumerator();

	/// <summary>
	/// Converts the current list into an array.
	/// </summary>
	/// <returns>An array of pairs of stepping grid and step.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Pair[] ToArray() => [.. Pairs];

	/// <summary>
	/// Try to fetch the step at the specified index.
	/// </summary>
	/// <param name="index">The desired index.</param>
	/// <returns>The step at the specified index.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Step StepAt(int index) => Steps[index];

	/// <inheritdoc cref="object.ToString"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString()
		=> $$"""{{nameof(SolvingPath)}} { {{nameof(SteppingGrids)}}.{{nameof(Array.Length)}} = {{SteppingGrids.Length}}, {{nameof(Steps)}}.{{nameof(Array.Length)}} = {{Steps.Length}} }""";
}
