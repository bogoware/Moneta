// ReSharper disable SuggestVarOrType_BuiltInTypes

using System.Globalization;
using System.Numerics;
// ReSharper disable MemberCanBePrivate.Global

namespace Bogoware.Money;

/// <summary>
/// An immutable monetary value.
/// </summary>
public partial class Money : IEquatable<Money>
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
}