namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Direct Intersection</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="cell"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="subtype"><inheritdoc/></param>
/// <param name="basedOn"><inheritdoc/></param>
/// <param name="isPointing">Indicates whether the current locked candidates pattern used is pointing.</param>
public sealed partial class DirectIntersectionStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Cell cell,
	Digit digit,
	SingleSubtype subtype,
	Technique basedOn,
	[RecordParameter] bool isPointing
) : ComplexSingleStep(
	conclusions,
	views,
	options,
	cell,
	digit,
	subtype,
	basedOn,
	[[isPointing ? Technique.Pointing : Technique.Claiming]]
)
{
	/// <summary>
	/// Indicates the "Not supported" message.
	/// </summary>
	private const string NotSupportedMessage = "This technique usage doesn't use this property.";


	/// <inheritdoc/>
	public override decimal BaseDifficulty
		=> BasedOn switch
		{
			Technique.FullHouse => 1.0M,
			Technique.CrosshatchingBlock => 1.2M,
			Technique.CrosshatchingRow or Technique.CrosshatchingColumn => 1.9M,
			Technique.HiddenSingleBlock => 1.2M,
			Technique.HiddenSingleRow or Technique.HiddenSingleColumn => 2.3M,
			Technique.NakedSingle => 2.3M,
			_ => throw new NotSupportedException(TechniqueNotSupportedMessage)
		} + .2M;

	/// <inheritdoc/>
	public override Technique Code
		=> BasedOn switch
		{
			Technique.FullHouse => Technique.PointingFullHouse,
			Technique.CrosshatchingBlock => Technique.PointingCrosshatchingBlock,
			Technique.HiddenSingleBlock => Technique.PointingCrosshatchingBlock,
			Technique.CrosshatchingRow or Technique.HiddenSingleRow => Technique.PointingCrosshatchingRow,
			Technique.CrosshatchingColumn or Technique.HiddenSingleColumn => Technique.PointingCrosshatchingColumn,
			Technique.NakedSingle => Technique.PointingNakedSingle,
			_ => throw new NotSupportedException(TechniqueNotSupportedMessage)
		};

	/// <inheritdoc/>
	protected override string PrefixName
	{
		[DoesNotReturn]
		get => throw new NotSupportedException(NotSupportedMessage);
	}

	/// <inheritdoc/>
	protected override int PrefixNameLength
	{
		[DoesNotReturn]
		get => throw new NotImplementedException(NotSupportedMessage);
	}


	/// <inheritdoc/>
	public override string GetName(CultureInfo? culture) => Code.GetName(culture ?? ResultCurrentCulture);
}
