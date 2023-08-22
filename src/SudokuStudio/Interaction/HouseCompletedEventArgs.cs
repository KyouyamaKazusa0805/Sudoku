namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="HouseCompletedEventHandler"/>.
/// </summary>
/// <param name="house">Indicates the house finished.</param>
/// <seealso cref="HouseCompletedEventHandler"/>
public sealed partial class HouseCompletedEventArgs([DataMember] House house) : EventArgs;
