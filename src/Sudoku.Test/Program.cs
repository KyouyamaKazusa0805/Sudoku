S? s = null;
R? r = null;

_ = s is null;
_ = s is not null;
_ = s.HasValue;
_ = s == null;
_ = s != null;
_ = s is { };
_ = s is not { };
_ = r is null;
_ = r is not null;
_ = r == null;
_ = r != null;
_ = r is { E: { } };
_ = r is not { };

record R(int A, double B, float C, string D, R? E = null);
readonly struct S
{
	public int A { get; init; }
}