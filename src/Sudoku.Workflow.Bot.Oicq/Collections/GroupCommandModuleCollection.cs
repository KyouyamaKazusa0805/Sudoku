namespace Sudoku.Workflow.Bot.Oicq.Collections;

/// <summary>
/// 表示一个列表，存储的就是一系列的 <see cref="GroupCommandModule"/> 的实例。
/// </summary>
/// <seealso cref="GroupCommandModule"/>
public sealed class GroupCommandModuleCollection : List<GroupCommandModule>
{
	/// <summary>
	/// 表示内置的所有 <see cref="GroupCommandModule"/> 序列。
	/// </summary>
	public static GroupCommandModuleCollection BuiltIn
	{
		get
		{
			var result = new GroupCommandModuleCollection();
			result.AddRange(
				from type in typeof(GroupCommandModuleCollection).Assembly.GetDerivedTypes<GroupCommandModule>()
				where type.GetConstructor(Array.Empty<Type>()) is not null && type.IsDefined(typeof(GroupCommandModuleAttribute))
				select (GroupCommandModule)Activator.CreateInstance(type)!
			);

			return result;
		}
	}
}
