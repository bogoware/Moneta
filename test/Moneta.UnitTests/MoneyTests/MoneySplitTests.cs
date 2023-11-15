namespace Bogoware.Moneta.UnitTests.MoneyTests;

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
		var moneyContext = new MonetaContext(roundingMode: roundingMode);
		var sut = moneyContext.CreateMoney(amount);

		// Act
		var parts = sut.Split(numberOfParts, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		var expectedPart = moneyContext.CreateMoney(expectedPartAmount);

		parts.Should().HaveCount(numberOfParts);
		parts.Should().AllBeEquivalentTo(expectedPart);
		unallocated.Amount.Should().Be(expectedResidueAmount);
		(totalAmount + unallocated).Amount.Should().Be(amount);
	}

	[Fact]
	public void Money_splitByNumberOfParts_worksWithPerfectSplit()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var parts = sut.Split(100, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(100);
		unallocated.Amount.Should().Be(0M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}

	[Fact]
	public void Money_splitByNumberOfParts_worksWithResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10.0001M);

		// Act
		var parts = sut.Split(100, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(100);
		unallocated.Amount.Should().Be(0.0001M);
		(totalAmount + unallocated).Amount.Should().Be(10.0001M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithPerfectSplit_01()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var parts = sut.Split(Enumerable.Repeat(1, 10), out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(10);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(1M));
		unallocated.Amount.Should().Be(0M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithPerfectSplit_02()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var weights = new[] { 2, 2, 1 };
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.CreateMoney(4M), 
			moneyContext.CreateMoney(4M), 
			moneyContext.CreateMoney(2M)
		});
		unallocated.Amount.Should().Be(0M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithPerfectSplit_03()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var weights = new[] { 3, 2, 1 };
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.CreateMoney(5.00M), 
			moneyContext.CreateMoney(3.3333M), 
			moneyContext.CreateMoney(1.6667M)
		});
		unallocated.Amount.Should().Be(0M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithResidue_01()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10.0001M);

		// Act
		var parts = sut.Split(Enumerable.Repeat(1, 10), out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(10);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(1.00M));
		unallocated.Amount.Should().Be(0.0001M);
		(totalAmount + unallocated).Amount.Should().Be(10.0001M);
	}

	[Fact]
	public void Money_splitByIntWeights_worksWithResidue_02()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var weights = new[] { 177, 53, 13 };
		var sut = moneyContext.CreateMoney(10M);

		// Act
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.CreateMoney(7.2840M), // 177 / (177 + 53 + 13) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(2.1811M), //  53 / (177 + 53 + 13) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(0.5350M)  //  13 / (177 + 53 + 13) * 10 rounded to 2 decimal places
		});
		unallocated.Amount.Should().Be(-0.0001M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}

	[Fact]
	public void Money_splitByFloatWeights_worksWithPerfectSplit_01()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);
		
		// Act
		var weights = new[] { 1.0f, 1.0f };
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(2);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(5M));
		unallocated.Amount.Should().Be(0M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}
	
	[Fact]
	public void Money_splitByFloatWeights_worksWithPerfectSplit_02()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);
		
		// Act
		var weights = new[] { 1.25f, 1.50f, 1.75f };
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.CreateMoney(2.7778M), // 1.25 / (1.25 + 1.50 + 1.75) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(3.3333M), // 1.50 / (1.25 + 1.50 + 1.75) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(3.8889M)  // 1.75 / (1.25 + 1.50 + 1.75) * 10 rounded to 2 decimal places
		});
		unallocated.Amount.Should().Be(0M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}

	[Fact]
	public void Money_splitByFloatWeights_worksWithResidue_01()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10.0001M);
		
		// Act
		var weights = new[] { 1.0f, 1.0f };
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(2);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(5.00M));
		unallocated.Amount.Should().Be(0.0001M);
		(totalAmount + unallocated).Amount.Should().Be(10.0001M);
	}

	[Fact]
	public void Money_splitByFloatWeights_worksWithResidue_02()
	{
		// Arrange
		var moneyContext = new MonetaContext(roundingMode: MidpointRounding.ToZero);
		var sut = moneyContext.CreateMoney(10M);
		
		// Act
		var weights = new[] { 1f, 1.3333f, 1.6666f };
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(3);
		parts.Should().BeEquivalentTo(new[]
		{
			moneyContext.CreateMoney(2.50M), // 1      / (1 + 1.3333 + 1.6666) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(3.3333M), // 1.3333 / (1 + 1.3333 + 1.6666) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(4.1666M)  // 1.6666 / (1 + 1.3333 + 1.6666) * 10 rounded to 2 decimal places
		});
		unallocated.Amount.Should().Be(0.0001M);
		(totalAmount + unallocated).Amount.Should().Be(10M);
	}



	public static IEnumerable<object[]> GetSplitByPartsData()
	{
		yield return new object[] { MidpointRounding.ToEven, 1.1234M, 2, 0.5617M, 0.0000M };
		yield return new object[] { MidpointRounding.ToEven, 1.1111M, 2, 0.5556M, -0.0001M };
		yield return new object[] { MidpointRounding.ToZero, 1.1977M, 2, 0.5988M, 0.0001M };
		yield return new object[] { MidpointRounding.ToPositiveInfinity, 1.1197M, 2, 0.5599M, -0.0001M };
	}
}