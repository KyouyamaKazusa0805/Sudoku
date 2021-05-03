#pragma warning disable CS0626
#pragma warning disable CS8618

using System.Collections.Generic;
using Sudoku.Globalization;

namespace Sudoku.Resources
{
	/// <summary>
	/// Provides a way to get the local text resources, and save the resources to the local path.
	/// </summary>
	/// <remarks>
	/// <i>
	/// Please note that this type is implemented in another file which is invisible for us. The only way
	/// to see them is to find and open them in the file explorer. They're in the real folders but not
	/// in this solution.
	/// </i>
	/// </remarks>
	public sealed class TextResources
	{
		/// <summary>
		/// Indicates the singleton to get items and method invokes.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This is a <see langword="dynamic"/> instance, which means you can get anything you want
		/// using the following code style to get the items in this class:
		/// <list type="bullet">
		/// <item><c>Current.PropertyName</c> (Property invokes)</item>
		/// <item><c>Current[PropertyIndex]</c> (Indexer invokes)</item>
		/// <item><c>Current.MethodName(Parameters)</c> (Method invokes)</item>
		/// </list>
		/// </para>
		/// <para>
		/// For example,
		/// if you want to get the <see cref="string"/> value from the key <c>"Bug"</c>, now you may
		/// write <c>Current.Bug</c> or <c>Current["Bug"]</c> to get that value.
		/// Before the version 0.3, you must write the code <c>TextResources.GetValue("Bug")</c>.
		/// </para>
		/// <para>
		/// All supported methods are:
		/// <list type="bullet">
		/// <item><c>Current.Serialize(string instanceNameToSerialize, string toPath)</c></item>
		/// <item><c>Current.Deserialize(string instanceNameToDeserialize, string fromPath)</c></item>
		/// <item><c>Current.ChangeLanguage(CountryCode countryCode)</c></item>
		/// </list>
		/// </para>
		/// </remarks>
		public static readonly dynamic Current;


		/// <summary>
		/// Indicates the current country code.
		/// </summary>
		public extern CountryCode CountryCode { get; }

		/// <summary>
		/// The language source for the globalization string "<c>en-us</c>".
		/// </summary>
		public extern IDictionary<string, string>? LangSourceEnUs { get; }

		/// <summary>
		/// The language source for the globalization string "<c>zh-cn</c>".
		/// </summary>
		public extern IDictionary<string, string>? LangSourceZhCn { get; }


		/// <summary>
		/// Try to deserialize the file specified as a file path, and converts it to the instance.
		/// </summary>
		/// <param name="instanceNameToDeserialize">The instance to covert to.</param>
		/// <param name="path">The file path.</param>
		/// <returns>The <see cref="bool"/> value indicating whether the operation is successful.</returns>
		internal static extern bool Deserialize(string instanceNameToDeserialize, string path);
	}
}
