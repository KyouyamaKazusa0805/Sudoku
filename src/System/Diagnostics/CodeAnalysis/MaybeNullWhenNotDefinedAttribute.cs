namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the return value may be <see langword="null"/> when the specified enumeration-typed
	/// argument isn't defined.
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public sealed class MaybeNullWhenNotDefinedAttribute : Attribute
	{
		/// <summary>
		/// Initializes the <see cref="MaybeNullWhenNotDefinedAttribute"/> instance
		/// with a specified argument name.
		/// </summary>
		/// <param name="argumentName">The argument name.</param>
		public MaybeNullWhenNotDefinedAttribute(string argumentName) => ArgumentName = argumentName;


		/// <summary>
		/// Indicates the argument name that is of an enumeration type.
		/// </summary>
		public string ArgumentName { get; }
	}
}
