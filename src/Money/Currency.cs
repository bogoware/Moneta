namespace Bogoware.Money;

public interface ICurrency
{
	/// <summary>
	/// Should be the ISO 4217 currency code.
	/// </summary>
	string Code { get; }

	/// <summary>
	/// The international name of the currency.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The international symbol of the currency.
	/// </summary>
	string Symbol { get; }

	/// <summary>
	/// The number of decimal places used by the currency.
	/// </summary>
	int DecimalPlaces { get; }

	/// <summary>
	/// A neutral currency is a currency that can be used in any operation with any other currency.
	/// </summary>
	bool IsNeutral { get; }
}

public abstract class Currency : ICurrency, IEquatable<Currency>
{
	/// <summary>
	/// An <see cref="UndefinedCurrency"/> with 2 decimal places.
	/// </summary>
	public static Currency Undefined { get; } = new UndefinedCurrency(2);


	public string Code { get; }


	public string Name { get; }

	public string Symbol { get; }

	public int DecimalPlaces { get; }


	public bool IsNeutral { get; }

	protected internal Currency(
		string code,
		string name,
		string symbol,
		int decimalPlaces,
		bool isNeutral)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(decimalPlaces);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(decimalPlaces, 28);

		Code = code;
		Name = name;
		DecimalPlaces = decimalPlaces;
		Symbol = symbol;
		IsNeutral = isNeutral;
	}

	/// <summary>
	/// Initialize a new <see cref="Currency"/>.
	/// </summary>
	/// <param name="code">Should be the ISO 4217 currency code. No constraints applied</param>
	/// <param name="name">The international name of the currency.</param>
	/// <param name="symbol">The international symbol of the currency.</param>
	/// <param name="decimalPlaces">The number of decimal places used by the currency.</param>
	public Currency(
		string code,
		string name,
		string symbol,
		int decimalPlaces)
		: this(code, name, symbol, decimalPlaces, false)
	{
	}

	public bool Equals(Currency? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Code == other.Code;
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		if (obj.GetType() != this.GetType()) return false;
		return Equals((Currency)obj);
	}

	public override int GetHashCode() => Code.GetHashCode();

	public static bool operator ==(Currency? left, Currency? right) => Equals(left, right);

	public static bool operator !=(Currency? left, Currency? right) => !Equals(left, right);
}

/// <summary>
/// This currency is used to represent a monetary value that has no currency and can
/// be used in any operation with any other currency.
/// </summary>
public sealed class UndefinedCurrency : Currency
{
	/// <summary>
	/// Instantiate a new <see cref="UndefinedCurrency"/> with the specified number of decimal places.
	/// </summary>
	public UndefinedCurrency(int decimalPlaces)
		: base("XXX", "No Currency", "¤", decimalPlaces,  true)
	{
	}
}