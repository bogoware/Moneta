namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyDivideTests
{
	[Fact]
	public void Divide_byTwoEqualMoneys_GetOne()
	{
		// Arrange
		using var context = new MonetaContext();
		var dividend = context.CreateMoney(2.00M);
		var divisor = context.CreateMoney(2.00M);
		var expected = 1.00M;
		
		// Act
		var result = dividend.Divide(divisor);
		
		// Assert
		result.Should().Be(expected);
	}
	
	[Fact]
	public void DivideOperator_byTwoEqualMoneys_GetOne()
	{
		// Arrange
		using var context = new MonetaContext();
		var dividend = context.CreateMoney(2.00M);
		var divisor = context.CreateMoney(2.00M);
		var expected = 1.00M;
		
		// Act
		var result = dividend / divisor;
		
		// Assert
		result.Should().Be(expected);
	}
}