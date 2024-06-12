namespace System.Linq.Providers;

/// <summary>
/// Represents a type that supports a certain method group defined in <see cref="Enumerable"/>, well-known as LINQ methods.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
/// <typeparam name="TSource">The type of each element that the type supports for iteration.</typeparam>
/// <seealso cref="Enumerable"/>
public interface ILinqMethod<TSelf, TSource> : IEnumerable<TSource>
	where TSelf : ILinqMethod<TSelf, TSource>, allows ref struct
{
	/// <summary>
	/// Indicates whether the provider type <typeparamref name="TSelf"/> won't calculate the final result immediately.
	/// The value can be <see langword="true"/> if such calculation is applied like <see cref="IEnumerable{T}"/> values.
	/// By default, the value is <see langword="false"/>.
	/// </summary>
	/// <seealso cref="IEnumerable{T}"/>
	public static virtual bool IsValueLazilyCalculated => false;

	/// <summary>
	/// Indicates whether the methods defined in type <typeparamref name="TSelf"/> support for query syntax also.
	/// By default, the value is <see langword="false"/>.
	/// </summary>
	/// <remarks>
	/// All possible query clauses are:
	/// <list type="table">
	/// <listheader>
	/// <term>Query syntax</term>
	/// <description>Equivalent method</description>
	/// </listheader>
	/// </list>
	/// <list type="bullet">
	/// <item>
	/// <term><see langword="from"/> element <see langword="in"/> collection</term>
	/// <description>N/A</description>
	/// </item>
	/// <item>
	/// <term>
	/// <see langword="from"/> element1 <see langword="in"/> collection1<br/>
	/// <see langword="from"/> element2 <see langword="in"/> collection2
	/// </term>
	/// <description>
	/// <see cref="Enumerable.SelectMany{TSource, TResult}(IEnumerable{TSource}, Func{TSource, IEnumerable{TResult}})"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="where"/> condition</term>
	/// <description><see cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/></description>
	/// </item>
	/// <item>
	/// <term><see langword="let"/> variable = expression</term>
	/// <description><see cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/></description>
	/// </item>
	/// <item>
	/// <term><see langword="select"/> { variable | expression } [<see langword="into"/> variable]</term>
	/// <description><see cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/></description>
	/// </item>
	/// <item>
	/// <term><see langword="orderby"/> { variable | expression }</term>
	/// <description><see cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/></description>
	/// </item>
	/// <item>
	/// <term><see langword="orderby"/> { variable | expression } <see langword="ascending"/></term>
	/// <description><see cref="Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/></description>
	/// </item>
	/// <item>
	/// <term><see langword="orderby"/> { variable | expression } <see langword="descending"/></term>
	/// <description>
	/// <see cref="Enumerable.OrderByDescending{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="orderby"/> { v1 | e1 }, { v2 | e2 } ...</term>
	/// <description><see cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/></description>
	/// </item>
	/// <item>
	/// <term>
	/// <see langword="orderby"/> { v1 | e1 }, { v2 | e2 } <see langword="ascending"/> ...
	/// </term>
	/// <description><see cref="Enumerable.ThenBy{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/></description>
	/// </item>
	/// <item>
	/// <term>
	/// <see langword="orderby"/> { v1 | e1 }, { v2 | e2 } <see langword="descending"/> ...
	/// </term>
	/// <description>
	/// <see cref="Enumerable.ThenByDescending{TSource, TKey}(IOrderedEnumerable{TSource}, Func{TSource, TKey})"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="group"/> variable <see langword="by"/> { variable | expression } [<see langword="into"/> variable]</term>
	/// <description>
	/// <see cref="Enumerable.GroupBy{TSource, TKey}(IEnumerable{TSource}, Func{TSource, TKey})"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="group"/> expression <see langword="by"/> expression [<see langword="into"/> variable]</term>
	/// <description>
	/// <see cref="Enumerable.GroupBy{TSource, TKey, TElement}(IEnumerable{TSource}, Func{TSource, TKey}, Func{TSource, TElement})"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term>
	/// <see langword="join"/> variable <see langword="in"/> collection
	/// <see langword="on"/> previousVariable <see langword="equals"/> variable
	/// </term>
	/// <description>
	/// <br/>
	/// <see cref="Enumerable.Join{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, TInner, TResult})"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term>
	/// <see langword="join"/> variable2 <see langword="in"/> collection2
	/// <see langword="on"/> variable1 <see langword="equals"/> variable2
	/// <see langword="into"/> joinedGroup
	/// </term>
	/// <description>
	/// <br/>
	/// <see cref="Enumerable.GroupJoin{TOuter, TInner, TKey, TResult}(IEnumerable{TOuter}, IEnumerable{TInner}, Func{TOuter, TKey}, Func{TInner, TKey}, Func{TOuter, IEnumerable{TInner}, TResult})"/>
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public static virtual bool SupportsQuerySyntax => false;
}
