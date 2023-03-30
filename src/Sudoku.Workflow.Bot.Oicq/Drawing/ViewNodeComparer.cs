namespace Sudoku.Workflow.Bot.Oicq.Drawing;

/// <summary>
/// 提供一个比较 <see cref="ViewNode"/> 相等性的实例类型。
/// </summary>
/// <remarks>
/// <para>
/// 说实话我是很不想写这个类型的。这种设计有一个毛病就是，我每次更新一个 <see cref="ViewNode"/> 的创建，就得在这里新添加一个判断。
/// 这种设计很不好，尤其是在公司上班的时候，用这种设计。
/// 不过，因为 <see cref="ViewNode"/> 的设计不太理想，它的 <see cref="ViewNode.Equals(ViewNode?)"/> 方法比较的对象，包含了颜色标识符
/// <see cref="Identifier"/> 的判断。而实际上，我们在追加颜色到绘图面板上的时候，只判断单元格啊、候选数之类的，有没有重复使用的情况。
/// 如果有，就说明这里已经绘图的时候被用过了，就不再继续了；但是由于 <see cref="ViewNode"/> 的相等性比较包含了 <see cref="Identifier"/>，
/// 就导致这种“模糊的”判断反倒大概率不成立，于是就被视为不同的 <see cref="ViewNode"/> 并按两次计算在内。
/// </para>
/// <para>
/// 所以，我们为了避免这种比较 <see cref="Identifier"/> 的情况，我们不得不自己写一个比较器对象，来避免运行时 <see cref="HashSet{T}"/> 这种
/// 要使用 <see cref="IEqualityComparer{T}"/> 对象的比较的类型，自动去获取默认的 <c>Equals</c> 比较规则。
/// </para>
/// </remarks>
/// <seealso cref="ViewNode"/>
/// <seealso cref="ViewNode.Equals(ViewNode?)"/>
/// <seealso cref="Identifier"/>
/// <seealso cref="HashSet{T}"/>
/// <seealso cref="IEqualityComparer{T}"/>
/// <seealso cref="EqualityComparer{T}.Default"/>
/// <seealso cref="EqualityComparer{T}.Equals(T, T)"/>
internal sealed class ViewNodeComparer : IEqualityComparer<ViewNode>
{
	/// <summary>
	/// 提供一个默认的实例。
	/// </summary>
	public static readonly ViewNodeComparer Default = new();


	/// <summary>
	/// 无参构造器。该构造器设置为 <see langword="private"/> 修饰是为了防止外部调用该构造器；因为我们这里为了考虑性能的问题，
	/// 使用到的是一个 <see langword="static readonly"/> 组合修饰的字段，它在程序运行期间只创建一次。
	/// </summary>
	private ViewNodeComparer()
	{
	}


	/// <inheritdoc/>
	public bool Equals(ViewNode? x, ViewNode? y)
		=> (x, y) switch
		{
			(CellViewNode a, CellViewNode b) => a.Cell == b.Cell,
			(CandidateViewNode a, CandidateViewNode b) => a.Candidate == b.Candidate,
			(HouseViewNode a, HouseViewNode b) => a.House == b.House,
			(ChuteViewNode a, ChuteViewNode b) => a.ChuteIndex == b.ChuteIndex,
			(BabaGroupViewNode a, BabaGroupViewNode b) => a.Cell == b.Cell,
			_ => false
		};

	/// <inheritdoc/>
	public int GetHashCode(ViewNode obj)
		=> obj switch
		{
			CellViewNode o => o.Cell,
			CandidateViewNode o => o.Candidate,
			HouseViewNode o => o.House,
			ChuteViewNode o => o.ChuteIndex,
			BabaGroupViewNode o => o.Cell,
			_ => 0
		};
}
