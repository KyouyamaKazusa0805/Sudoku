namespace System;

/// <inheritdoc cref="FuncRef{T, TResult}"/>
public delegate TResult FuncRefReadOnly<T, out TResult>(scoped ref readonly T arg);
