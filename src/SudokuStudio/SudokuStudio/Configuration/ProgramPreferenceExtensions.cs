namespace SudokuStudio.Configuration;

/// <summary>
/// Provides with extension methods on <see cref="ProgramPreference"/>.
/// </summary>
/// <seealso cref="ProgramPreference"/>
public static class ProgramPreferenceExtensions
{
	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="new"/>.
	/// </summary>
	/// <param name="this">The current instance to be covered.</param>
	/// <param name="new">The newer instance that is used for covering the current instance.</param>
	/// <exception cref="NotSupportedException">Throws when the property type is not supported to be serialized.</exception>
	public static void CoverBy(this ProgramPreference @this, ProgramPreference @new)
	{
		foreach (var propertyInfo in typeof(ProgramPreference).GetProperties())
		{
			try
			{
				var a = (dynamic?)propertyInfo.GetValue(@this);
				var b = (dynamic?)propertyInfo.GetValue(@new);
				if (a is null || b is null)
				{
					continue;
				}

				a.CoverBy(b);
			}
			catch (RuntimeBinderException ex)
			{
				throw new NotSupportedException(
					"Target property is not supported to be directly cloned. See inner exception to learn more information.",
					ex
				);
			}
		}
	}
}
