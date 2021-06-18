using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Fishes
{
	/// <summary>
	/// Provides a usage of <b>fish</b> technique.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	/// <param name="Digit">The digit used.</param>
	/// <param name="BaseSets">The base sets.</param>
	/// <param name="CoverSets">The cover sets.</param>
	public abstract record FishStepInfo(
		IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views, int Digit,
		IReadOnlyList<int> BaseSets, IReadOnlyList<int> CoverSets
	) : StepInfo(Conclusions, Views)
	{
		/// <summary>
		/// The names of all fishes by their sizes.
		/// </summary>
		protected static readonly string[] FishNames =
		{
			string.Empty, "Cyclopsfish", "X-Wing", "Swordfish", "Jellyfish",
			"Squirmbag", "Whale", "Leviathan", "Octopus", "Dragon"
		};


		/// <summary>
		/// Indicates the size of this fish instance.
		/// </summary>
		/// <remarks>
		/// The name of the corresponding names are:
		/// <list type="table">
		/// <item>
		/// <term>1</term>
		/// <description>Cyclops (But out of use)</description>
		/// </item>
		/// <item>
		/// <term>2</term>
		/// <description>X-Wing</description>
		/// </item>
		/// <item>
		/// <term>3</term>
		/// <description>Swordfish</description>
		/// </item>
		/// <item>
		/// <term>4</term>
		/// <description>Jellyfish</description>
		/// </item>
		/// <item>
		/// <term>5</term>
		/// <description>Squirmbag or Starfish</description>
		/// </item>
		/// <item>
		/// <term>6</term>
		/// <description>Whale</description>
		/// </item>
		/// <item>
		/// <term>7</term>
		/// <description>Leviathan</description>
		/// </item>
		/// <item>
		/// <term>&gt; 7</term>
		/// <description>No name</description>
		/// </item>
		/// </list>
		/// </remarks>
		public int Size => BaseSets.Count;

		/// <summary>
		/// Indicates the rank of the fish.
		/// </summary>
		public int Rank => CoverSets.Count - BaseSets.Count;

		/// <inheritdoc/>
		public sealed override bool ShowDifficulty => base.ShowDifficulty;

		/// <inheritdoc/>
		public sealed override TechniqueTags TechniqueTags => TechniqueTags.Fishes | TechniqueTags.RankTheory;
	}
}
