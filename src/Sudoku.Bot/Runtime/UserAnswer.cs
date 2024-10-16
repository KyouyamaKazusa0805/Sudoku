namespace Sudoku.Runtime;

/// <summary>
/// 表示用户的填充答案。
/// </summary>
/// <param name="userId"><inheritdoc cref="UserId" path="/summary"/></param>
/// <param name="answerString"><inheritdoc cref="AnswerString" path="/summary"/></param>
[TypeImpl(
	TypeImplFlags.AllObjectMethods | TypeImplFlags.EqualityOperators,
	OtherModifiersOnEquals = "sealed",
	OtherModifiersOnGetHashCode = "sealed",
	OtherModifiersOnToString = "sealed")]
public abstract partial class UserAnswer(string userId, string answerString) :
	IEquatable<UserAnswer>,
	IEqualityOperators<UserAnswer, UserAnswer, bool>
{
	/// <summary>
	/// 表示回复的用户编号。
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public string UserId { get; } = userId;

	/// <summary>
	/// 表示用户回复的结果。
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public string AnswerString { get; } = answerString;

	/// <summary>
	/// 表示游戏的类型。
	/// </summary>
	[HashCodeMember]
	[StringMember]
	public abstract GameMode Mode { get; }

	/// <summary>
	/// 表示用户输入的字符串应该解析成的目标类型，用于处理和校验正确性。
	/// </summary>
	public abstract Type ResultType { get; }

	[StringMember("ResultType")]
	private string ResultTypeString
		=> ResultType.FullName is { } fullName ? $"typeof({fullName})" : "<Unknown>";


	/// <inheritdoc/>
	public bool Equals([NotNullWhen(true)] UserAnswer? other)
		=> other is not null
		&& (UserId, AnswerString, Mode) == (other.UserId, other.AnswerString, other.Mode);

	/// <summary>
	/// 检测当前输入字符串是否是正确的内容。
	/// </summary>
	/// <param name="result">解析后的结果。</param>
	/// <returns>表示验证是否正确。</returns>
	public abstract bool IsValidAnswer([NotNullWhen(true)] out object? result);
}
