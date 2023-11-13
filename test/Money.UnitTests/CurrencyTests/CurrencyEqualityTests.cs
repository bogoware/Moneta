namespace Bogoware.Money.UnitTests.CurrencyTests;

public class CurrencyEqualityTests
{
	private class FakeCurrencyOne : Currency<FakeCurrencyOne>
	{
		internal FakeCurrencyOne()
			: base("FakeOne", "Fake", "Fake", 2)
		{
		}
	}
	
	private class FakeCurrencyTwo : Currency<FakeCurrencyOne>
	{
		internal FakeCurrencyTwo()
			: base("FakeTwo", "Fake", "Fake", 2)
		{
		}
	}
	
	[Fact]
	public void Currency_equality_worksForTheSameInstance()
	{
		// Arrange
		var sut = new FakeCurrencyOne();
		
		// Act
		var result = sut.Equals(sut);
		
		// Assert
		result.Should().BeTrue();
	}
	
	[Fact]
	public void Currency_equality_worksForDifferentInstances()
	{
		// Arrange
		var sut = new FakeCurrencyOne();
		var other = new FakeCurrencyOne();
		
		// Act
		var result = sut.Equals(other);
		
		// Assert
		result.Should().BeTrue();
	}
	
	[Fact]
	public void Currency_equality_worksForNull()
	{
		// Arrange
		var sut = new FakeCurrencyOne();
		
		// Act
		var result = sut.Equals(null);
		
		// Assert
		result.Should().BeFalse();
	}
	
	[Fact]
	public void Currency_equality_worksForDifferentTypes()
	{
		// Arrange
		var sut = new FakeCurrencyOne();
		var other = new FakeCurrencyTwo();
		
		// Act
		var result = sut.Equals(other);
		
		// Assert
		result.Should().BeFalse();
	}
	
	
}