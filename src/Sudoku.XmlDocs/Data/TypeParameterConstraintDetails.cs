namespace Sudoku.XmlDocs.Data
{
	/// <summary>
	/// Indicates a type parameter constraint details.
	/// </summary>
	public readonly struct TypeParameterConstraintDetails
	{
		/// <summary>
		/// Initializes an instance with the specified type parameter name, base types or constraints.
		/// </summary>
		/// <param name="typeParameter">Indicates the type parameter name.</param>
		/// <param name="base">Indicates the base types.</param>
		/// <param name="constraints">Indicates the constraints.</param>
		public TypeParameterConstraintDetails(string? typeParameter, string[]? @base, Constraints constraints)
		{
			TypeParameter = typeParameter;
			BaseTypes = @base;
			Constraints = constraints;
		}


		/// <summary>
		/// Indicates the type parameter name.
		/// </summary>
		public string? TypeParameter { get; init; }

		/// <summary>
		/// Indicates the base <see langword="class"/>es derived, <see langword="interface"/>s implemented
		/// or generic type parameters.
		/// </summary>
		public string[]? BaseTypes { get; init; }

		/// <summary>
		/// Indicates the constraints.
		/// </summary>
		public Constraints Constraints { get; init; }
	}
}
