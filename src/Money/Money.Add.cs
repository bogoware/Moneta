using System.Numerics;
using System.Runtime.CompilerServices;

// ReSharper disable MemberCanBePrivate.Global

namespace Bogoware.Money;

public partial class Money
{
	/// <summary>
	/// Add the specified amount to the money.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	public Money Add<T>(T amount, MidpointRounding rounding, out decimal error) where T : INumber<T>, IConvertible
	{
		ValidateType<T>();
		decimal decimalAmount = Convert.ToDecimal(amount);
		var internalAmount = Math.Round(Amount + decimalAmount, Context.RoundingErrorDecimals, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		error = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="M:Bogoware.Money.Money.Add``1(``0,System.MidpointRounding,System.Decimal@)"/>
	public Money Add<T>(T amount, out decimal error) where T : INumber<T>, IConvertible
	{
		var result = Add(amount, Context.RoundingMode, out error);
		return result;
	}

	/// <summary>
	/// Add the specified amount to the money.
	/// </summary>
	public Money Add<T>(T amount, MidpointRounding rounding) where T : INumber<T>, IConvertible
	{
		var result = Add(amount, rounding, out var residue);
		var errorRoundingOperation = new AddOperation(residue, Currency);
		Context.AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	/// <inheritdoc cref="M:Bogoware.Money.Money.Add``1(``0,System.MidpointRounding)"/>
	//[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
	public Money Add<T>(T amount) where T : INumber<T>, IConvertible
	{
		var result = Add(amount, Context.RoundingMode, out var residue);
		var errorRoundingOperation = new AddOperation(residue, Currency);
		Context.AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	public static Money operator +(Money left, Money right)
	{
		ValidateOperands(left, right);
		return new(left.Amount + right.Amount, left.Currency, left.Context);
	}

	public static Money operator +(Money left, decimal right) => left.Add(right);
	public static Money operator +(Money left, long right) => left.Add(right);
	public static Money operator +(Money left, int right) => left.Add(right);
	public static Money operator +(Money left, short right) => left.Add(right);
	public static Money operator +(Money left, ulong right) => left.Add(right);
	public static Money operator +(Money left, uint right) => left.Add(right);
	public static Money operator +(Money left, ushort right) => left.Add(right);
	public static Money operator +(Money left, double right) => left.Add(right);
	public static Money operator +(Money left, float right) => left.Add(right);
	public static Money operator +(decimal left, Money right) => right.Add(left);
	public static Money operator +(long left, Money right) => right.Add(left);
	public static Money operator +(int left, Money right) => right.Add(left);
	public static Money operator +(short left, Money right) => right.Add(left);
	public static Money operator +(ulong left, Money right) => right.Add(left);
	public static Money operator +(uint left, Money right) => right.Add(left);
	public static Money operator +(ushort left, Money right) => right.Add(left);
	public static Money operator +(double left, Money right) => right.Add(left);
	public static Money operator +(float left, Money right) => right.Add(left);
}