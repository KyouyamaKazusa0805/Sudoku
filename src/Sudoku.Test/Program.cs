using System;

class I : object, IEquatable<I>
{
	public override bool Equals(object? obj) => base.Equals(obj);
	public bool Equals(I? other) => throw new NotImplementedException();
	public override int GetHashCode() => base.GetHashCode();
	public override string ToString() => base.ToString()!;

	public static bool operator ==(I left, I right) => left.Equals(right);
	public static bool operator !=(I left, I right) => !(left == right);
}