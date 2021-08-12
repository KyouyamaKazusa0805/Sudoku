namespace Sudoku.Solving.Manual;

/// <summary>
/// Indicates the options aiming to <see cref="StepSearcher"/>s while searching, which is the newer
/// implementation of the type <see cref="TechniqueProperties"/>.
/// </summary>
/// <param name="Priority">Indicates the priority of this technique.</param>
/// <param name="EnabledAreas">
/// Indicates which areas the step searcher is enabled and works well.
/// The default value is both <see cref="EnabledAreas.Default"/> and <see cref="EnabledAreas.FullSearching"/>.
/// </param>
/// <param name="DisplayingLevel">
/// <para>Indicates the displaying level of this technique.</para>
/// <para>
/// The display level means the which level the technique is at. All higher leveled techniques
/// won't display on the screen when the searchers at the current level have found technique
/// instances.
/// </para>
/// <para>
/// In order to enhance the performance, this attribute is used on <see cref="AllStepSearcher"/>.
/// For example, if Alternating Inference Chain (AIC) is at the level <see cref="DisplayingLevel.D"/>
/// but Forcing Chains (FC) is at the level <see cref="DisplayingLevel.E"/>,
/// when we find any AIC technique instances, FC won't be checked at the same time
/// </para>
/// <para>
/// This attribute is also used for grouping those the searchers, especially in Sudoku Explainer mode.
/// </para>
/// </param>
/// <param name="DisabledReason">
/// Indicates whether the current searcher has bug to fix, or something else to describe why
/// the searcher is (or should be) disabled.
/// </param>
/// <param name="Stableness">
/// <para>Indicates how stable the technique behaves.</para>
/// <para>
/// In the future, if the technique is unstable, we may modify the order slightly via algorithm to
/// find some special techniques, such as exocets.
/// </para>
/// </param>
/// <seealso cref="StepSearcher"/>
/// <seealso cref="TechniqueProperties"/>.
/// <seealso cref="AllStepSearcher"/>
[StructLayout(LayoutKind.Explicit)]
public readonly record struct SearchingOptions(
	[field: FieldOffset(4)] short Priority,
	[field: FieldOffset(1)] DisplayingLevel DisplayingLevel,
	[field: FieldOffset(0)] EnabledAreas EnabledAreas = EnabledAreas.Default | EnabledAreas.FullSearching,
	[field: FieldOffset(6)] DisabledReason DisabledReason = DisabledReason.None,
	[field: FieldOffset(2)] Stableness Stableness = Stableness.Stable
);
