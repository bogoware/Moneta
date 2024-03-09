using Bogoware.Moneta.CommonCurrencies;

namespace Bogoware.Moneta.UnitTests.MonetaContextTests;

public class MonetaContextCommonCurrenciesExtensionsTests
{
	[Fact]
	public void Dollar_createsMoney_with_DollarCurrency()
	{
		// Arrange
		var sut = new MonetaContext();
		var originalValue = 1.12m;
		
		// Act
		var money = sut.Dollar(originalValue);
		
		// Assert
		money.Currency.Should().Be(Dollar.Instance);
	}
	
	[Fact]
	public void Euro_createsMoney_with_EuroCurrency()
	{
		// Arrange
		var sut = new MonetaContext();
		var originalValue = 1.12m;
		
		// Act
		var money = sut.Euro(originalValue);
		
		// Assert
		money.Currency.Should().Be(Euro.Instance);
	}
	
	[Fact]
	public void Yuan_createsMoney_with_YuanCurrency()
	{
		// Arrange
		var sut = new MonetaContext();
		var originalValue = 1.12m;
		
		// Act
		var money = sut.Yuan(originalValue);
		
		// Assert
		money.Currency.Should().Be(Yuan.Instance);
	}
	
	[Fact]
	public void Yen_createsMoney_with_YenCurrency()
	{
		// Arrange
		var sut = new MonetaContext();
		var originalValue = 1.12m;
		
		// Act
		var money = sut.Yen(originalValue);
		
		// Assert
		money.Currency.Should().Be(Yen.Instance);
	}
	
	[Fact]
	public void PoundSterling_createsMoney_with_PoundSterlingCurrency()
	{
		// Arrange
		var sut = new MonetaContext();
		var originalValue = 1.12m;
		
		// Act
		var money = sut.PoundSterling(originalValue);
		
		// Assert
		money.Currency.Should().Be(PoundSterling.Instance);
	}
}