namespace Sudoku.Runtime.InterceptorServices;

/// <summary>
/// Represents an attribute type that can be applied to a method, indicating the method uses interceptor,
/// but the target method is an instance method with inheritance. Specified types are the possible derived types of the instance.
/// </summary>
/// <param name="types">Indicates all possible types.</param>
/// <remarks>
/// <para>
/// This attribute will be used by source generator to generate an extra entry to consume all possible types,
/// which is useful for <see langword="abstract"/>, <see langword="virtual"/> and <see langword="sealed"/> methods.
/// </para>
/// <para>
/// Usage:
/// <code><![CDATA[
/// [InterceptorMethodCaller]
/// [InterceptorInstanceTypes(typeof(XChainingRule), typeof(YChainingRule))]
/// public static void InitializeLinks(ref readonly Grid grid, LinkType linkTypes, StepGathererOptions options, out ChainingRuleCollection rules)
/// {
///     rules = from linkType in linkTypes select ChainingRulePool.TryCreate(linkType)!;
///     if (!StrongLinkTypesCollected.HasFlag(linkTypes) || !WeakLinkTypesCollected.HasFlag(linkTypes))
///     {
///         var context = new ChainingRuleLinkContext(in grid, new LinkDictionary(), new LinkDictionary(), options);
///         foreach (var rule in rules)
///             rule.GetLinks(ref context);
///     }
///
///     // ...
/// }
/// ]]></code>
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed partial class InterceptorInstanceTypesAttribute([Property] params Type[] types) : Attribute
{
	/// <summary>
	/// Indicates the routing default behavior on generated method for <see langword="default"/> label
	/// or <see langword="_"/> token in <see langword="switch"/> statement or expression respectively.
	/// </summary>
	public InterceptorInstanceRoutingDefaultBehavior DefaultBehavior { get; init; }
}