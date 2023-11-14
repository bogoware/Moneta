// ReSharper disable SuggestVarOrType_BuiltInTypes



// ReSharper disable MemberCanBePrivate.Global

namespace Bogoware.Moneta;

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
	/// The <see cref="MonetaContext"/> of the money.
	/// </summary>
	public MonetaContext Context { get; }

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance.
	/// </summary>
	/// <param name="amount"></param>
	/// <param name="currency"></param>
	/// <param name="context"></param>
	internal Money(decimal amount, ICurrency currency, MonetaContext context)
	{
		Amount = amount;
		Currency = currency;
		Context = context;
	}
}