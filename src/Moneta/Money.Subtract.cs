using static System.Math;

namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Subtract the number from the <see cref="Money"/>.
	/// </summary>
	public Money Subtract<T>(T amount, MidpointRounding rounding, out decimal error) where T : INumber<T>, IConvertible
	{
		var decimalAmount = ValidateAndGetDecimalValue(amount);
		var internalAmount = Round(Amount - decimalAmount, Context.RoundingErrorDecimals, rounding);
		var newAmount = Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Subtract <see cref="other"/> from the current instance and returns a new <see cref="Money"/> instance
	/// with the the most specific currency between the two.
	/// </summary>
	/// <seealso cref="M:Bogoware.Moneta.Abstractions.ICurrency.GetMostSpecificCurrency(Bogoware.Moneta.Abstractions.ICurrency,Bogoware.Moneta.Abstractions.ICurrency,Bogoware.Moneta.Abstractions.ICurrency@,Bogoware.Moneta.Abstractions.ICurrency@)"/>
	/// <exception cref="CurrencyIncompatibleException">Thrown when the two currencies are not compatible.</exception>
	public Money Subtract(Money other, MidpointRounding rounding, out decimal error)
	{
		ICurrency.MustBeCompatible(Currency, other.Currency);
		
		return Add(-other.Amount, rounding, out error);
	}

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Subtract(Bogoware.Moneta.Money,System.MidpointRounding,System.Decimal@)"/>
	public Money Subtract(Money other, out decimal error) => Subtract(other, Context.RoundingMode, out error);

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Subtract(Bogoware.Moneta.Money,System.MidpointRounding,System.Decimal@)"/>
	public Money Subtract(Money other)
	{
		var result = Subtract(other, Context.RoundingMode, out var error);
		var roundingErrorOperation = new SubtractOperation(error, Currency);
		Context.AddRoundingErrorOperation(roundingErrorOperation);
		return result;
	}

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Subtract``1(``0,System.MidpointRounding,System.Decimal@)"/>
	public Money Subtract<T>(T amount, out decimal error) where T : INumber<T>, IConvertible =>
		Subtract(amount, Context.RoundingMode, out error);
	
	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Subtract``1(``0,System.MidpointRounding,System.Decimal@)"/>
	public Money Subtract<T>(T amount, MidpointRounding rounding) where T : INumber<T>, IConvertible
	{
		var result = Subtract(amount, rounding, out var residue);
		var errorRoundingOperation = new SubtractOperation(residue, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return result;
	}
	
	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Subtract``1(``0,System.MidpointRounding,System.Decimal@)"/>
	public Money Subtract<T>(T amount) where T : INumber<T>, IConvertible
	{
		var result = Subtract(amount, Context.RoundingMode, out var residue);
		var errorRoundingOperation = new SubtractOperation(residue, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return result;
	}

	public static Money operator -(Money left, Money right) => left.Subtract(right);
	public static Money operator -(Money left, decimal right) => left.Subtract(right);
	public static Money operator -(Money left, long right) => left.Subtract(right);
	public static Money operator -(Money left, int right) => left.Subtract(right);
	public static Money operator -(Money left, short right) => left.Subtract(right);
	public static Money operator -(Money left, ulong right) => left.Subtract(right);
	public static Money operator -(Money left, uint right) => left.Subtract(right);
	public static Money operator -(Money left, ushort right) => left.Subtract(right);
	public static Money operator -(Money left, double right) => left.Subtract(right);
	public static Money operator -(Money left, float right) => left.Subtract(right);
}