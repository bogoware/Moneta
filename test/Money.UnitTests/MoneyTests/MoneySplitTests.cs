namespace Bogoware.Money.UnitTests.MoneyTests;

public class MoneySplitTests
{
	[Theory]
	[MemberData(nameof(GetSplitByPartsData))]
	public void Money_splitByNumberOfParts_works(
		MidpointRounding roundingMode,
		decimal amount,
		int numberOfParts,
		decimal expectedPartAmount,
		decimal expectedResidueAmount)
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, roundingMode);
		var sut = moneyContext.NewMoney(amount);

		// Act
		var parts = sut.Split(numberOfParts, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		var expectedPart = moneyContext.NewMoney(expectedPartAmount, Currency.Undefined);

		parts.Should().HaveCount(numberOfParts);
		parts.Should().AllBeEquivalentTo(expectedPart);
		residue.Should().Be(expectedResidueAmount);
		(totalAmount + residue).Should().Be(amount);
	}

	[Fact]
	public void Money_splitByNumberOfParts_worksWithPerfectSplit()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var sut = moneyContext.NewMoney(10M);

		// Act
		var parts = sut.Split(100, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(100);
		residue.Should().Be(0M);
		(totalAmount + residue).Should().Be(10M);
	}

	[Fact]
	public void Money_splitByNumberOfParts_worksWithResidue()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var sut = moneyContext.NewMoney(10.01M);

		// Act
		var parts = sut.Split(100, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(100);
		residue.Should().Be(0.01M);
		(totalAmount + residue).Should().Be(10.01M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithPerfectSplit_01()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var sut = moneyContext.NewMoney(10M);

		// Act
		var parts = sut.Split(Enumerable.Repeat(1, 10), out var residue);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(10);
		parts.Should().AllBeEquivalentTo(moneyContext.NewMoney(1M));
		residue.Should().Be(0M);
		(totalAmount + residue).Should().Be(10M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithPerfectSplit_02()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var weights = new[] { 2, 2, 1 };
		var sut = moneyContext.NewMoney(10M);

		// Act
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.NewMoney(4M), 
			moneyContext.NewMoney(4M), 
			moneyContext.NewMoney(2M)
		});
		residue.Should().Be(0M);
		(totalAmount + residue).Should().Be(10M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithPerfectSplit_03()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var weights = new[] { 3, 2, 1 };
		var sut = moneyContext.NewMoney(10M);

		// Act
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.NewMoney(5.00M), 
			moneyContext.NewMoney(3.33M), 
			moneyContext.NewMoney(1.67M)
		});
		residue.Should().Be(0M);
		(totalAmount + residue).Should().Be(10M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithResidue_01()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var sut = moneyContext.NewMoney(10.01M);

		// Act
		var parts = sut.Split(Enumerable.Repeat(1, 10), out var residue);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(10);
		parts.Should().AllBeEquivalentTo(moneyContext.NewMoney(1.00M));
		residue.Should().Be(0.01M);
		(totalAmount + residue).Should().Be(10.01M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithResidue_02()
	{
		// Arrange
		var moneyContext = new MonetaryContext(Currency.Undefined, MidpointRounding.ToEven);
		var weights = new[] { 177, 53, 13 };
		var sut = moneyContext.NewMoney(10M);

		// Act
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.NewMoney(7.28M), // 177 / (177 + 53 + 13) * 10 rounded to 2 decimal places
			moneyContext.NewMoney(2.18M), //  53 / (177 + 53 + 13) * 10 rounded to 2 decimal places
			moneyContext.NewMoney(0.53M)  //  13 / (177 + 53 + 13) * 10 rounded to 2 decimal places
		});
		residue.Should().Be(0.01000000M);
		(totalAmount + residue).Should().Be(10M);
	}

	[Fact]
	public void Money_splitByFloatWeights_worksWithPerfectSplit_01()
	{
		// Arrange
		var moneyContext = new MonetaryContext();
		var sut = moneyContext.NewMoney(10M);
		
		// Act
		var weights = new[] { 1.0f, 1.0f };
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(2);
		parts.Should().AllBeEquivalentTo(moneyContext.NewMoney(5M));
		residue.Should().Be(0M);
		(totalAmount + residue).Should().Be(10M);
	}
	
	[Fact]
	public void Money_splitByFloatWeights_worksWithPerfectSplit_02()
	{
		// Arrange
		var moneyContext = new MonetaryContext();
		var sut = moneyContext.NewMoney(10M);
		
		// Act
		var weights = new[] { 1.25f, 1.50f, 1.75f };
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.NewMoney(2.78M), // 1.25 / (1.25 + 1.50 + 1.75) * 10 rounded to 2 decimal places
			moneyContext.NewMoney(3.33M), // 1.50 / (1.25 + 1.50 + 1.75) * 10 rounded to 2 decimal places
			moneyContext.NewMoney(3.89M)  // 1.75 / (1.25 + 1.50 + 1.75) * 10 rounded to 2 decimal places
		});
		residue.Should().Be(0M);
		(totalAmount + residue).Should().Be(10M);
	}

	[Fact]
	public void Money_splitByFloatWeights_worksWithResidue_01()
	{
		// Arrange
		var moneyContext = new MonetaryContext();
		var sut = moneyContext.NewMoney(10.01M);
		
		// Act
		var weights = new[] { 1.0f, 1.0f };
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(2);
		parts.Should().AllBeEquivalentTo(moneyContext.NewMoney(5.00M));
		residue.Should().Be(0.01M);
		(totalAmount + residue).Should().Be(10.01M);
	}

	[Fact]
	public void Money_splitByFloatWeights_worksWithResidue_02()
	{
		// Arrange
		var moneyContext = new MonetaryContext(roundingMode: MidpointRounding.ToZero);
		var sut = moneyContext.NewMoney(10M);
		
		// Act
		var weights = new[] { 1f, 1.3333f, 1.6666f };
		var parts = sut.Split(weights, out var residue);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.NewMoney(2.50M), // 1      / (1 + 1.3333 + 1.6666) * 10 rounded to 2 decimal places
			moneyContext.NewMoney(3.33M), // 1.3333 / (1 + 1.3333 + 1.6666) * 10 rounded to 2 decimal places
			moneyContext.NewMoney(4.16M)  // 1.6666 / (1 + 1.3333 + 1.6666) * 10 rounded to 2 decimal places
		});
		residue.Should().Be(0.01M);
		(totalAmount + residue).Should().Be(10M);
	}



	public static IEnumerable<object[]> GetSplitByPartsData()
	{
		yield return new object[] { MidpointRounding.ToEven, 1.10M, 2, 0.55M, 0.00M };
		yield return new object[] { MidpointRounding.ToEven, 1.11M, 2, 0.56M, -0.01M };
		yield return new object[] { MidpointRounding.ToZero, 1.19M, 2, 0.59M, 0.01M };
		yield return new object[] { MidpointRounding.ToPositiveInfinity, 1.11M, 2, 0.56M, -0.01M };
	}
}