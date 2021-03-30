
using System;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.XmlDocs
{
	/// <summary>
	/// Indicates the custom modifier.
	/// </summary>
	[Flags, Closed]
	public enum CustomModifier : short
	{
		/// <summary>
		/// Indicates the modifier is none.
		/// </summary>
		None = 0,

		/// <summary>
		/// Indicates the <see langword="abstract"/> modifier.
		/// </summary>
		Abstract = 1,

		/// <summary>
		/// Indicates the <see langword="async"/> modifier.
		/// </summary>
		Async = 2,

		/// <summary>
		/// Indicates the <see langword="const"/> modifier.
		/// </summary>
		Const = 4,

		/// <summary>
		/// Indicates the <see langword="extern"/> modifier.
		/// </summary>
		Extern = 8,

		/// <summary>
		/// Indicates the <see langword="in"/> modifier.
		/// </summary>
		In = 16,

		/// <summary>
		/// Indicates the <see langword="new"/> modifier.
		/// </summary>
		New = 32,

		/// <summary>
		/// Indicates the <see langword="out"/> modifier.
		/// </summary>
		Out = 64,

		/// <summary>
		/// Indicates the <see langword="override"/> modifier.
		/// </summary>
		Override = 128,

		/// <summary>
		/// Indicates the <see langword="readonly"/> modifier.
		/// </summary>
		ReadOnly = 256,

		/// <summary>
		/// Indicates the <see langword="sealed"/> modifier.
		/// </summary>
		Sealed = 512,

		/// <summary>
		/// Indicates the <see langword="static"/> modifier.
		/// </summary>
		Static = 1024,

		/// <summary>
		/// Indicates the <see langword="unsafe"/> modifier.
		/// </summary>
		Unsafe = 2048,

		/// <summary>
		/// Indicates the <see langword="virtual"/> modifier.
		/// </summary>
		Virtual = 4096,

		/// <summary>
		/// Indicates the <see langword="volatile"/> modifier.
		/// </summary>
		Volatile = 8192,

		/// <summary>
		/// Indicates the <see langword="ref"/> modifier.
		/// </summary>
		Ref = 16384,

		/// <summary>
		/// Indicates all modififers.
		/// </summary>
		All = 32767
	}
}
