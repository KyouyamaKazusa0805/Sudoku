// Try to invoke the root command.
return (int)Parser.Default
	.ParseArguments<SolveGridOptions, CheckGridOptions, VisitOptions>(args)
	.MapResult(
		static (SolveGridOptions o) => solveGridHandler(o),
		static (CheckGridOptions o) => checkGridHandler(o),
		static (VisitOptions o) => visitHandler(o),
		parseFailedHandler
	);


static ErrorCode solveGridHandler(SolveGridOptions options)
{
	if (tryParseGrid(options.GridValue, out var grid) is var errorCode and not 0)
	{
		return errorCode;
	}

	if (grid.Solution is not { IsUndefined: false } solution)
	{
		return ErrorCode.ArgGridValueIsNotUnique;
	}

	Console.WriteLine(
		$"""
		Puzzle:
		{grid:#}
		
		Solution grid:
		{solution:!}
		"""
	);

	return ErrorCode.None;
}

static ErrorCode checkGridHandler(CheckGridOptions options)
{
	switch (options)
	{
		case { ChecksForValidity: true }:
		{
			if (tryParseGrid(options.GridValue, out var grid) is var errorCode and not 0)
			{
				return errorCode;
			}

			Console.WriteLine(
				$"""
				Puzzle:
				{grid:#}
			
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

static ErrorCode parseFailedHandler(IEnumerable<Error> _) => ErrorCode.ParseFailed;

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
