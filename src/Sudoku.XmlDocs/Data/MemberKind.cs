using System.Diagnostics.CodeAnalysis;

namespace Sudoku.XmlDocs.Data
{
	/// <summary>
	/// Provides a member kind.
	/// </summary>
	[Closed]
	public enum MemberKind
	{
		/// <summary>
		/// Indicates the member is a field.
		/// </summary>
		Field,

		/// <summary>
		/// Indicates the member is a constructor.
		/// </summary>
		Constructor,

		/// <summary>
		/// Indicates the member is a primary constructor. This member now only appears
		/// in the <see langword="record"/>s.
		/// </summary>
		PrimaryConstructor,

		/// <summary>
		/// Indicates the member is a property.
		/// </summary>
		Property,

		/// <summary>
		/// Indicates the member is an indexer.
		/// </summary>
		Indexer,

		/// <summary>
		/// Indicates the member is an event.
		/// </summary>
		Event,

		/// <summary>
		/// Indicates the member is a method.
		/// </summary>
		Method,

		/// <summary>
		/// Indicates the member is an operator overloading.
		/// </summary>
		Operator,

		/// <summary>
		/// Indicates the member is an implicit or explicit cast.
		/// </summary>
		Cast,
	}
}
