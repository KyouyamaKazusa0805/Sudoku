global using System;
global using System.Buffers;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Globalization;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Text.RegularExpressions;
global using System.Threading;
global using System.Threading.Tasks;
global using Sudoku.Concepts;
global using Sudoku.Concepts.Collections;
global using Sudoku.Concepts.Collections.Handlers;
global using Sudoku.Concepts.Enumerators;
global using Sudoku.Concepts.Solving;
global using Sudoku.Concepts.Solving.ChainNodes;
global using Sudoku.Presentation;
global using Sudoku.Presentation.Nodes;
global using Sudoku.Resources;
global using Sudoku.Runtime.Reflection;
global using Sudoku.Solving.BruteForces;
global using Sudoku.Solving.Collections;
global using Sudoku.Solving.Manual;
global using Sudoku.Solving.Manual.Buffer;
global using Sudoku.Solving.Manual.Checkers;
global using Sudoku.Solving.Manual.Searchers;
global using Sudoku.Solving.Manual.Steps;
global using static System.Algorithm.Combinatorics;
global using static System.Algorithm.Sequences;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Runtime.AnalysisServices.CommonReadOnlies;
global using static Sudoku.Solving.Manual.Buffer.FastProperties;

[assembly: SearcherConfiguration<SingleStepSearcher>(1, DisplayingLevel.A)]
[assembly: SearcherConfiguration<LockedCandidatesStepSearcher>(2, DisplayingLevel.A)]
[assembly: SearcherConfiguration<SubsetStepSearcher>(3, DisplayingLevel.B)]
[assembly: SearcherConfiguration<NormalFishStepSearcher>(4, DisplayingLevel.B)]
[assembly: SearcherConfiguration<TwoStrongLinksStepSearcher>(5, DisplayingLevel.B)]
[assembly: SearcherConfiguration<RegularWingStepSearcher>(6, DisplayingLevel.B)]
[assembly: SearcherConfiguration<WWingStepSearcher>(7, DisplayingLevel.B)]
[assembly: SearcherConfiguration<UniqueRectangleStepSearcher>(8, DisplayingLevel.B)]
[assembly: SearcherConfiguration<AlmostLockedCandidatesStepSearcher>(9, DisplayingLevel.B)]
[assembly: SearcherConfiguration<UniqueLoopStepSearcher>(10, DisplayingLevel.B)]
[assembly: SearcherConfiguration<ExtendedRectangleStepSearcher>(11, DisplayingLevel.B)]
[assembly: SearcherConfiguration<EmptyRectangleStepSearcher>(12, DisplayingLevel.B)]
[assembly: SearcherConfiguration<AlternatingInferenceChainStepSearcher>(13, DisplayingLevel.B)]
[assembly: SearcherConfiguration<BivalueOddagonStepSearcher>(14, DisplayingLevel.B)]
[assembly: SearcherConfiguration<SueDeCoqStepSearcher>(15, DisplayingLevel.C)]
[assembly: SearcherConfiguration<UniqueSquareStepSearcher>(16, DisplayingLevel.B)]
[assembly: SearcherConfiguration<UniquePolygonStepSearcher>(17, DisplayingLevel.B)]
[assembly: SearcherConfiguration<GuardianStepSearcher>(18, DisplayingLevel.B)]
[assembly: SearcherConfiguration<BowmanBingoStepSearcher>(19, DisplayingLevel.C, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.TooSlow | DisabledReason.LastResort)]
[assembly: SearcherConfiguration<PatternOverlayStepSearcher>(20, DisplayingLevel.C, EnabledAreas = EnabledAreas.Gathering, DisabledReason = DisabledReason.LastResort)]
[assembly: SearcherConfiguration<TemplateStepSearcher>(21, DisplayingLevel.C, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.LastResort)]
[assembly: SearcherConfiguration<SueDeCoq3DemensionStepSearcher>(22, DisplayingLevel.B)]
[assembly: SearcherConfiguration<AlmostLockedSetsXzStepSearcher>(23, DisplayingLevel.B)]
[assembly: SearcherConfiguration<BivalueUniversalGraveStepSearcher>(24, DisplayingLevel.B)]
[assembly: SearcherConfiguration<QiuDeadlyPatternStepSearcher>(25, DisplayingLevel.B)]
[assembly: SearcherConfiguration<EmptyRectangleIntersectionPairStepSearcher>(26, DisplayingLevel.B)]
[assembly: SearcherConfiguration<AlmostLockedSetsXyWingStepSearcher>(27, DisplayingLevel.B)]
[assembly: SearcherConfiguration<AlmostLockedSetsWWingStepSearcher>(28, DisplayingLevel.B)]
[assembly: SearcherConfiguration<ComplexFishStepSearcher>(29, DisplayingLevel.B)]
[assembly: SearcherConfiguration<JuniorExocetStepSearcher>(30, DisplayingLevel.D, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherConfiguration<SeniorExocetStepSearcher>(31, DisplayingLevel.D, EnabledAreas = EnabledAreas.None, DisabledReason = DisabledReason.DeprecatedOrNotImplemented)]
[assembly: SearcherConfiguration<MultisectorLockedSetsStepSearcher>(32, DisplayingLevel.D)]
[assembly: SearcherConfiguration<DominoLoopStepSearcher>(33, DisplayingLevel.D)]
[assembly: SearcherConfiguration<BruteForceStepSearcher>(34, DisplayingLevel.E, EnabledAreas = EnabledAreas.Default, DisabledReason = DisabledReason.LastResort)]
[module: SkipLocalsInit]