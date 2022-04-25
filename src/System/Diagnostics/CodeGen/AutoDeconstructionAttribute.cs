using static System.AttributeTargets;

namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for controlling the source generation on deconstruction methods.
/// </summary>
[AttributeUsage(Class | Struct | Interface, AllowMultiple = true, Inherited = false)]
public sealed class AutoDeconstructionAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="AutoDeconstructionAttribute"/> instance
	/// via the specified member expressions you want to deconstruct.
	/// </summary>
	/// <param name="memberExpression">The name of the members you want to deconstruct.</param>
	/// <exception cref="ArgumentException">Throws when the argument is empty.</exception>
	public AutoDeconstructionAttribute(params string[] memberExpression)
		=> MemberExpression = memberExpression is []
			? throw new ArgumentException("You must set at least one instance to be deconstructed.")
			: memberExpression;


	/// <summary>
	/// <para>
	/// Indicates whether the source generator will generate the current deconstruction method as extension ones.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool GenerateAsExtension { get; init; }

	/// <summary>
	/// Indicates the suffix of the generated file. The property is available when the property
	/// <see cref="GenerateAsExtension"/> is <see langword="true"/>.
	/// </summary>
	public string GeneratedFileNameSuffix
	{
		get
		{
			var hashCode = new HashCode();
			foreach (string propertyName in MemberExpression)
			{
				hashCode.Add(propertyName);
			}

			return $"Extensions_{hashCode.ToHashCode():X}";
		}
	}

	/// <summary>
	/// Indicates the member names whose corresponding members will be able to be deconstructed.
	/// </summary>
	public string[] MemberExpression { get; }
}
