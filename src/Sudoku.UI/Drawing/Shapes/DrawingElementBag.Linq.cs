namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Gets all possible elements that are all of type <typeparamref name="TDrawingElement"/>.
	/// </summary>
	/// <typeparam name="TDrawingElement">The type of the elements to be iterated.</typeparam>
	/// <returns>
	/// The enumerator instance that allows you using <see langword="foreach"/> loop to iterate on them.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeEnumerator<TDrawingElement> OfType<TDrawingElement>() where TDrawingElement : DrawingElement =>
		new(_elements, Count);

	/// <summary>
	/// Gets all possible elements that are all of either type <typeparamref name="T1"/>
	/// or <typeparamref name="T2"/>.
	/// </summary>
	/// <typeparam name="T1">The first allowed type whose instances being iterated.</typeparam>
	/// <typeparam name="T2">The second allowed type whose instances being iterated.</typeparam>
	/// <returns>
	/// The enumerator instance that allows you using <see langword="foreach"/> loop to iterate on them.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeEnumerator<T1, T2> OfType<T1, T2>() where T1 : DrawingElement where T2 : DrawingElement =>
		new(_elements, Count);

	/// <summary>
	/// Gets all possible elements that are all of type <typeparamref name="T1"/>,
	/// <typeparamref name="T2"/> or <typeparamref name="T3"/>.
	/// </summary>
	/// <typeparam name="T1">The first allowed type whose instances being iterated.</typeparam>
	/// <typeparam name="T2">The second allowed type whose instances being iterated.</typeparam>
	/// <typeparam name="T3">The third allowed type whose instances being iterated.</typeparam>
	/// <returns>
	/// The enumerator instance that allows you using <see langword="foreach"/> loop to iterate on them.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OfTypeEnumerator<T1, T2, T3> OfType<T1, T2, T3>()
		where T1 : DrawingElement where T2 : DrawingElement where T3 : DrawingElement => new(_elements, Count);

	/// <summary>
	/// Makes a projection that converts each element to the target value of type <typeparamref name="T"/>,
	/// using the specified method to convert.
	/// </summary>
	/// <typeparam name="T">The type of the target result that each element converted.</typeparam>
	/// <param name="selector">The selector to convert the element.</param>
	/// <returns>
	/// The enumerator that allows you using <see langword="select"/> clause to get the result.
	/// </returns>
	/// <remarks>
	/// The method can be used by the following two ways:
	/// <list type="number">
	/// <item>
	/// Using query expression syntax: <c>var controls = from e in list select e.GetControl();</c>.
	/// </item>
	/// <item>
	/// Using method invocation syntax: <c>var controls = list.Select(static e => e.GetControl());</c>.
	/// </item>
	/// </list>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public SelectEnumerator<T> Select<T>(Func<DrawingElement, T> selector) => new(_elements, Count, selector);

	/// <summary>
	/// Makes a projection that converts each element to the target value of type <typeparamref name="T"/>,
	/// using the specified method to convert.
	/// </summary>
	/// <typeparam name="T">The type of the target result that each element converted.</typeparam>
	/// <param name="selector">The selector to convert the element.</param>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe SelectEnumerator<T> Select<T>(delegate*<DrawingElement, T> selector) =>
		new(_elements, Count, selector);
}
