namespace Sudoku.Bot.Annotations;

/// <summary>
/// 表示一种游戏玩法的名称。
/// </summary>
/// <param name="name">名称。</param>
public sealed partial class GameModeNameAttribute([Property] string name) : GameModeAnnotationAttribute;
