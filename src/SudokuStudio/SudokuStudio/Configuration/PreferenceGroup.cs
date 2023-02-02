namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a preference group.
/// </summary>
public abstract class PreferenceGroup : INotifyPropertyChanged
{
	/// <inheritdoc/>
	public event PropertyChangedEventHandler? PropertyChanged;


	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="other"/>.
	/// </summary>
	/// <param name="other">The newer instance that is used for covering the current instance.</param>
	[DebuggerStepThrough]
	public void CoverBy(PreferenceGroup other)
	{
		foreach (var propertyInfo in GetType().GetProperties())
		{
			propertyInfo.SetValue(this, propertyInfo.GetValue(other));
		}
	}

	/// <summary>
	/// To trigger the event <see cref="PropertyChanged"/>.
	/// </summary>
	/// <param name="propertyName">
	/// The property name. You may not assign this property with the specified value;
	/// the value will be replaced with suitable value by compiler.
	/// </param>
	protected void TriggerPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new(propertyName));
}
