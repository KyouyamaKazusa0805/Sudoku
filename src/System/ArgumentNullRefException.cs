namespace System;

/// <inheritdoc/>
/// <param name="paramName">The parameter name.</param>
public sealed class ArgumentNullRefException(string? paramName) : ArgumentNullException(paramName);
