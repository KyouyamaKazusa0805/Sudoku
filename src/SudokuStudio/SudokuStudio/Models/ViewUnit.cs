namespace SudokuStudio.Models;

/// <summary>
/// Defines a view unit.
/// </summary>
[DependencyProperty<Conclusion[]>("Conclusions", DocSummary = "Indicates the candidates as conclusions in a single <see cref=\"IStep\"/>.")]
[DependencyProperty<View>("View", DocSummary = "Indicates a view of highlight elements.")]
public sealed partial class ViewUnit : DependencyObject;
