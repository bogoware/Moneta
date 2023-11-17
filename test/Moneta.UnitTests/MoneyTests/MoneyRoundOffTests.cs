namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyRoundOffTests : MoneyBaseTests
{
	[Fact]
	public void RoundToSubunit_cant_create_value_005()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: MidpointRounding.ToPositiveInfinity);
		var sut = context.CreateMoney(1.48);
		var subunit = context.CreateMoney(0.05);

		// Act
		var result = sut.RoundOff(subunit, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(1.50);
		var expectedError = context.CreateMoney(-0.02);
		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}
	
	[Fact]
	public void RoundToSubunit_cant_destroy_value_005()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: MidpointRounding.ToZero);
		var sut = context.CreateMoney(1.48);
		var subunit = context.CreateMoney(0.05);

		// Act
		var result = sut.RoundOff(subunit, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(1.45);
		var expectedError = context.CreateMoney(0.03);
		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}
	
	[Fact]
	public void RoundToSubunit_cant_create_value_025()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: MidpointRounding.ToPositiveInfinity);
		var sut = context.CreateMoney(1.23);
		var subunit = context.CreateMoney(0.25);

		// Act
		var result = sut.RoundOff(subunit, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(1.25);
		var expectedError = context.CreateMoney(-0.02);
		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}
	
	[Fact]
	public void RoundToSubunit_cant_destroy_value_025()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: MidpointRounding.ToZero);
		var sut = context.CreateMoney(1.23);
		var subunit = context.CreateMoney(0.25);

		// Act
		var result = sut.RoundOff(subunit, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(1.00);
		var expectedError = context.CreateMoney(0.23);
		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}
	
	[Fact]
	public void RoundToSubunit_cant_create_value_500()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: MidpointRounding.ToPositiveInfinity);
		var sut = context.CreateMoney(34726351.48);
		var subunit = context.CreateMoney(5.00);

		// Act
		var result = sut.RoundOff(subunit, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(34726355.00);
		var expectedError = context.CreateMoney(-3.52);
		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}
	
	[Fact]
	public void RoundToSubunit_cant_destroy_value_500()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: MidpointRounding.ToZero);
		var sut = context.CreateMoney(34726351.48);
		var subunit = context.CreateMoney(5.00);

		// Act
		var result = sut.RoundOff(subunit, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(34726350.00);
		var expectedError = context.CreateMoney(1.48);
		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}
}