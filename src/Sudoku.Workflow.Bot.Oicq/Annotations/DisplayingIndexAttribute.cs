namespace Sudoku.Workflow.Bot.Oicq.Annotations;

/// <summary>
/// 提供一个特性，表示某指令里的某个参数的显示顺序。
/// 该特性专门用于显示帮助信息的时候，按显示位置（即 <see cref="Index"/> 属性）从小到大排列出来，提供一个更加良好的帮助信息文字的呈现。
/// </summary>
/// <param name="index">表示该指令的显示位置。</param>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed partial class DisplayingIndexAttribute([PrimaryConstructorParameter] int index) : CommandAnnotationAttribute;
