namespace Bogoware.Money;

/// <summary>
/// A monetary context. 
/// </summary>
public sealed class MonetaryContext
{
	/// <summary>
	/// The default <see cref="Currency"/> for monetary operations
	/// </summary>
	public Currency DefaultCurrency { get; }
	
	/// <summary>
	/// The default rounding mode for monetary operations
	/// belonging to the context.
	/// </summary>
	public MidpointRounding Rounding { get; }
	
	/// <summary>
	/// A list of residual amounts that have not been handled by the user.
	/// </summary>
	private List<Money> UnhandledResidualAmounts { get; }
	/// <summary>
	/// Signals that some rounding errors have occurred during operations without residual handling.
	/// </summary>
	public bool HasRoundingErrors => UnhandledResidualAmounts.Count > 0;

	public MonetaryContext(Currency defaultCurrency, MidpointRounding rounding)
	{
		DefaultCurrency = defaultCurrency;
		Rounding = rounding;
		UnhandledResidualAmounts = new();
	}
	
	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// </summary>
	public Money NewMoney(decimal amount) => new(amount, DefaultCurrency, this);
	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// </summary>
	public Money NewMoney(decimal amount, Currency currency) => new(amount, currency, this);
	
	internal void AddResidualAmount(Money money)
	{
		if (money.Amount == 0) return;
		UnhandledResidualAmounts.Add(money);
	}
}