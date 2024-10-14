namespace System.IO;

/// <summary>
/// Provides with extension methods on <see cref="StreamReader"/>.
/// </summary>
/// <seealso cref="StreamReader"/>
public static class StreamReaderExtensions
{
	/// <summary>
	/// Determines whether a file ends with new line character.
	/// </summary>
	/// <param name="this">The <see cref="StreamReader"/> instance.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks><i>
	/// This method only supports for Windows now. For other OS platforms, this method cannot determine the end line characters
	/// because I have already forgotten them...
	/// </i></remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SupportedOSPlatform(PlatformNames.Windows)]
	public static bool EndsWithNewLine(this StreamReader @this)
	{
		// If the length are not enough to get two characters, just return false.
		if (@this.BaseStream.Length < 2)
		{
			return false;
		}

		// Move pointer to the last position, and revert 2 characters to check what the last two characters are.
		@this.BaseStream.Seek(-2, SeekOrigin.End);

		// Check for the two.
		return ((char)@this.Read(), (char)@this.Read()) is ('\r', '\n');
	}
}
