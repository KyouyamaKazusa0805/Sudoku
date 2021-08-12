namespace Sudoku.Solving.Manual.Alses;

/// <summary>
/// Provides a usage of <b>almost locked sets XZ rule</b> (ALS-XZ)
/// or <b>extended subset principle</b> technique.
/// </summary>
/// <param name="Conclusions">All conclusions.</param>
/// <param name="Views">All views.</param>
/// <param name="Als1">The ALS 1 used.</param>
/// <param name="Als2">The ALS 2 used.</param>
/// <param name="XDigitsMask">The X digits mask.</param>
/// <param name="ZDigitsMask">The Z digits mask.</param>
/// <param name="IsDoublyLinked">
/// <para>Indicates whether the instance is a doubly linked ALS-XZ.</para>
/// <para>
/// The property contains three different values:
/// <list type="table">
/// <item>
/// <term><c><see langword="true"/></c></term>
/// <description>The current instance is a Doubly Linked ALS-XZ.</description>
/// </item>
/// <item>
/// <term><c><see langword="false"/></c></term>
/// <description>The current instance is a Singly Linked ALS-XZ.</description>
/// </item>
/// <item>
/// <term><c><see langword="null"/></c></term>
/// <description>The current instance is a Extended Subset Principle.</description>
/// </item>
/// </list>
/// </para>
/// </param>
public sealed record AlsXzStepInfo(
	IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, in Als Als1, in Als Als2,
	short XDigitsMask, short ZDigitsMask, bool? IsDoublyLinked
) : AlsStepInfo(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => IsDoublyLinked is true ? 5.7M : 5.5M;

	/// <inheritdoc/>
	public override string? Acronym => "ALS-XZ";

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => base.TechniqueTags | TechniqueTags.ShortChaining;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.AlsChainingLike;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public override Technique TechniqueCode => IsDoublyLinked switch
	{
		true => Technique.DoublyLinkedAlsXz,
		false => Technique.SinglyLinkedAlsXz,
		null => Technique.Esp
	};

	/// <inheritdoc/>
	public override string? Format =>
		IsDoublyLinked is null
			// Extended Subset Principle.
			? ZDigitsMask == 0
				? TextResources.Current.Format_AlsXzStepInfo_1
				: TextResources.Current.Format_AlsXzStepInfo_2
			// Normal ALS-XZ.
			: TextResources.Current.Format_AlsXzStepInfo_3;

	[FormatItem]
	private string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Als1.Map | Als2.Map).ToString();
	}

	[FormatItem]
	private string EspDigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (TrailingZeroCount(ZDigitsMask) + 1).ToString();
	}

	[FormatItem]
	private string Als1Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als1.ToString();
	}

	[FormatItem]
	private string Als2Str
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Als2.ToString();
	}

	[FormatItem]
	private string XStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(XDigitsMask).ToString();
	}

	[FormatItem]
	private string ZResultStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ZDigitsMask != 0 ? $", Z = {new DigitCollection(ZDigitsMask).ToString()}" : string.Empty;
	}
}
