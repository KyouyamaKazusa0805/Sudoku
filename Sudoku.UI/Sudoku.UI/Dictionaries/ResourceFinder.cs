using System.Collections.Generic;
using System.Dynamic;
using Microsoft.UI.Xaml;

namespace Sudoku.UI.Dictionaries
{
	/// <summary>
	/// Provides the methods on fetching the text resources in merged dictionaries.
	/// </summary>
	/// <remarks>
	/// If you want to fetch a value, please use the code:
	/// <list type="bullet">
	/// <item><c>ResourceFinder.Current.PropertyName</c></item>
	/// <item><c>ResourceFinder.Current["PropertyName"]</c></item>
	/// </list>
	/// Both are fine. If you are stuck in the operation, please see the doc of that
	/// <see langword="public static readonly dynamic"/> field called <see cref="Current"/>.
	/// </remarks>
	/// <seealso cref="Current"/>
	public sealed class ResourceFinder : DynamicObject
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
		public static readonly dynamic Current = new ResourceFinder();


		/// <summary>
		/// Indicates a <see langword="private"/> constructor that can't be called outside the class.
		/// </summary>
		private ResourceFinder()
		{
		}


		/// <inheritdoc/>
		public override bool TryGetMember(GetMemberBinder binder, out object? result)
		{
			try
			{
				result = Application.Current.Resources[binder.Name];
				return true;
			}
			catch (KeyNotFoundException)
			{
				result = null;
				return false;
			}
		}

		/// <inheritdoc/>
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
		{
			if (indexes is null || indexes.Length == 0 || indexes[0] is not string index)
			{
				result = null;
				return false;
			}

			try
			{
				result = Application.Current.Resources[index];
				return true;
			}
			catch (KeyNotFoundException)
			{
				result = null;
				return false;
			}
		}
	}
}
