namespace Bogoware.Money.UnitTests;

public class MoneySplitTests
{
	
	[Theory]
	[MemberData(nameof(GetSplitByPartsData))]
	public void Money_splitByNumberOfParts_works(
		MidpointRounding roundingMode,
		Currency currency,
		decimal amount,
		int numberOfParts,
		decimal expectedPartAmount,
		decimal expectedResidueAmount)
	{
		// Arrange
		var moneyContext = new MonetaryContext(currency, roundingMode);
		var moneyValue = moneyContext.NewMoney(amount);
		
		// Act
		var parts = moneyValue.Split(numberOfParts, out var residue);
		
		// Assert
		var expectedPart = new Money(expectedPartAmount, Currency.Undefined, moneyContext);
		
		parts.Should().HaveCount(numberOfParts);
		parts.Should().AllBeEquivalentTo(expectedPart);
		residue.Amount.Should().Be(expectedResidueAmount);
		residue.Currency.Should().Be(moneyValue.Currency);
	}

    public static IEnumerable<object[]> GetSplitByPartsData()
	{
		yield return new object[] { MidpointRounding.ToEven, Currency.Undefined, 1.10M, 2, 0.55M, 0.00M };
		yield return new object[] { MidpointRounding.ToEven, Currency.Undefined, 1.11M, 2, 0.56M, -0.01M };
		yield return new object[] { MidpointRounding.ToZero, Currency.Undefined, 1.19M, 2, 0.59M, 0.01M };
		yield return new object[] { MidpointRounding.ToPositiveInfinity, Currency.Undefined, 1.11M, 2, 0.56M, -0.01M };
	}
}