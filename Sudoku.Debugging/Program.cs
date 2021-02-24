unsafe
{
	delegate*<int, bool> ptrIsEven = &isEven; // SUDOKU015.
}

static bool isEven(int val) => (val & 1) == 0;
