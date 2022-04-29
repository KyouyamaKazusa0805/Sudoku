namespace Sudoku.Bot.Oicq.Concepts.CodeProviders;

/// <summary>
/// Indicates the code provider.
/// </summary>
public abstract class CodeProvider
{
	/// <summary>
	/// The dictionary that can convert the codes between CQ and IR framework.
	/// </summary>
	private static readonly Dictionary<string, (string Ir, string Cq)> Ir2CqRegexPatterns = new()
	{
		{ "Image", ("""[pic=$1]""", """[CQ:image,file=$1]""") },
		{ "Face", ("""[Face$1.gif]""", """[CQ:face,id=$1]""") },
		{ "Record", ("""[Voi=$1]""", """[CQ:record,file=$1]""") },
		{ "At", ("""[@$1]""", """[CQ:at,qq=$1]""") },
	};


	/// <summary>
	/// Converts the specified string value as CQ code to IR code.
	/// </summary>
	/// <param name="str">The string value as CQ code.</param>
	/// <returns>The IR code.</returns>
	public static string ConvertCqCodeToIrCode(string str)
	{
		foreach (var regex in Ir2CqRegexPatterns)
		{
			var (ir, cq) = regex.Value;
			str = Regex.Replace(
				str,
				cq.Replace("[", """\[""").Replace("]", """\]""").Replace("$1", """(.*?)"""),
				ir
			);
		}

		return str;
	}

	/// <summary>
	/// Converts the specified string value as IR code to CQ code.
	/// </summary>
	/// <param name="str">The string value as IR code.</param>
	/// <returns>The CQ code.</returns>
	public static string ConvertIrCodeToCqCode(string str)
	{
		foreach (var regex in Ir2CqRegexPatterns)
		{
			var (ir, cq) = regex.Value;
			str = Regex.Replace(
				str,
				ir.Replace("[", """\[""").Replace("]", """\]""").Replace("$1", """(.*?)"""),
				cq
			);
		}

		return str;
	}

	/// <summary>
	/// Make an mention to somebody.
	/// </summary>
	/// <param name="target">
	/// The object you want to mention. The value is a <see cref="string"/> representation of the QQ number for him.
	/// </param>
	/// <returns>The string representation of the command code for the mention operation.</returns>
	public virtual string At(string target) => $"[@{target}]";

	/// <summary>
	/// Make an image via the specified local path.
	/// </summary>
	/// <param name="path">The path.</param>
	/// <returns>The string representation of the command code for the image operation.</returns>
	public virtual string Image(string path) => $"[pic={path}]";
}
