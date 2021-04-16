using System;
using Sudoku.CodeGen.PrimaryConstructor.Annotations;

namespace Sudoku.CodeGen.StructParameterlessConstructor.Annotations
{
	/// <summary>
	/// Marks on a <see langword="struct"/>, to tell the users the parameterless constructor is disabled
	/// and can't be called or used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct)]
	[AutoGeneratePrimaryConstructor]
	public sealed partial class DisallowParameterlessConstructorAttribute : Attribute
	{
		/// <summary>
		/// Initializes a <see cref="DisallowParameterlessConstructorAttribute"/> instance
		/// with the default instantiation behavior.
		/// </summary>
		public DisallowParameterlessConstructorAttribute()
		{
		}


		/// <summary>
		/// Indicates the recommend member name.
		/// </summary>
		/// <remarks>
		/// This property holds the value that tells users which member they should use and replace with.
		/// </remarks>
		public string? RecommendMemberName { get; }
	}
}
