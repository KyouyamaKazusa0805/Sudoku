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
	public static void CoverBy(this ProgramPreference @this, ProgramPreference @new)
	{
#if true
		foreach (var fieldInfo in typeof(ProgramPreference).GetFields())
		{
			fieldInfo.SetValue(@this, fieldInfo.GetValue(@new));
		}
#else
		/**
			<para>
			This <see langword="foreach"/> loop uses a lost keyword <see langword="__makeref"/>,
			which will return an instance of type <see cref="TypedReference"/>, indicating the referenced information of the target object,
			which is helpful to assign values if the containing type is a <see langword="struct"/>.	
			</para>
			<para>
			Please note that it may produce an implicit boxing behavior if we assign a value-typed instance
			into a reference-typed object that is compatible with it.
			</para>
		*/
		foreach (var fieldInfo in typeof(ProgramPreference).GetFields())
		{
			fieldInfo.SetValueDirect(__makeref(@this), fieldInfo.GetValueDirect(__makeref(@new))!);
		}
#endif
	}
}
