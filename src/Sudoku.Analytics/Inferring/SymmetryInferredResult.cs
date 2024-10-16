namespace Sudoku.Inferring;

/// <summary>
/// Indicates the result value after <see cref="SymmetryInferrer.TryInfer(ref readonly Grid, out SymmetryInferredResult)"/> called.
/// </summary>
/// <param name="symmetricType">The symmetric type returned.</param>
/// <param name="mappingDigits">The mapping digits returned.</param>
/// <param name="selfPairedDigitsMask">A mask that contains a list of digits self-paired.</param>
/// <seealso cref="SymmetryInferrer.TryInfer(ref readonly Grid, out SymmetryInferredResult)"/>
[TypeImpl(TypeImplFlags.AllObjectMethods)]
public readonly ref partial struct SymmetryInferredResult(
	[Property] SymmetricType symmetricType,
	[Property] ReadOnlySpan<Digit?> mappingDigits,
	[Property] Mask selfPairedDigitsMask
)
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out SymmetricType symmetricType, out ReadOnlySpan<Digit?> mappingDigits, out Mask selfPairedDigitsMask)
	{
		symmetricType = SymmetricType;
		mappingDigits = MappingDigits;
		selfPairedDigitsMask = SelfPairedDigitsMask;
	}
}
