namespace Sudoku.MetaProgramming;

/// <summary>
/// Represents a document that is loaded from local path, describing a script.
/// </summary>
/// <param name="filePath">Indicates the document file path.</param>
public sealed class ScriptDocument(string filePath) : IAsyncDisposable
{
	/// <summary>
	/// Indicates whether the object has been already disposed.
	/// </summary>
	private bool _isDisposed;


	/// <summary>
	/// Indicates the diagnostic results if the loading operation is failed.
	/// </summary>
	public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

	/// <summary>
	/// Represents a <see cref="MemoryStream"/> object that describes the method can be run in.
	/// The value can be <see langword="null"/> if the method <see cref="LoadAsync"/> is not called.
	/// </summary>
	/// <seealso cref="LoadAsync"/>
	public MemoryStream? Stream { get; private set; }

	/// <summary>
	/// Indicates the loaded method information. This value can also be used by invocation.
	/// </summary>
	public MethodInfo? Method { get; private set; }

	/// <summary>
	/// Indicates the compilation instance.
	/// </summary>
	public Compilation? Compilation { get; private set; }


	/// <inheritdoc/>
	public async ValueTask DisposeAsync()
	{
		ObjectDisposedException.ThrowIf(_isDisposed, nameof(Stream));

		if (Stream is not null)
		{
			await Stream.DisposeAsync();
		}

		_isDisposed = true;
	}

	/// <summary>
	/// Loads a file from local path, and parse it into a valid method, making the property <see cref="Method"/>
	/// not <see langword="null"/>.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the loading operation.</param>
	/// <returns>
	/// A <see cref="Task"/> instance that holds the asynchronous operation; the result value is a <see cref="bool"/> value
	/// indicating whether the operation is succeeded.
	/// </returns>
	/// <seealso cref="Method"/>
	public async Task<bool> LoadAsync(CancellationToken cancellationToken = default)
	{
		if (!File.Exists(filePath))
		{
			return false;
		}

		var fileContent = await File.ReadAllTextAsync(filePath, cancellationToken);
		var syntaxTree = CSharpSyntaxTree.ParseText(
			$$"""
			#nullable enable

			{{Script.UsingDirectives}}

			public static unsafe class {{Script.DefaultTypeName}}
			{
				{{fileContent}}
			}
			""",
			cancellationToken: cancellationToken
		);
		var compilation = AnalyticsCompilation.CreateCompilation().AddSyntaxTrees(syntaxTree);
		var stream = new MemoryStream();
		var result = compilation.Emit(stream, cancellationToken: cancellationToken);
		if (!result.Success)
		{
			Diagnostics = result.Diagnostics;
			goto ReturnFalseAndReleaseStream;
		}

		var typeSymbol = compilation.GetTypeByMetadataName(Script.DefaultTypeName);
		if (typeSymbol is null)
		{
			goto ReturnFalseAndReleaseStream;
		}

		var methodsFound = typeSymbol.GetMembers().OfType<IMethodSymbol>().ToArray();
		if (methodsFound is not [{ Parameters.Length: 1, ReturnsVoid: false, Name: var methodName }])
		{
			goto ReturnFalseAndReleaseStream;
		}

		stream.Seek(0, SeekOrigin.Begin);
		var method = Assembly.Load(stream.ToArray()).GetType(Script.DefaultTypeName)!.GetMethod(methodName, Script.DefaultBindingFlags);
		if (method is null)
		{
			goto ReturnFalseAndReleaseStream;
		}

		(Compilation, Stream, Method) = (compilation, stream, method);
		return true;

	ReturnFalseAndReleaseStream:
		await stream.DisposeAsync();
		return false;
	}
}
