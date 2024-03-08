using static System.MidpointRounding;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneySplitTests: MoneyBaseTests
{
	[Fact]
	public void Splitting_byNumberOfParts_cant_create_value()
	{
		// Arrange
		var moneyContext = new MonetaContext(Euro, roundingMode: ToEven);
		const decimal initialAmount = 10M;
		var sut = moneyContext.CreateMoney(initialAmount);

		// Act
		var parts = sut.Split(3, rounding: ToPositiveInfinity, out var unallocated);

		// Assert
		parts.Should().HaveCount(3);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(3.34M));
		unallocated.Amount.Should().Be(-0.02M);
		var totalAmount = parts.Sum(x => x.Amount);
		(totalAmount + unallocated.Amount).Should().Be(initialAmount);
	}
	
	[Fact]
	public void Splitting_byNumberOfParts_cant_destroy_value()
	{
		// Arrange
		var moneyContext = new MonetaContext(Euro, roundingMode: ToEven);
		const decimal initialAmount = 10.01M;
		var sut = moneyContext.CreateMoney(initialAmount);

		// Act
		var parts = sut.Split(100, out var unallocated);

		// Assert
		parts.Should().HaveCount(100);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(0.10M));
		unallocated.Amount.Should().Be(0.01M);
		var totalAmount = parts.Sum(x => x.Amount);
		(totalAmount + unallocated.Amount).Should().Be(initialAmount);		
	}
	
	[Fact]
	public void Splitting_byWeights_cant_create_value()
	{
		// Arrange
		var moneyContext = new MonetaContext(Euro, roundingMode: ToPositiveInfinity);
		const decimal initialAmount = 1M;
		var weights = new[] { 1, 1, 1 };
		var sut = moneyContext.CreateMoney(initialAmount);
		
		// Act
		var allocated = sut.Split(weights, out var unallocated);
		
		// Assert
		var expectedAllocated = moneyContext.CreateMoney(0.34M);
		var expectedUnallocated = moneyContext.CreateMoney(-0.02M);
		allocated.Should().HaveCount(3);
		allocated.Should().AllBeEquivalentTo(expectedAllocated);
		unallocated.Should().BeEquivalentTo(expectedUnallocated);
		(allocated.Sum(x => x.Amount) + unallocated.Amount).Should().Be(initialAmount);
	}
	
	[Fact]
	public void Splitting_byWeights_cant_destroy_value()
	{
		// Arrange
		var moneyContext = new MonetaContext(Euro, roundingMode: ToZero);
		const decimal initialAmount = 1M;
		var weights = new[] { 1, 1, 1 };
		var sut = moneyContext.CreateMoney(initialAmount);
		
		// Act
		var allocated = sut.Split(weights, out var unallocated);
		
		// Assert
		var expectedAllocated = moneyContext.CreateMoney(0.33M);
		var expectedUnallocated = moneyContext.CreateMoney(0.01M);
		
		allocated.Should().HaveCount(3);
		allocated.Should().AllBeEquivalentTo(expectedAllocated);
		unallocated.Should().BeEquivalentTo(expectedUnallocated);
		(allocated.Sum(x => x.Amount) + unallocated.Amount).Should().Be(initialAmount);
	}
	
	[Theory]
	[MemberData(nameof(GetSplitByPartsData))]
	public void Splitting_byNumberOfParts_worksWithError(
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
		(totalAmount + unallocated.Amount).Should().Be(amount);
	}

	[Fact]
	public void Splitting_byNumberOfParts_worksWithPerfectSplit()
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
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	[Fact]
	public void Splitting_byIntWeights_worksWithPerfectSplit_01()
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
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	[Fact]
	public void Splitting_byIntWeights_worksWithPerfectSplit_02()
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
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	[Fact]
	public void Splitting_byIntWeights_worksWithPerfectSplit_03()
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
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	[Fact]
	public void Splitting_byIntWeights_worksWithResidue_01()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10.01M);

		// Act
		var parts = sut.Split(Enumerable.Repeat(1, 10), out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);

		// Assert
		parts.Should().HaveCount(10);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(1.00M));
		unallocated.Amount.Should().Be(0.01M);
		(totalAmount + unallocated.Amount).Should().Be(10.01M);
	}

	[Fact]
	public void Splitting_byIntWeights_worksWithResidue_02()
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
			moneyContext.CreateMoney(7.28M), // 177 / (177 + 53 + 13) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(2.18M), //  53 / (177 + 53 + 13) * 10 rounded to 2 decimal places
			moneyContext.CreateMoney(0.53M)  //  13 / (177 + 53 + 13) * 10 rounded to 2 decimal places
		});
		unallocated.Amount.Should().Be(0.01M);
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	[Fact]
	public void Splitting_byFloatWeights_worksWithPerfectSplit_01()
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
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}
	
	[Fact]
	public void Splitting_byFloatWeights_worksWithPerfectSplit_02()
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
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	[Fact]
	public void Splitting_bByFloatWeights_worksWithResidue_01()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10.01M);
		
		// Act
		var weights = new[] { 1.0f, 1.0f };
		var parts = sut.Split(weights, out var unallocated);
		var totalAmount = parts.Sum(x => x.Amount);
		
		// Assert
		parts.Should().HaveCount(2);
		parts.Should().AllBeEquivalentTo(moneyContext.CreateMoney(5.00M));
		unallocated.Amount.Should().Be(0.01M);
		(totalAmount + unallocated.Amount).Should().Be(10.01M);
	}

	[Fact]
	public void Splitting_byFloatWeights_worksWithResidue_02()
	{
		// Arrange
		var moneyContext = new MonetaContext(roundingMode: ToZero);
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
		unallocated.Amount.Should().Be(0.01M);
		(totalAmount + unallocated.Amount).Should().Be(10M);
	}

	public static IEnumerable<object[]> GetSplitByPartsData()
	{
		yield return new object[] { ToEven, 1.12M, 2, 0.5617M, 0.00M };
		yield return new object[] { ToEven, 1.16M, 2, 0.58M, -0.00M };
		yield return new object[] { ToZero, 1.19M, 2, 0.5988M, 0.01M };
		yield return new object[] { ToPositiveInfinity, 1.12M, 2, 0.5599M, -0.00M };
	}
}