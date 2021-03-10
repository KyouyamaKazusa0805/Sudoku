using Sudoku.DocComments;

namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Marks on a <see langword="struct"/>, to tell the users the parameterless constructor is disabled
	/// and can't be called or used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Struct)]
	public sealed class DisableParameterlessConstructorAttribute : Attribute
	{
		/// <inheritdoc cref="DefaultConstructor"/>
		public DisableParameterlessConstructorAttribute()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified recommend member name.
		/// </summary>
		/// <param name="recommendMemberName">The recommend member name.</param>
		public DisableParameterlessConstructorAttribute(string recommendMemberName) =>
			RecommendMemberName = recommendMemberName;


		/// <summary>
		/// Indicates the recommend member name.
		/// </summary>
		/// <remarks>
		/// This property holds the value that tells users which member they should use and replace with.
		/// </remarks>
		public string? RecommendMemberName { get; init; }
	}
}
