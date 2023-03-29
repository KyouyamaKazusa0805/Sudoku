namespace Sudoku.Workflow.Bot.Oicq.Drawing.EqualityComparers;

/// <summary>
/// 提供一个比较 <see cref="CellViewNode"/> 相等性的实例类型。
/// </summary>
public sealed class CellViewNodeComparer : IEqualityComparer<CellViewNode>
{
	/// <summary>
	/// 提供一个默认的实例。
	/// </summary>
	public static readonly CellViewNodeComparer Instance = new();


	/// <summary>
	/// 无参构造器。该构造器设置为 <see langword="private"/> 修饰是为了防止外部调用该构造器；因为我们这里为了考虑性能的问题，
	/// 使用到的是一个 <see langword="static readonly"/> 组合修饰的字段，它在程序运行期间只创建一次。
	/// </summary>
	private CellViewNodeComparer()
	{
	}


	/// <inheritdoc/>
	public bool Equals(CellViewNode? x, CellViewNode? y) => x!.Cell == y!.Cell;

	/// <inheritdoc/>
	public int GetHashCode(CellViewNode obj) => obj.Cell;
}
