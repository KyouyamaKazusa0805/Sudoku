namespace System.CommandLine;

/// <summary>
/// Defines a help command.
/// </summary>
/// <typeparam name="TErrorCode"><inheritdoc/></typeparam>
public interface IHelpCommand<TErrorCode> : IRootCommand<TErrorCode> where TErrorCode : Enum
{
}
