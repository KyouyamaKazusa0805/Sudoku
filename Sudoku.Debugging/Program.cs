using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

int count = 3;
var complexList = new Collection { 1, 20, 50, 100, 200, 500, 1000 };
if (complexList.Count() >= count) // SUDOKU019.
{
	Console.WriteLine($"{nameof(complexList)} contains at least {count} elements."); // SUDOKU016.
}

// Implementation sample.
class Collection : IEnumerable<int>
{
	private readonly IList<int> _list = new List<int>();

	public Collection() { }

	public void Add(int value) => _list.Add(value);
	public IEnumerator<int> GetEnumerator() => _list.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
}