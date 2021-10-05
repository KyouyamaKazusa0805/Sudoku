namespace Sudoku.CodeGenerating;

/// <summary>
/// Defines the constants used in this project.
/// </summary>
internal static partial class Constants
{
	/// <summary>
	/// Indictaes the version of this project.
	/// </summary>
	public const string VersionValue = "0.3";


	/// <summary>
	/// Indicates the collection that stores the conversion relations from type keywords to their BCL names.
	/// </summary>
	public static readonly IReadOnlyDictionary<string, string> KeywordsToBclNames = new Dictionary<string, string>
	{
		["int"] = "System.Int32",
		["uint"] = "System.UInt32",
		["short"] = "System.Int16",
		["ushort"] = "System.UInt16",
		["long"] = "System.Int64",
		["ulong"] = "System.UInt64",
		["byte"] = "System.Byte",
		["sbyte"] = "System.SByte",
		["bool"] = "System.Boolean",
		["nint"] = "System.IntPtr",
		["nuint"] = "System.UIntPtr",
		["float"] = "System.Single",
		["double"] = "System.Double",
		["decimal"] = "System.Decimal",
		["char"] = "System.Char",
		["string"] = "System.String",
		["object"] = "System.Object"
	};
}
