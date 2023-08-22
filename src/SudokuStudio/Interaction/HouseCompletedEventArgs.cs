namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="HouseCompletedEventHandler"/>.
/// </summary>
/// <param name="lastCell">Indicates the last cell finished.</param>
/// <param name="house">Indicates the house finished.</param>
/// <seealso cref="HouseCompletedEventHandler"/>
public sealed partial class HouseCompletedEventArgs([DataMember] Cell lastCell, [DataMember] House house) : EventArgs;
