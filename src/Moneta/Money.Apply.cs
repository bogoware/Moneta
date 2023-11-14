namespace Bogoware.Moneta;

public partial class Money
{
	#region Apply & Map

	/// <summary>
	/// Apply the specified functor to the money and return the result.
	/// </summary>
	/// <param name="functor">The function to apply</param>
	/// <param name="error">The residual part after conversion to money decimal</param>
	public Money Apply<T>(Func<Money, T> functor, out decimal error) where T : INumber<T>, IConvertible
	{
		ValidateType<T>();
		var rawNewAmount = functor(this);
		var decimalNewAmount = Convert.ToDecimal(rawNewAmount);
		var newAmount = Math.Round(decimalNewAmount, Currency.DecimalPlaces, Context.RoundingMode);
		error = decimalNewAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Apply{T}(System.Func{Bogoware.Moneta.Money,T},out decimal)"/>
	public Money Apply<T>(Func<decimal, T> functor, out decimal error) where T : INumber<T>, IConvertible
	{
		ValidateType<T>();
		var rawNewAmount = functor(this.Amount);
		var decimalNewAmount = Convert.ToDecimal(rawNewAmount);
		var newAmount = Math.Round(decimalNewAmount, Currency.DecimalPlaces, Context.RoundingMode);
		error = decimalNewAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	#endregion Apply & Map
}