namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the code for default initialization on type <c>Preference</c>.
/// </summary>
[Generator]
public sealed class PreferenceDefaultInitializationGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		var compilation = context.Compilation;
		if (compilation.GetTypeByMetadataName("Sudoku.UI.Data.Preference") is not { } typeSymbol)
		{
			return;
		}

		var stringSymbol = compilation.GetSpecialType(SpecialType.System_String);
		var attributeSymbol = compilation.GetTypeByMetadataName(typeof(DefaultValueAttribute).FullName);
		string initialization = string.Join(
			"\r\n\r\n\t\t",
			from propertySymbol in typeSymbol.GetMembers().OfType<IPropertySymbol>()
			where !propertySymbol.IsIndexer
			let attributesData = propertySymbol.GetAttributes()
			let attributeData = attributesData.FirstOrDefault(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeSymbol))
			where attributeData is not null
			select (Symbol: propertySymbol, AttributeData: attributeData) into pair
			let rawCtorArgs = pair.AttributeData.ConstructorArguments[0].Values
			let rawConstructorArguments = from arg in rawCtorArgs select ((string?)arg.Value)?.Trim()
			let constructorArguments = rawConstructorArguments.ToArray()
			select getInitializationCode(pair.Symbol, constructorArguments)
		);

		context.AddSource(
			"Preference",
			"di",
			$@"#nullable enable

namespace {typeSymbol.ContainingNamespace};

partial class Preference
{{
	/// <summary>
	/// Initializes a <see cref=""Preference""/> instance.
	/// </summary>
	[global::System.CodeDom.Compiler.GeneratedCode(""{GetType().FullName}"", ""{VersionValue}"")]
	[global::System.Runtime.CompilerServices.CompilerGenerated]
	public Preference()
	{{
		{initialization}
	}}
}}"
		);


		string getInitializationCode(IPropertySymbol propertySymbol, string?[] values)
		{
			var innerLogic = new StringBuilder();

			switch (values.Length)
			{
				case 0:
				case var l and not 1 when (l & 1) != 0:
				{
					// Invalid case.
					break;
				}
				case 1 when propertySymbol.Name is var name:
				{
					switch (values[0])
					{
						case "_":
						{
							// Don't assign anything.
							break;
						}
						case var value:
						{
							if (SymbolEqualityComparer.Default.Equals(stringSymbol, propertySymbol.Type))
							{
								innerLogic.AppendLine($"\t\t{name} = \"{value}\";");
							}
							else
							{
								innerLogic.AppendLine($"\t\t{name} = {value};");
							}

							break;
						}
					}

					break;
				}
				case var l when propertySymbol.Name is var name:
				{
					for (int i = 0; i < l; i += 2)
					{
						string? symbol = values[i], value = values[i + 1];
						if (symbol is not null)
						{
							innerLogic.AppendLine($@"{(i == 0 ? "#if" : "#elif")} {symbol}");
						}
						if (value != "_")
						{
							if (SymbolEqualityComparer.Default.Equals(stringSymbol, propertySymbol.Type))
							{
								innerLogic.AppendLine($"\t\t{name} = \"{value}\";");
							}
							else
							{
								innerLogic.AppendLine($"\t\t{name} = {value};");
							}
						}
						if (symbol is not null)
						{
							innerLogic.AppendLine("#endif");
						}
					}

					break;
				}
			}


			return $"#region Property '{propertySymbol.Name}'\r\n{innerLogic}\t\t#endregion";
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
