namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于标记到一个命令模块的属性上，表示该属性的默认数值是由哪一个同类型里的成员产生而赋值得到。
/// </summary>
/// <remarks>
/// 这个特性区别于 <see cref="DefaultValueAttribute{T}"/>，这个特性专门用于 <see cref="DefaultValueAttribute{T}"/> 不支持的情况。
/// 比如特性里的传参必须为常量（编译期可计算的、无副作用的数据），那么如果不能以常量形式表达出来的情况，就必须通过创建新的成员，然后通过该特性引用到此成员，
/// 来达到初始化的目的。
/// </remarks>
/// <seealso cref="DefaultValueAttribute{T}"/>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DefaultValueReferencingMemberAttribute : CommandAnnotationAttribute
{
	/// <summary>
	/// 实例化一个 <see cref="DefaultValueReferencingMemberAttribute"/> 的实例，并给出该成员名称。
	/// </summary>
	/// <param name="defaultValueInvokerName">
	/// <para>表示成员的名称。</para>
	/// <para><inheritdoc cref="DefaultValueInvokerName" path="/remarks"/></para>
	/// </param>
	public DefaultValueReferencingMemberAttribute(string defaultValueInvokerName) => DefaultValueInvokerName = defaultValueInvokerName;


	/// <summary>
	/// 表示成员的名称。
	/// </summary>
	/// <remarks><b><i>
	/// 该成员必须得是一个静态的成员，要么它是字段，要么它是属性，要么它就必须得是无参的方法。如果是无参的方法，还必须保证它的名字和反射时候得到的名称一致。
	/// 比如，本地函数的名字本身，和本地函数在程序里使用的实际名称是不同的。所以，这里传入的必须得是反射使用的实际名称。
	/// </i></b></remarks>
	public string DefaultValueInvokerName { get; }
}
