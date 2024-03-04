namespace System;

/// <inheritdoc cref="GetHashCodeHandler{T}"/>
public delegate int GetHashCodeFunc<in T>([DisallowNull] T obj);
