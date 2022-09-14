namespace Microsoft.UI.Xaml.Documents;

/// <summary>
/// Provides extension methods on <see cref="Run"/>.
/// </summary>
/// <seealso cref="Run"/>
public static class RunExtensions
{
	/// <summary>
	/// Sets the property <see cref="Run.Text"/> with the specified value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Run WithText(this Run @this, string text)
	{
		@this.Text = text;
		return @this;
	}
}
