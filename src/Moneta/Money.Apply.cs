namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Apply the specified function to the money amount and return the result.
	/// </summary>
	/// <param name="function">The function to apply</param>
	/// <param name="rounding"></param>
	/// <param name="error">The residual part after conversion to money decimal</param>
	public Money Apply<T>(Func<Money, T> function, MidpointRounding rounding, out decimal error)
		where T : INumber<T>, IConvertible
	{
		var rawNewAmount = function(this);
		var decimalNewAmount = ValidateAndGetDecimalValue(rawNewAmount);
		decimal internalAmount = Math.Round(decimalNewAmount, Context.RoundingErrorDecimals, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Apply the specified function to the money amount and return the result.
	/// </summary>
	/// <param name="function"></param>
	/// <param name="error"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Money Apply<T>(Func<Money, T> function, out decimal error) where T : INumber<T>, IConvertible =>
		Apply(function, Context.RoundingMode, out error);

	/// <summary>
	/// Apply the specified function to the money amount and return the result.
	/// This transformation is potentially unsafe, an <see cref="ApplyOperationError"/> could be added to the <see cref="MonetaContext"/>.
	/// </summary>
	/// <param name="function"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Money Apply<T>(Func<Money, T> function) where T : INumber<T>, IConvertible
	{
		var returnValue = Apply(function, out var error);
		var errorRoundingOperation = new ApplyOperationError(error, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return returnValue;
	}

	/// <summary>
	/// Apply the specified function to the money amount and return the result.
	/// </summary>
	/// <param name="function"></param>
	/// <param name="rounding"></param>
	/// <param name="error"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Money Apply<T>(Func<decimal, T> function, MidpointRounding rounding, out decimal error) where T : INumber<T>, IConvertible
	{
		var rawNewAmount = function(Amount);
		var decimalNewAmount = ValidateAndGetDecimalValue(rawNewAmount);
		decimal internalAmount = Math.Round(decimalNewAmount, Context.RoundingErrorDecimals, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}
	
	/// <summary>
	/// Apply the specified function to the money amount and return the result.
	/// </summary>
	/// <param name="function"></param>
	/// <param name="error"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	
	public Money Apply<T>(Func<decimal, T> function, out decimal error) where T : INumber<T>, IConvertible => Apply(function, Context.RoundingMode, out error);
	
	/// <summary>
	/// Apply the specified function to the money amount and return the result.
	/// This transformation is potentially unsafe, an <see cref="ApplyOperationError"/> could be added to the <see cref="MonetaContext"/>.
	/// </summary>
	/// <param name="function"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Money Apply<T>(Func<decimal, T> function) where T : INumber<T>, IConvertible
	{
		var returnValue = Apply(function, out var error);
		var errorRoundingOperation = new ApplyOperationError(error, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return returnValue;
	}
	
	/// <summary>
	/// Apply the specified function to the money amount and return a new <see cref="Money"/>, potentially with a different currency.
	/// </summary>
	/// <param name="function"></param>
	/// <returns></returns>
	public Money Apply(Func<Money, Money> function) => function(this);
}