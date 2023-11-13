namespace Bogoware.Money.UnitTests.MoneyTests;

public class MoneyContextDoubleCastTests
{
	[Fact]
	void MoneyContext_safeDoubleCast_work()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		double originalValue = 1.12;
		
		// Act
		var money = moneyContext.NewMoney(originalValue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		moneyContext.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	void MoneyContext_unsafeDoubleCast_work()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		double originalValue = 1.123;
		
		// Act
		var money = moneyContext.NewMoney(originalValue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		moneyContext.HasRoundingErrors.Should().BeTrue();
		moneyContext.ErrorRoundingOperations.Should().HaveCount(1);
		var errorRoundingOperation = moneyContext.ErrorRoundingOperations[0];
		errorRoundingOperation.Should().BeOfType<ConvertFromDoubleOperation>();
		errorRoundingOperation.As<ConvertFromDoubleOperation>().Residue.Should().Be(0.003M);
	}
}