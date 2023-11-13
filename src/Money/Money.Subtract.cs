namespace Bogoware.Money;

public partial class Money
{
	/// <summary>
	/// Subtract the specified amount to the money.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount to subtract.</param>
	/// <param name="rounding">The rounding mode to use.</param> 
	/// <param name="error">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The difference.</returns> 
	public Money Subtract(decimal amount, MidpointRounding rounding, out decimal error)
	{
		var internalAmount = Math.Round(Amount - amount, Context.RoundingErrorDecimals, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Subtract the specified amount to the money using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount to subtract.</param>
	/// <param name="error">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The difference.</returns>
	public Money Subtract(decimal amount, out decimal error) => Subtract(amount, Context.RoundingMode, out error);

	/// <inheritdoc cref="Subtract(decimal,System.MidpointRounding,out Bogoware.Money.Money)"/>
	public Money Subtract(double amount, MidpointRounding rounding, out decimal error)
	{
		decimal internalAmount = Math.Round(Amount - (decimal)amount, Context.RoundingErrorDecimals, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Subtract(decimal,out Bogoware.Money.Money)"/>
	public Money Subtract(double amount, out decimal error) => Subtract(amount, Context.RoundingMode, out error);
	
	public static Money operator -(Money left, Money right)
	{
		ValidateOperands(left, right);
		return new(left.Amount - right.Amount, left.Currency, left.Context);
	}

	public static Money operator -(Money left, decimal right) => new(left.Amount - right, left.Currency, left.Context);
	public static Money operator -(Money left, double right) => left - left.Context.CreateMoney(right, left.Currency);

}