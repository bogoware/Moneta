using Bogoware.Moneta.Abstractions;
using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta.UnitTests.CurrencyTests;

public class CurrencyMostSpecificTests : CurrencyBaseTests
{
	[Fact]
	public void Euro_and_Usd_are_not_comparable_therefore_mostSpecific_cannotBeInvoked()
	{
		Eur.Invoking(eur => ICurrency.GetMostSpecificCurrency(eur, Usd, out _, out _))
			.Should().Throw<CurrencyIncompatibleException>();
	}
	
	[Fact]
	public void Euro_is_more_specific_than_undefined2digits()
	{
		ICurrency.GetMostSpecificCurrency(Eur, Undefined, out var lessSpecific, out var mostSpecific);
		
		lessSpecific.Should().Be(Undefined);
		mostSpecific.Should().Be(Eur);
	}
	
}