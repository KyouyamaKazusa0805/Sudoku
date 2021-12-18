#pragma warning disable CS0169, CS0649

using System;

Console.WriteLine("Hello, world!");

unsafe partial class Program
{
	public static readonly void* P;
	private static readonly delegate*<void> Q;
	public static readonly delegate*<void> R;

	public ref int GetPinnableReferece() => ref *(int*)null;
}