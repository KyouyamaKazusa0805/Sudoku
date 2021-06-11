S? s = null;

_ = s.HasValue;
_ = !s.HasValue;
_ = s == null;
_ = s != null;

readonly struct S
{
	public int A { get; init; }
}