namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Encapsulates all information after searched a solving step,
	/// which include the conclusion, the difficulty and so on.
	/// </summary>
	/// <param name="Conclusions">All conclusions.</param>
	/// <param name="Views">All views.</param>
	[AutoDeconstruct(nameof(Name), nameof(Difficulty), nameof(DifficultyLevel))]
	[AutoDeconstruct(nameof(Name), nameof(Difficulty), nameof(DifficultyLevel), nameof(Conclusions))]
	[AutoDeconstruct(nameof(Name), nameof(Difficulty), nameof(DifficultyLevel), nameof(Conclusions), nameof(Views))]
	public abstract partial record StepInfo(IReadOnlyList<Conclusion> Conclusions, IReadOnlyList<View> Views)
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
		/// </summary>
		public virtual bool ShowDifficulty => true;

		/// <summary>
		/// Indicates the technique name.
		/// </summary>
		public virtual string Name => TextResources.Current[TechniqueCode.ToString()];

		/// <summary>
		/// Indicates the acronym of the step name. For example, the acronym of the technique
		/// "Almost Locked Candidates" is ALC.
		/// </summary>
		/// <remarks>
		/// This property only contains the result in English. Other languages doesn't contain any
		/// abbreviations by default. On the other hand, this is really an easier way to implement
		/// than storing values in resource dictionary files.
		/// </remarks>
		public virtual string? Acronym => null;

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
		/// Indicates the technique name alias.
		/// </summary>
		public string[]? NameAlias
		{
			get
			{
				try
				{
					return TextResources.Current[$"{TechniqueCode.ToString()}Alias"]?.Split(new[] { ';', ' ' });
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
		public void ApplyTo(ref SudokuGrid grid)
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
		public bool HasTag(TechniqueTags flags) =>
			flags.IsFlag() ? TechniqueTags.Flags(flags) : TechniqueTags.MultiFlags(flags);

		/// <summary>
		/// Returns a string that only contains the name and the basic information.
		/// </summary>
		/// <returns>The string instance.</returns>
		/// <remarks>
		/// From version 0.7, this method will use <see langword="sealed record"/> <c>ToString</c>
		/// method to prevent the compiler overriding the method; the default behavior is changed to
		/// output as the method <see cref="Formatize(bool)"/> invoking.
		/// </remarks>
		/// <seealso cref="Formatize(bool)"/>
		public
#if false
		sealed
#endif
		override string ToString() => Formatize();

		/// <summary>
		/// Returns a string that only contains the name and the conclusions.
		/// </summary>
		/// <returns>The string instance.</returns>
		public string ToSimpleString() => $"{Name} => {ElimStr}";

		/// <summary>
		/// Returns a string that contains the name, the conclusions and its all details.
		/// This method is used for displaying details in text box control.
		/// </summary>
		/// <returns>The string instance.</returns>
		public virtual string ToFullString() => ToString();

		/// <summary>
		/// Formatizes the <see cref="Format"/> property string and output the result.
		/// </summary>
		/// <param name="handleEscaping">Indicates whether the method will handle the escaping characters.</param>
		/// <returns>The result string.</returns>
		/// <exception cref="ArgumentException">
		/// Throws when the format is invalid. The possible cases are:
		/// <list type="bullet">
		/// <item>The format is null.</item>
		/// <item>The interpolation part contains the empty value.</item>
		/// <item>Missing the closed brace character <c>'}'</c>.</item>
		/// <item>The number of interpolations failed to match.</item>
		/// </list>
		/// </exception>
		/// <seealso cref="Format"/>
		[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.NonPublicProperties)]
		public partial string Formatize(bool handleEscaping)
		{
			// Check whether the format property is not null.
			if (Format is not { } format)
			{
				throw new ArgumentException("The format can't be null.");
			}

			// Get the interpolation values, and extract them into a new collection to store the format values.
			int length = format.Length;
			var sb = new StringBuilder(length);
			var formats = new List<string>();
			int formatCount = 0;
			for (int i = 0, iterationLength = length - 1; i < iterationLength; i++)
			{
				switch ((Left: format[i], Right: format[i + 1]))
				{
					case (Left: '{', Right: '}'):
					{
						throw new ArgumentException(
							"The format is invalid. The interpolation part cannot contain empty value."
						);
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
							throw new ArgumentException(
								"The format is invalid. Missing the closed brace character '}'."
							);
						}

						sb.Append('{').Append(formatCount++).Append('}');
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
				throw new ArgumentException("The format is invalid. The number of interpolations failed to match.");
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
}
