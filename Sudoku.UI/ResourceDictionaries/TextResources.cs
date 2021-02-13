using System.Dynamic;
using System.Windows;
using Sudoku.DocComments;

namespace Sudoku.UI.ResourceDictionaries
{
	/// <summary>
	/// Provides a way to get the local text resources, and save the resources to the local path.
	/// </summary>
	public sealed class TextResources : DynamicObject
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
		public static readonly dynamic CurrentApp = new TextResources();


		/// <see cref="DefaultConstructor"/>
		private TextResources()
		{
		}


		/// <inheritdoc/>
		public override bool TryGetMember(GetMemberBinder binder, out object? result) =>
			(result = TryGetValue(binder.Name)) is not null;

		/// <inheritdoc/>
		public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
		{
			if (indexes is null || indexes.Length == 0 || indexes[0] is not string index)
			{
				result = null;
				return false;
			}

			return (result = TryGetValue(index)) is not null;
		}


		/// <summary>
		/// Try to get the inner value.
		/// </summary>
		/// <param name="index">The index that you want to get. The index is a <see cref="string"/> key.</param>
		/// <returns>The result.</returns>
		private static object? TryGetValue(string index) => Application.Current.Resources[index];
	}
}
