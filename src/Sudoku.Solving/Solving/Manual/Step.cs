#pragma warning disable CS1584, CS1658

namespace Sudoku.Solving.Manual;

/// <summary>
/// Provides with a possible step.
/// </summary>
/// <param name="Conclusions">Indicates the conclusions that the step can be eliminated or assigned to.</param>
/// <param name="Views">
/// Indicates the views of the step that may be displayed onto the screen using pictures.
/// </param>
public abstract record Step(in ImmutableArray<Conclusion> Conclusions, in ImmutableArray<PresentationData> Views)
{
	/// <summary>
	/// <para>
	/// Indicates whether the difficulty rating of this technique should be
	/// shown in the output screen. Some techniques such as <b>Gurth's symmetrical placement</b>
	/// doesn't need to show the difficulty (because the difficulty of this technique
	/// is unstable).
	/// </para>
	/// <para>
	/// If the value is <see langword="true"/>, the analysis result won't show the difficulty
	/// of this instance.
	/// </para>
	/// <para>The default value is <see langword="true"/>.</para>
	/// </summary>
	public virtual bool ShowDifficulty => true;

	/// <summary>
	/// <para>Indicates whether the step is an SSTS (i.e. Simple Sudoku Technique Set) step.</para>
	/// <para>
	/// Here we define that the basic commonly appearing techniques are SSTS techniques:
	/// <list type="bullet">
	/// <item>Full House, Last Digit, Hidden Single, Naked Single</item>
	/// <item>Pointing, Claiming</item>
	/// <item>Naked Pair, Naked Triple, Naked Quarduple</item>
	/// <item>Naked Pair (+), Naked Triple (+), Naked Quarduple (+)</item>
	/// <item>Hidden Pair, Hidden Triple, Hidden Quarduple</item>
	/// <item>Locked Pair, Locked Triple</item>
	/// </list>
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public virtual bool IsSstsStep => false;

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
	/// <item><see cref="#">Regular wings</see> (XY-Wing, XYZ-Wing, WXYZ-Wing, etc.)</item>
	/// <item>
	/// <see cref="#">Irregular wings</see>
	/// (W-Wing, M-Wing, Split-Wing, Local-Wing, Hybrid-Wing)
	/// </item>
	/// </list>
	/// </item>
	/// <item>
	/// Short chains
	/// <list type="bullet">
	/// <item>
	/// <see cref="#">Two strong links</see>
	/// (Skyscraper, Two-string kite, Turbot fish)
	/// </item>
	/// <item>
	/// ALS chaining-like techniques
	/// (<see cref="#">ALS-XZ</see>,
	/// <see cref="#">ALS-XY-Wing</see>,
	/// <see cref="#">ALS-W-Wing</see>)
	/// </item>
	/// <item><see cref="#">Empty rectangle</see></item>
	/// </list>
	/// </item>
	/// <item>
	/// Long chains
	/// <list type="bullet">
	/// <item><see cref="#">Forcing chains</see></item>
	/// <item><see cref="#">Dynamic forcing chains</see></item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool IsChaining => HasTag(TechniqueTags.Wings | TechniqueTags.ShortChaining | TechniqueTags.LongChaining);

	/// <summary>
	/// Indicates whether the corresponding technique of the current step is a deadly pattern.
	/// The deadly pattern techniques are:
	/// <list type="bullet">
	/// <item>
	/// Bi-value patterns
	/// <list type="bullet">
	/// <item><see cref="#">Unique rectangle</see> (i.e. Uniqueness test)</item>
	/// <item><see cref="#">Unique loop</see></item>
	/// <item><see cref="#">Bi-value universal grave</see></item>
	/// </list>
	/// </item>
	/// <item>
	/// Multi-value patterns
	/// <list type="bullet">
	/// <item><see cref="#">Extended rectangle</see></item>
	/// <item><see cref="#">Unique square</see></item>
	/// <item><see cref="#">Borescoper's deadly pattern</see></item>
	/// <item><see cref="#">Qiu's deadly pattern</see></item>
	/// </list>
	/// </item>
	/// <item>
	/// Other deadly patterns
	/// <list type="bullet">
	/// <item><see cref="#">Reverse bi-value universal grave</see></item>
	/// </list>
	/// </item>
	/// </list>
	/// </summary>
	public bool IsDeadlyPattern => HasTag(TechniqueTags.DeadlyPattern);

	/// <summary>
	/// Indicates the technique name. The default value is in the resource dictionary.
	/// </summary>
	public virtual string Name => TextResources.Current[TechniqueCode.ToString()];

	/// <summary>
	/// Gets the format of the current instance.
	/// </summary>
	/// <returns>
	/// Returns a <see cref="string"/> result. If the resource dictionary doesn't contain
	/// any valid formats here, the result value will be <see langword="null"/>.
	/// </returns>
	/// <remarks>
	/// <para>
	/// A <b>format</b> is the better way to format the result text of this technique information instance,
	/// It'll be represented by the normal characters and the placeholders, e.g.
	/// <code>
	/// <![CDATA["{Name}: Cells {CellsStr} => {ElimsStr}"]]>
	/// </code>
	/// Here the string result <b>shouldn't</b> be with the leading <c>'$'</c> character, because this is a
	/// format string, rather than a interpolated string.
	/// </para>
	/// <para>
	/// Here the property <c>Name</c>, <c>CellsStr</c> and <c>ElimsStr</c> should be implemented before
	/// the property invoked, you should creates those 3 properties, returns the corresponding correct string
	/// result, makes them <see langword="private"/> or <see langword="protected"/> and marks the attribute
	/// <see cref="FormatItemAttribute"/> to help the code analyzer (if the code analyzer is available).
	/// The recommended implementation pattern is:
	/// <code><![CDATA[
	/// [FormatItem]
	/// private string CellsStr
	/// {
	///     [MethodImpl(MethodImplOptions.AggressiveInlining)]
	///	    get => Cells.ToString();
	/// }
	/// ]]></code>
	/// You can use the code snippet <c>fitem</c> to create the pattern, whose corresponding file is added
	/// into the <c>required/vssnippets</c> folder. For more information, please open the markdown file
	/// <see href="#">README.md</see> in the <c>required</c> folder to learn more information.
	/// </para>
	/// <para>
	/// Because this property will get the value from the resource dictionary, the property supports
	/// multiple language switching, which is better than the normal methods <see cref="ToString"/>
	/// and <see cref="ToFullString"/>. Therefore, this property is the substitution plan of those two methods.
	/// </para>
	/// </remarks>
	/// <seealso cref="ToString"/>
	/// <seealso cref="ToFullString"/>
	public virtual string? Format
	{
		get
		{
			try
			{
				return TextResources.Current[$"Format_{GetType().Name}"];
			}
			catch (RuntimeBinderException)
			{
				return null;
			}
		}
	}

	/// <summary>
	/// The difficulty or this step.
	/// </summary>
	public abstract decimal Difficulty { get; }

	/// <summary>
	/// The technique code of this instance used for comparison
	/// (e.g. search for specified puzzle that contains this technique).
	/// </summary>
	public abstract Technique TechniqueCode { get; }

	/// <summary>
	/// The technique tags of this instance.
	/// </summary>
	public abstract TechniqueTags TechniqueTags { get; }

	/// <summary>
	/// The technique group that this technique instance belongs to.
	/// </summary>
	public virtual TechniqueGroup TechniqueGroup =>
		Enum.TryParse<TechniqueGroup>(TechniqueCode.ToString(), out var inst) ? inst : TechniqueGroup.None;

	/// <summary>
	/// The difficulty level of this step.
	/// </summary>
	public abstract DifficultyLevel DifficultyLevel { get; }

	/// <summary>
	/// Indicates the stableness of this technique. The default value is <see cref="Stableness.Stable"/>.
	/// </summary>
	/// <seealso cref="Stableness.Stable"/>
	public virtual Stableness Stableness { get; } = Stableness.Stable;

	/// <summary>
	/// Indicates the rarity of this technique appears.
	/// </summary>
	public abstract Rarity Rarity { get; }

	/// <summary>
	/// Indicates the string representation of the conclusions.
	/// </summary>
	/// <remarks>
	/// Most of techniques uses eliminations
	/// so this property is named <c>ElimStr</c>. In other words, if the conclusion is an assignment one,
	/// the property will still use this name rather than <c>AssignmentStr</c>.
	/// </remarks>
	[FormatItem]
	protected string ElimStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new ConclusionCollection(Conclusions).ToString();
	}

	
	/// <summary>
	/// Put this instance into the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
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

	/// <summary>
	/// Returns a string that only contains the name and the basic information.
	/// </summary>
	/// <returns>The string instance.</returns>
	/// <remarks>
	/// This method uses <see langword="sealed"/> and <see langword="override"/> modifiers
	/// to prevent the compiler overriding the method; in additional, the default behavior is changed to
	/// output as the method <see cref="Formatize(bool)"/> invoking.
	/// </remarks>
	/// <seealso cref="Formatize(bool)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => Formatize();

	/// <summary>
	/// Returns a string that only contains the name and the conclusions.
	/// </summary>
	/// <returns>The string instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToSimpleString() => $"{Name} => {ElimStr}";

	/// <summary>
	/// Returns a string that contains the name, the conclusions and its all details.
	/// This method is used for displaying details in text box control.
	/// </summary>
	/// <returns>The string instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual string ToFullString() => ToString();

	/// <summary>
	/// Formatizes the <see cref="Format"/> property string and output the result.
	/// </summary>
	/// <param name="handleEscaping">Indicates whether the method will handle the escaping characters.</param>
	/// <returns>The result string.</returns>
	/// <exception cref="ArgumentException">
	/// Throws when the format is invalid. The possible cases are:
	/// <list type="bullet">
	/// <item>The format is <see langword="null"/>.</item>
	/// <item>The interpolation part contains the empty value.</item>
	/// <item>Missing the closed brace character <c>'}'</c>.</item>
	/// <item>The number of interpolations failed to match.</item>
	/// </list>
	/// </exception>
	/// <seealso cref="Format"/>
	[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicProperties)]
	public string Formatize(bool handleEscaping = false)
	{
		// Check whether the format property is not null.
		if (Format is not { } format)
		{
			throw new ArgumentException("The format can't be null.");
		}

		// Get the interpolation values, and extract them into a new collection to store the format values.
		int length = format.Length;
		var sb = new ValueStringBuilder(stackalloc char[length]);
		var formats = new List<string>(10);
		int formatCount = 0;
		for (int i = 0, iterationLength = length - 1; i < iterationLength; i++)
		{
			switch ((Left: format[i], Right: format[i + 1]))
			{
				case (Left: '{', Right: '}'):
				{
					throw new ArgumentException("The interpolation part cannot contain empty value.");
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
						throw new ArgumentException("Missing the closed brace character '}'.");
					}

					sb.Append('{');
					sb.Append(formatCount++);
					sb.Append('}');
					formats.Add(format[(i + 1)..pos]);

					i = pos;

					break;
				}
				case (Left: '\\', Right: var right) when handleEscaping: // De-escape the escaping characters.
				{
					sb.Append(right);
					i++;

					break;
				}
				case (Left: var left, _):
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
			where property?.IsDefined<FormatItemAttribute>() ?? false
			let propertyGetMethod = property.GetMethod
			where propertyGetMethod is not null && isPrivateOrProtected(propertyGetMethod)
			select property.GetValue(propertyGetMethod.IsStatic ? null : this) as string into result
			where result is not null
			select result
		).Prepend(Name).ToArray();

		// Check the length validity.
		if (formatCount != matchedFormats.Length)
		{
			throw new ArgumentException("The number of interpolations failed to match.");
		}

		// Format and return the value.
		return string.Format(sb.ToString(), matchedFormats);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool isPrivateOrProtected(MethodInfo propertyGetMethod) =>
			propertyGetMethod.IsPrivate // private
			|| propertyGetMethod.IsFamily // protected
			|| propertyGetMethod.IsPrivate && propertyGetMethod.IsFamily; // private protected
	}
}
