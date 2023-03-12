#if false
namespace Sudoku.Platforms.QQ.Modules.Group;

[BuiltIn]
[SupportedOSPlatform("windows")]
file sealed class PuzzleLibraryExtractModule : GroupModule
{
	/// <inheritdoc/>
	public override string RaisingCommand => "抽题";

	[DoubleArgument("群号")]
	public string? PuzzleLibraryContainingGroupId { get; set; }

	[DoubleArgument("群名")]
	public string? PuzzleLibraryContainingGroupName { get; set; }

	[DoubleArgument("题库名")]
	public string? PuzzleLibraryName { get; set; }

	[DoubleArgument("编号")]
	[ArgumentValueConverter<Int32Converter>]
	public int PuzzleId { get; set; }


	/// <inheritdoc/>
	protected override async Task ExecuteCoreAsync(GroupMessageReceiver messageReceiver)
	{
		if (messageReceiver is not { GroupId: var groupId })
		{
			return;
		}

		await messageReceiver.SendMessageAsync("正在解析题库，请稍候……");

		switch (PuzzleLibraryContainingGroupId, PuzzleLibraryContainingGroupName, PuzzleLibraryName, PuzzleId)
		{
			case (null, null, null, _):
			{
				switch (InternalReadWrite.ReadLibraries(groupId))
				{
					case null or []:
					{
						await messageReceiver.SendMessageAsync("本群不包含任何的题库。请联系群主、管理员和机器人设计人员存储题库后重试。");
						break;
					}
					case [{ Name: var libName, PuzzleFilePath: var path }]:
					{
						await extractPuzzleAsync(libName, path, groupId);
						break;
					}
					default:
					{
						await messageReceiver.SendMessageAsync("本群存在至少两个题库，因此无法使用简便查询语法搜索题库。请至少带出题库名称。");
						break;
					}
				}

				break;
			}
			case ({ } containingGroupId, null, { } puzzleLibName, var puzzleId):
			{
				switch (InternalReadWrite.ReadLibraries(containingGroupId))
				{
					case null or []:
					{
						await messageReceiver.SendMessageAsync("本群不包含任何的题库。请联系群主、管理员和机器人设计人员存储题库后重试。");
						break;
					}
					case var libs:
					{
						var lib = default(PuzzleLibraryData?);
						for (var i = 0; i < libs.Count; i++)
						{
							var currentLib = libs[i];
							if (currentLib.Name != puzzleLibName)
							{
								continue;
							}


						}
					}
				}

				break;
			}


			async Task<bool> extractPuzzleAsync(string name, string path, string groupId, int? specifiedNumber = null)
			{
				var lines = (
					from line in await File.ReadAllLinesAsync(path)
					where !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _)
					select line
				).ToArray();
				if (lines.Length == 0)
				{
					goto PuzzleIsBroken;
				}

				if (InternalReadWrite.ReadLibraryConfiguration(groupId) is not { } libs)
				{
					goto PuzzleIsBroken;
				}

				var lib = libs.PuzzleLibraries.FirstOrDefault(lib => lib.Name == name);
				if (lib is not { FinishedPuzzlesCount: var finishedCount })
				{
					goto PuzzleIsBroken;
				}

				var index = specifiedNumber switch { { } l => l, _ => finishedCount };
				if (specifiedNumber is null && index >= lines.Length)
				{
					goto PuzzleLibraryIsAllFinished;
				}

				if (specifiedNumber is { } specifiedIndex && (specifiedIndex < 0 || specifiedIndex >= lines.Length))
				{
					goto SpecifiedPuzzleLibraryIndexIsOutOfRange;
				}

				var grid = Grid.Parse(lines[index]);
				GridAutoFiller.Fill(ref grid);

				var picturePath = InternalReadWrite.GenerateCachedPicturePath(() => Creator.CreateDefaultSudokuPainter(grid, name, index))!;
				await messageReceiver.SendMessageAsync(new ImageMessage { Path = picturePath });

				File.Delete(picturePath);

				await messageReceiver.SendMessageAsync($"题目代码：{grid}");

				if (specifiedNumber is null)
				{
					lib.FinishedPuzzlesCount++;

					InternalReadWrite.WriteLibraryConfiguration(libs);
				}

				return true;

			PuzzleIsBroken:
				await messageReceiver.SendMessageAsync("题库解析失败，可能存在损坏。请联系管理员、群主和机器人开发人员。");
				return true;

			PuzzleLibraryIsAllFinished:
				await messageReceiver.SendMessageAsync(
					"""
					恭喜，该题库的题目已经全部完成！
					如果你还想重新做这个题库的题，请联系管理员重新初始化题库配置文件后方可。
					"""
				);
				return true;

			SpecifiedPuzzleLibraryIndexIsOutOfRange:
				await messageReceiver.SendMessageAsync("当前指定的题库的抽题索引不合适：它不能小于或等于 0，也不能超过该题库的题目数量。");
				return true;
			}
		}
	}
}

#endif