namespace Sudoku.SourceGeneration;

public partial class CachedMethodGenerator
{
	/// <summary>
	/// Represents message "Necessary type is missing".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0100 = new(
		"INTERCEPTOR0100",
		"Necessary type is missing",
		"Necessary type '{0}' is missing",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Method marked '[InterceptorMethodCaller]' or '[Cached]' can only be block body".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0101 = new(
		"INTERCEPTOR0101",
		"Method marked '[InterceptorMethodCaller]' or '[Cached]' can only be block body",
		"Method '{0}' marked '[InterceptorMethodCaller]' or '[Cached]' can only be block body",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Method marked '[InterceptorMethodCaller]' requires at least one invocation expression
	/// that references to a method marked '[Cached]'".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0102 = new(
		"INTERCEPTOR0102",
		"Method marked '[InterceptorMethodCaller]' requires at least one invocation expression that references to a method marked '[Cached]'",
		"Method '{0}' marked '[InterceptorMethodCaller]' requires at least one invocation expression that references to a method marked '[Cached]'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Method marked '[Cached]' cannot be <see langword="partial"/>".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0103 = new(
		"INTERCEPTOR0103",
		"Method marked '[Cached]' cannot be partial",
		"Method '{0}' marked '[Cached]' cannot be partial",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Lacks of usage of necessary comments:
	/// '<c>VARIABLE_DECLARATION_BEGIN</c>' and '<c>VARIABLE_DECLARATION_END</c>'".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0104 = new(
		"INTERCEPTOR0104",
		$"Lacks of usage of necessary comments: '{CommentLineBegin}' and '{CommentLineEnd}'",
		$"Lacks of usage of necessary comments: '{CommentLineBegin}' and '{CommentLineEnd}'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Duplicate comments:
	/// '<c>VARIABLE_DECLARATION_BEGIN</c>' and '<c>VARIABLE_DECLARATION_END</c>'".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0105 = new(
		"INTERCEPTOR0105",
		$"Duplicate comments: '{CommentLineBegin}' and '{CommentLineEnd}'",
		$"Duplicate comments: '{CommentLineBegin}' and '{CommentLineEnd}'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "If method is marked '[Cached]',
	/// it is disallowed to consume instance members by using <see langword="this"/> or <see langword="base"/>".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0106 = new(
		"INTERCEPTOR0106",
		"If method is marked '[Cached]', it is disallowed to consume instance members by using 'this' or 'base'",
		"If method is marked '[Cached]', it is disallowed to consume instance members by using 'this' or 'base'",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);

	/// <summary>
	/// Represents message "Method marked '[Cached]' cannot be generic".
	/// </summary>
	private static readonly DiagnosticDescriptor Descriptor_Interceptor0107 = new(
		"INTERCEPTOR0107",
		"Method marked '[Cached]' cannot be generic",
		"Method '{0}' marked '[Cached]' cannot be generic",
		"Interceptor.Design",
		DiagnosticSeverity.Error,
		true
	);
}
