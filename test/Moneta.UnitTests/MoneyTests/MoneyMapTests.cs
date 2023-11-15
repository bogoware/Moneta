namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyMapTests
{
	[Fact]
	public void Map_withMoney_worksWithoutResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var result = sut.Map(MoneyApplyTests.SafeMoneyTransformation);

		// Assert
		result.Amount.Should().Be(20.23M);
		moneyContext.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void Map_withDecimal_worksWithoutResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var result = sut.Map(MoneyApplyTests.SafeDecimalTransformation);

		// Assert
		result.Amount.Should().Be(20.23M);
		moneyContext.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void Map_withMoney_worksWithResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(1M);

		// Act
		var result = sut.Map(MoneyApplyTests.UnsafeMoneyTransformation);

		// Assert
		result.Amount.Should().Be(0.8415M);
		moneyContext.HasRoundingErrors.Should().BeTrue();
	}
	
	[Fact]
	public void Map_withDecimal_worksWithResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(1M);

		// Act
		var result = sut.Map(MoneyApplyTests.UnsafeDecimalTransformation);

		// Assert
		result.Amount.Should().Be(0.8415M);
		moneyContext.HasRoundingErrors.Should().BeTrue();
	}
}