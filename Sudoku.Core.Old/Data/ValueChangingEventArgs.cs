using System;
using Sudoku.Data.Meta;

namespace Sudoku.Data
{
	public class ValueChangingEventArgs : EventArgs
	{
		public ValueChangingEventArgs(Candidate candidate) => Candidate = candidate;


		public bool Cancel { get; set; }

		public Candidate Candidate { get; }
	}
}
