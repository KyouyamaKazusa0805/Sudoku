namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that generates the method and the corresponding values,
/// forming a <see langword="switch"/> expression.
/// </summary>
/// <remarks>This source generator does not support generic types.</remarks>
[Generator(LanguageNames.CSharp)]
public sealed partial class EnumSwitchExpressionGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context is not { SyntaxContextReceiver: Receiver { Collection: var collection } })
		{
			return;
		}

		// Iterates on each tuple array.
		// An array of tuples is a group, storing a set of tuples whose key and type are same.
		foreach (var (type, tuples) in collection)
		{
			string fullName = type.ToDisplayString(TypeFormats.FullName);

			var emittedMethods = new List<string>();

			// Iterates on each tuple element in the array.
			foreach (var (_, key, typeAttributeData, listOfFieldsAndTheirOwnAttributesData) in tuples)
			{
				var innerParts = new List<string>();

				foreach (var (fieldSymbol, attributeData) in listOfFieldsAndTheirOwnAttributesData)
				{
					string value = (string)attributeData.ConstructorArguments[1].Value!;
					innerParts.Add($"""{fullName}.{fieldSymbol.Name} => "{value}",""");
				}

				string notFoundBehaviorStr = typeAttributeData.GetNamedArgument<byte>("NotDefinedBehavior", 1) switch
				{
					// ReturnByCorrespondingIntegerValue
					0 => "@this.ToString()",

					// ThrowForNotDefined
					_ => "throw new global::System.ArgumentOutOfRangeException(nameof(@this))"
				};

				emittedMethods.Add(
					$$"""
					/// <summary>
						/// Method <c>{{key}}</c>.
						/// </summary>
						/// <returns>The result value.</returns>
						/// <!--TODO: Wait for adding options to modify on documentation comments here.-->
						[global::System.Runtime.CompilerServices.CompilerGenerated]
						[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
						[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
						public static string {{key}}(this {{fullName}} @this)
							=> @this switch
							{
								{{string.Join("\r\n\t\t\t", innerParts)}}
								_ => {{notFoundBehaviorStr}}
							};
					"""
				);
			}

			context.AddSource(
				type.ToFileName(),
				Shortcuts.EnumSwitchExpression,
				$$"""
				#nullable enable
				
				namespace {{SymbolOutputInfo.FromSymbol(type).NamespaceName}};
				
				/// <summary>
				/// Provides with extension methods for switching on the current type.
				/// </summary>
				[global::System.Runtime.CompilerServices.CompilerGenerated]
				[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
				public static class {{type.Name}}_EnumSwitchExpressionExtensions
				{
					{{string.Join("\r\n\r\n\t", emittedMethods)}}
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(() => new Receiver(context.CancellationToken));
}
