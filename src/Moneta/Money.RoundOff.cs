using System.Diagnostics;
using System.Globalization;
using static System.Math;

namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Round off the amount to the nearest round-off unit.
	/// Round-off unit is the smallest unit of currency that is used in transactions.
	/// For example, some Euro countries use 0.05 EUR as round-ff unit for cash transactions.
	/// Other countries that adopt currency without decimal places, are used to round-off their final monetary values
	/// to multiple of 5 units of their currency.
	/// </summary>
	/// <param name="roundOffUnit"></param>
	/// <param name="rounding"></param>
	/// <param name="unallocated"></param>
	/// <returns></returns>
	public Money RoundOff(Money roundOffUnit, MidpointRounding rounding, out Money unallocated)
	{
		ICurrency.MustBeCompatible(Currency, roundOffUnit.Currency);
		ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(roundOffUnit.Amount, 0);
		
		var invSubunit = 1 / roundOffUnit.Amount;
		
		var amount = Round(Amount * invSubunit, rounding) / invSubunit;
		
		var errorAmount = Amount - amount;
		
		unallocated = new (errorAmount, Currency, Context);
		return new (amount, Currency, Context);
	}
	
	/// <inheritdoc cref="RoundOff(Bogoware.Moneta.Money,System.MidpointRounding,out Bogoware.Moneta.Money)"/>
	public Money RoundOff(Money roundOffUnit, MidpointRounding rounding)
	{
		var result = RoundOff(roundOffUnit, rounding, out var error);
		var roundingErrorOperation = new RoundOffOperation(error);
		Context.AddRoundingErrorOperation(roundingErrorOperation);
		return result;
	}

	/// <inheritdoc cref="RoundOff(Bogoware.Moneta.Money,System.MidpointRounding,out Bogoware.Moneta.Money)"/>
	public Money RoundOff(Money roundOffUnit, out Money unallocated) =>
		RoundOff(roundOffUnit, Context.RoundingMode, out unallocated);

	/// <inheritdoc cref="RoundOff(Bogoware.Moneta.Money,System.MidpointRounding,out Bogoware.Moneta.Money)"/>
	public Money RoundOff(Money roundOffUnit)
	{
		var result = RoundOff(roundOffUnit, Context.RoundingMode, out var error);
		var roundingErrorOperation = new RoundOffOperation(error);
		Context.AddRoundingErrorOperation(roundingErrorOperation);
		return result;
	}
}