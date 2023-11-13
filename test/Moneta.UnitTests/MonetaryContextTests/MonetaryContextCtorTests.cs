using Bogoware.Moneta.Abstractions;
using Bogoware.Moneta.CurrencyProviders;

namespace Bogoware.Moneta.UnitTests.MonetaryContextTests;

public class MonetaryContextCtorTests
{

	[Fact]
	public void MonetaryContext_voidCtor()
	{
		// Act
		var sut = new MonetaryContext();
		
		// Assert
		sut.DefaultCurrency.Should().Be(Currency.DefaultUndefined);
		sut.RoundingMode.Should().Be(MidpointRounding.ToEven);
		sut.RoundingErrorDecimals.Should().Be(8);
		sut.HasRoundingErrors.Should().BeFalse();
		sut.RoundingErrors.Should().BeEmpty();
	}

	private class FakeCurrency : Currency<FakeCurrency>
	{
		public static ICurrency Instance { get; } = new FakeCurrency();
		private FakeCurrency()
			: base("Fake", "Fake", "Fake", 2)
		{
		}
	}
	
	[Fact]
	public void MonetaryContext_customCtor()
	{
		// Act
		var currencyProvider = new IsoCurrencyProvider();
		var sut = new MonetaryContext(FakeCurrency.Instance, currencyProvider, MidpointRounding.AwayFromZero, 4);
		
		// Assert
		sut.DefaultCurrency.Should().Be(FakeCurrency.Instance);
		sut.CurrencyProvider.Should().Be(currencyProvider);
		sut.RoundingMode.Should().Be(MidpointRounding.AwayFromZero);
		sut.RoundingErrorDecimals.Should().Be(4);
		sut.HasRoundingErrors.Should().BeFalse();
		sut.RoundingErrors.Should().BeEmpty();
	}
}