using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO;
using System.Text.RegularExpressions;

namespace Sudoku.Bot.Resources
{
	/// <summary>
	/// Indicates the text resources getter.
	/// </summary>
	internal sealed class TextResources : DynamicObject
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
		/// </list>
		/// </para>
		/// <para>
		/// For example,
		/// if you want to get the <see cref="string"/> value from the key <c>"Bug"</c>, now you may
		/// write <c>Current.Bug</c> or <c>Current["Bug"]</c> to get that value.
		/// </para>
		/// </remarks>
		public static readonly dynamic Current = new TextResources("Resources.dic");


		/// <summary>
		/// Indicates the memory stream to store the key-value pairs.
		/// </summary>
		private readonly string _resourceDictionary;


		/// <summary>
		/// Initializes an instance with the specified resource file path.
		/// </summary>
		/// <param name="path">The path.</param>
		private TextResources(string path) => _resourceDictionary = File.ReadAllText(path);


		/// <inheritdoc/>
		public override bool TryGetMember(GetMemberBinder binder, out object? result)
		{
			bool r = TryGetValue(binder.Name, out string? m);
			result = m;
			return r;
		}

		/// <inheritdoc/>
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
		{
			if (indexes is null || indexes.Length == 0 || indexes[0] is not string index)
			{
				result = null;
				return false;
			}

			bool r = TryGetValue(index, out string? m);
			result = m;
			return r;
		}

		/// <summary>
		/// Try to get the value via the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="result">The result.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		private bool TryGetValue(string key, [NotNullWhen(true)] out string? result)
		{
			var match = Regex.Match(_resourceDictionary, $@"""{key}""\:\s*(""([^""]+)""?)");
			if (match.Success)
			{
				// Temporary solution: The path always incorrect.
				result = match.Groups[2].Value.Replace(@"\\", @"\");
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}
	}
}
