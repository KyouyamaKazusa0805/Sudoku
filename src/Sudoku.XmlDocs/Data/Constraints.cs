using System;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.XmlDocs.Data
{
	/// <summary>
	/// Indicates the type parameter constraints.
	/// </summary>
	[Closed, Flags]
	public enum Constraints
	{
		/// <summary>
		/// Indicates the type parameter doesn't contain any constraints.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the base <see langword="class"/>, <see langword="interface"/> or
		/// type parameter.
		/// </summary>
		Base = 1,

		/// <summary>
		/// Indicates the <see langword="class"/> constraint.
		/// </summary>
		ClassNotNull = 2,

		/// <summary>
		/// Indicates the <see langword="class"/>? constraint.
		/// </summary>
		ClassMaybeNull = 4,

		/// <summary>
		/// Indicates the <see langword="struct"/> constraint.
		/// </summary>
		StructNotNull = 8,

		/// <summary>
		/// Indicates the <see langword="struct"/>? constraint.
		/// </summary>
		StructMaybeNull = 16,

		/// <summary>
		/// Indicates the <see langword="notnull"/> constraint.
		/// </summary>
		NotNull = 32,

		/// <summary>
		/// Indicates the <see langword="unmanaged"/> constraint.
		/// </summary>
		Unmanaged = 64,

		/// <summary>
		/// Indicates the <see cref="System.Delegate"/> constraint.
		/// </summary>
		/// <seealso cref="System.Delegate"/>
		Delegate = 128,

		/// <summary>
		/// Indicates the <see cref="System.Enum"/> constraint.
		/// </summary>
		/// <seealso cref="System.Enum"/>
		Enum = 256,

		/// <summary>
		/// Indicates the <see langword="default"/> constraint.
		/// </summary>
		Default = 512
	}
}
