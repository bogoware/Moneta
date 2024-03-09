using Bogoware.Moneta.CommonCurrencies;

namespace Bogoware.Moneta.UnitTests.CurrencyTests;

public class CommonCurrenciesTests
{
	[Fact]
	public void EuroTest()
	{
		var euro = Euro.Instance;
		Assert.Equal("EUR", euro.Code);
		Assert.Equal("Euro", euro.Name);
		Assert.Equal("€", euro.Symbol);
		Assert.Equal(2, euro.DecimalPlaces);
	}
	
	[Fact]
	public void DollarTest()
	{
		var dollar = Dollar.Instance;
		Assert.Equal("USD", dollar.Code);
		Assert.Equal("Dollar", dollar.Name);
		Assert.Equal("$", dollar.Symbol);
		Assert.Equal(2, dollar.DecimalPlaces);
	}
	
	[Fact]
	public void YuanTest()
	{
		var yuan = Yuan.Instance;
		Assert.Equal("CNY", yuan.Code);
		Assert.Equal("Yuan", yuan.Name);
		Assert.Equal("¥", yuan.Symbol);
		Assert.Equal(2, yuan.DecimalPlaces);
	}
	
	[Fact]
	public void YenTest()
	{
		var yen = Yen.Instance;
		Assert.Equal("JPY", yen.Code);
		Assert.Equal("Yen", yen.Name);
		Assert.Equal("¥", yen.Symbol);
		Assert.Equal(0, yen.DecimalPlaces);
	}
	
	[Fact]
	public void PoundSterlingTest()
	{
		var poundSterling = PoundSterling.Instance;
		Assert.Equal("GBP", poundSterling.Code);
		Assert.Equal("Pound Sterling", poundSterling.Name);
		Assert.Equal("£", poundSterling.Symbol);
		Assert.Equal(2, poundSterling.DecimalPlaces);
	}
}