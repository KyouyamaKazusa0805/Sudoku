using System.Diagnostics.CodeAnalysis;

namespace Sudoku.XmlDocs.Data
{
	/// <summary>
	/// Indicates a type kind.
	/// </summary>
	[Closed]
	public enum TypeKind
	{
		/// <summary>
		/// Indicates the type is a <see langword="struct"/>.
		/// </summary>
		Struct,

		/// <summary>
		/// Indicates the type is a <see langword="class"/>.
		/// </summary>
		Class,

		/// <summary>
		/// Indicates the type is a <see langword="record class"/>.
		/// </summary>
		RecordClass,

		/// <summary>
		/// Indicates the type is a <see langword="record struct"/>.
		/// </summary>
		RecordStruct,

		/// <summary>
		/// Indicates the type is a <see langword="interface"/>.
		/// </summary>
		Interface
	}
}
