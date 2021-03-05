namespace Sudoku.Data
{
	/// <summary>
	/// Indicates a link type.
	/// </summary>
	/*closed*/
	public enum LinkType : byte
	{
		/// <summary>
		/// Indicates the default link (<c>off</c> -&gt; <c>off</c> or <c>on</c> -&gt; <c>on</c>).
		/// </summary>
		Default,

		/// <summary>
		/// Indicates the weak link (<c>on</c> -&gt; <c>off</c>).
		/// </summary>
		Weak,

		/// <summary>
		/// Indicates the strong link (<c>off</c> -&gt; <c>on</c>).
		/// </summary>
		Strong,

		/// <summary>
		/// Indicates the link is used for rendering as a normal line (without start and end node).
		/// </summary>
		Line,
	}
}
