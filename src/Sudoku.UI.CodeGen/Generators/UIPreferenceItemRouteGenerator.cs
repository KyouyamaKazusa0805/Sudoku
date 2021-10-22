namespace Sudoku.UI.CodeGenerating.Generators;

/// <summary>
/// Defines a source generator that generates the code for UI preference item.
/// </summary>
[Generator]
public sealed unsafe class UIPreferenceItemRouteGenerator : ISourceGenerator
{
	/// <summary>
	/// Indicates the routers.
	/// </summary>
	private readonly (string ControlTypeName, ControlRouting Routing)[] _routers = new (string, ControlRouting)[]
	{
		(
			"Microsoft.UI.Xaml.Controls.ToggleSwitch",
			RouteToggleSwitch
		)
	};


	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		if (compilation.GetTypeByMetadataName("Sudoku.UI.Data.Preference") is not { } preferenceTypeSymbol)
		{
			return;
		}

		if (compilation.GetTypeByMetadataName("Sudoku.UI.Pages.MainWindow.SettingsPage") is not { } settingsPageTypeSymbol)
		{
			return;
		}

		var fieldsInSettingsPage = settingsPageTypeSymbol.GetMembers().OfType<IFieldSymbol>();
		var routeTypeSymbol = compilation.GetTypeByMetadataName(typeof(PreferenceItemRouteAttribute).FullName);

		foreach (var (property, attributeData) in
			from property in preferenceTypeSymbol.GetMembers().OfType<IPropertySymbol>()
			where property is { IsIndexer: false, IsReadOnly: false, IsWriteOnly: false }
			let rawAttributeData =
				from attributeData in property.GetAttributes()
				where SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, routeTypeSymbol)
				select attributeData
			let attributeData = rawAttributeData.FirstOrDefault()
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
			if (
				fieldsInSettingsPage.FirstOrDefault(f => f.Name == controlName) is not
				{
					IsFixedSizeBuffer: false,
					IsConst: false,
					IsReadOnly: false,
					Type: var typeSymbolOfField
				}
			)
			{
				continue;
			}

			string fullTypeNameOfField = typeSymbolOfField.ToDisplayString(FormatOptions.TypeFormat);
			int index = Array.FindIndex(_routers, router => router.ControlTypeName == fullTypeNameOfField);
			if (index == -1)
			{
				continue;
			}

			_routers[index].Routing(ref context, propertyName, ctorArgs, namedArgs, controlName);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}


	private static void RouteToggleSwitch(
		ref GeneratorExecutionContext context,
		string propertyName,
		ImmutableArray<TypedConstant> ctorArgs,
		ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs,
		string controlName
	)
	{
		string effectControlName = (string)ctorArgs[1].Value!;
		string conversion = namedArgs switch
		{
			{ Length: >= 1 } when namedArgs.First(predicate).Value.Value is string methodName => $"{methodName}(isOn)",
			_ => $"_preference.{propertyName} = isOn"
		};

		context.AddSource(
			propertyName,
			"r",
			$@"namespace Sudoku.UI.Pages.MainWindow;

partial class SettingsPage
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{typeof(UIPreferenceItemRouteGenerator).FullName}"", ""0.7"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	private partial void {controlName}_Toggled(object sender, [Discard] RoutedEventArgs e)
	{{
		if (
			(Sender: sender, PageIsInitialized: _pageIsInitialized) is not (
				Sender: ToggleSwitch {{ IsOn: var isOn }},
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


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool predicate<T>(KeyValuePair<string, T> arg) =>
			arg.Key == nameof(PreferenceItemRouteAttribute.PreferenceSetterMethodName);
	}
}
