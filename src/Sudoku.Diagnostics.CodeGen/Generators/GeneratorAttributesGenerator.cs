namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines a source generator that produces generator-related attributes.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class GeneratorAttributesGenerator : IIncrementalGenerator
{
	/// <inheritdoc/>
	public void Initialize(IncrementalGeneratorInitializationContext context)
		=> context.RegisterSourceOutput(
			context.CompilationProvider,
			static (spc, compilation) =>
			{
				if (compilation is not { AssemblyName: Projects.System })
				{
					return;
				}

				spc.AddSource(
					$"Attributes.g.{Shortcuts.GeneratorAttributes}.cs",
					"""
					#pragma warning disable CS1591
					#nullable enable

					namespace System.Diagnostics.CodeGen;

					[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Class | global::System.AttributeTargets.Struct | global::System.AttributeTargets.Interface, AllowMultiple = true)]
					public sealed class AutoDeconstructionAttribute : global::System.Attribute
					{
						public AutoDeconstructionAttribute(params string[] memberExpressions) => MemberExpressions = memberExpressions;

						public string[] MemberExpressions { get; }
					}

					[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
					public sealed class AutoExtensionDeconstructionAttribute : global::System.Attribute
					{
						public AutoExtensionDeconstructionAttribute(Type type, params string[] memberExpressions)
							=> (Type, MemberExpressions) = (
								type.IsAssignableTo(typeof(global::System.Delegate)) || type.IsAssignableTo(typeof(global::System.Enum))
									? throw new global::System.ArgumentException("The type cannot be a delegate or enumeration.", nameof(type))
									: type,
								memberExpressions.Length == 0
									? throw new global::System.ArgumentException("The argument cannot be empty.", nameof(memberExpressions))
									: memberExpressions
							);

						public bool EmitsInKeyword { get; init; } = false;

						public string? Namespace { get; init; } = null;

						public string[] MemberExpressions { get; }

						public global::System.Type Type { get; }
					}

					[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
					public sealed class EnumSwitchExpressionArmAttribute : global::System.Attribute
					{
						public EnumSwitchExpressionArmAttribute(string key, string value) => (Key, Value) = (key, value);


						public string Key { get; }

						public string Value { get; }
					}

					public enum EnumSwitchExpressionDefaultBehavior : byte
					{
						ReturnIntegerValue,

						ReturnNull,

						Throw
					}
					
					[global::System.AttributeUsageAttribute(global::System.AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
					public sealed class EnumSwitchExpressionRootAttribute : global::System.Attribute
					{
						public EnumSwitchExpressionRootAttribute(string key) => Key = key;


						public string? MethodDescription { get; init; }

						public string? ThisParameterDescription { get; init; }

						public string? ReturnValueDescription { get; init; }

						public string Key { get; }

						public global::System.Diagnostics.CodeGen.EnumSwitchExpressionDefaultBehavior DefaultBehavior { get; init; } = global::System.Diagnostics.CodeGen.EnumSwitchExpressionDefaultBehavior.Throw;
					}
					"""
				);
			});
}
