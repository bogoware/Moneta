using Bogoware.Moneta.Abstractions;

namespace Bogoware.Moneta;

/// <summary>
/// A currency
/// </summary>
public class Currency: Currency<Currency>
{
	/// <summary>
	/// An <see cref="UndefinedCurrency"/> with 2 decimal places.
	/// </summary>
	public static ICurrency DefaultUndefined { get; } = new UndefinedCurrency(2);

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
	public bool IsNeutral { get; }

	internal Currency(
		string code,
		string name,
		string symbol,
		int decimalPlaces,
		bool isNeutral)
	{
		ArgumentOutOfRangeException.ThrowIfNegative(decimalPlaces);
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
/// This currency is used to represent a monetary value that has no currency and can
/// be used in any operation with any other currency.
/// </summary>
public sealed class UndefinedCurrency : Currency<UndefinedCurrency>
{
	/// <summary>
	/// Instantiate a new <see cref="UndefinedCurrency"/> with the specified number of decimal places.
	/// </summary>
	public UndefinedCurrency(int decimalPlaces)
		: base("XXX", "No Currency", "¤", decimalPlaces,  true)
	{
	}
}