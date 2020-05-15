using System;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// <para>
	/// To mark on a technique searcher (<see cref="TechniqueSearcher"/>) to indicate the searcher
	/// is used for finding all deadly patterns.
	/// </para>
	/// <para>
	/// This attribute is used when the puzzle is a sukaku, they will be disabled.
	/// </para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class UniquenessSearcherAttribute : Attribute
	{
	}
}
