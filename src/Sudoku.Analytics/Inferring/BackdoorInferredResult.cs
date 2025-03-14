namespace Sudoku.Inferring;

/// <summary>
/// Indicates the result value after <see cref="BackdoorInferrer.TryInfer(in Grid, out BackdoorInferredResult)"/> called.
/// </summary>
/// <param name="candidates">Indicates the found candidates.</param>
/// <seealso cref="BackdoorInferrer.TryInfer(in Grid, out BackdoorInferredResult)"/>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct BackdoorInferredResult([Property] ReadOnlySpan<Conclusion> candidates);
