namespace System.Collections.Generic;

/// <summary>
/// Represents an entry that can be used as a type parameter <c>TKey</c> in <see cref="Dictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TSelf">The type itself.</typeparam>
/// <remarks>
/// <b>The type is only used as a constraint, while implementing a data type for key usages temporarily.</b>
/// </remarks>
/// <seealso cref="Dictionary{TKey, TValue}"/>
public interface IDictionaryEntry<TSelf> : IEquatable<TSelf>, IEqualityOperators<TSelf, TSelf, bool>
	where TSelf : IDictionaryEntry<TSelf>;
