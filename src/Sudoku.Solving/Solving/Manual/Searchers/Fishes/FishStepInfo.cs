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


		/// <summary>
		/// Try to get the <see cref="Technique"/> code instance from the specified name, where the name belongs
		/// to a complex fish name, such as "Finned Franken Swordfish".
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The <see cref="Technique"/> code instance.</returns>
		/// <seealso cref="Technique"/>
		protected static unsafe Technique GetComplexFishTechniqueCodeFromName(string name)
		{
			// Creates a buffer to store the characters that isn't a space or a bar.
			char* buffer = stackalloc char[name.Length];
			int bufferLength = 0;
			fixed (char* p = name)
			{
				for (char* ptr = p; *ptr != '\0'; ptr++)
				{
					if (*ptr is not ('-' or ' '))
					{
						buffer[bufferLength++] = *ptr;
					}
				}
			}

			// Parses via the buffer, and returns the result.
			return Enum.Parse<Technique>(new string(Pointer.GetArrayFromStart(buffer, bufferLength, 0)));
		}
	}
}
