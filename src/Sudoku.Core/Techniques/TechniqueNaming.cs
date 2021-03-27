using System;
using System.Extensions;

namespace Sudoku.Techniques
{
	/// <summary>
	/// Provides the methods that uses or handles about types <see cref="Technique"/> and <see cref="string"/>.
	/// </summary>
	/// <seealso cref="Technique"/>
	public static class TechniqueNaming
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
		public static unsafe Technique GetComplexFishTechniqueCodeFromName(string name)
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
