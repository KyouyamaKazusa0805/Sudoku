namespace System.CommandLine;

/// <summary>
/// Defines a version command.
/// </summary>
/// <typeparam name="TErrorCode"><inheritdoc/></typeparam>
public interface IVersionCommand<TErrorCode> : ISpecialCommand<TErrorCode> where TErrorCode : Enum
{
}
