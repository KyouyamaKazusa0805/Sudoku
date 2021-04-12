namespace Sudoku.XmlDocs.MemberInfo
{
	/// <summary>
	/// Indicates the type parameter details.
	/// </summary>
	public readonly struct TypeParameterDetails
	{
		/// <summary>
		/// Initializes an instance with the specified type parameter name, and the description.
		/// </summary>
		/// <param name="typeParameter">The type parameter.</param>
		/// <param name="description">The description.</param>
		public TypeParameterDetails(string typeParameter, string? description)
		{
			TypeParameter = typeParameter;
			Description = description;
		}


		/// <summary>
		/// Indicates the type parameter name.
		/// </summary>
		public string TypeParameter { get; init; }

		/// <summary>
		/// Indicates the description.
		/// </summary>
		public string? Description { get; init; }
	}
}
