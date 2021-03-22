namespace Sudoku.Techniques
{
	/// <summary>
	/// Provides the <see cref="string"/> values to be used.
	/// </summary>
	public static class TechniqueStrings
	{
		/// <summary>
		/// <para>The names of all subsets by their sizes.</para>
		/// <para>
		/// For example, if you want to get the name of the size 3, the code will be
		/// <code>
		/// string name = TechniqueStrings.SubsetNames[3];
		/// </code>
		/// Here the digit <c>3</c> is the size you want to get.
		/// </para>
		/// </summary>
		public static readonly string[] SubsetNames =
		{
			string.Empty, "Single", "Pair", "Triple", "Quadruple",
			"Quintuple", "Sextuple", "Septuple", "Octuple", "Nonuple"
		};

		/// <summary>
		/// <para>The names of all regular wings by their sizes.</para>
		/// <para>
		/// For example, if you want to get the name of the size 3, the code will be
		/// <code>
		/// string name = TechniqueStrings.RegularWingNames[3];
		/// </code>
		/// Here the digit <c>3</c> is the size you want to get.
		/// </para>
		/// </summary>
		public static readonly string[] RegularWingNames =
		{
			string.Empty, string.Empty, string.Empty, string.Empty, "WXYZ-Wing", "VWXYZ-Wing",
			"UVWXYZ-Wing", "TUVWXYZ-Wing", "STUVWXYZ-Wing", "RSTUVWXYZ-Wing"
		};


		/// <summary>
		/// Try to get the <see cref="Technique"/> code instance from the specified name, where the name belongs
		/// to a complex fish name, such as "Finned Franken Swordfish".
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The <see cref="Technique"/> code instance.</returns>
		/// <seealso cref="Technique"/>
		public static Technique GetComplexFishTechniqueCodeFromName(string name) => name switch
		{
			"X-Wing" => Technique.XWing,
			"Finned X-Wing" => Technique.FinnedXWing,
			"Sashimi X-Wing" => Technique.SashimiXWing,
			"Franken X-Wing" => Technique.FrankenXWing,
			"Finned Franken X-Wing" => Technique.FinnedFrankenXWing,
			"Sashimi Franken X-Wing" => Technique.SashimiFrankenXWing,
			"Mutant X-Wing" => Technique.MutantXWing,
			"Finned Mutant X-Wing" => Technique.FinnedMutantXWing,
			"Sashimi Mutant X-Wing" => Technique.SashimiMutantXWing,
			"Swordfish" => Technique.Swordfish,
			"Finned Swordfish" => Technique.FinnedSwordfish,
			"Sashimi Swordfish" => Technique.SashimiSwordfish,
			"Franken Swordfish" => Technique.FrankenSwordfish,
			"Finned Franken Swordfish" => Technique.FinnedFrankenSwordfish,
			"Sashimi Franken Swordfish" => Technique.SashimiFrankenSwordfish,
			"Mutant Swordfish" => Technique.MutantSwordfish,
			"Finned Mutant Swordfish" => Technique.FinnedMutantSwordfish,
			"Sashimi Mutant Swordfish" => Technique.SashimiMutantSwordfish,
			"Jellyfish" => Technique.Jellyfish,
			"Finned Jellyfish" => Technique.FinnedJellyfish,
			"Sashimi Jellyfish" => Technique.SashimiJellyfish,
			"Franken Jellyfish" => Technique.FrankenJellyfish,
			"Finned Franken Jellyfish" => Technique.FinnedFrankenJellyfish,
			"Sashimi Franken Jellyfish" => Technique.SashimiFrankenJellyfish,
			"Mutant Jellyfish" => Technique.MutantJellyfish,
			"Finned Mutant Jellyfish" => Technique.FinnedMutantJellyfish,
			"Sashimi Mutant Jellyfish" => Technique.SashimiMutantJellyfish,
			"Squirmbag" => Technique.Squirmbag,
			"Finned Squirmbag" => Technique.FinnedSquirmbag,
			"Sashimi Squirmbag" => Technique.SashimiSquirmbag,
			"Franken Squirmbag" => Technique.FrankenSquirmbag,
			"Finned Franken Squirmbag" => Technique.FinnedFrankenSquirmbag,
			"Sashimi Franken Squirmbag" => Technique.SashimiFrankenSquirmbag,
			"Mutant Squirmbag" => Technique.MutantSquirmbag,
			"Finned Mutant Squirmbag" => Technique.FinnedMutantSquirmbag,
			"Sashimi Mutant Squirmbag" => Technique.SashimiMutantSquirmbag,
			"Whale" => Technique.Whale,
			"Finned Whale" => Technique.FinnedWhale,
			"Sashimi Whale" => Technique.SashimiWhale,
			"Franken Whale" => Technique.FrankenWhale,
			"Finned Franken Whale" => Technique.FinnedFrankenWhale,
			"Sashimi Franken Whale" => Technique.SashimiFrankenWhale,
			"Mutant Whale" => Technique.MutantWhale,
			"Finned Mutant Whale" => Technique.FinnedMutantWhale,
			"Sashimi Mutant Whale" => Technique.SashimiMutantWhale,
			"Leviathan" => Technique.Leviathan,
			"Finned Leviathan" => Technique.FinnedLeviathan,
			"Sashimi Leviathan" => Technique.SashimiLeviathan,
			"Franken Leviathan" => Technique.FrankenLeviathan,
			"Finned Franken Leviathan" => Technique.FinnedFrankenLeviathan,
			"Sashimi Franken Leviathan" => Technique.SashimiFrankenLeviathan,
			"Mutant Leviathan" => Technique.MutantLeviathan,
			"Finned Mutant Leviathan" => Technique.FinnedMutantLeviathan,
			"Sashimi Mutant Leviathan" => Technique.SashimiMutantLeviathan
		};
	}
}
