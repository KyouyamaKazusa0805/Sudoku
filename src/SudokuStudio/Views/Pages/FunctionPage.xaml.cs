namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a function page.
/// </summary>
public sealed partial class FunctionPage : Page
{
	/// <summary>
	/// Initializes a <see cref="FunctionPage"/> instance.
	/// </summary>
	public FunctionPage() => InitializeComponent();


	private void Page_Loaded(object sender, RoutedEventArgs e)
	{
		// Check for local path. If the local contains at least one function file, we won't check for APIs.
		var existsValidFile = true;
		if (!Directory.Exists(CommonPaths.Functions)
			|| !new DirectoryInfo(CommonPaths.Functions).EnumerateFiles().Any())
		{
			existsValidFile = false;
		}

		if (existsValidFile)
		{
			// Export functions that marks [ExportFunction].
			foreach (var m in method())
			{

			}
		}


		static IEnumerable<MethodInfo> method()
		{
			yield break;
#if false
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod;
			foreach (var type in typeof(Analyzer).Assembly.GetTypes())
			{
				if (!type.IsAssignableTo(typeof(IFunctionProvider)))
				{
					continue;
				}

				foreach (var methodInfo in type.GetMethods(bindingFlags))
				{
					if (methodInfo.GetParameters() is not [{ ParameterType: { IsByRef: false, IsFunctionPointer: false } }]
						|| methodInfo.ReturnType == typeof(void))
					{
						continue;
					}

					if (methodInfo.GetCustomAttribute<ExportedMemberAttribute>() is not { FunctionName: var functionName })
					{
						continue;
					}
				}
			}
#endif
		}
	}
}
