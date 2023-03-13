namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，用于一个命令模块类型的某属性，表示该属性在它可能需要进行数据转换的时候，用于转换的转换器类型。经常被用在它是非 <see cref="string"/>
/// 类型，但需要转换为目标类型的时候。
/// </summary>
/// <typeparam name="T">表示转换器的类型。转换器必须实现 <see cref="IValueConverter"/> 接口，并包含一个无参构造器。</typeparam>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class ValueConverterAttribute<T> : CommandLineAnnotationAttribute where T : class, IValueConverter, new()
{
}
