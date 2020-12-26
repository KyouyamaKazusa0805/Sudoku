namespace System.Extensions
{
	/// <summary>
	/// Provides extension method on <see cref="Type"/>.
	/// </summary>
	/// <seealso cref="Type"/>
	public static class TypeEx
	{
		/// <summary>
		/// Determine whether the type is the subclass of the specified one.
		/// </summary>
		/// <typeparam name="TClass">The specified type to check.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The type to check.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public static bool IsSubclassOf<TClass>(this Type @this) where TClass : class? =>
			@this.IsSubclassOf(typeof(TClass));
	}
}
