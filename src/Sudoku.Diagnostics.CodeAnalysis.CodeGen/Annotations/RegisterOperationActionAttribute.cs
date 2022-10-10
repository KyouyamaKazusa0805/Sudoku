namespace Sudoku.Diagnostics.CodeGen.Annotations;

/// <summary>
/// The registered operation attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RegisterOperationActionAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="RegisterOperationActionAttribute"/> instance via the specified registered action name.
	/// </summary>
	/// <param name="registeredActionName">The action name.</param>
	/// <param name="registeredKindType">The registered kind type.</param>
	/// <param name="rawRegisteredKindExpression">The registered kind expression.</param>
	public RegisterOperationActionAttribute(string registeredActionName, Type registeredKindType, string rawRegisteredKindExpression)
		=> (RegisteredActionName, RegisteredKindType, RegisteredKindExpression) = (registeredActionName, registeredKindType, rawRegisteredKindExpression);


	/// <summary>
	/// Indicates the registered action name. The name references to method names in type <see cref="AnalysisContext"/>.
	/// For example, if you want to execute the method
	/// <see cref="AnalysisContext.RegisterOperationAction(Action{OperationAnalysisContext}, OperationKind[])"/>,
	/// the property value should be <c>"RegisterOperationAction"</c>.
	/// </summary>
	public string RegisteredActionName { get; }

	/// <summary>
	/// The registered kind expression.
	/// </summary>
	public string RegisteredKindExpression { get; }

	/// <summary>
	/// The registered kind type. For example, <see langword="typeof"/>(<see cref="OperationKind"/>).
	/// </summary>
	public Type RegisteredKindType { get; }
}
