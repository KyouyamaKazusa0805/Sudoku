unsafe
{
	delegate*<int, bool> ptrIsOdd; // SUDOKU015.
	delegate* managed<int, bool> ptrIsEven;
	delegate* unmanaged[Cdecl]<nint*, nint, void> ptrBubbleSort;
	delegate* unmanaged[Cdecl, Stdcall]<nint*, nint, void> ptrSort;

	System.Console.WriteLine();
}
