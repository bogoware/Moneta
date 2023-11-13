// ReSharper disable SuggestVarOrType_BuiltInTypes

using System.Globalization;
using System.Numerics;

namespace Bogoware.Money;

/// <summary>
/// An immutable monetary value.
/// </summary>
public class Money : IEquatable<Money>
{
	/// <summary>
	/// The amount of money.
	/// </summary>
	public decimal Amount { get; }

	/// <summary>
	/// The <see cref="Currency"/> of the money.
	/// </summary>
	public ICurrency Currency { get; }

	/// <summary>
	/// The <see cref="MonetaryContext"/> of the money.
	/// </summary>
	public MonetaryContext Context { get; }

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance.
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="currency"></param>
	/// <param name="context"></param>
	internal Money(decimal amount, ICurrency currency, MonetaryContext context)
	{
		Amount = amount;
		Currency = currency;
		Context = context;
	}

	#region Split

	/// <summary>
	/// Split the money into the specified number of parts.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding, out decimal residue)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfParts);
		var internalAmount = Math.Round(Amount / numberOfParts, Context.OperationDecimalPlaces, rounding);
		var partAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		var partMoney = new Money(partAmount, Currency, Context);
		var parts = Enumerable.Repeat(partMoney, numberOfParts).ToList();
		residue = (internalAmount - partAmount) * numberOfParts;
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, out decimal residue) =>
		Split(numberOfParts, Context.RoundingMode, out residue);

	/// <summary>
	/// Split the money into the specified number of parts.
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding)
	{
		var parts = Split(numberOfParts, rounding, out var residue);
		var errorRoundingOperation = new SplitOperation(residue, Currency);
		Context.AddErrorRoundingOperation(errorRoundingOperation);
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts) => Split(numberOfParts, Context.RoundingMode);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, MidpointRounding rounding, out decimal residue)
		where T : INumber<T>, IConvertible
	{
		// ReSharper disable PossibleMultipleEnumeration
		// ReSharper disable LoopCanBeConvertedToQuery
		ValidateType<T>();
		ValidateWeights(weights);

		var parts = new List<Money>(weights.Count());
		var totalWeight = T.Zero;
		foreach (var weight in weights)
		{
			totalWeight += weight;
		}

		var decimalTotalWeight = Convert.ToDecimal(totalWeight);
		
		residue = 0;
		foreach (var weight in weights)
		{
			var decimalWeight = Convert.ToDecimal(weight);
			var proportion = decimalWeight / decimalTotalWeight;
			var amount = Math.Round(Amount * proportion, Currency.DecimalPlaces, rounding);
			residue += -amount;
			parts.Add(new(amount, Currency, Context));
		}

		residue += Amount;
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, out decimal residue) where T : INumber<T>, IConvertible =>
		Split(weights, Context.RoundingMode, out residue);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, MidpointRounding rounding) where T : INumber<T>, IConvertible
	{
		var parts = Split(weights, rounding, out var residue);
		var errorRoundingOperation = new SplitOperation(residue, Currency);
		Context.AddErrorRoundingOperation(errorRoundingOperation);
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights) where T : INumber<T>, IConvertible =>
		Split(weights, Context.RoundingMode);

	#endregion Split

	#region Add

	/// <summary>
	/// Add the specified amount to the money.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount to add.</param>
	/// <param name="rounding">The rounding mode to use.</param> 
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The product.</returns> 
	public Money Add<T>(T amount, MidpointRounding rounding, out decimal residue) where T : INumber<T>, IConvertible
	{
		ValidateType<T>();
		decimal decimalAmount = Convert.ToDecimal(amount);
		var internalAmount = Math.Round(Amount + decimalAmount, Context.OperationDecimalPlaces, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Add the specified amount to the money using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount to add.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The product.</returns>
	public Money Add<T>(T amount, out decimal residue) where T : INumber<T>, IConvertible
	{
		var result = Add(amount, Context.RoundingMode, out residue);
		return result;
	}

	#endregion Add

	#region Subtract

	/// <summary>
	/// Subtract the specified amount to the money.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount to subtract.</param>
	/// <param name="rounding">The rounding mode to use.</param> 
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The difference.</returns> 
	public Money Subtract(decimal amount, MidpointRounding rounding, out decimal residue)
	{
		var internalAmount = Math.Round(Amount - amount, Context.OperationDecimalPlaces, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Subtract the specified amount to the money using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount to subtract.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The difference.</returns>
	public Money Subtract(decimal amount, out decimal residue) => Subtract(amount, Context.RoundingMode, out residue);

	/// <inheritdoc cref="Subtract(decimal,System.MidpointRounding,out Bogoware.Money.Money)"/>
	public Money Subtract(double amount, MidpointRounding rounding, out decimal residue)
	{
		decimal internalAmount = Math.Round(Amount - (decimal)amount, Context.OperationDecimalPlaces, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Subtract(decimal,out Bogoware.Money.Money)"/>
	public Money Subtract(double amount, out decimal residue) => Subtract(amount, Context.RoundingMode, out residue);

	#endregion Subtract

	#region Divide

	/// <summary>
	/// Divide the money into the specified number of parts.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>.
	/// </summary>
	/// <param name="divisor">The divisor.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The quotient.</returns>
	public Money Divide(decimal divisor, MidpointRounding rounding, out decimal residue)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(divisor);
		decimal internalAmount = Math.Round(Amount / divisor, Context.OperationDecimalPlaces, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Divide the money into the specified number of parts using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="divisor">The divisor.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The quotient.</returns>
	public Money Divide(decimal divisor, out decimal residue) =>
		Divide(divisor, Context.RoundingMode, out residue);

	/// <inheritdoc cref="Divide(decimal,System.MidpointRounding,out Bogoware.Money.Money)"/>
	public Money Divide(double divisor, MidpointRounding rounding, out decimal residue)
	{
		decimal internalAmount = Math.Round(Amount / (decimal)divisor, Context.OperationDecimalPlaces, rounding);
		decimal newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Divide(decimal,out Bogoware.Money.Money)"/>
	public Money Divide(double divisor, out decimal residue) => Divide(divisor, Context.RoundingMode, out residue);

	#endregion Divide

	#region Multiply

	/// <summary>
	/// Multiply the money by the specified multiplier.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="multiplier">The multiplier.</param>
	/// <param name="rounding">The rounding mode to use.</param> 
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The product.</returns> 
	public Money Multiply(decimal multiplier, MidpointRounding rounding, out decimal residue)
	{
		var internalAmount = Math.Round(Amount * multiplier, Context.OperationDecimalPlaces, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <summary>
	/// Multiply the money by the specified multiplier using the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="multiplier">The multiplier.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The product.</returns>
	public Money Multiply(decimal multiplier, out decimal residue) =>
		Multiply(multiplier, Context.RoundingMode, out residue);

	/// <inheritdoc cref="Multiply(decimal,System.MidpointRounding,out Bogoware.Money.Money)"/>
	public Money Multiply(double multiplier, MidpointRounding rounding, out decimal residue)
	{
		decimal internalAmount = Math.Round(Amount * (decimal)multiplier, Context.OperationDecimalPlaces, rounding);
		var newAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		residue = internalAmount - newAmount;
		return new(newAmount, Currency, Context);
	}

	/// <inheritdoc cref="Multiply(decimal,out Bogoware.Money.Money)"/>
	public Money Multiply(double multiplier, out decimal residue) =>
		Multiply(multiplier, Context.RoundingMode, out residue);

	#endregion Multiply
	
	#region Apply & Map
	
	/// <summary>
	/// Apply the specified functor to the money and return the result.
	/// </summary>
	/// <param name="functor">The function to apply</param>
	/// <param name="residue">The residual part after conversion to money decimal</param>
	public Money Apply<T>(Func<Money, T> functor, out decimal residue) where T: INumber<T>, IConvertible {
		ValidateType<T>();
		var rawNewAmount = functor(this);
		var decimalNewAmount = Convert.ToDecimal(rawNewAmount);
		var newAmount = Math.Round(decimalNewAmount, Currency.DecimalPlaces, Context.RoundingMode);
		residue = decimalNewAmount - newAmount;
		return new(newAmount, Currency, Context);
	}
	
	/// <inheritdoc cref="Apply{T}(System.Func{Bogoware.Money.Money,T},out decimal)"/>
	public Money Apply<T>(Func<decimal, T> functor, out decimal residue) where T: INumber<T>, IConvertible {
		ValidateType<T>();
		var rawNewAmount = functor(this.Amount);
		var decimalNewAmount = Convert.ToDecimal(rawNewAmount);
		var newAmount = Math.Round(decimalNewAmount, Currency.DecimalPlaces, Context.RoundingMode);
		residue = decimalNewAmount - newAmount;
		return new(newAmount, Currency, Context);
	}
	
	/// <summary>
	/// Map the specified functor to the money and return the result.
	/// </summary>
	/// <param name="functor"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Money Map<T>(Func<Money, T> functor) where T : INumber<T>, IConvertible
	{
		var result = Apply(functor, out var residue);
		Context.AddErrorRoundingOperation(new MapOperation(residue, Currency));
		return result;
	}
	
	/// <inheritdoc cref="Map{T}(System.Func{Bogoware.Money.Money,T})"/>
	public Money Map<T>(Func<decimal, T> functor) where T : INumber<T>, IConvertible
	{
		var result = Apply(functor, out var residue);
		Context.AddErrorRoundingOperation(new MapOperation(residue, Currency));
		return result;
	}

	#endregion Apply & Map
	
	#region Validation Helpers

	internal static void ValidateType<T>() where T : INumber<T>, IConvertible
	{
		try
		{
			decimal d = Convert.ToDecimal(T.Zero);
		}
		catch (InvalidCastException)
		{
			throw new InvalidCastException($"The type {typeof(T)} cannot be converted to decimal.");
		}
	}

	private static void ValidateWeights<T>(IEnumerable<T> weights) where T : INumber<T>
	{
		// all weights must be positive
		if (weights.Any(w => T.IsZero(w) || T.IsNegative(w)))
		{
			throw new ArgumentOutOfRangeException(nameof(weights), "All weights must be positive.");
		}
	}


	private static void ValidateOperands(Money left, Money right)
	{
		if (left.Currency.IsNeutral
		    || right.Currency.IsNeutral
		    || left.Currency == right.Currency) return;

		throw new InvalidOperationException("Cannot operate on money with different currencies.");
	}
	
	#endregion Validation Helpers

	#region Operators
	public static Money operator +(Money left, Money right)
	{
		ValidateOperands(left, right);
		return new(left.Amount + right.Amount, left.Currency, left.Context);
	}

	public static Money operator +(Money left, decimal right)
	{
		var result = left.Add(right, out var residue);
		var errorRoundingOperation = new AddOperation(residue, left.Currency);
		left.Context.AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	public static Money operator +(decimal left, Money right)
	{
		var result = right.Add(left, out var residue);
		var errorRoundingOperation = new AddOperation(residue, right.Currency);
		right.Context.AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	public static Money operator +(Money left, double right)
	{
		var result = left.Add(right, out var residue);
		var errorRoundingOperation = new AddOperation(residue, left.Currency);
		left.Context.AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	public static Money operator +(double left, Money right)
	{
		var result = right.Add(left, out var residue);
		var errorRoundingOperation = new AddOperation(residue, right.Currency);
		right.Context.AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}


	public static Money operator -(Money left, Money right)
	{
		ValidateOperands(left, right);
		return new(left.Amount - right.Amount, left.Currency, left.Context);
	}

	public static Money operator -(Money left, decimal right) => new(left.Amount - right, left.Currency, left.Context);
	public static Money operator -(Money left, double right) => left - left.Context.NewMoney(right, left.Currency);
	public static Money operator *(Money left, decimal right) => new(left.Amount * right, left.Currency, left.Context);

	public static Money operator *(decimal left, Money right) =>
		new(left * right.Amount, right.Currency, right.Context);

	public static Money operator /(Money left, decimal right)
	{
		var returnValue = left.Divide(right, out var residue);
		var errorRoundingOperation = new DivideOperation(residue, left.Currency);
		left.Context.AddErrorRoundingOperation(errorRoundingOperation);
		return returnValue;
	}
	
	#endregion Operators

	#region Equality

	public bool Equals(Money? other)
	{
		if (ReferenceEquals(null, other)) return false;
		if (ReferenceEquals(this, other)) return true;
		return Amount == other.Amount && Currency.Equals(other.Currency);
	}

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(null, obj)) return false;
		if (ReferenceEquals(this, obj)) return true;
		return obj.GetType() == GetType() && Equals((Money)obj);
	}

	public override int GetHashCode() => HashCode.Combine(Amount, Currency);

	public static bool operator ==(Money? left, Money? right) => Equals(left, right);

	public static bool operator !=(Money? left, Money? right) => !Equals(left, right);

	#endregion Equality
	
	#region ToString
	public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0} {1}", Currency, Amount);

	#endregion ToString
}