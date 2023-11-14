namespace Bogoware.Moneta;

public partial class Money
{
	
	/// <summary>
	/// Subtract the <see cref="Money"/> from the current instance and return a new <see cref="Money"/> instance
	/// with the the most specific currency between the two.
	/// </summary>
	/// <seealso cref="M:Bogoware.Moneta.Abstractions.ICurrency.GetMostSpecificCurrency(Bogoware.Moneta.Abstractions.ICurrency,Bogoware.Moneta.Abstractions.ICurrency,Bogoware.Moneta.Abstractions.ICurrency@,Bogoware.Moneta.Abstractions.ICurrency@)"/>
	/// <exception cref="CurrencyIncompatibleException">Thrown when the two currencies are not compatible.</exception>
	public Money Subtract(Money other, MidpointRounding rounding, out decimal error)
	{
		ICurrency.GetMostSpecificCurrency(Currency, other.Currency, out var lessSpecificCurrency,
			out var mostSpecificCurrency);

		if (mostSpecificCurrency.DecimalPlaces < lessSpecificCurrency.DecimalPlaces)
		{
			// This case happens when adding a non neutral currency to a neutral currency with more decimal places.
			var internalAmount = Math.Round(Amount - other.Amount, Context.RoundingErrorDecimals, rounding);
			var newAmount = Math.Round(internalAmount, mostSpecificCurrency.DecimalPlaces, rounding);
			error = internalAmount - newAmount;
			return new(newAmount, mostSpecificCurrency, Context);
		}

		error = 0;
		return new(Amount - other.Amount, mostSpecificCurrency, Context);
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
	
	/// <summary>
	/// Subtract the number from the <see cref="Money"/>.
	/// </summary>
	public Money Subtract<T>(T amount, MidpointRounding rounding, out decimal error) where T : INumber<T>, IConvertible
	{
		ValidateType<T>();
		decimal decimalAmount = Convert.ToDecimal(amount);
		var internalAmount = Math.Round(Amount - decimalAmount, Context.RoundingErrorDecimals, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
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
	public static Money operator -(decimal left, Money right) => right.Subtract(left);
	public static Money operator -(long left, Money right) => right.Subtract(left);
	public static Money operator -(int left, Money right) => right.Subtract(left);
	public static Money operator -(short left, Money right) => right.Subtract(left);
	public static Money operator -(ulong left, Money right) => right.Subtract(left);
	public static Money operator -(uint left, Money right) => right.Subtract(left);
	public static Money operator -(ushort left, Money right) => right.Subtract(left);
	public static Money operator -(double left, Money right) => right.Subtract(left);
	public static Money operator -(float left, Money right) => right.Subtract(left);

}