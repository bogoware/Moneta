using Bogoware.Moneta.Abstractions;
using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta.UnitTests.CurrencyTests;

public class CurrencyMostSpecificTests : CurrencyBaseTests
{
	[Fact]
	public void Euro_and_Usd_are_not_comparable()
	{
		ICurrency.AreCompatible(Eur, Usd).Should().BeFalse();
		this.Invoking(_ => ICurrency.MustBeCompatible(Eur, Usd))
			.Should().Throw<CurrencyIncompatibleException>();
	}

	[Fact]
	public void AnyCurrency_isCompatible_withItself()
	{
		ICurrency.AreCompatible(Eur, Eur).Should().BeTrue();
		ICurrency.MustBeCompatible(Usd, Usd);
	}
	
}