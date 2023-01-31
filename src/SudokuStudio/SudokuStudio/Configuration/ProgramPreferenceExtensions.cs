namespace SudokuStudio.Configuration;

/// <summary>
/// Provides with extension methods on <see cref="ProgramPreference"/>.
/// </summary>
/// <seealso cref="ProgramPreference"/>
public static class ProgramPreferenceExtensions
{
	/// <summary>
	/// Try to create a new instance that contains same values with the current instance.
	/// </summary>
	/// <param name="this">The instance.</param>
	/// <returns>The copied one.</returns>
	public static ProgramPreference Clone(this ProgramPreference @this)
	{
		var result = new ProgramPreference();
		foreach (var fieldInfo in typeof(ProgramPreference).GetFields())
		{
			fieldInfo.SetValue(result, fieldInfo.GetValue(@this));
		}

		return result;
	}

	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="new"/>.
	/// </summary>
	/// <param name="this">The current instance to be covered.</param>
	/// <param name="new">The newer instance that is used for covering the current instance.</param>
	public static void Cover(this ProgramPreference @this, ProgramPreference @new)
	{
		foreach (var fieldInfo in typeof(ProgramPreference).GetFields())
		{
			fieldInfo.SetValue(@this, fieldInfo.GetValue(@new));
		}
	}
}
