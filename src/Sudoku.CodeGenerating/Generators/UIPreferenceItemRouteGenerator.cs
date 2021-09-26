using C = Microsoft.CodeAnalysis.GeneratorExecutionContext;
using CArgs = System.Collections.Immutable.ImmutableArray<Microsoft.CodeAnalysis.TypedConstant>;
using NArgs = System.Collections.Immutable.ImmutableArray<System.Collections.Generic.KeyValuePair<string, Microsoft.CodeAnalysis.TypedConstant>>;
using Ptr = System.IntPtr;

namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Defines a source generator that generates the code for UI preference item.
/// </summary>
[Generator]
public sealed unsafe class UIPreferenceItemRouteGenerator : ISourceGenerator
{
	/// <summary>
	/// Indicates the routers.
	/// </summary>
	private readonly (string ControlTypeName, Ptr MethodPtr)[] _routers = new[]
	{
		(
			"Microsoft.UI.Xaml.Controls.ToggleSwitch",
			(Ptr)(void*)(delegate*<ref C, string, CArgs, NArgs, string, void>)&RouteToggleSwitch
		)
	};


	/// <inheritdoc/>
	public void Execute(C context)
	{
		if (context.IsNotInProject(ProjectNames.WindowsUI))
		{
			return;
		}

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
		var routeTypeSymbol = compilation.GetTypeByMetadataName<PreferenceItemRouteAttribute>();
		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;

		foreach (var (property, attributeData) in
			from property in preferenceTypeSymbol.GetMembers().OfType<IPropertySymbol>()
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
			if (
				fieldsInSettingsPage.FirstOrDefault(fieldSymbol => fieldSymbol.Name == controlName) is not
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

			(
				(delegate*<ref C, string, CArgs, NArgs, string, void>)(void*)_routers[index].MethodPtr
			)(ref context, propertyName, ctorArgs, namedArgs, controlName);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}


	private static void RouteToggleSwitch(
		ref C context,
		string propertyName,
		CArgs ctorArgs,
		NArgs namedArgs,
		string controlName
	)
	{
		string effectControlName = (string)ctorArgs[1].Value!;
		string conversion = namedArgs switch
		{
			{ Length: >= 1 }
			when namedArgs.First(
				static arg => arg.Key == nameof(PreferenceItemRouteAttribute.PreferenceSetterMethodName)
			) is { Value.Value: string methodName } => $"{methodName}(isOn)",
			_ => $"_preference.{propertyName} = isOn"
		};

		context.AddSource(
			propertyName,
			"PreferenceItemRoute",
			$@"namespace Sudoku.UI.Pages.MainWindow;

partial class SettingsPage
{{
	[global::System.CodeDom.Compiler.GeneratedCode(""{typeof(UIPreferenceItemRouteGenerator).FullName}"", ""{VersionValue}"")]
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
