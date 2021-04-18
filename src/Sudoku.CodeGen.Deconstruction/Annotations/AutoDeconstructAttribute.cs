using System;

namespace Sudoku.CodeGen.Deconstruction.Annotations
{
	/// <summary>
	/// To mark on a <see langword="class"/>, <see langword="struct"/> or a <see langword="interface"/>,
	/// to describe this type is deconstructable.
	/// </summary>
	/// <remarks>
	/// Please note that all <see langword="record"/>s have already contained a deconstruction method
	/// that deconstruct all members declared in the primary constructor.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class AutoDeconstructAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified members.
		/// </summary>
		/// <param name="members">The members to deconstruct.</param>
		public AutoDeconstructAttribute(params string[]? members) => FieldsOrProperties = members;


		/// <summary>
		/// Indicates the field or property names of this type to deconstruct.
		/// </summary>
		public string[]? FieldsOrProperties { get; }
	}
}
