namespace System.Resources;

public partial class SR
{
	/// <summary>
	/// Register a new resource manager for the current assembly calling this method.
	/// </summary>
	/// <typeparam name="TResourceManagerProvider">
	/// <para>The type of the resource manager provider.</para>
	/// <para>
	/// This type should point to a generated type, bound with your resource dictionary manifest file (*.resx),
	/// named like the file name of it.
	/// For example, if you create a resource dictionary manifest file called <c>Resource.resx</c>,
	/// a generated type will be named <c>Resource</c>.
	/// You should pass in this type (<c>Resource</c> here) as type argument to this method.
	/// </para>
	/// </typeparam>
	/// <exception cref="MissingResourceManagerException">
	/// Throws when the current calling assembly doesn't contain any resource manager.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void RegisterResourceManager<TResourceManagerProvider>() where TResourceManagerProvider : class
		=> ResourceManagers.Add(
			(
				Assembly.GetCallingAssembly(),
				(ResourceManager)typeof(TResourceManagerProvider)
					.GetProperty("ResourceManager", DefaultBindingFlags)!
					.GetValue(null)!
			)
		);
}
