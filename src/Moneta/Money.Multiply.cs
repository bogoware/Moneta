namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Multiply the money by the specified multiplier.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="multiplier">The multiplier.</param>
	/// <param name="rounding">The rounding mode to use.</param> 
	/// <param name="error">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The product.</returns> 
	public Money Multiply(decimal multiplier, MidpointRounding rounding, out decimal error)
	{
		var internalAmount = Math.Round(Amount * multiplier, Context.RoundingErrorDecimals, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Multiply the money by the specified multiplier using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="multiplier">The multiplier.</param>
	/// <param name="error">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The product.</returns>
	public Money Multiply(decimal multiplier, out decimal error) =>
		Multiply(multiplier, Context.RoundingMode, out error);

	/// <inheritdoc cref="Multiply(decimal,System.MidpointRounding,out Bogoware.Money.Money)"/>
	public Money Multiply(double multiplier, MidpointRounding rounding, out decimal error)
	{
		decimal internalAmount = Math.Round(Amount * (decimal)multiplier, Context.RoundingErrorDecimals, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Multiply(decimal,out Bogoware.Money.Money)"/>
	public Money Multiply(double multiplier, out decimal error) =>
		Multiply(multiplier, Context.RoundingMode, out error);
	
	public static Money operator *(Money left, decimal right) => new(left.Amount * right, left.Currency, left.Context);

	public static Money operator *(decimal left, Money right) =>
		new(left * right.Amount, right.Currency, right.Context);

}