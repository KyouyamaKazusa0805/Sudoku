namespace Sudoku.Workflow.Bot.Oicq.Collections;

/// <summary>
/// 表示一个列表，存储的就是一系列的 <see cref="Command"/> 的实例。
/// </summary>
/// <seealso cref="Command"/>
public sealed class CommandCollection : List<Command>
{
	/// <summary>
	/// 表示内置的所有 <see cref="Command"/> 序列。
	/// </summary>
	public static CommandCollection BuiltIn
	{
		get
		{
			var result = new CommandCollection();
			result.AddRange(
				from type in typeof(CommandCollection).Assembly.GetDerivedTypes<Command>()
				where type.GetConstructor(Type.EmptyTypes) is not null && type.IsDefined(typeof(CommandAttribute))
				select (Command)Activator.CreateInstance(type)!
			);

			return result;
		}
	}
}
