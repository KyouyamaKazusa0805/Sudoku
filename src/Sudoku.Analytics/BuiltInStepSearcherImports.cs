using static Sudoku.Analytics.Metadata.StepSearcherLevel;
using static Sudoku.Analytics.Metadata.StepSearcherRunningArea;

/// This file stores built-in <see cref="Sudoku.Analytics.StepSearcher"/> instances,
/// in order to create default collection for type <see cref="Sudoku.Analytics.StepSearcherPool.BuiltIn"/>.

#pragma warning disable format

[assembly:
	StepSearcherImport<                        SingleStepSearcher>(Elementary),
	StepSearcherImport<              LockedCandidatesStepSearcher>(Elementary),
	StepSearcherImport<                        SubsetStepSearcher>(  Moderate),
	StepSearcherImport<                    NormalFishStepSearcher>(  Moderate),
//	StepSearcherImport<                TwoStrongLinksStepSearcher>(  Moderate),
//	StepSearcherImport<                   RegularWingStepSearcher>(  Moderate),
//	StepSearcherImport<                         WWingStepSearcher>(  Moderate),
//	StepSearcherImport<              MultiBranchWWingStepSearcher>(  Moderate),
//	StepSearcherImport<               UniqueRectangleStepSearcher>(  Moderate),
//	StepSearcherImport<        AlmostLockedCandidatesStepSearcher>(  Moderate),
//	StepSearcherImport<                      SueDeCoqStepSearcher>(  Moderate),
//	StepSearcherImport<            SueDeCoq3DimensionStepSearcher>(  Moderate),
//	StepSearcherImport<                    UniqueLoopStepSearcher>(  Moderate),
//	StepSearcherImport<             ExtendedRectangleStepSearcher>(  Moderate),
//	StepSearcherImport<                EmptyRectangleStepSearcher>(  Moderate),
//	StepSearcherImport<                  UniqueMatrixStepSearcher>(  Moderate),
//	StepSearcherImport<                 UniquePolygonStepSearcher>(  Moderate),
//	StepSearcherImport<              QiuDeadlyPatternStepSearcher>(  Moderate),
//	StepSearcherImport<         BivalueUniversalGraveStepSearcher>(  Moderate),
//	StepSearcherImport<  ReverseBivalueUniversalGraveStepSearcher>(  Moderate),
//	StepSearcherImport<           UniquenessClueCoverStepSearcher>(  Moderate),
//	StepSearcherImport<               RwDeadlyPatternStepSearcher>(  Moderate, Areas =      None),
//	StepSearcherImport<EmptyRectangleIntersectionPairStepSearcher>(  Moderate),
//	StepSearcherImport<                      FireworkStepSearcher>(  Moderate),
//	StepSearcherImport<     GurthSymmetricalPlacementStepSearcher>(Elementary),
//	StepSearcherImport<           NonMultipleChainingStepSearcher>(      Hard),
//	StepSearcherImport<            AlmostLockedSetsXzStepSearcher>(  Moderate),
//	StepSearcherImport<        AlmostLockedSetsXyWingStepSearcher>(  Moderate),
//	StepSearcherImport<         AlmostLockedSetsWWingStepSearcher>(  Moderate),
//	StepSearcherImport<                      GuardianStepSearcher>(      Hard),
//	StepSearcherImport<                   ComplexFishStepSearcher>(      Hard),
//	StepSearcherImport<                BivalueOddagonStepSearcher>(      Hard),
//	StepSearcherImport<              ChromaticPatternStepSearcher>(      Hard),
//	StepSearcherImport<                  DeathBlossomStepSearcher>(      Hard, Areas =      None),
//	StepSearcherImport<              MultipleChainingStepSearcher>(      Hard),
//	StepSearcherImport<                   BowmanBingoStepSearcher>(      Hard, Areas =      None),
//	StepSearcherImport<                      TemplateStepSearcher>(      Hard, Areas =      None),
//	StepSearcherImport<                PatternOverlayStepSearcher>(      Hard, Areas = Gathering),
//	StepSearcherImport<                  JuniorExocetStepSearcher>(  Fiendish),
//	StepSearcherImport<                  SeniorExocetStepSearcher>(  Fiendish, Areas =      None),
//	StepSearcherImport<                    DominoLoopStepSearcher>(  Fiendish),
//	StepSearcherImport<         MultisectorLockedSetsStepSearcher>(  Fiendish),
//	StepSearcherImport<      AdvancedMultipleChainingStepSearcher>(  Fiendish, Areas =      None),
	StepSearcherImport<                    BruteForceStepSearcher>(    Hidden, Areas = Searching)
]
