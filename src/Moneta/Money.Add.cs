

// ReSharper disable MemberCanBePrivate.Global

using static System.Math;

namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Add the number to the <see cref="Money"/>.
	/// </summary>
	public Money Add<T>(T amount, MidpointRounding rounding, out decimal error) where T : INumber<T>, IConvertible
	{
		var decimalAmount = ValidateAndGetDecimalValue(amount);
		var internalAmount = Round(Amount + decimalAmount, Context.RoundingErrorDecimals, rounding);
		var newAmount = Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Add the two <see cref="Money"/> instances and return a new <see cref="Money"/> instance
	/// with the the most specific currency between the two.
	/// </summary>
	/// <seealso cref="M:Bogoware.Moneta.Abstractions.ICurrency.GetMostSpecificCurrency(Bogoware.Moneta.Abstractions.ICurrency,Bogoware.Moneta.Abstractions.ICurrency,Bogoware.Moneta.Abstractions.ICurrency@,Bogoware.Moneta.Abstractions.ICurrency@)"/>
	/// <exception cref="CurrencyIncompatibleException">Thrown when the two currencies are not compatible.</exception>
	public Money Add(Money other, MidpointRounding rounding, out decimal error)
	{
		ICurrency.MustBeCompatible(Currency, other.Currency);
		
		return Add(other.Amount, rounding, out error);
	}


	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Add(Bogoware.Moneta.Money,System.MidpointRounding,System.Decimal@)"/>
	public Money Add(Money other, out decimal error) => Add(other, Context.RoundingMode, out error);

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Add(Bogoware.Moneta.Money,System.MidpointRounding,System.Decimal@)"/>
	public Money Add(Money other)
	{
		var result = Add(other, Context.RoundingMode, out var error);
		var roundingErrorOperation = new AddOperation(error, Currency);
		Context.AddRoundingErrorOperation(roundingErrorOperation);
		return result;
	}

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Add``1(``0,System.MidpointRounding,System.Decimal@)"/>
	public Money Add<T>(T amount, out decimal error) where T : INumber<T>, IConvertible =>
		Add(amount, Context.RoundingMode, out error);

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Add(Bogoware.Moneta.Money,System.Decimal@)"/>
	public Money Add<T>(T amount, MidpointRounding rounding) where T : INumber<T>, IConvertible
	{
		var result = Add(amount, rounding, out var residue);
		var errorRoundingOperation = new AddOperation(residue, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return result;
	}

	/// <inheritdoc cref="M:Bogoware.Moneta.Money.Add``1(``0,System.MidpointRounding,System.Decimal@)"/>
	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public Money Add<T>(T amount) where T : INumber<T>, IConvertible
	{
		var result = Add(amount, Context.RoundingMode, out var residue);
		var errorRoundingOperation = new AddOperation(residue, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return result;
	}

	public static Money operator +(Money left, Money right) => left.Add(right);
	public static Money operator +(Money left, decimal right) => left.Add(right);
	public static Money operator +(Money left, long right) => left.Add(right);
	public static Money operator +(Money left, int right) => left.Add(right);
	public static Money operator +(Money left, short right) => left.Add(right);
	public static Money operator +(Money left, ulong right) => left.Add(right);
	public static Money operator +(Money left, uint right) => left.Add(right);
	public static Money operator +(Money left, ushort right) => left.Add(right);
	public static Money operator +(Money left, double right) => left.Add(right);
	public static Money operator +(Money left, float right) => left.Add(right);
}