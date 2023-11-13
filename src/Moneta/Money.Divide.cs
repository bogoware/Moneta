namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Divide the money into the specified number of parts.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>.
	/// </summary>
	/// <param name="divisor">The divisor.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="error">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The quotient.</returns>
	public Money Divide(decimal divisor, MidpointRounding rounding, out decimal error)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(divisor);
		decimal internalAmount = Math.Round(Amount / divisor, Context.RoundingErrorDecimals, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Divide the money into the specified number of parts using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="divisor">The divisor.</param>
	/// <param name="error">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The quotient.</returns>
	public Money Divide(decimal divisor, out decimal error) =>
		Divide(divisor, Context.RoundingMode, out error);

	/// <inheritdoc cref="Divide(decimal,System.MidpointRounding,out Bogoware.Money.Money)"/>
	public Money Divide(double divisor, MidpointRounding rounding, out decimal error)
	{
		decimal internalAmount = Math.Round(Amount / (decimal)divisor, Context.RoundingErrorDecimals, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Divide(decimal,out Bogoware.Money.Money)"/>
	public Money Divide(double divisor, out decimal error) => Divide(divisor, Context.RoundingMode, out error);
	
	public static Money operator /(Money left, decimal right)
	{
		var returnValue = left.Divide(right, out var residue);
		var errorRoundingOperation = new DivideOperation(residue, left.Currency);
		left.Context.AddErrorRoundingOperation(errorRoundingOperation);
		return returnValue;
	}

}