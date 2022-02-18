namespace Sudoku.UI.Drawing.Shapes;

partial class DrawingElementBag
{
	/// <summary>
	/// Gets the first element that satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>The result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when no elements satisfy the specified condition.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElement First(Predicate<DrawingElement> predicate)
	{
		foreach (var de in this)
		{
			if (predicate(de))
			{
				return de;
			}
		}

		throw new InvalidOperationException("Cannot found any element that satisfies the specified condition.");
	}

	/// <summary>
	/// Gets the first element that satisfies the specified condition.
	/// </summary>
	/// <param name="predicate">The condition.</param>
	/// <returns>The result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DrawingElement? FirstOrDefault(Predicate<DrawingElement> predicate)
	{
		foreach (var de in this)
		{
			if (predicate(de))
			{
				return de;
			}
		}

		return null;
	}

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

	/// <summary>
	/// Makes a filerting that removes the elements not satisfied the specified condition.
	/// </summary>
	/// <param name="predicate">The filtering condition method.</param>
	/// <returns>
	/// The enumerator that allows you using <see langword="where"/> clause to filter each element,
	/// but you cannot use <see langword="select"/> clause as the continuation to make the projection
	/// to another typed instance.
	/// </returns>
	/// <remarks>
	/// The method can be used by the following two ways:
	/// <list type="number">
	/// <item>
	/// Using query expression syntax: <c>var controls = from e in list where e is CellDigit select e;</c>.
	/// </item>
	/// <item>
	/// Using method invocation syntax: <c>var controls = list.Where(static e => e is CellDigit);</c>.
	/// </item>
	/// </list>
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public WhereEnumerator Where(Predicate<DrawingElement> predicate) => new(_elements, Count, predicate);

	/// <summary>
	/// Makes a filerting that removes the elements not satisfied the specified condition.
	/// </summary>
	/// <param name="predicate">The filtering condition method.</param>
	/// <returns>The enumerator instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe WhereEnumerator Where(delegate*<DrawingElement, bool> predicate) =>
		new(_elements, Count, predicate);
}
