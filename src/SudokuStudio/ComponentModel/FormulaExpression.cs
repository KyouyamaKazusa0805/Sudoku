namespace SudokuStudio.ComponentModel;

/// <summary>
/// Represents a formula expression.
/// </summary>
public sealed class FormulaExpression
{
	/// <summary>
	/// Indicates the name of the formula.
	/// </summary>
	public string Name { get; init; } = "";

	/// <summary>
	/// Indicates the file ID.
	/// </summary>
	public string FileId { get; init; } = "";

	/// <summary>
	/// Indicates the description of the formula.
	/// </summary>
	public string Description { get; init; } = "";

	/// <summary>
	/// Indicates the expression of the formula.
	/// </summary>
	public string Expression { get; init; } = "";

	/// <summary>
	/// Indicates the techniques that the expression can be applied to, if <see cref="Step.Code"/> is in the current set.
	/// </summary>
	/// <seealso cref="Step.Code"/>
	public TechniqueSet AppliedTechniques { get; init; } = TechniqueSets.None;


	/// <summary>
	/// Saves the file to local.
	/// </summary>
	public void SaveToLocal()
	{
		var filePath = $@"{CommonPaths.Formulae}\{FileId}{FileExtensions.UserFormulae}";
		File.WriteAllText(filePath, JsonSerializer.Serialize(this));
	}


	/// <summary>
	/// Loads the file from local path.
	/// </summary>
	/// <param name="filePath">The file to be loaded.</param>
	/// <param name="result">The result parsed.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	public static bool TryLoadFromLocal(string filePath, [NotNullWhen(true)] out FormulaExpression? result)
	{
		try
		{
			if (JsonSerializer.Deserialize<FormulaExpression>(File.ReadAllText(filePath)) is { } r
				&& r.FileId == io::Path.GetFileNameWithoutExtension(filePath))
			{
				result = r;
				return true;
			}
		}
		catch
		{
		}

		result = null;
		return false;
	}

	/// <summary>
	/// Loads the file from local path.
	/// </summary>
	/// <param name="filePath">The file to be loaded.</param>
	/// <returns>The instance.</returns>
	public static FormulaExpression LoadFromLocal(string filePath)
		=> TryLoadFromLocal(filePath, out var result)
			? result
			: throw new JsonException(ResourceDictionary.ExceptionMessage("ParseFailedOnFormula"));
}
