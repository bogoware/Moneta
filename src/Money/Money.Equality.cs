namespace Bogoware.Money;

public partial class Money
{
	public bool Equals(Money? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Amount == other.Amount && Currency.Equals(other.Currency);
	}

	public override bool Equals(object? obj) => Equals(obj as Money);
	public override int GetHashCode() => HashCode.Combine(Amount, Currency);

	public static bool operator ==(Money? left, Money? right) => Equals(left, right);
	public static bool operator !=(Money? left, Money? right) => !Equals(left, right);
}