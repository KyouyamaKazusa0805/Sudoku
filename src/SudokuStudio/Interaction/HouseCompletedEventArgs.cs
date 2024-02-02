namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="HouseCompletedEventHandler"/>.
/// </summary>
/// <param name="lastCell">Indicates the last cell finished.</param>
/// <param name="house">Indicates the house finished.</param>
/// <param name="method">Indicates a method kind that makes a house be completed.</param>
/// <seealso cref="HouseCompletedEventHandler"/>
public sealed partial class HouseCompletedEventArgs([PrimaryConstructorParameter] Cell lastCell, [PrimaryConstructorParameter] House house, [PrimaryConstructorParameter] PuzzleUpdatingMethod method) :
	EventArgs;
