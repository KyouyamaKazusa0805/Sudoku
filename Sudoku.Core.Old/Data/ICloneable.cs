using System;

namespace Sudoku.Data
{
	public interface ICloneable<T> : ICloneable where T : class
	{
		new T Clone();
	}
}
