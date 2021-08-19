namespace Sudoku.Solving.Manual;

/// <summary>
/// <para>
/// Indicates the technique searcher can or can't be used in some scenarios
/// where they aren't in traversing mode to call
/// <see cref="IStepSearcher.GetAll(ICollection{Step}, in Grid, bool)"/> in <see cref="IStepSearcher"/>s one by one.
/// </para>
/// <para>
/// If <see langword="true"/>, the searcher can't use those <see langword="static"/>
/// properties such as <see cref="CandMaps"/> in its method
/// <see cref="IStepSearcher.GetAll(ICollection{Step}, in Grid, bool)"/>.
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// All disallowed properties are:
/// <list type="bullet">
/// <item><see cref="DigitMaps"/></item>
/// <item><see cref="ValueMaps"/></item>
/// <item><see cref="CandMaps"/></item>
/// <item><see cref="BivalueMap"/></item>
/// <item><see cref="EmptyMap"/></item>
/// </list>
/// The disallowed method is:
/// <list type="bullet">
/// <item><see cref="InitializeMaps"/></item>
/// </list>
/// </para>
/// <para>
/// Those properties or methods can optimize the performance to analyze a sudoku grid, but
/// sometimes they may cause a potential bug that is hard to find and fix. The attribute
/// is created and used for solving the problem.
/// </para>
/// </remarks>
/// <seealso cref="IStepSearcher"/>
/// <seealso cref="FastProperties"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class IsDirectAttribute : Attribute
{
}
