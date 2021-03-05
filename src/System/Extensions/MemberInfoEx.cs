using System.Reflection;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="MemberInfo"/>.
	/// </summary>
	/// <seealso cref="MemberInfo"/>
	public static class MemberInfoEx
	{
		/// <summary>
		/// Indicates whether custom attributes of a specified type are applied to a specified member.
		/// </summary>
		/// <typeparam name="TAttribute">The type of that attribute.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The member information instance.</param>
		/// <returns>
		/// <see langword="true"/> if an attribute of the specified type is applied to <paramref name="this"/>;
		/// otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsDefined<TAttribute>(this MemberInfo @this) where TAttribute : Attribute =>
			@this.IsDefined(typeof(TAttribute));
	}
}
