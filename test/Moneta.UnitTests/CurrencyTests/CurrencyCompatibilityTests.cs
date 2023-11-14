using Bogoware.Moneta.Abstractions;
using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta.UnitTests.CurrencyTests;

public class CurrencyCompatibilityTests : CurrencyBaseTests
{
	[Fact]
	public void Euro_and_Usd_are_not_compatible()
	{
		ICurrency.AreCompatible(Eur, Usd).Should().BeFalse();

		Eur.Invoking(x => ICurrency.MustBeCompatible(x, Usd))
			.Should().Throw<CurrencyIncompatibleException>();
	}

	[Fact]
	public void Euro_and_Euro_are_compatible()
	{
		ICurrency.AreCompatible(Eur, Eur).Should().BeTrue();

		Eur.Invoking(x => ICurrency.MustBeCompatible(x, Eur))
			.Should().NotThrow();
	}

	[Fact]
	public void Euro_and_undefined2digits_are_compatible()
	{
		ICurrency.AreCompatible(Eur, Undefined2digits).Should().BeTrue();

		Eur.Invoking(x => ICurrency.MustBeCompatible(x, Undefined2digits))
			.Should().NotThrow();
	}

	[Fact]
	public void Undefined2digits_and_undefined4digits_are_compatible()
	{
		ICurrency.AreCompatible(Undefined2digits, Undefined4digits).Should().BeTrue();

		Undefined2digits.Invoking(x => ICurrency.MustBeCompatible(x, Undefined4digits))
			.Should().NotThrow();
	}
}