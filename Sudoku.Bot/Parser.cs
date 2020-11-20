#if AUTHOR_RESERVED

using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;

namespace Sudoku.Bot
{
	/// <summary>
	/// 封装一个常用的解析工具类。
	/// </summary>
	public static class Parser
	{
		/// <summary>
		/// 尝试解析字符串，并转换为一个表示颜色的 ID。这个颜色 ID 一般用来封装和保存到 <see cref="DrawingInfo"/>
		/// 对象里。
		/// </summary>
		/// <param name="str">字符串。</param>
		/// <param name="withTransparency">表示当前解析后的颜色是否保留透明度。</param>
		/// <param name="colorId">(<see langword="out"/> 参数) 转换后的颜色 ID。</param>
		/// <returns>表示是否转换成功。</returns>
		public static bool TryParseColorId(string str, bool withTransparency, out long colorId)
		{
			switch (str)
			{
				case "红色" or "红": colorId = ColorId.ToCustomColorId(withTransparency ? 128 : 255, 235, 0, 0); return true;
				case "浅红色" or "浅红": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 247, 165, 167); return true;
				case "橙色" or "橙": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 255, 192, 89); return true;
				case "浅橙色" or "浅橙": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 247, 222, 143); return true;
				case "黄色" or "黄": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 255, 255, 150); return true;
				case "绿色" or "绿": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 134, 242, 128); return true;
				case "浅绿色" or "浅绿": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 215, 255, 215); return true;
				case "青色" or "青": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 134, 232, 208); return true;
				case "浅青色" or "浅青": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 206, 251, 237); return true;
				case "蓝色" or "蓝": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 0, 0, 255); return true;
				case "浅蓝色" or "浅蓝": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 127, 187, 255); return true;
				case "紫色" or "紫": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 177, 165, 243); return true;
				case "浅紫色" or "浅紫": colorId = ColorId.ToCustomColorId(withTransparency ? 64 : 255, 220, 212, 252); return true;
				default: return ColorIdParser.TryParse(str, out colorId, withTransparency ? 64 : null, new[] { ',', '，' });
			}
		}
	}
}

#endif