using Sudoku.Solving.Annotations;

namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	/// <summary>
	/// Indicates the type code for each UR type. Some types have the another name,
	/// which have been listed also.
	/// </summary>
	/// <remarks>
	/// All types can be found in
	/// <a href="https://github.com/Sunnie-Shine/Sudoku/wiki/The-gallery-of-Unique-Rectangles">this link</a>.
	/// </remarks>
	/*closed*/
	public enum UrTypeCode : byte
	{
		/// <summary>
		/// Indicates the type 1 (UR + 1).
		/// </summary>
		[Name("Type 1")]
		[Alias(nameof(TechniqueCode.UrType1))]
		Plus1 = 1,

		/// <summary>
		/// Indicates the type 1 (UR + 1).
		/// </summary>
		[Name("Type 1")]
		[Alias(nameof(TechniqueCode.UrType1))]
		Type1 = 1,

		/// <summary>
		/// Indicates the type 2 (UR + 2x).
		/// </summary>
		[Name("Type 2")]
		[Alias(nameof(TechniqueCode.UrType2))]
		Plus2x = 2,

		/// <summary>
		/// Indicates the type 2 (UR + 2x).
		/// </summary>
		[Name("Type 2")]
		[Alias(nameof(TechniqueCode.UrType2))]
		Type2 = 2,

		/// <summary>
		/// Indicates the type 3 (UR + 2X).
		/// </summary>
		[Name("Type 3")]
		[Alias(nameof(TechniqueCode.UrType3))]
		Plus2X = 3,

		/// <summary>
		/// Indicates the type 3 (UR + 2X).
		/// </summary>
		[Name("Type 3")]
		[Alias(nameof(TechniqueCode.UrType3))]
		Type3 = 3,

		/// <summary>
		/// Indicates the type 4 (UR + 2X / 1SL).
		/// </summary>
		[Name("Type 4")]
		[Alias(nameof(TechniqueCode.UrType4))]
		Plus2X1SL = 4,

		/// <summary>
		/// Indicates the type 4 (UR + 2X / 1SL).
		/// </summary>
		[Name("Type 4")]
		[Alias(nameof(TechniqueCode.UrType4))]
		Type4 = 4,

		/// <summary>
		/// Indicates the type 5 (UR + 2d).
		/// </summary>
		[Name("Type 5")]
		[Alias(nameof(TechniqueCode.UrType5))]
		Plus2d = 5,

		/// <summary>
		/// Indicates the type 5 (UR + 3x).
		/// </summary>
		[Name("Type 5")]
		[Alias(nameof(TechniqueCode.UrType5))]
		Plus3x = 5,

		/// <summary>
		/// Indicates the type 5 (UR + 2d or UR + 3x).
		/// </summary>
		[Name("Type 5")]
		[Alias(nameof(TechniqueCode.UrType5))]
		Type5 = 5,

		/// <summary>
		/// Indicates the type 6.
		/// </summary>
		[Name("Type 6")]
		[Alias(nameof(TechniqueCode.UrType6))]
		Type6 = 6,

		/// <summary>
		/// Indicates the hidden UR (UR + 3C / 2SL).
		/// </summary>
		[Name("Hidden Unique Rectangle")]
		[Alias(nameof(TechniqueCode.HiddenUr))]
		Hidden = 7,

		/// <summary>
		/// Indicates the hidden UR (UR + 3C / 2SL).
		/// </summary>
		[Name("Hidden Unique Rectangle")]
		[Alias(nameof(TechniqueCode.HiddenUr))]
		Plus3C2SL = 7,

		/// <summary>
		/// Indicates the UR + 2D.
		/// </summary>
		[Name("+ 2D")]
		[Alias(nameof(TechniqueCode.UrPlus2D))]
		Plus2D = 8,

		/// <summary>
		/// Indicates the UR + 2B / 1SL.
		/// </summary>
		[Name("+ 2B / 1SL")]
		[Alias(nameof(TechniqueCode.UrPlus2B1SL))]
		Plus2B1SL = 9,

		/// <summary>
		/// Indicates the UR + 2D / 1SL.
		/// </summary>
		[Name("+ 2D / 1SL")]
		[Alias(nameof(TechniqueCode.UrPlus2D1SL))]
		Plus2D1SL = 10,

		/// <summary>
		/// Indicates the UR + 3X.
		/// </summary>
		[Name("+ 3X")]
		[Alias(nameof(TechniqueCode.UrPlus3X))]
		Plus3X = 11,

		/// <summary>
		/// Indicates the UR + 3x / 1SL.
		/// </summary>
		[Name("+ 3x / 1SL")]
		[Alias(nameof(TechniqueCode.UrPlus3x1SL))]
		Plus3x1SL = 12,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		[Name("+ 3X / 1SL")]
		[Alias(nameof(TechniqueCode.UrPlus3X1SL))]
		Plus3X1SL = 13,

		/// <summary>
		/// Indicates the UR + 3X / 2SL.
		/// </summary>
		[Name("+ 3X / 2SL")]
		[Alias(nameof(TechniqueCode.UrPlus3X2SL))]
		Plus3X2SL = 14,

		/// <summary>
		/// Indicates the UR + 3N / 2SL.
		/// </summary>
		[Name("+ 3N / 2SL")]
		[Alias(nameof(TechniqueCode.UrPlus3N2SL))]
		Plus3N2SL = 15,

		/// <summary>
		/// Indicates the UR + 3U / 2SL.
		/// </summary>
		[Name("+ 3U / 2SL")]
		[Alias(nameof(TechniqueCode.UrPlus3U2SL))]
		Plus3U2SL = 16,

		/// <summary>
		/// Indicates the UR + 3E / 2SL.
		/// </summary>
		[Name("+ 3E / 2SL")]
		[Alias(nameof(TechniqueCode.UrPlus3E2SL))]
		Plus3E2SL = 17,

		/// <summary>
		/// Indicates the UR + 4x / 1SL.
		/// </summary>
		[Name("+ 4x / 1SL")]
		[Alias(nameof(TechniqueCode.UrPlus4x1SL))]
		Plus4x1SL = 18,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		[Name("+ 4X / 1SL")]
		[Alias(nameof(TechniqueCode.UrPlus4X1SL))]
		Plus4X1SL = 19,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		[Name("+ 4x / 2SL")]
		[Alias(nameof(TechniqueCode.UrPlus4x2SL))]
		Plus4x2SL = 20,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		[Name("+ 4X / 2SL")]
		[Alias(nameof(TechniqueCode.UrPlus4X2SL))]
		Plus4X2SL = 21,

		/// <summary>
		/// Indicates the UR + 4X / 3SL.
		/// </summary>
		[Name("+ 4X / 3SL")]
		[Alias(nameof(TechniqueCode.UrPlus4X3SL))]
		Plus4X3SL = 22,

		/// <summary>
		/// Indicates the UR + 4C / 3SL.
		/// </summary>
		[Name("+ 4C / 3SL")]
		[Alias(nameof(TechniqueCode.UrPlus4C3SL))]
		Plus4C3SL = 23,

		/// <summary>
		/// Indicates the UR-XY-Wing.
		/// </summary>
		[Name("XY-Wing")]
		[Alias(nameof(TechniqueCode.UrXyWing))]
		XyWing = 24,

		/// <summary>
		/// Indicates the UR-XYZ-Wing.
		/// </summary>
		[Name("XYZ-Wing")]
		[Alias(nameof(TechniqueCode.UrXyzWing))]
		XyzWing = 25,

		/// <summary>
		/// Indicates the UR-WXYZ-Wing.
		/// </summary>
		[Name("WXYZ-Wing")]
		[Alias(nameof(TechniqueCode.UrWxyzWing))]
		WxyzWing = 26,

		/// <summary>
		/// Indicates the UR-SdC.
		/// </summary>
		[Name("Sue de Coq")]
		[Alias(nameof(TechniqueCode.UrSdc))]
		Sdc = 27,

		/// <summary>
		/// Indicates the type 1 (AR + 1).
		/// </summary>
		[Name("Type 1")]
		[Alias(nameof(TechniqueCode.ArType1))]
		APlus1 = 28,

		/// <summary>
		/// Indicates the type 1 (AR + 1).
		/// </summary>
		[Name("Type 1")]
		[Alias(nameof(TechniqueCode.ArType1))]
		AType1 = 28,

		/// <summary>
		/// Indicates the type 2 (AR + 2x).
		/// </summary>
		[Name("Type 2")]
		[Alias(nameof(TechniqueCode.ArType2))]
		APlus2x = 29,

		/// <summary>
		/// Indicates the type 2 (AR + 2x).
		/// </summary>
		[Name("Type 2")]
		[Alias(nameof(TechniqueCode.ArType2))]
		AType2 = 29,

		/// <summary>
		/// Indicates the type 3 (AR + 2X).
		/// </summary>
		[Name("Type 3")]
		[Alias(nameof(TechniqueCode.ArType3))]
		APlus2X = 30,

		/// <summary>
		/// Indicates the type 3 (AR + 2X).
		/// </summary>
		[Name("Type 3")]
		[Alias(nameof(TechniqueCode.ArType3))]
		AType3 = 30,

		/// <summary>
		/// Indicates the type 5 (AR + 2d).
		/// </summary>
		[Name("Type 5")]
		[Alias(nameof(TechniqueCode.ArType5))]
		APlus2d = 32,

		/// <summary>
		/// Indicates the type 5 (AR + 3x).
		/// </summary>
		[Name("Type 5")]
		[Alias(nameof(TechniqueCode.ArType5))]
		APlus3x = 32,

		/// <summary>
		/// Indicates the type 5 (AR + 2d or AR + 3x).
		/// </summary>
		[Name("Type 5")]
		[Alias(nameof(TechniqueCode.ArType5))]
		AType5 = 32,

		/// <summary>
		/// Indicates the hidden AR (AR + 3C / 2SL).
		/// </summary>
		[Name("Hidden Unique Rectangle")]
		[Alias(nameof(TechniqueCode.HiddenAr))]
		AHidden = 34,

		/// <summary>
		/// Indicates the hidden AR (AR + 3C / 2SL).
		/// </summary>
		[Name("Hidden Unique Rectangle")]
		[Alias(nameof(TechniqueCode.HiddenAr))]
		APlus3C2SL = 34,

		/// <summary>
		/// Indicates the AR + 2D.
		/// </summary>
		[Name("+ 2D")]
		[Alias(nameof(TechniqueCode.ArPlus2D))]
		APlus2D = 35,

		/// <summary>
		/// Indicates the AR + 3X.
		/// </summary>
		[Name("+ 3X")]
		[Alias(nameof(TechniqueCode.ArPlus3X))]
		APlus3X = 38,

		/// <summary>
		/// Indicates the AR-XY-Wing.
		/// </summary>
		[Name("XY-Wing")]
		[Alias(nameof(TechniqueCode.ArXyWing))]
		AXyWing = 51,

		/// <summary>
		/// Indicates the AR-XYZ-Wing.
		/// </summary>
		[Name("XYZ-Wing")]
		[Alias(nameof(TechniqueCode.ArXyzWing))]
		AXyzWing = 51,

		/// <summary>
		/// Indicates the AR-WXYZ-Wing.
		/// </summary>
		[Name("WXYZ-Wing")]
		[Alias(nameof(TechniqueCode.ArWxyzWing))]
		AWxyzWing = 52,

		/// <summary>
		/// Indicates the AR-SdC.
		/// </summary>
		[Name("Sue de Coq")]
		[Alias(nameof(TechniqueCode.ArSdc))]
		ASdc = 53,
	}
}
