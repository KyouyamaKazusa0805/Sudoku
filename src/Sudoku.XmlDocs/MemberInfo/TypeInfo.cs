namespace Sudoku.XmlDocs.MemberInfo
{
	/// <summary>
	/// Indicates the information to introduce a type.
	/// </summary>
	/// <param name="Summary">Indicates the "summary" block in the documentation comments.</param>
	/// <param name="Remarks">Indicates the "remarks" block in the documentation comments.</param>
	/// <param name="TypeParameters">Indicates the "typeparam" block in the documentation comments.</param>
	/// <param name="Example">Indicates the "example" block in the documentation comments.</param>
	/// <param name="Exceptions">Indicates the "exception" block in the documentation comments.</param>
	/// <param name="SeeAlsos">Indicates the "seealso" block in the documentation comments.</param>
	public sealed record TypeInfo(
		string Summary,
		string Remarks,
		(string TypeParameter, string? Description)[]? TypeParameters,
		string Example,
		(string ExceptionName, string? Description)[]? Exceptions,
		string[]? SeeAlsos
	);
}
