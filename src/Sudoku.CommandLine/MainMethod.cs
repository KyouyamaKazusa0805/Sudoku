var resultCode = Parser.Default
	.ParseArguments<FormatOptions, SolveGridOptions, CheckGridOptions, GenerateGridOptions, VisitOptions>(args)
	.MapResult<FormatOptions, SolveGridOptions, CheckGridOptions, GenerateGridOptions, VisitOptions, ErrorCode>(
		formatHandler, solveGridHandler, checkGridHandler, generateHandler, visitHandler,
		static _ => ErrorCode.ParseFailed
	);
return resultCode == ErrorCode.None ? 0 : -(int)resultCode;


static ErrorCode formatHandler(FormatOptions options)
{
	string rawGridValue = options.GridValue;
	if (tryParseGrid(rawGridValue, out var grid) is var errorCode and not 0)
	{
		return errorCode;
	}

	string format = options.FormatString;
	try
	{
		ConsoleExtensions.WriteLine(
			$"""
			Grid: '{rawGridValue}'
			Format: '{c(format)}'
			Result: {grid.ToString(format)}
			"""
		);

		return ErrorCode.None;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string c(string f) => f is null ? "<null>" : string.IsNullOrWhiteSpace(f) ? "<empty>" : f;
	}
	catch (FormatException)
	{
		return ErrorCode.ArgFormatIsInvalid;
	}
}

static ErrorCode solveGridHandler(SolveGridOptions options)
{
	if (tryParseGrid(options.GridValue, out var grid) is var errorCode and not ErrorCode.None)
	{
		return errorCode;
	}

	if (grid.Solution is not { IsUndefined: false } solution)
	{
		return ErrorCode.ArgGridValueIsNotUnique;
	}

	string? methodNameUsed = null;
	foreach (var type in
		from type in typeof(ISimpleSolver).Assembly.GetTypes()
		where type.IsClass && type.IsAssignableTo(typeof(ISimpleSolver)) && type.GetConstructor(Array.Empty<Type>()) is not null
		select type)
	{
		string methodName = options.Method;
		string name = (string)type.GetProperty(nameof(ISimpleSolver.Name))!.GetValue(null)!;
		string shortcutStr = ((char)type.GetProperty(nameof(ISimpleSolver.Shortcut))!.GetValue(null)!).ToString();
		string? uriLink = (string?)type.GetProperty(nameof(ISimpleSolver.UriLink))!.GetValue(null);
		if (methodName != name && methodName != shortcutStr)
		{
			continue;
		}

		switch (Activator.CreateInstance(type))
		{
			case ISimpleSolver simpleSolver:
			{
				if (simpleSolver.Solve(grid, out _) is not true)
				{
					return ErrorCode.ArgGridValueIsNotUnique;
				}

				// .NET Runtime issue: If the type does not implement 'IFormattable',
				// the format string is meaningless to be used in the interpolated string holes.
				// In this invocation, type 'Grid' does not implement the type 'IFormattable',
				// therefore, we cannot use the interpolated string syntax like '$"{grid:#}"'
				// to get the same result as 'grid.ToString("#")'; on contrast, 'grid.ToString("#")'
				// as expected will be replaced with 'grid.ToString()'.
				// Same reason for the below output case.
				ConsoleExtensions.WriteLine(
					$"""
					Puzzle: {grid:#}
					Method name used: '{name}'{(
						uriLink is null
							? string.Empty
							: $"""
							
							URI link: '{uriLink}'
							"""
					)}
					---
					Solution: {solution:!}
					"""
				);

				break;
			}
			case IComplexSolver<ManualSolverResult> puzzleSolver:
			{
				if (puzzleSolver.Solve(grid) is not { IsSolved: true } solverResult)
				{
					return ErrorCode.ArgGridValueIsNotUnique;
				}

				ConsoleExtensions.WriteLine(
					$"""
					Puzzle: {grid:#}
					Method name used: '{methodNameUsed}'
					---
					Solution: {solution:!}
					Solving details:
					{solverResult}
					"""
				);

				break;
			}
		}
	}

	return methodNameUsed is null ? ErrorCode.ArgMethodIsInvalid : ErrorCode.None;
}

static ErrorCode checkGridHandler(CheckGridOptions options)
{
	switch (options)
	{
		case { ChecksForValidity: true }:
		{
			if (tryParseGrid(options.GridValue, out var grid) is var errorCode and not ErrorCode.None)
			{
				return errorCode;
			}

			ConsoleExtensions.WriteLine(
				$"""
				Puzzle: '{grid:#}'
				The puzzle {(grid.IsValid ? "has" : "doesn't have")} a unique solution.
				"""
			);

			return ErrorCode.None;
		}
		default:
		{
			return ErrorCode.ArgAttributeNameIsInvalid;
		}
	}
}

static ErrorCode generateHandler(GenerateGridOptions options)
{
	string rangePattern = options.Range;
	if (tryParseRange(rangePattern, out int min, out int max) is var errorCode and not ErrorCode.None)
	{
		return errorCode;
	}

	var generator = new HardPatternPuzzleGenerator();
	while (true)
	{
		var targetPuzzle = generator.Generate();
		int c = targetPuzzle.GivensCount;
		if (c < min || c >= max)
		{
			continue;
		}

		ConsoleExtensions.WriteLine($"""The puzzle generated: '{targetPuzzle:0}'""");

		return ErrorCode.None;
	}
}

static ErrorCode visitHandler(VisitOptions options)
{
	foreach (var propertyInfo in typeof(VisitOptions).GetProperties())
	{
		if (propertyInfo.PropertyType != typeof(bool))
		{
			// Skip the properties whose types are not boolean.
			continue;
		}

		if (propertyInfo is not { CanWrite: true, CanRead: true })
		{
			// Skip the properties which are not both writable and readable.
			continue;
		}

		if (!(bool)propertyInfo.GetValue(options)!)
		{
			// Skip the properties whose value are not true.
			continue;
		}

		string? uriSite = propertyInfo.GetCustomAttribute<DescriptionAttribute>()?.Description;
		if (!Uri.TryCreate(uriSite, UriKind.Absolute, out var targetUri))
		{
			// Skip the properties whose corresponding sites are not found.
			continue;
		}

		try
		{
#if VISIT_SITE_DIRECTLY
			// Directly visit the site.
			Process.Start(
#if NET5_0_OR_GREATER
				new ProcessStartInfo(targetUri.AbsoluteUri) { UseShellExecute = true }
#else
				new ProcessStartInfo(targetUri.AbsoluteUri)
#endif
			);
#else
			// Output the site link.
			Console.WriteLine(
				$"""
				Please visit the following site to learn more information.
				{uriSite}
				"""
			);
#endif

			// Just return, because only one value will be set to true in the target option set.
			return ErrorCode.None;
		}
		catch
		{
			return ErrorCode.SiteIsFailedToVisit;
		}
	}

	throw new("Unexpected error. The program cannot be reached here.");
}

static ErrorCode tryParseGrid(string? gridValue, out Grid result)
{
	Unsafe.SkipInit(out result);
	if (gridValue is null)
	{
		return ErrorCode.ArgGridValueIsNull;
	}

	if (!Grid.TryParse(gridValue, out var grid))
	{
		return ErrorCode.ArgGridValueIsInvalidWhileParsing;
	}

	result = grid;
	return ErrorCode.None;
}

static ErrorCode tryParseRange(string rangeValue, out int min, out int max)
{
	Unsafe.SkipInit(out min);
	Unsafe.SkipInit(out max);
	if (!CellRange.TryParse(rangeValue, out var cellRange))
	{
		return ErrorCode.RangePatternIsInvalid;
	}

	(min, max) = cellRange;
	return ErrorCode.None;
}
