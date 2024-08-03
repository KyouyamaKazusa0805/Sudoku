namespace Sudoku.Bot.CosService;

/// <summary>
/// 表示使用对象存储服务后，上传文件后返回的结果。
/// </summary>
/// <param name="Link">文件的在线访问路径。该数值可被 QQ 读取使用，用于转存图片，达成发图的作用。</param>
/// <param name="ETag">对象存储服务成功后返回的文件校验结果（一个被 MD5 加密过的字符串）。</param>
public readonly record struct UploadedResult(string Link, string ETag);
