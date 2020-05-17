using System;
using System.Reflection;
using Sudoku.Solving.Manual;
using static System.AttributeTargets;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// <para>
	/// To mark on a technique searcher class (i.e. <see cref="TechniqueSearcher"/>),
	/// to provide additional displaying messages which are used in UI forms, such as
	/// technique priority settings form.
	/// </para>
	/// <para>
	/// You can use this instance to mark on a enumeration field such as <see cref="TechniqueCode"/>
	/// to show the name on the screen also.
	/// </para>
	/// </summary>
	/// <seealso cref="TechniqueSearcher"/>
	/// <seealso cref="TechniqueCode"/>
	[AttributeUsage(Class | Field, Inherited = false)]
	public sealed class TechniqueDisplayAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the specified displaying name.
		/// </summary>
		/// <param name="displayName">The name.</param>
		public TechniqueDisplayAttribute(string displayName) => DisplayName = displayName;


		/// <summary>
		/// Indicates the display name of this technique.
		/// </summary>
		public string DisplayName { get; }

		/// <summary>
		/// <para>
		/// Indicates the category of this instance to display.
		/// This property is used for displaying when need grouping. The value is
		/// <see langword="null"/> in default case.
		/// </para>
		/// <para>
		/// The category will be displayed like <c>"Fishes&gt;Normal Fishes&gt;Finned"</c>,
		/// which used '<c>&gt;</c>' character to make a new group or a new sub-group.
		/// </para>
		/// </summary>
		public string? Category { get; set; }


		/// <summary>
		/// Get the display name of the specified enum field.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enum field.</typeparam>
		/// <param name="enumField">The enum field to check.</param>
		/// <returns>
		/// The display name. Return <see langword="null"/> when the field does not mark this attribute.
		/// </returns>
		public static string? GetDisplayName<TEnum>(TEnum enumField)
			where TEnum : Enum
		{
			var fieldInfo = typeof(TEnum).GetField(enumField.ToString());
			if (fieldInfo is null)
			{
				return null;
			}

			var attribute = fieldInfo.GetCustomAttribute<TechniqueDisplayAttribute>();
			if (attribute is null)
			{
				return null;
			}

			return attribute.DisplayName;
		}
	}
}
