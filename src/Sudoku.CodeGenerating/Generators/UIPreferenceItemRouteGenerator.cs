namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Defines a source generator that generates the code for UI preference item.
/// </summary>
[Generator]
public sealed class UIPreferenceItemRouteGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context.IsNotInProject(ProjectNames.WindowsUI))
		{
			return;
		}

		ToggleSwitchRoute(ref context);
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}

	/// <summary>
	/// Routes the toggle switch instances and records them
	/// into the <see cref="GeneratorExecutionContext"/> instance.
	/// </summary>
	/// <param name="context">The context.</param>
	private void ToggleSwitchRoute(ref GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		if (compilation.GetTypeByMetadataName("Sudoku.UI.Data.Preference") is not { } type)
		{
			return;
		}

		var routeTypeSymbol = compilation.GetTypeByMetadataName<ToggleSwitchRouteAttribute>();
		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;

		foreach (var (property, attributeData) in
			from property in type.GetMembers().OfType<IPropertySymbol>()
			where property is { IsIndexer: false, IsReadOnly: false, IsWriteOnly: false }
			let attributeData = (
				from attributeData in property.GetAttributes()
				where f(attributeData.AttributeClass, routeTypeSymbol)
				select attributeData
			).FirstOrDefault()
			where attributeData is not null
			select (property, attributeData))
		{
			if (
				(Property: property, AttributeData: attributeData) is not (
					Property: { Name: var propertyName },
					AttributeData:
					{
						ConstructorArguments: { Length: 2 } ctorArgs,
						NamedArguments: var namedArgs
					}
				)
			)
			{
				continue;
			}

			string controlName = (string)ctorArgs[0].Value!;
			string effectControlName = (string)ctorArgs[1].Value!;
			string conversion = namedArgs switch
			{
				{ Length: >= 1 }
				when namedArgs.First(
					static arg =>
						arg.Key == nameof(ToggleSwitchRouteAttribute.PreferenceSetterMethodName)
				).Value.Value is string methodName => $"{methodName}(isOn)",
				_ => $"_preference.{propertyName} = isOn"
			};

			context.AddSource(
				propertyName,
				"PreferenceItemRoute",
				$@"namespace Sudoku.UI.Pages.MainWindow;

partial class SettingsPage
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private partial void {controlName}_Toggled(object sender, RoutedEventArgs e)
	{{
		if (
			(
				Sender: sender,
				EventArgs: e,
				PageIsInitialized: _pageIsInitialized
			) is not (
				Sender: ToggleSwitch {{ IsOn: var isOn }},
				_,
				PageIsInitialized: true
			)
		)
		{{
			return;
		}}

		_boundSteps.Add(
			new(
				{effectControlName},
				() => {conversion}
			)
		);
	}}
}}
"
			);
		}
	}
}
