using Bogoware.Moneta.Abstractions;
using Bogoware.Moneta.CurrencyProviders;
using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta.UnitTests.MonetaryContextTests;

public class MonetaryContextCtorTests
{

	[Fact]
	public void MonetaryContext_voidCtor()
	{
		// Act
		var sut = new MonetaContext();
		
		// Assert
		sut.DefaultCurrency.Should().Be(new UndefinedCurrency());
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
		var sut = new MonetaContext(FakeCurrency.Instance, currencyProvider, MidpointRounding.AwayFromZero, 4);
		
		// Assert
		sut.DefaultCurrency.Should().Be(FakeCurrency.Instance);
		sut.CurrencyProvider.Should().Be(currencyProvider);
		sut.RoundingMode.Should().Be(MidpointRounding.AwayFromZero);
		sut.RoundingErrorDecimals.Should().Be(4);
		sut.HasRoundingErrors.Should().BeFalse();
		sut.RoundingErrors.Should().BeEmpty();
	}

	[Fact]
	public void MonetaryContext_cannotCreateMoney_withMoreDecimalPlaces_thanTheRoundingErrorDecimals()
	{
		// Arrange
		var currency = new Currency("CUR", "Currency", "C", 10);
		var sut = new MonetaContext(roundingErrorDecimals: 4);
		
		// Act
		sut
			.Invoking(s => s.CreateMoney(1, currency))
			.Should().Throw<MonetaryContextInvalidConfigurationException>()
			.WithMessage("The currency CUR has more decimal places (10) than the rounding error decimals (4).");
	}
}