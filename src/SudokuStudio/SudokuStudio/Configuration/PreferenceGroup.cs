namespace SudokuStudio.Configuration;

/// <summary>
/// Defines a preference group.
/// </summary>
public abstract class PreferenceGroup : ICloneable<PreferenceGroup>, INotifyPropertyChanged
{
	/// <inheritdoc/>
	public abstract event PropertyChangedEventHandler? PropertyChanged;


	/// <inheritdoc/>
	public abstract PreferenceGroup Clone();

	/// <summary>
	/// Try to cover the preference from the specified instance <paramref name="other"/>.
	/// </summary>
	/// <param name="other">The newer instance that is used for covering the current instance.</param>
	public abstract void CoverBy(PreferenceGroup other);
}
