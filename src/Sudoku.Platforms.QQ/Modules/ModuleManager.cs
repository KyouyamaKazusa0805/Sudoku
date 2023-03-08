namespace Sudoku.Platforms.QQ.Modules;

/// <summary>
/// Represents a list of <see cref="GroupModule"/> instances.
/// </summary>
/// <seealso cref="GroupModule"/>
public sealed class ModuleManager : List<IModule>
{
	/// <summary>
	/// Initializes a <see cref="ModuleManager"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private ModuleManager() : base()
	{
	}


	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static ModuleManager Default => new();

	/// <summary>
	/// Indicates an instance that contains all built-in commands included in this assembly.
	/// </summary>
	public static ModuleManager BuiltIn
	{
		get
		{
			var result = Default;
			result.AddRange(
				from type in typeof(ModuleManager).Assembly.GetDerivedTypes<IModule>()
				where type.GetConstructor(Array.Empty<Type>()) is not null && type.IsDefined(typeof(BuiltInAttribute))
				select (IModule)Activator.CreateInstance(type)!
			);

			return result;
		}
	}
}
