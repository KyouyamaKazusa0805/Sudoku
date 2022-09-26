#pragma warning disable format
[assembly: SearcherConfiguration<                        SingleStepSearcher>(SearcherDisplayingLevel.A)]
[assembly: SearcherConfiguration<              LockedCandidatesStepSearcher>(SearcherDisplayingLevel.A)]
[assembly: SearcherConfiguration<                        SubsetStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                    NormalFishStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                TwoStrongLinksStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                   RegularWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                         WWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<              MultiBranchWWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<               UniqueRectangleStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<        AlmostLockedCandidatesStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                      SueDeCoqStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<            SueDeCoq3DimensionStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                    UniqueLoopStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<             ExtendedRectangleStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                EmptyRectangleStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                  UniqueMatrixStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                 UniquePolygonStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<              QiuDeadlyPatternStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<         BivalueUniversalGraveStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<  ReverseBivalueUniversalGraveStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<           UniquenessClueCoverStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<               RwDeadlyPatternStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<EmptyRectangleIntersectionPairStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                      FireworkStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<     GurthSymmetricalPlacementStepSearcher>(SearcherDisplayingLevel.A)]
[assembly: SearcherConfiguration<                      GuardianStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<                   ComplexFishStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<                BivalueOddagonStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<              ChromaticPatternStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<            AlmostLockedSetsXzStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<        AlmostLockedSetsXyWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<         AlmostLockedSetsWWingStepSearcher>(SearcherDisplayingLevel.B)]
[assembly: SearcherConfiguration<                  DeathBlossomStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<     AlternatingInferenceChainStepSearcher>(SearcherDisplayingLevel.C)]
[assembly: SearcherConfiguration<                   BowmanBingoStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.                LastResort)]
[assembly: SearcherConfiguration<                PatternOverlayStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.Gathering, DisabledReason = SearcherDisabledReason.                LastResort)]
[assembly: SearcherConfiguration<                      TemplateStepSearcher>(SearcherDisplayingLevel.C, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.                LastResort)]
[assembly: SearcherConfiguration<                  JuniorExocetStepSearcher>(SearcherDisplayingLevel.D)]
[assembly: SearcherConfiguration<                  SeniorExocetStepSearcher>(SearcherDisplayingLevel.D, EnabledArea = SearcherEnabledArea.     None, DisabledReason = SearcherDisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherConfiguration<                    DominoLoopStepSearcher>(SearcherDisplayingLevel.D)]
[assembly: SearcherConfiguration<         MultisectorLockedSetsStepSearcher>(SearcherDisplayingLevel.D)]
[assembly: SearcherConfiguration<                    BruteForceStepSearcher>(SearcherDisplayingLevel.E, EnabledArea = SearcherEnabledArea.  Default, DisabledReason = SearcherDisabledReason.                LastResort)]
#pragma warning restore format

#pragma warning disable IDE0130
namespace Sudoku.Solving.Logical;

/// <include
///     file='../../global-doc-comments.xml'
///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="type"]' />
internal static class ModuleInitializer
{
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="module-initializer"]/target[@name="method"]' />
	[ModuleInitializer]
	[RequiresUnreferencedCode("This is a method called 'Module Initializer'. The method is reserved for compiler usage and you cannot call this method explicitly.")]
	public static void Initialize()
	{
		var listOfStepSearchers = new List<IStepSearcher>();
		foreach (var type in typeof(ModuleInitializer).Assembly.GetTypes())
		{
			// The step searcher must be applied the attribute 'StepSearcherAttribute'.
			if (!type.IsDefined(typeof(StepSearcherAttribute)))
			{
				continue;
			}

			// The step searcher cannot be deprecated.
			if (type.GetCustomAttribute<StepSearcherOptionsAttribute>() is { IsDeprecated: true })
			{
				continue;
			}

			// The step searcher must implement the interface 'IStepSearcher'.
			if (!type.IsAssignableTo(typeof(IStepSearcher)))
			{
				continue;
			}

			// The step searcher must contain a parameterless instance constructor.
			if (type.GetConstructors().All(static c => c.GetParameters().Length != 0))
			{
				continue;
			}

			// Now checks whether the step searcher can be separated into multiple instances.
			// If so, we should create instances one by one, and assign the properties with values
			// using the values inside the type 'SeparatedStepSearcherAttribute'.
			switch (type.GetCustomAttributes<SeparatedStepSearcherAttribute>().ToArray())
			{
				case { Length: not 0 } optionalAssignments:
				{
					// Sort the attribute instances via the priority.
					Array.Sort(optionalAssignments, static (x, y) => x.Priority.CompareTo(y.Priority));

					// Iterate on each attribute instances.
					foreach (var attributeInstance in optionalAssignments)
					{
						// Creates an instance.
						var instance = (IStepSearcher)Activator.CreateInstance(type)!;

						// Checks the inner values, in order to be used later.
						var propertyNamesAndValues = attributeInstance.PropertyNamesAndValues;
						switch (propertyNamesAndValues.Length)
						{
							case 0:
							{
								t("The array is empty.");
								break;
							}
							case var length when (length & 1) != 0:
							{
								t("The property value is invalid.");
								break;
							}
							case var length:
							{
								for (var i = 0; i < length; i += 2)
								{
									var propertyName = (string)propertyNamesAndValues[i];
									var propertyValue = propertyNamesAndValues[i + 1];

									(
										type.GetProperty(propertyName) switch
										{
											null => t("Such property name cannot be found."),
											{ CanWrite: false } => t("The property is read-only and cannot be assigned."),
											var p => p
										}
									).SetValue(instance, propertyValue);
								}

								instance.Options = instance.Options with
								{
									SeparatedStepSearcherPriority = attributeInstance.Priority
								};

								break;
							}
						}

						listOfStepSearchers.Add(instance);
					}

					break;
				}
				default:
				{
					listOfStepSearchers.Add((IStepSearcher)Activator.CreateInstance(type)!);

					break;
				}


				[DoesNotReturn]
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				static PropertyInfo? t(string s) => throw new InvalidOperationException(s);
			}
		}

		// Assign the result.
		listOfStepSearchers.Sort(static (s1, s2) =>
		{
			if (s1.Options.Priority > s2.Options.Priority)
			{
				return 1;
			}
			else if (s1.Options.Priority < s2.Options.Priority)
			{
				return -1;
			}
			else if (s1.Options.SeparatedStepSearcherPriority > s2.Options.SeparatedStepSearcherPriority)
			{
				return 1;
			}
			else if (s1.Options.SeparatedStepSearcherPriority < s2.Options.SeparatedStepSearcherPriority)
			{
				return -1;
			}

			return 0;
		});
		StepSearcherPool.Collection = listOfStepSearchers.ToArray();
	}
}
