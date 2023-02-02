namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a preference group.
/// </summary>
public abstract class PreferenceGroup : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public abstract event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="other"/>.
	/// </summary>
	/// <param name="other">The newer instance that is used for covering the current instance.</param>
	[DebuggerStepThrough]
	[RequiresUnreferencedCode(RequiresCompilerInvocationMessage.CompilerInvocationOnly)]
	[Obsolete(RequiresCompilerInvocationMessage.CompilerInvocationOnly, false, DiagnosticId = "SCA0116")]
	public void CoverBy(PreferenceGroup other)
	{
		foreach (var propertyInfo in GetType().GetProperties())
		{
			propertyInfo.SetValue(this, propertyInfo.GetValue(other));
		}
	}
}
