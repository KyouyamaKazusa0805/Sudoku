namespace SudokuStudio.Interaction;

/// <summary>
/// Defines a self-reporting progress type.
/// </summary>
/// <param name="handler"><inheritdoc cref="Progress{T}.Progress(Action{T})" path="/param[@name='handler']"/></param>
/// <typeparam name="TProgressDataProvider"><inheritdoc cref="Progress{T}" path="/typeparam[@name='T']"/></typeparam>
internal sealed class SelfReportingProgress<TProgressDataProvider>(Action<TProgressDataProvider> handler) : Progress<TProgressDataProvider>(handler)
	where TProgressDataProvider : struct, IEquatable<TProgressDataProvider>, IProgressDataProvider<TProgressDataProvider>
{
	/// <inheritdoc cref="Progress{T}.OnReport(T)"/>
	public void Report(TProgressDataProvider value) => OnReport(value);
}
