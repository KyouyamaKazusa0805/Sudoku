using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.SourceGeneration;
using System.Text.Json;
using System.Text.Json.Serialization;
using SudokuStudio.Storage;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.BindableSource;

/// <summary>
/// Describes for a puzzle library.
/// </summary>
public sealed partial class PuzzleLibraryBindableSource
{
	/// <summary>
	/// Indicates the number of puzzles.
	/// </summary>
	public int PuzzlesCount { get; set; }

	/// <summary>
	/// Indicates the name of the library.
	/// </summary>
	public string Name { get; set; } = GetString("LibraryPage_UndefinedName");

	/// <summary>
	/// Indicates the description of the library.
	/// </summary>
	public string Description { get; set; } = GetString("LibraryPage_UndefinedDescription");

	/// <summary>
	/// Indicates the name of the user.
	/// </summary>
	public string UserName { get; set; } = GetString("LibraryPage_UndefinedUserName");

	/// <summary>
	/// Idnicates the file ID used as storage file name. <b>Note: The ID should be unique in all puzzle libraries.</b>
	/// </summary>
	[ImplicitField]
	[DisallowNull]
	public string? FileId
	{
		get => _fileId;

		set => _fileId = value.IndexOfAny(Path.GetInvalidFileNameChars()) == -1
			? value
			: throw new ArgumentException("The file ID cannot contain invalid characters cannot be used in a file name.", nameof(value));
	}

	/// <summary>
	/// Indicates the path of the library storage file.
	/// </summary>
	[JsonIgnore]
	public string FilePath => $@"{CommonPaths.PuzzleLibrariesFolder}\{FileId}{FileExtensions.PuzzleLibrary}";

	/// <summary>
	/// Indicates the tags of the library.
	/// </summary>
	public string[] Tags { get; set; } = [];

	/// <summary>
	/// The puzzles loaded.
	/// </summary>
	public Grid[] Puzzles { get; set; } = [];


	/// <summary>
	/// Try to fetch all puzzle libraries stored in the local path.
	/// </summary>
	/// <returns>
	/// All puzzle libraies, having been converted into <see cref="PuzzleLibraryBindableSource"/> instances to be used and replaced.
	/// </returns>
	public static ObservableCollection<PuzzleLibraryBindableSource> LocalPuzzleLibraries
	{
		get
		{
			var result = new List<PuzzleLibraryBindableSource>();
			foreach (var file in new DirectoryInfo(CommonPaths.PuzzleLibrariesFolder).GetFiles())
			{
				PuzzleLibraryBindableSource instance;
				try
				{
					var json = File.ReadAllText(file.FullName);
					instance = JsonSerializer.Deserialize<PuzzleLibraryBindableSource>(json) ?? throw new JsonException();
				}
				catch (JsonException)
				{
					continue;
				}

#if false
				var newGrids = new List<Grid>(instance.Puzzles.Length);
				foreach (ref readonly var grid in instance.Puzzles.EnumerateRef())
				{
					if (grid.IsValid)
					{
						newGrids.Add(grid);
					}
				}
				instance.Puzzles = [.. newGrids];
#endif

				instance.PuzzlesCount = instance.Puzzles.Length;
				instance.FileId = Path.GetFileNameWithoutExtension(file.FullName);
				result.Add(instance);
			}

			return [.. result];
		}
	}
}
