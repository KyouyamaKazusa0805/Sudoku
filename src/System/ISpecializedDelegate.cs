namespace System;

/// <summary>
/// Represents a delegate type.
/// </summary>
/// <typeparam name="T">The type of delegate</typeparam>
internal interface ISpecializedDelegate<T> where T : Delegate;
