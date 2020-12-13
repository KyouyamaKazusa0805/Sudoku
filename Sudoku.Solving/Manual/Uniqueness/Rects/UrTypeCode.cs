#pragma warning disable CA1069

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
		[Alias(nameof(TechniqueCode.UrType1))]
		Plus1 = 1,

		/// <summary>
		/// Indicates the type 1 (UR + 1).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType1))]
		Type1 = 1,

		/// <summary>
		/// Indicates the type 2 (UR + 2x).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType2))]
		Plus2x = 2,

		/// <summary>
		/// Indicates the type 2 (UR + 2x).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType2))]
		Type2 = 2,

		/// <summary>
		/// Indicates the type 3 (UR + 2X).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType3))]
		Type3 = 3,

		/// <summary>
		/// Indicates the type 4 (UR + 2X / 1SL).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType4))]
		Plus2X1SL = 4,

		/// <summary>
		/// Indicates the type 4 (UR + 2X / 1SL).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType4))]
		Type4 = 4,

		/// <summary>
		/// Indicates the type 5 (UR + 2d).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType5))]
		Plus2d_Lower = 5,

		/// <summary>
		/// Indicates the type 5 (UR + 3x).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType5))]
		Plus3x_Lower = 5,

		/// <summary>
		/// Indicates the type 5 (UR + 2d or UR + 3x).
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType5))]
		Type5 = 5,

		/// <summary>
		/// Indicates the type 6.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrType6))]
		Type6 = 6,

		/// <summary>
		/// Indicates the hidden UR (UR + 3C / 2SL).
		/// </summary>
		[Alias(nameof(TechniqueCode.HiddenUr))]
		Hidden = 7,

		/// <summary>
		/// Indicates the hidden UR (UR + 3C / 2SL).
		/// </summary>
		[Alias(nameof(TechniqueCode.HiddenUr))]
		Plus3C2SL = 7,

		/// <summary>
		/// Indicates the UR + 2D.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus2D))]
		Plus2D_Upper = 8,

		/// <summary>
		/// Indicates the UR + 2B / 1SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus2B1SL))]
		Plus2B1SL = 9,

		/// <summary>
		/// Indicates the UR + 2D / 1SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus2D1SL))]
		Plus2D1SL = 10,

		/// <summary>
		/// Indicates the UR + 3X.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3X))]
		Plus3X_Upper = 11,

		/// <summary>
		/// Indicates the UR + 3x / 1SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3x1SL_Lower))]
		Plus3x1SL_Lower = 12,

		/// <summary>
		/// Indicates the UR + 3X / 1SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3X1SL_Upper))]
		Plus3X1SL_Upper = 13,

		/// <summary>
		/// Indicates the UR + 3X / 2SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3X2SL))]
		Plus3X2SL = 14,

		/// <summary>
		/// Indicates the UR + 3N / 2SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3N2SL))]
		Plus3N2SL = 15,

		/// <summary>
		/// Indicates the UR + 3U / 2SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3U2SL))]
		Plus3U2SL = 16,

		/// <summary>
		/// Indicates the UR + 3E / 2SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus3E2SL))]
		Plus3E2SL = 17,

		/// <summary>
		/// Indicates the UR + 4x / 1SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus4x1SL_Lower))]
		Plus4x1SL_Lower = 18,

		/// <summary>
		/// Indicates the UR + 4X / 1SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus4X1SL_Upper))]
		Plus4X1SL_Upper = 19,

		/// <summary>
		/// Indicates the UR + 4x / 2SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus4x2SL_Lower))]
		Plus4x2SL_Lower = 20,

		/// <summary>
		/// Indicates the UR + 4X / 2SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus4X2SL_Upper))]
		Plus4X2SL_Upper = 21,

		/// <summary>
		/// Indicates the UR + 4X / 3SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus4X3SL))]
		Plus4X3SL = 22,

		/// <summary>
		/// Indicates the UR + 4C / 3SL.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrPlus4C3SL))]
		Plus4C3SL = 23,

		/// <summary>
		/// Indicates the UR-XY-Wing.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrXyWing))]
		XyWing = 24,

		/// <summary>
		/// Indicates the UR-XYZ-Wing.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrXyzWing))]
		XyzWing = 25,

		/// <summary>
		/// Indicates the UR-WXYZ-Wing.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrWxyzWing))]
		WxyzWing = 26,

		/// <summary>
		/// Indicates the UR-SdC.
		/// </summary>
		[Alias(nameof(TechniqueCode.UrSdc))]
		Sdc = 27,

		/// <summary>
		/// Indicates the type 1 (AR + 1).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType1))]
		APlus1 = 28,

		/// <summary>
		/// Indicates the type 1 (AR + 1).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType1))]
		AType1 = 28,

		/// <summary>
		/// Indicates the type 2 (AR + 2x).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType2))]
		AType2 = 29,

		/// <summary>
		/// Indicates the type 3 (AR + 2X).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType3))]
		APlus2X = 30,

		/// <summary>
		/// Indicates the type 3 (AR + 2X).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType3))]
		AType3 = 30,

		/// <summary>
		/// Indicates the type 5 (AR + 2d).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType5))]
		APlus2d_Lower = 32,

		/// <summary>
		/// Indicates the type 5 (AR + 3x).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType5))]
		APlus3x_Lower = 32,

		/// <summary>
		/// Indicates the type 5 (AR + 2d or AR + 3x).
		/// </summary>
		[Alias(nameof(TechniqueCode.ArType5))]
		AType5 = 32,

		/// <summary>
		/// Indicates the hidden AR (AR + 3C / 2SL).
		/// </summary>
		[Alias(nameof(TechniqueCode.HiddenAr))]
		AHidden = 34,

		/// <summary>
		/// Indicates the hidden AR (AR + 3C / 2SL).
		/// </summary>
		[Alias(nameof(TechniqueCode.HiddenAr))]
		APlus3C2SL = 34,

		/// <summary>
		/// Indicates the AR + 2D.
		/// </summary>
		[Alias(nameof(TechniqueCode.ArPlus2D))]
		APlus2D_Upper = 35,

		/// <summary>
		/// Indicates the AR + 3X.
		/// </summary>
		[Alias(nameof(TechniqueCode.ArPlus3X))]
		APlus3X_Upper = 38,

		/// <summary>
		/// Indicates the AR-XY-Wing.
		/// </summary>
		[Alias(nameof(TechniqueCode.ArXyWing))]
		AXyWing = 51,

		/// <summary>
		/// Indicates the AR-XYZ-Wing.
		/// </summary>
		[Alias(nameof(TechniqueCode.ArXyzWing))]
		AXyzWing = 52,

		/// <summary>
		/// Indicates the AR-WXYZ-Wing.
		/// </summary>
		[Alias(nameof(TechniqueCode.ArWxyzWing))]
		AWxyzWing = 53,

		/// <summary>
		/// Indicates the AR-SdC.
		/// </summary>
		[Alias(nameof(TechniqueCode.ArSdc))]
		ASdc = 54,
	}
}
