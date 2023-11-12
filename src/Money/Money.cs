namespace Bogoware.Money;

/// <summary>
/// A monetary value with a <see cref="Currency"/> and <see cref="MonetaryContext"/>.
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
	public Currency Currency { get; }

	/// <summary>
	/// The <see cref="MonetaryContext"/> of the money.
	/// </summary>
	public MonetaryContext Context { get; }

	public Money(decimal amount, Currency currency, MonetaryContext context)
	{
		Amount = amount;
		Currency = currency;
		Context = context;
	}

	/// <summary>
	/// Split the money into the specified number of parts.
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding, out Money residue)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfParts);
		var partAmount = Math.Round(Amount / numberOfParts, Currency.DecimalPlaces, rounding);
		var partMoney = new Money(partAmount, Currency, Context);
		var parts = Enumerable.Repeat(partMoney, numberOfParts).ToList();
		var residueAmount = Amount - (partAmount * numberOfParts);
		residue = new(residueAmount, Currency, Context);
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public List<Money> Split(int numberOfParts, out Money residue) =>
		Split(numberOfParts, Context.Rounding, out residue);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public List<Money> Split(IEnumerable<int> weights, MidpointRounding rounding, out Money residue)
	{
		// ReSharper disable PossibleMultipleEnumeration
		// ReSharper disable LoopCanBeConvertedToQuery
		ValidateWeights(weights);
		
		var parts = new List<Money>(weights.Count());
		var totalWeight = weights.Sum();
		foreach (var weight in weights)
		{
			var partAmount = Math.Round(Amount * weight / totalWeight, Currency.DecimalPlaces, rounding);
			parts.Add(new(partAmount, Currency, Context));
		}
		var partsAmount = parts.Sum(p => p.Amount);
		var residueAmount = Amount - partsAmount;
		residue = new(residueAmount, Currency, Context);
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	/// <exception cref="NotImplementedException"></exception>
	public List<Money> Split(IEnumerable<int> weights, out Money residue) =>
		Split(weights, Context.Rounding, out residue);

	/// <summary>
	/// Divide the money into the specified number of parts.
	/// </summary>
	/// <param name="divisor">The divisor.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The quotient.</returns>
	public Money Divide(decimal divisor, MidpointRounding rounding, out Money residue)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(divisor);
		var quotient = Math.Round(Amount / divisor, Currency.DecimalPlaces, rounding);
		var residueAmount = Amount - (quotient * divisor);
		residue = new(residueAmount, Currency, Context);
		return new(quotient, Currency, Context);
	}

	/// <summary>
	/// Divide the money into the specified number of parts using the <see cref="Context"/>'s rounding mode.
	/// </summary>
	/// <param name="divisor">The divisor.</param>
	/// <param name="residue">The cumulative residual part after the division. This value can be positive or negative.</param>
	/// <returns>The quotient.</returns>
	public Money Divide(decimal divisor, out Money residue) =>
		Divide(divisor, Context.Rounding, out residue);

	private static void ValidateWeights(IEnumerable<int> weights)
	{
		// all weights must be positive
		if (weights.Any(w => w <= 0))
		{
			throw new ArgumentOutOfRangeException(nameof(weights), "All weights must be positive.");
		}
	}
	
	private static void ValidateOperands(Money left, Money right)
	{
		if (left.Currency.IsNeutral || right.Currency.IsNeutral)
		{
			return;
		}
		if (left.Currency != right.Currency)
		{
			throw new InvalidOperationException("Cannot operate on money with different currencies.");
		}
	}
	
	public static Money operator +(Money left, Money right)
	{
		ValidateOperands(left, right);
		return new(left.Amount + right.Amount, left.Currency, left.Context);
	}
	public static Money operator +(Money left, decimal right) => new(left.Amount + right, left.Currency, left.Context);
	public static Money operator +(decimal left, Money right) => new(left + right.Amount, right.Currency, right.Context);
	public static Money operator -(Money left, Money right)
	{
		ValidateOperands(left, right);
		return new(left.Amount - right.Amount, left.Currency, left.Context);
	}
	public static Money operator -(Money left, decimal right) => new(left.Amount - right, left.Currency, left.Context);
	
	public static Money operator *(Money left, decimal right) => new(left.Amount * right, left.Currency, left.Context);
	public static Money operator *(decimal left, Money right) => new(left * right.Amount, right.Currency, right.Context);
	public static Money operator /(Money left, decimal right)
	{
		var returnValue = left.Divide(right, out var residue);
		left.Context.AddDivisionErrorRounding(residue);
		return returnValue;
	}

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
}