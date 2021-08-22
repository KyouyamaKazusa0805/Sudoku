namespace Sudoku.Solving.Manual.Steps.Fishes;

/// <summary>
/// Provides with a step that is a <b>Fish</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="BaseSetsMask">Indicates the mask that contains the base sets.</param>
/// <param name="CoverSetsMask">Indicates the mask that contains the cover sets.</param>
public abstract record FishStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	int Digit,
	int BaseSetsMask,
	int CoverSetsMask
) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool IsSstsStep => base.IsSstsStep;

	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <summary>
	/// Indicates the size of this fish instance.
	/// </summary>
	/// <remarks>
	/// The name of the corresponding names are:
	/// <list type="table">
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
	/// </list>
	/// </remarks>
	public int Size => PopCount((uint)BaseSetsMask);

	/// <summary>
	/// Indicates the rank of the fish.
	/// </summary>
	public int Rank => PopCount((uint)CoverSetsMask) - PopCount((uint)BaseSetsMask);

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Fishes | TechniqueTags.RankTheory;


	/// <summary>
	/// Try to get the <see cref="Technique"/> code instance from the specified name, where the name belongs
	/// to a complex fish name, such as "Finned Franken Swordfish".
	/// </summary>
	/// <param name="name">The name.</param>
	/// <returns>The <see cref="Technique"/> code instance.</returns>
	/// <seealso cref="Technique"/>
	protected internal static unsafe Technique GetComplexFishTechniqueCodeFromName(string name)
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
		return Enum.Parse<Technique>(new string(PointerMarshal.GetArrayFromStart(buffer, bufferLength, 0)));
	}
}
