namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports method group <c>Chunk</c>.
/// </summary>
/// <inheritdoc/>
public interface IChunkMethod<TSelf, TSource> : ILinqMethod<TSelf, TSource>
	where TSelf :
		IChunkMethod<TSelf, TSource>
#if NET9_0_OR_GREATER
		,
		allows ref struct
#endif
{
	/// <inheritdoc cref="Enumerable.Chunk{TSource}(IEnumerable{TSource}, int)"/>
	public virtual IEnumerable<TSource[]> Chunk(int size)
	{
		return size > 0 ? [.. produceValues()] : throw new InvalidOperationException();


		IEnumerable<TSource[]> produceValues()
		{
			var list = new List<TSource>(size);
			foreach (var element in this)
			{
				list.Add(element);
				if (list.Count == size)
				{
					yield return list.ToArray();
					list.Clear();
				}
			}
			if (list.Count == 0)
			{
				yield break;
			}

			yield return list.ToArray();
		}
	}
}
