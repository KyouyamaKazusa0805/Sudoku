namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a user-defined program preference.
/// </summary>
public sealed class ProgramPreference
{
	/// <inheritdoc cref="LogicalAnalysisPreference"/>
	public LogicalAnalysisPreference LogicalAnalysisPreferences { get; set; } = new();

	/// <inheritdoc cref="Configuration.ColorScheme"/>
	public ColorScheme ColorScheme { get; set; } = new();


	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="new"/>.
	/// </summary>
	/// <param name="new">The newer instance that is used for covering the current instance.</param>
	/// <exception cref="NotSupportedException">Throws when the property type is not supported to be serialized, or not found.</exception>
	public void CoverBy(ProgramPreference @new)
	{
		const string error_NotFound = "One of two possible property values cannot be found in target property group type.";
		const string error_NotSupported = "Target property is not supported to be directly cloned. See inner exception to learn more information.";

		foreach (var propertyInfo in typeof(ProgramPreference).GetProperties())
		{
			try
			{
				f(this).CoverBy(f(@new));
			}
			catch (RuntimeBinderException ex)
			{
				throw new NotSupportedException(error_NotSupported, ex);
			}


			PreferenceGroup f(ProgramPreference p)
				=> propertyInfo.GetValue(p) switch { PreferenceGroup t => t, _ => throw new NotSupportedException(error_NotFound) };
		}
	}
}
