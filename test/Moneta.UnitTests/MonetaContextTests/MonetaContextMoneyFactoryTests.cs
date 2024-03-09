namespace Bogoware.Moneta.UnitTests.MonetaContextTests;

public class MonetaContextMoneyFactoryTests
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
		double originalValue = 1.125;
		
		// Act
		var money = sut.CreateMoney(originalValue, out var residue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		residue.Should().Be(0.005M);
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
		double originalValue = 1.125;
		
		// Act
		var money = sut.CreateMoney(originalValue);
		
		// Assert
		money.Amount.Should().Be(1.12M);
		sut.HasRoundingErrors.Should().BeTrue();
		sut.RoundingErrors.Should().HaveCount(1);
		var errorRoundingOperation = sut.RoundingErrors[0];
		errorRoundingOperation.Should().BeOfType<CreateOperationError>();
		errorRoundingOperation.As<CreateOperationError>().Error.Should().Be(0.005M);
	}
}