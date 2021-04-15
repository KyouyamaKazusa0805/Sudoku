namespace Sudoku.XmlDocs.Data
{
	/// <summary>
	/// Indicates the type parameter details.
	/// </summary>
	public readonly struct ExceptionDetails
	{
		/// <summary>
		/// Initializes an instance with the specified exception name, and the description.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="description">The description.</param>
		public ExceptionDetails(string exception, string? description)
		{
			Exception = exception;
			Description = description;
		}


		/// <summary>
		/// Indicates the exception name.
		/// </summary>
		public string Exception { get; init; }

		/// <summary>
		/// Indicates the description.
		/// </summary>
		public string? Description { get; init; }
	}
}
