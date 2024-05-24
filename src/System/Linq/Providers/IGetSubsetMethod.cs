namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>GetSubset</c>.
/// </summary>
/// <inheritdoc/>
public interface IGetSubsetMethod<TSelf, TSource> :
	ICountMethod<TSelf, TSource>,
	ICustomLinqMethod<TSelf, TSource>,
	IElementAtMethod<TSelf, TSource>
	where TSelf : IGetSubsetMethod<TSelf, TSource>
{
	/// <summary>
	/// Get all subsets from the specified number of the values to take.
	/// </summary>
	/// <param name="subsetSize">The number of elements you want to take.</param>
	/// <returns>
	/// The subsets of the list.
	/// For example, if the input array is <c>[1, 2, 3]</c> and the argument <paramref name="subsetSize"/> is 2, the result will be
	/// <code><![CDATA[
	/// [[1, 2], [1, 3], [2, 3]]
	/// ]]></code>
	/// 3 cases.
	/// </returns>
	public virtual IEnumerable<TSource[]> GetSubsets(int subsetSize)
	{
		if (subsetSize == 0)
		{
			return [];
		}

		var result = new List<TSource[]>();
		var Length = Count();
		g(Length, subsetSize, subsetSize, stackalloc int[subsetSize], (TSelf)this, result);
		return result;


		static void g(int last, int count, int index, Span<int> indexSequence, TSelf @this, List<TSource[]> resultList)
		{
			for (var i = last; i >= index; i--)
			{
				indexSequence[index - 1] = i - 1;
				if (index > 1)
				{
					g(i - 1, count, index - 1, indexSequence, @this, resultList);
				}
				else
				{
					var temp = new TSource[count];
					for (var j = 0; j < indexSequence.Length; j++)
					{
						temp[j] = @this.ElementAt(indexSequence[j]);
					}

					resultList.Add(temp);
				}
			}
		}
	}
}
