using System;
using System.Reflection;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// To mark on a enum field to give an alias.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class AliasAttribute : Attribute
	{
		/// <summary>
		/// Initializes an instance with the enum field.
		/// </summary>
		/// <param name="fieldName">The field name.</param>
		public AliasAttribute(string fieldName) => FieldName = fieldName;


		/// <summary>
		/// The field name.
		/// </summary>
		public string FieldName { get; }


		/// <summary>
		/// Convert the specified enum field to the specified type field.
		/// </summary>
		/// <typeparam name="TEnumBase">The base type of the enum field.</typeparam>
		/// <typeparam name="TEnumTarget">The target enum type to convert to.</typeparam>
		/// <param name="enumField">The enum field.</param>
		/// <returns>The result. Return <see langword="null"/> when the conversion is failed.</returns>
		public static TEnumTarget? Convert<TEnumBase, TEnumTarget>(TEnumBase enumField)
			where TEnumBase : Enum where TEnumTarget : struct, Enum =>
			typeof(TEnumBase).GetField(enumField.ToString()) switch
			{
				var fieldInfo and not null
				when fieldInfo.GetCustomAttribute<AliasAttribute>() is AliasAttribute attribute =>
					Enum.Parse<TEnumTarget>(attribute.FieldName),
				_ => default
			};
	}
}
