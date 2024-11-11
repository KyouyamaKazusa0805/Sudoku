namespace Sudoku.Runtime.InterceptorServices;

/// <summary>
/// Represents an attribute type that can be applied to a method, indicating the method uses interceptor,
/// but the target method is an instance method with inheritance. Specified types are the possible derived types of the instance.
/// </summary>
/// <param name="types">Indicates all possible types.</param>
/// <remarks>
/// This attribute will be used by source generator to generate an extra entry to consume all possible types,
/// which is useful for <see langword="abstract"/>, <see langword="virtual"/> and <see langword="sealed"/> methods.
/// </remarks>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed partial class InterceptorInstanceTypesAttribute([Property] params Type[] types) : Attribute;
