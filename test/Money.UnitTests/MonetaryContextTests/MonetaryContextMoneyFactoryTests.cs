namespace Bogoware.Money.UnitTests.MonetaryContextTests;

public class MonetaryContextMoneyFactoryTests
{
	[Fact]
	public void CreateMoney_works_withNoResidual()
	{
		// Arrange
		var sut = new MonetaryContext();
		double originalValue = 1.12;
		
		// Act
		var money = sut.CreateMoney(originalValue, out var residue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		residue.Should().Be(0M);
		sut.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void CreateMoney_works_withResidual()
	{
		// Arrange
		var sut = new MonetaryContext();
		double originalValue = 1.123;
		
		// Act
		var money = sut.CreateMoney(originalValue, out var residue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		residue.Should().Be(0.003M);
		sut.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void CreateMoney_withSafeCast()
	{
		// Arrange
		var sut = new MonetaryContext();
		double originalValue = 1.12;
		
		// Act
		var money = sut.CreateMoney(originalValue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		sut.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void CreateMoney_withUnsafeCast()
	{
		// Arrange
		var sut = new MonetaryContext();
		double originalValue = 1.123;
		
		// Act
		var money = sut.CreateMoney(originalValue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		sut.HasRoundingErrors.Should().BeTrue();
		sut.ErrorRoundingOperations.Should().HaveCount(1);
		var errorRoundingOperation = sut.ErrorRoundingOperations[0];
		errorRoundingOperation.Should().BeOfType<CreateOperation>();
		errorRoundingOperation.As<CreateOperation>().Residue.Should().Be(0.003M);
	}
}