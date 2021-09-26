namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// To mark onto a method or a constructor, to tell the compiler that
/// the exception must, should or may be thrown by the current method.
/// </summary>
/// <typeparam name="TException">The type of the exception.</typeparam>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, AllowMultiple = true, Inherited = true)]
[Conditional("SOLUTION_WIDE_CODE_ANALYSIS")]
[AutoPrimaryConstructor]
public sealed partial class ThrowsAttribute<TException> : Attribute where TException : Exception
{
	/// <summary>
	/// Initializes a <see cref="ThrowsAttribute{TException}"/> instance
	/// with <see cref="DiagnosticResultSeverity.Warning"/> as the default value.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ThrowsAttribute() : this(DiagnosticResultSeverity.Warning)
	{
	}


	/// <summary>
	/// Indicates the diagnostic severity that controls how the compiler validate the exception to be thrown.
	/// </summary>
	public DiagnosticResultSeverity Severity { get; }
}
