using System;
using System.Collections.Generic;
using System.Extensions;
using System.Linq;
using System.Reflection;
using Sudoku.Data;
using Sudoku.Resources;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates a step finder that used in solving in <see cref="ManualSolver"/>.
	/// </summary>
	/// <seealso cref="ManualSolver"/>
	public abstract class StepSearcher : IStepSearcher
	{
		/// <summary>
		/// Indicates the necessary property name.
		/// </summary>
		private const string NecessaryPropertyName = "Properties";


		/// <summary>
		/// Indicates all step searchers and their type info used in the current solution.
		/// </summary>
		/// <remarks>
		/// Please note that the return value is a list of elements that contain its type and its
		/// searcher properties.
		/// </remarks>
		public static IEnumerable<(Type CurrentType, string SearcherName, TechniqueProperties Properties)> AllStepSearchers =>
			from type in Assembly.GetExecutingAssembly().GetTypes()
			where !type.IsAbstract && type.IsSubclassOf<StepSearcher>() && !type.IsDefined<ObsoleteAttribute>()
			orderby TechniqueProperties.FromType(type)!.Priority
			let v = type.GetProperty(NecessaryPropertyName, BindingFlags.Public | BindingFlags.Static)
			let casted = v?.GetValue(null) as TechniqueProperties
			where casted is not null && !casted.DisabledReason.Flags(DisabledReason.HasBugs)
			select (type, (string)TextResources.Current[$"Progress{casted.DisplayLabel}"], casted);


		/// <inheritdoc/>
		public abstract void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid);

		/// <summary>
		/// Take a technique step after searched all solving steps.
		/// </summary>
		/// <param name="grid">The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public StepInfo? GetOne(in SudokuGrid grid)
		{
			var bag = new NotifyChangedList<StepInfo>();
			bag.ElementAdded += static (_, _) => throw new InvalidOperationException(nameof(GetOne));

			try
			{
				GetAll(bag, grid);
			}
			catch (InvalidOperationException ex) when (ex.Message == nameof(GetOne))
			{
			}

			return bag.FirstOrDefault();
		}
	}
}
