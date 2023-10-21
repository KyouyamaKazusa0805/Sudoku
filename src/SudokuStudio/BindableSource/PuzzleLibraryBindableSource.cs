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
/// <param name="isAddingOperationPlaceholder">A <see cref="bool"/> value indicating whether the value is for a placeholder.</param>
public sealed partial class PuzzleLibraryBindableSource([DataMember] bool isAddingOperationPlaceholder)
{
	/// <summary>
	/// Initializes a <see cref="PuzzleLibraryBindableSource"/> instance.
	/// </summary>
	public PuzzleLibraryBindableSource() : this(false)
	{
	}

	/// <summary>
	/// Initializes a <see cref="PuzzleLibraryBindableSource"/> instance via another instance. <see cref="Puzzles"/> won't copy.
	/// </summary>
	/// <param name="other">Another instance.</param>
	/// <param name="puzzles">The puzzles.</param>
	internal PuzzleLibraryBindableSource(PuzzleLibraryBindableSource other, Grid[] puzzles) : this(false)
	{
		PuzzlesCount = other.PuzzlesCount;
		Name = other.Name;
		Description = other.Description;
		Author = other.Author;
		FileId = other.FileId!;
		Tags = (string[])Tags.Clone();
		Puzzles = puzzles;
		PuzzlesCount = puzzles.Length;
	}


	/// <summary>
	/// Indicates the number of puzzles.
	/// </summary>
	public Count PuzzlesCount { get; set; }

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
	public string Author { get; set; } = GetString("LibraryPage_UndefinedUserName");

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
	/// <remarks>
	/// This property always returns a list that contain at least one element. If the directory doesn't contain any valid puzzle libraries,
	/// this property will return a collection with one <see cref="PuzzleLibraryBindableSource"/> element indicating the adding operation
	/// as a placeholder; otherwise, a list of valid <see cref="PuzzleLibraryBindableSource"/> elements
	/// and a placeholder mentioned in the previous case.
	/// </remarks>
	public static ObservableCollection<PuzzleLibraryBindableSource> LocalPuzzleLibraries(bool autoAdding)
	{
		var di = new DirectoryInfo(CommonPaths.PuzzleLibrariesFolder);
		if (!di.Exists || di.GetFiles() is not (var files and not []))
		{
			di.Create(); // Implicit behavior: if the puzzle library does not exist, create a new directory.
			return autoAdding ? [new(true)] : [];
		}

		var result = new List<PuzzleLibraryBindableSource>();
		foreach (var file in files)
		{
			if (Path.GetExtension(file.FullName) != FileExtensions.PuzzleLibrary)
			{
				// Filters invalid file extensions.
				continue;
			}

			if (tryDeserialize(file.FullName, out var instance))
			{
				instance.PuzzlesCount = instance.Puzzles.Length;
				instance.FileId = Path.GetFileNameWithoutExtension(file.FullName);
				result.Add(instance);
			}
		}

		return autoAdding ? [.. result, new(true)] : [.. result];


		static bool tryDeserialize(string fileName, [NotNullWhen(true)] out PuzzleLibraryBindableSource? result)
		{
			try
			{
				var json = File.ReadAllText(fileName);
				result = JsonSerializer.Deserialize<PuzzleLibraryBindableSource>(json) ?? throw new JsonException();
				return true;
			}
			catch (JsonException)
			{
				result = null;
				return false;
			}
		}
	}
}
