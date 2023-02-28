﻿namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Fish</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
/// <param name="BaseSetsMask">Indicates the mask that contains the base sets.</param>
/// <param name="CoverSetsMask">Indicates the mask that contains the cover sets.</param>
internal abstract record FishStep(ConclusionList Conclusions, ViewList Views, int Digit, int BaseSetsMask, int CoverSetsMask) :
	Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	/// <remarks>
	/// The name of the corresponding names are:
	/// <list type="table">
	/// <item><term>2</term><description>X-Wing</description></item>
	/// <item><term>3</term><description>Swordfish</description></item>
	/// <item><term>4</term><description>Jellyfish</description></item>
	/// <item><term>5</term><description>Squirmbag (or Starfish)</description></item>
	/// <item><term>6</term><description>Whale</description></item>
	/// <item><term>7</term><description>Leviathan</description></item>
	/// </list>
	/// Other fishes of sizes not appearing in above don't have well-known names.
	/// </remarks>
	public int Size => PopCount((uint)BaseSetsMask);

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
		var buffer = stackalloc char[name.Length];
		var bufferLength = 0;
		fixed (char* p = name)
		{
			for (var ptr = p; *ptr != '\0'; ptr++)
			{
				if (*ptr is not ('-' or ' '))
				{
					buffer[bufferLength++] = *ptr;
				}
			}
		}

		// Parses via the buffer, and returns the result.
		return Enum.Parse<Technique>(new string(PointerOperations.GetArrayFromStart(buffer, bufferLength, 0)));
	}
}
