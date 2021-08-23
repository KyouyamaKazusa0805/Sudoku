namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Defines a step that can be distinctable.
/// </summary>
/// <typeparam name="TClass">The type of the element to compare.</typeparam>
/// <remarks>
/// A <b>distinctable step</b> is a step that is with the unique information, in order that
/// multiple steps can be distinctable.
/// </remarks>
public interface IDistinctableStep<in TClass> where TClass : class
{
	/// <summary>
	/// To compare two <typeparamref name="TClass"/>-typed elements, to check whether
	/// two elements are same.
	/// </summary>
	/// <param name="other">Other element to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the elements are same.
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>Two elements are same.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>Two elements holds the different values.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// The method can be the same implemented as the method <see cref="object.Equals(object?)"/>,
	/// but <see langword="record"/>s are automatically implemented the method, which is useless
	/// and unmeaningful.
	/// </remarks>
	bool IsSameAs(TClass other);

	/// <summary>
	/// Same as <see cref="IsSameAs(TClass)"/>, but with nullable annotation.
	/// </summary>
	/// <param name="other"><inheritdoc cref="IsSameAs(TClass)"/></param>
	/// <returns><inheritdoc cref="IsSameAs(TClass)"/></returns>
	/// <seealso cref="IsSameAs(TClass)"/>
	bool NullableIsSameAs(TClass? other);
}
