namespace System.CommandLine;

/// <summary>
/// Defines a special command for the special use.
/// </summary>
/// <typeparam name="TErrorCode"><inheritdoc/></typeparam>
public interface ISpecialCommand<TErrorCode> : IRootCommand<TErrorCode> where TErrorCode : Enum
{
}
