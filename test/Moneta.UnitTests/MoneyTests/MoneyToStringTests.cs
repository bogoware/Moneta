namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyToStringTests: MoneyBaseTests
{
	[Fact]
	public void Money_EuroToString_works()
	{
		// Arrange
		var moneyContext = new MonetaryContext(defaultCurrency: Euro);
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var result = sut.ToString();

		// Assert
		result.Should().Be("EUR 10.00");
	}
	
	[Fact]
	public void Money_UndefinedToString_works()
	{
		// Arrange
		var moneyContext = new MonetaryContext(defaultCurrency: UndefinedCurrencyWith4Digits);
		var sut = moneyContext.CreateMoney(10.1M);

		// Act
		var result = sut.ToString();

		// Assert
		result.Should().Be("XXX 10.1000");
	}
}