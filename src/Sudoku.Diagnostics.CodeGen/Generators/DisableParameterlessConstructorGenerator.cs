namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Indicates the generator that generates the parameterless constructor on <see langword="struct"/> types,
/// to disallow any invokes for them from user.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class DisableParameterlessConstructorGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (
			context is not
			{
				SyntaxContextReceiver: DisableParameterlessConstructorReceiver
				{
					Diagnostics: var diagnostics,
					Collection: var typeSymbols
				} receiver,
				Compilation: var compilation
			}
		)
		{
			return;
		}

		if (diagnostics.Count != 0)
		{
			diagnostics.ForEach(context.ReportDiagnostic);
			return;
		}

		foreach (var (type, attributeData, location) in typeSymbols)
		{
			var (_, _, namespaceName, genericParameterList, _, _, readOnlyKeyword, _, _, _) = SymbolOutputInfo.FromSymbol(type);

			string? rawMessage = (
				from namedArg in attributeData.NamedArguments
				where namedArg is { Key: "Message", Value.Value: string }
				select (string)namedArg.Value.Value!
			).FirstOrDefault();
			string? memberName = (
				from namedArg in attributeData.NamedArguments
				where namedArg is { Key: "SuggestedMemberName", Value.Value: string }
				select (string)namedArg.Value.Value!
			).FirstOrDefault();
			string message = (rawMessage, memberName) switch
			{
				(not null, not null) => nameof(SCA0005),
				(null, null) => nameof(SCA0004),
				(null, not null) => $"Please use the member '{memberName}' instead.",
				_ => rawMessage
			};

			void c(DiagnosticDescriptor d) => context.ReportDiagnostic(Diagnostic.Create(d, location, messageArgs: null));
			(
				message switch
				{
					nameof(SCA0004) => () => c(SCA0004),
					nameof(SCA0005) => () => c(SCA0005),
					_ => default(Action)
				}
			)?.Invoke();

			context.AddSource(
				type.ToFileName(),
				"dpc",
				$$"""
				#nullable enable
				
				namespace {{namespaceName}};
				
				partial struct {{type.Name}}{{genericParameterList}}
				{
					/// <summary>
					/// Throws a <see cref="global::System.NotSupportedException"/>.
					/// </summary>
					/// <exception cref="global::System.NotSupportedException">
					/// The exception will always be thrown.
					/// </exception>
					/// <remarks>
					/// The main idea of the paramterless constructor is to create a new instance
					/// without any extra information, but the current type is special:
					/// the author wants to make you use another member instead of it to get a better experience.
					/// Therefore, the paramterless constructor is disallowed to be invoked
					/// no matter what kind of invocation, reflection or strongly reference.
					/// </remarks>
					[global::System.Runtime.CompilerServices.CompilerGenerated]
					[global::System.CodeDom.Compiler.GeneratedCode("{{GetType().FullName}}", "{{VersionValue}}")]
					[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
					[global::System.Obsolete("{{message}}", true)]
					public {{type.Name}}() => throw new global::System.NotSupportedException();
				}
				"""
			);
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
		=> context.RegisterForSyntaxNotifications(
			() => new DisableParameterlessConstructorReceiver(context.CancellationToken));
}
