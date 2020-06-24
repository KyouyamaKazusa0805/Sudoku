namespace Sudoku.Data
{
	/// <summary>
	/// Indicates a link type.
	/// </summary>
	public enum LinkType : byte
	{
		/// <summary>
		/// Indicates the default link (<c>off</c> -&gt; <c>off</c> or <c>on</c> -&gt; <c>on</c>).
		/// </summary>
		[Name(" -> ")]
		Default,

		/// <summary>
		/// Indicates the weak link (<c>on</c> -&gt; <c>off</c>).
		/// </summary>
		[Name(" -- ")]
		Weak,

		/// <summary>
		/// Indicates the strong link (<c>off</c> -&gt; <c>on</c>).
		/// </summary>
		[Name(" == ")]
		Strong
	}
}
