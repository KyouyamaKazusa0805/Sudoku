using System.Reflection;
using Sudoku.Collections;
using Sudoku.Presentation;
using Sudoku.Resources;
using Sudoku.Solving.Manual.Searchers;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with a manual solving step that is a technique usage, and contains the conclusions.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
public abstract record Step(ImmutableArray<Conclusion> Conclusions, ImmutableArray<PresentationData> Views) : IStep
{
	/// <inheritdoc/>
	public virtual bool ShowDifficulty => true;

	/// <inheritdoc/>
	public virtual bool IsElementary => false;

	/// <summary>
	/// Indicates whether the corresponding technique of the current step is an Almost Locked Sets
	/// (ALS in abbreviation).
	/// </summary>
	public bool IsAlmostLockedSets => HasTag(TechniqueTags.Als);

	/// <summary>
	/// Indicates whether the corresponding technique of the current step is a chain. The chain techiques are:
	/// <list type="bullet">
	/// <item>
	/// Wings
	/// <list type="bullet">
	/// <item>
	/// <see cref="IRegularWingStepSearcher">Regular wings</see> (XY-Wing, XYZ-Wing, WXYZ-Wing, etc.)
	/// </item>
	/// <item>
	/// <see cref="IIregularWingStepSearcher">Irregular wings</see>
	/// (W-Wing, M-Wing, Split-Wing, Local-Wing, Hybrid-Wing)
	/// </item>
	/// </list>
	/// </item>
	/// <item>
	/// Short chains
	/// <list type="bullet">
	/// <item>
	/// <see cref="ITwoStrongLinksStepSearcher">Two strong links</see>
	/// (Skyscraper, Two-string kite, Turbot fish)
	/// </item>
	/// <item>
	/// ALS chaining-like techniques
	/// <list type="bullet">
	/// <item><see cref="IAlmostLockedSetsXzStepSearcher">ALS-XZ</see></item>
	/// <item><see cref="IAlmostLockedSetsXyWingStepSearcher">ALS-XY-Wing</see></item>
	/// <item><see cref="IAlmostLockedSetsWWingStepSearcher">ALS-W-Wing</see></item>
	/// </list>
	/// </item>
	/// <item><see cref="IEmptyRectangleStepSearcher">Empty rectangle</see></item>
	/// </list>
	/// </item>
	/// <item>
	/// Long chains
	/// <list type="bullet">
	/// <item><see cref="IForcingChainStepSearcher">Forcing chains</see></item>
	/// <item><see cref="IDynamicForcingChainStepSearcher">Dynamic forcing chains</see></item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool IsChaining =>
		HasTag(TechniqueTags.Wings | TechniqueTags.ShortChaining | TechniqueTags.LongChaining);

	/// <summary>
	/// Indicates whether the corresponding technique of the current step is a deadly pattern.
	/// The deadly pattern techniques are:
	/// <list type="bullet">
	/// <item>
	/// Bi-value patterns
	/// <list type="bullet">
	/// <item><see cref="IUniqueRectangleStepSearcher">Unique rectangle</see> (i.e. Uniqueness test)</item>
	/// <item><see cref="IUniqueLoopStepSearcher">Unique loop</see></item>
	/// <item><see cref="IBivalueUniversalGraveStepSearcher">Bi-value universal grave</see></item>
	/// </list>
	/// </item>
	/// <item>
	/// Multi-value patterns
	/// <list type="bullet">
	/// <item><see cref="IExtendedRectangleStepSearcher">Extended rectangle</see></item>
	/// <item><see cref="IUniqueSquareStepSearcher">Unique square</see></item>
	/// <item>
	/// <see cref="IUniquePolygonStepSearcher">Unique polygon</see> (Borescoper's deadly pattern as its alias)
	/// </item>
	/// <item><see cref="IQiuDeadlyPatternStepSearcher">Qiu's deadly pattern</see></item>
	/// </list>
	/// </item>
	/// <!--
	/// <item>
	/// Other deadly patterns
	/// <list type="bullet">
	/// <item><see cref="#">Reverse bi-value universal grave</see></item>
	/// </list>
	/// </item>
	/// -->
	/// </list>
	/// </summary>
	public bool IsDeadlyPattern => HasTag(TechniqueTags.DeadlyPattern);

	/// <inheritdoc/>
	public virtual unsafe string Name
	{
		get
		{
			// The resource document stores the values with the camel-casing key.
			// Therefore, we should convert the code into the camel-casing one
			// to match the key in the resource document.
			string techniqueCodeStr = TechniqueCode.ToString();
			fixed (char* p = techniqueCodeStr)
			{
				if (*p is >= 'A' and <= 'Z')
				{
					*p += ' ';
				}
			}

			return ResourceManager.Shared[techniqueCodeStr];
		}
	}

	/// <inheritdoc/>
	public virtual string? Format
	{
		get
		{
			try
			{
				var type = GetType();
				return ResourceManager.Shared[
					type.GetCustomAttribute<FormatForwardAttribute>() is { IdentifierName: var name }
						? $"TechniqueFormat_{name}"
						: $"TechniqueFormat_{type.Name}"
				];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}
	}

	/// <inheritdoc/>
	public abstract decimal Difficulty { get; }

	/// <inheritdoc/>
	public abstract Technique TechniqueCode { get; }

	/// <inheritdoc/>
	public abstract TechniqueTags TechniqueTags { get; }

	/// <inheritdoc/>
	public virtual TechniqueGroup TechniqueGroup =>
		Enum.TryParse<TechniqueGroup>(TechniqueCode.ToString(), out var inst) ? inst : TechniqueGroup.None;

	/// <inheritdoc/>
	public abstract DifficultyLevel DifficultyLevel { get; }

	/// <inheritdoc/>
	public virtual Stableness Stableness { get; } = Stableness.Stable;

	/// <inheritdoc/>
	public abstract Rarity Rarity { get; }

	/// <inheritdoc cref="IStep.ElimStr"/>
	[FormatItem]
	internal string ElimStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new ConclusionCollection(Conclusions).ToString();
	}

	/// <inheritdoc/>
	string IStep.ElimStr => ElimStr;


	/// <inheritdoc/>
	public void ApplyTo(ref Grid grid)
	{
		foreach (var conclusion in Conclusions)
		{
			conclusion.ApplyTo(ref grid);
		}
	}

	/// <summary>
	/// Determine whether the current step information instance with the specified flags.
	/// </summary>
	/// <param name="flags">
	/// The flags. If the argument contains more than one set bit, all flags will be checked
	/// one by one.
	/// </param>
	/// <returns>A <see cref="bool"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool HasTag(TechniqueTags flags) =>
		flags.IsFlag() ? TechniqueTags.Flags(flags) : TechniqueTags.MultiFlags(flags);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => Formatize();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ElimStr}";

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual string ToFullString() => ToString();

	/// <inheritdoc/>
	public string Formatize(bool handleEscaping = false)
	{
		// Check whether the format property is not null.
		if (Format is not { } format)
		{
			throw new InvalidOperationException("The format can't be null.");
		}

		// Get the interpolation values, and extract them into a new collection to store the format values.
		int length = format.Length;
		var sb = new StringHandler(length);
		var formats = new List<string>(10);
		int formatCount = 0;
		for (int i = 0, iterationLength = length - 1; i < iterationLength; i++)
		{
			switch ((Left: format[i], Right: format[i + 1]))
			{
				case (Left: '{', Right: '}'):
				{
					throw new InvalidOperationException("The interpolation part cannot contain empty value.");
				}
				case (Left: '{', Right: '{'):
				{
					sb.Append("{{");
					i++;

					break;
				}
				case (Left: '}', Right: '}'):
				{
					sb.Append("}}");
					i++;

					break;
				}
				case (Left: '{', Right: not '{'):
				{
					int pos = -1;
					for (int j = i + 1; j < length; j++)
					{
						if (format[j] == '}')
						{
							pos = j;
							break;
						}
					}
					if (pos == -1)
					{
						throw new InvalidOperationException("Missing the closed brace character '}'.");
					}

					sb.Append('{');
					sb.Append(formatCount++);
					sb.Append('}');

					formats.Add(format[(i + 1)..pos]);

					i = pos;

					break;
				}
				case (Left: '\\', var right) when handleEscaping: // Unescape the escaping characters.
				{
					sb.Append(right);
					i++;

					break;
				}
				case (var left, _):
				{
					sb.Append(left);

					break;
				}
			}
		}

		// Use reflection to invoke each properties, and get the interpolation result.
		var type = GetType();
		const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
		string[] matchedFormats = (
			from f in formats
			select type.GetProperty(f, flags) into property
			where property?.IsDefined(typeof(FormatItemAttribute)) ?? false
			let propertyGetMethod = property.GetMethod
			where propertyGetMethod is not null && isPrivateOrProtected(propertyGetMethod)
			select property.GetValue(propertyGetMethod.IsStatic ? null : this) as string into result
			where result is not null
			select result
		).Prepend(Name).ToArray();

		// Check the length validity.
		if (formatCount != matchedFormats.Length)
		{
			throw new InvalidOperationException("The number of interpolations failed to match.");
		}

		// Format and return the value.
		return string.Format(sb.ToStringAndClear(), matchedFormats);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool isPrivateOrProtected(MethodInfo propertyGetMethod) =>
			propertyGetMethod is { IsPrivate: true } or { IsFamily: true } or { IsPrivate: true, IsFamily: true };
	}
}
