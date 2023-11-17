namespace Bogoware.Moneta.UnitTests.MonetaContextTests;

public class MonetaryContextMoneyFactoryTests
{
	[Fact]
	public void CreateMoney_works_withNoResidual()
	{
		// Arrange
		var sut = new MonetaContext();
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
		var sut = new MonetaContext();
		double originalValue = 1.12345;
		
		// Act
		var money = sut.CreateMoney(originalValue, out var residue);
		
		// Assert
		money.Amount.Should().Be(1.1234M);
		residue.Should().Be(0.00005M);
		sut.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void CreateMoney_withSafeCast()
	{
		// Arrange
		var sut = new MonetaContext();
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
		var sut = new MonetaContext();
		double originalValue = 1.12345;
		
		// Act
		var money = sut.CreateMoney(originalValue);
		
		// Assert
		money.Amount.Should().Be(1.1234M);
		sut.HasRoundingErrors.Should().BeTrue();
		sut.RoundingErrors.Should().HaveCount(1);
		var errorRoundingOperation = sut.RoundingErrors[0];
		errorRoundingOperation.Should().BeOfType<CreateOperation>();
		errorRoundingOperation.As<CreateOperation>().Error.Should().Be(0.00005M);
	}
}