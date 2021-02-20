namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Provides diagnostic IDs.
	/// </summary>
	internal static class DiagnosticIds
	{
		public const string Sudoku001 = "SUDOKU001";
		public const string Sudoku002 = "SUDOKU002";
		public const string Sudoku003 = "SUDOKU003";
		public const string Sudoku004 = "SUDOKU004";
		public const string Sudoku005 = "SUDOKU005";
	}

	/// <summary>
	/// Provides help links.
	/// </summary>
	internal static class HelpLinks
	{
		public const string Sudoku001 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU001%EF%BC%9AProperties%20%E5%B1%9E%E6%80%A7%E5%BF%85%E9%A1%BB%E6%98%AF%20public%20%E7%9A%84?sort_id=3599808";
		public const string Sudoku002 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU002%EF%BC%9AProperties%20%E5%B1%9E%E6%80%A7%E5%BF%85%E9%A1%BB%E6%98%AF%E5%8F%AA%E8%AF%BB%E7%9A%84?sort_id=3599816";
		public const string Sudoku003 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU003%EF%BC%9AProperties%20%E6%8B%A5%E6%9C%89%E9%94%99%E8%AF%AF%E7%9A%84%E6%95%B0%E6%8D%AE%E7%B1%BB%E5%9E%8B?sort_id=3599818";
		public const string Sudoku004 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU004%EF%BC%9A%E4%BB%8E%20StepSearcher%20%E6%B4%BE%E7%94%9F%E7%9A%84%E7%B1%BB%E5%BF%85%E9%A1%BB%E6%8B%A5%E6%9C%89%20Properties%20%E5%B1%9E%E6%80%A7?sort_id=3599824";
		public const string Sudoku005 = "https://gitee.com/SunnieShine/Sudoku/wikis/SUDOKU005%EF%BC%9AProperties%20%E5%B1%9E%E6%80%A7%E4%B8%8D%E5%8F%AF%E4%B8%BA%20null?sort_id=3599826";
	}

	/// <summary>
	/// Provides the categories.
	/// </summary>
	internal static class Categories
	{
		/// <summary>
		/// Indicates the categories is the requirement.
		/// </summary>
		public const string Requirements = nameof(Requirements);

		/// <summary>
		/// Indicates the categories is the usage.
		/// </summary>
		public const string Usage = nameof(Usage);
	}

	/// <summary>
	/// Provides the titles.
	/// </summary>
	internal static class Titles
	{
		public const string Sudoku001 = "A public property 'Properties' expected";
		public const string Sudoku002 = "The property 'Properties' should be read-only";
		public const string Sudoku003 = "The property 'Properties' has a wrong type";
		public const string Sudoku004 = "A property named 'Properties' expected";
		public const string Sudoku005 = "The property 'Properties' can't be null";
	}

	/// <summary>
	/// Provides the messages.
	/// </summary>
	internal static class Messages
	{
		public const string Sudoku001 = "A public property 'Properties' expected";
		public const string Sudoku002 = "The property 'Properties' should be read-only";
		public const string Sudoku003 = "The property 'Properties' should be of type 'Sudoku.Solving.Manual.TechiqueProperties'";
		public const string Sudoku004 = "A property named 'Properties' expected";
		public const string Sudoku005 = "The property 'Properties' can't be null; try to remove the nullable notation '?'";
	}
}
