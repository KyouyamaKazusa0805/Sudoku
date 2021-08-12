namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	/// <summary>
	/// Provides a usage of <b>Borescoper's deadly pattern type 4</b> (BDP) technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Map">The cells used.</param>
	/// <param name="DigitsMask">The digits mask.</param>
	/// <param name="ConjugateRegion">
	/// <para>The so-called conjugate region.</para>
	/// <para>
	/// A <b>conjugate region</b> is a serial of cells that all lies in a same region, but only these cells
	/// can be filled with the digits specified, other cells in this region can't be filled with that digit.
	/// </para>
	/// <para>
	/// Although the name of this term is <b>conjugate "region"</b>, but a region can contain not only
	/// those cells at present.
	/// </para>
	/// </param>
	/// <param name="ExtraMask">Indicates the mask of digits that is the combination.</param>
	public sealed record BdpType4StepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views,
		in Cells Map, short DigitsMask, in Cells ConjugateRegion, short ExtraMask
	) : BdpStepInfo(Conclusions, Views, Map, DigitsMask)
	{
		/// <inheritdoc/>
		public override decimal Difficulty => 5.5M;

		/// <inheritdoc/>
		public override Technique TechniqueCode => Technique.BdpType4;

		[FormatItem]
		private string ExtraCombStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new DigitCollection(ExtraMask).ToString();
		}

		[FormatItem]
		private string ConjRegionStr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ConjugateRegion.ToString();
		}
	}
}
