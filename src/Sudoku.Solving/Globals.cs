global using System.Buffers;
global using System.ComponentModel;
global using System.Globalization;
global using System.Reflection;
global using System.Runtime.Intrinsics;
global using System.Runtime.Serialization;
global using Sudoku.Concepts;
global using Sudoku.Concepts.Collections;
global using Sudoku.Concepts.Collections.Handlers;
global using Sudoku.Concepts.Enumerators;
global using Sudoku.Presentation;
global using Sudoku.Resources;
global using Sudoku.Solving.BruteForces;
global using Sudoku.Solving.Collections;
global using Sudoku.Solving.Manual;
global using Sudoku.Solving.Manual.Buffer;
global using Sudoku.Solving.Manual.Checkers;
global using Sudoku.Solving.Manual.Searchers;
global using Sudoku.Solving.Manual.Steps;
global using Sudoku.Solving.Manual.Text;
global using Sudoku.Techniques;
global using static System.Algorithm.Combinatorics;
global using static System.Algorithm.Sequences;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Runtime.AnalysisServices.CommonReadOnlies;
global using static Sudoku.Solving.Manual.Buffer.FastProperties;

[assembly: SearcherInitializerOption<SingleStepSearcher>(1, DisplayingLevel.A)]
[assembly: SearcherInitializerOption<LockedCandidatesStepSearcher>(2, DisplayingLevel.A)]
[assembly: SearcherInitializerOption<SubsetStepSearcher>(3, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<NormalFishStepSearcher>(4, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<TwoStrongLinksStepSearcher>(5, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<RegularWingStepSearcher>(6, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<WWingStepSearcher>(7, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<UniqueRectangleStepSearcher>(8, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<AlmostLockedCandidatesStepSearcher>(9, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<UniqueLoopStepSearcher>(10, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<ExtendedRectangleStepSearcher>(11, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<EmptyRectangleStepSearcher>(12, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<AlternatingInferenceChainStepSearcher>(13, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<BivalueOddagonStepSearcher>(14, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<SueDeCoqStepSearcher>(15, DisplayingLevel.C)]
[assembly: SearcherInitializerOption<UniqueSquareStepSearcher>(16, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<UniquePolygonStepSearcher>(17, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<GuardianStepSearcher>(18, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<BowmanBingoStepSearcher>(19, DisplayingLevel.C, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.TooSlow | DisabledReason.LastResort)]
[assembly: SearcherInitializerOption<PatternOverlayStepSearcher>(20, DisplayingLevel.C, EnabledAreas = EnabledAreas.Gathering, DisabledReason = DisabledReason.LastResort)]
[assembly: SearcherInitializerOption<TemplateStepSearcher>(21, DisplayingLevel.C, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.LastResort)]
[assembly: SearcherInitializerOption<SueDeCoq3DemensionStepSearcher>(22, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<AlmostLockedSetsXzStepSearcher>(23, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<BivalueUniversalGraveStepSearcher>(24, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<QiuDeadlyPatternStepSearcher>(25, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<EmptyRectangleIntersectionPairStepSearcher>(26, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<AlmostLockedSetsXyWingStepSearcher>(27, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<AlmostLockedSetsWWingStepSearcher>(28, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<ComplexFishStepSearcher>(29, DisplayingLevel.B)]
[assembly: SearcherInitializerOption<JuniorExocetStepSearcher>(30, DisplayingLevel.D, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherInitializerOption<SeniorExocetStepSearcher>(31, DisplayingLevel.D, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherInitializerOption<MultisectorLockedSetsStepSearcher>(32, DisplayingLevel.D)]
[assembly: SearcherInitializerOption<DominoLoopStepSearcher>(33, DisplayingLevel.D)]
[assembly: SearcherInitializerOption<BruteForceStepSearcher>(34, DisplayingLevel.E, EnabledAreas = EnabledAreas.Default, DisabledReason = DisabledReason.LastResort)]
[module: SkipLocalsInit]