namespace Sudoku.Diagnostics.Permissions
{
	/// <summary>
	/// Provides an access level.
	/// </summary>
	public enum AccessLevel : byte
	{
		/// <summary>
		/// The private modifier.
		/// </summary>
		Private,
		/// <summary>
		/// The private protected modifier.
		/// </summary>
		PrivateProtected,
		/// <summary>
		/// The protected modifier.
		/// </summary>
		Protected,
		/// <summary>
		/// The internal modifier.
		/// </summary>
		Internal,
		/// <summary>
		/// The protected internal modifier.
		/// </summary>
		ProtectedInternal,
		/// <summary>
		/// The public modifier.
		/// </summary>
		Public
	}
}
