// Copyright (C) 2008-12 Bernhard Hobiger
// 
// HoDoKu is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// HoDoKu is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with HoDoKu. If not, see <http://www.gnu.org/licenses/>.
// 
// This code is actually a Java port of code posted by Glenn Fowler in the Sudoku Player's Forum (http://www.setbb.com/sudoku).
// Many thanks for letting me use it!

namespace Sudoku.Generating;

/// <summary>
/// One entry in recursion stack.
/// </summary>
internal sealed class GeneratorRecursionStackEntry
{
	/// <summary>
	/// The candidates for cells <see cref="Cell"/>.
	/// </summary>
	/// <seealso cref="Cell"/>
	public Mask Candidates;

	/// <summary>
	/// The index of the cell that's being tried.
	/// </summary>
	public Cell Cell;

	/// <summary>
	/// The index of the last tried candidate in <see cref="Candidates"/>.
	/// </summary>
	/// <seealso cref="Candidates"/>
	public Digit CandidateIndex;

	/// <summary>
	/// The current state of the sudoku.
	/// </summary>
	public Grid SudokuGrid = Grid.Empty;
}
