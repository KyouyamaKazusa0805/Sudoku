#undef SYNC_ROOT_VIA_METHODIMPL
#define SYNC_ROOT_VIA_OBJECT
#undef SYNC_ROOT_VIA_THREAD_LOCAL
#if SYNC_ROOT_VIA_METHODIMPL && SYNC_ROOT_VIA_OBJECT && SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#error You cannot set all three symbols 'SYNC_ROOT_VIA_METHODIMPL', 'SYNC_ROOT_VIA_OBJECT' and 'SYNC_ROOT_VIA_THREAD_LOCAL' - they are designed by the same purpose. You should only define at most one of three symbols.
#line default
#elif SYNC_ROOT_VIA_METHODIMPL && SYNC_ROOT_VIA_OBJECT
#line 1 "Grid.cs"
#error Don't set both symbols 'SYNC_ROOT_VIA_METHODIMPL' and 'SYNC_ROOT_VIA_OBJECT'.
#line default
#elif SYNC_ROOT_VIA_METHODIMPL && SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#error Don't set both symbols 'SYNC_ROOT_VIA_METHODIMPL' and 'SYNC_ROOT_VIA_THREAD_LOCAL'.
#line default
#elif SYNC_ROOT_VIA_OBJECT && SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#error Don't set both symbols 'SYNC_ROOT_VIA_OBJECT' and 'SYNC_ROOT_VIA_THREAD_LOCAL'.
#line default
#elif !SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_THREAD_LOCAL
#line 1 "Grid.cs"
#warning No sync-root mode is selected, meaning we cannot use this type in multi-threading (i.e. this type becomes thread-unsafe) because some members will rely on pointers and shared memory, which is unsafe. You can ONLY use property 'IsValid', 'SolutionGrid' and method 'ExactlyValidate' in this type inside a lock statement.
#line default
#endif

namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods on <see cref="Grid"/> for solving.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridSolvingExtensions
{
#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
	/// <summary>
	/// The internal field that can be used for making threads run in order while using <see cref="Solver"/>,
	/// keeping the type being thread-safe.
	/// </summary>
	/// <seealso cref="Solver"/>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#if NET9_0_OR_GREATER
	private static readonly Lock PuzzleSolvingSynchronizer = new();
#else
	private static readonly object PuzzleSolvingSynchronizer = new();
#endif
#endif

	/// <summary>
	/// Indicates the backing solver.
	/// </summary>
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[EditorBrowsable(EditorBrowsableState.Never)]
#if SYNC_ROOT_VIA_THREAD_LOCAL
	private static readonly ThreadLocal<BitwiseSolver> Solver = new(static () => new());
#else
	private static readonly BitwiseSolver Solver = new();
#endif


	/// <summary>
	/// Indicates whether the puzzle is valid (solved or a normal puzzle with a unique solution).
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool GetIsValid(this scoped in Grid @this) => @this.IsSolved || @this.GetUniqueness() == Uniqueness.Unique;

	/// <summary>
	/// Checks the uniqueness of the current sudoku puzzle.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has already been solved.</exception>
#if SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.Synchronized)]
#endif
	public static Uniqueness GetUniqueness(this scoped in Grid @this)
	{
		if (@this.IsSolved)
		{
			throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("CannotSolveAPuzzleAlreadySolved"));
		}

		long count;
#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
		lock (PuzzleSolvingSynchronizer)
#endif
		{
			count = Solver
#if SYNC_ROOT_VIA_THREAD_LOCAL
				.Value!
#endif
				.SolveString(@this.ToString(), out _, 2);
		}

		return count switch { 0 => Uniqueness.Bad, 1 => Uniqueness.Unique, _ => Uniqueness.Multiple };
	}

	/// <summary>
	/// Indicates the solution of the current grid. If the puzzle has no solution or multiple solutions,
	/// this property will return <see cref="Grid.Undefined"/>.
	/// </summary>
	/// <seealso cref="Grid.Undefined"/>
#if SYNC_ROOT_VIA_METHODIMPL && !SYNC_ROOT_VIA_OBJECT
	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.Synchronized)]
#endif
	public static Grid GetSolutionGrid(this scoped in Grid @this)
	{
#if SYNC_ROOT_VIA_OBJECT && !SYNC_ROOT_VIA_METHODIMPL
		lock (PuzzleSolvingSynchronizer)
#endif
		{
			return Solver
#if SYNC_ROOT_VIA_THREAD_LOCAL
				.Value!
#endif
				.Solve(in @this) is { IsUndefined: false } solution ? unfix(in solution, @this.GivenCells) : Grid.Undefined;
		}


		static Grid unfix(ref readonly Grid solution, ref readonly CellMap pattern)
		{
			var result = solution;
			foreach (var cell in ~pattern)
			{
				if (result.GetState(cell) == CellState.Given)
				{
					result.SetState(cell, CellState.Modifiable);
				}
			}
			return result;
		}
	}
}
