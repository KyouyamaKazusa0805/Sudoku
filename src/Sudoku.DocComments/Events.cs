#nullable enable annotations

using System;

namespace Sudoku.DocComments
{
	/// <summary>
	/// Provides doc comments on event-delegating methods.
	/// </summary>
	public sealed class Events
	{
		/// <summary>
		/// Trigger execution when a control is clicked.
		/// </summary>
		/// <param name="sender">The control to trigger <c>Click</c> event.</param>
		/// <param name="e">The event information, whose value is specified as a parameter.</param>
		public void Click(object? sender, EventArgs e) => throw new NotImplementedException();
	}
}
