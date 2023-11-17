namespace Bogoware.Moneta;

/// <summary>
/// A currency
/// </summary>
public class Currency: Currency<Currency>
{
	/// <summary>
	/// Initializes a new <see cref="Currency"/> instance.
	/// </summary>
	public Currency(string code, string name, string symbol, int decimalPlaces)
		: base(code, name, symbol, decimalPlaces)
	{
	}
}

/// <summary>
/// An <see cref="ICurrency"/> implementation suitable to create new currencies.
/// Equality is based on the <see cref="Code"/> property only.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Currency<T> : ICurrency, IEquatable<T> where T: Currency<T>
{
	
	public string Code { get; }
	public string Name { get; }
	public string Symbol { get; }
	public int DecimalPlaces { get; }

	protected Currency(
		string code,
		string name,
		string symbol,
		int decimalPlaces)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(decimalPlaces);
		ArgumentOutOfRangeException.ThrowIfGreaterThan(decimalPlaces, 28);

		Code = code;
		Name = name;
		DecimalPlaces = decimalPlaces;
		Symbol = symbol;
	}

	public bool Equals(T? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		if (other.GetType() != GetType()) return false;
		return Code == other.Code;
	}
	public override bool Equals(object? obj) => Equals(obj as T);

	public override int GetHashCode() => Code.GetHashCode();

	public static bool operator ==(Currency<T> left, Currency<T> right) => Equals(left, right);

	public static bool operator !=(Currency<T> left, Currency<T> right) => !Equals(left, right);
	
	public override string ToString() => Code;
}

/// <summary>
/// This currency is used to represent a monetary value with 4 decimal places that has no currency and can
/// be used in any operation with any other currency.
/// </summary>
public sealed class UndefinedCurrency : Currency<UndefinedCurrency>
{
	private const int DECIMAL_PLACES = 4;
	
	public static readonly UndefinedCurrency Instance = new();
	
	/// <summary>
	/// Instantiate a new <see cref="UndefinedCurrency"/> with the specified number of decimal places.
	/// </summary>
	private UndefinedCurrency()
		: base("XXX", "No Currency", "¤", DECIMAL_PLACES)
	{
	}
}