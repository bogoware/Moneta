using System.Diagnostics.CodeAnalysis;

namespace Bogoware.Moneta.CurrencyProviders;

public class IsoCurrencyProvider : ICurrencyProvider
{
	private static readonly HashSet<Currency> _currencies = new()
	{
		// All the ISO 4217 currencies via ChatGPT
		new("AED", "United Arab Emirates Dirham", "د.إ", 2),
		new("AFN", "Afghan Afghani", "؋", 2),
		new("ALL", "Albanian Lek", "L", 2),
		new("AMD", "Armenian Dram", "֏", 2),
		new("ANG", "Netherlands Antillean Guilder", "ƒ", 2),
		new("AOA", "Angolan Kwanza", "Kz", 2),
		new("ARS", "Argentine Peso", "$", 2),
		new("AUD", "Australian Dollar", "$", 2),
		new("AWG", "Aruban Florin", "ƒ", 2),
		new("AZN", "Azerbaijani Manat", "₼", 2),
		new("BAM", "Bosnia and Herzegovina Convertible Mark", "KM", 2),
		new("BBD", "Barbadian Dollar", "$", 2),
		new("BDT", "Bangladeshi Taka", "৳", 2),
		new("BGN", "Bulgarian Lev", "лв", 2),
		new("BHD", "Bahraini Dinar", "ب.د", 3),
		new("BIF", "Burundian Franc", "Fr", 0),
		new("BMD", "Bermudian Dollar", "$", 2),
		new("BND", "Brunei Dollar", "$", 2),
		new("BOB", "Bolivian Boliviano", "Bs.", 2),
		new("BRL", "Brazilian Real", "R$", 2),
		new("BSD", "Bahamian Dollar", "$", 2),
		new("BTN", "Bhutanese Ngultrum", "Nu.", 2),
		new("BWP", "Botswana Pula", "P", 2),
		new("BYN", "Belarusian Ruble", "Br", 2),
		new("BZD", "Belize Dollar", "BZ$", 2),
		new("CAD", "Canadian Dollar", "$", 2),
		new("CDF", "Congolese Franc", "Fr", 2),
		new("CHF", "Swiss Franc", "Fr", 2),
		new("CLP", "Chilean Peso", "$", 0),
		new("CNY", "Chinese Yuan", "¥", 2),
		new("COP", "Colombian Peso", "$", 2),
		new("CRC", "Costa Rican Colón", "₡", 2),
		new("CUP", "Cuban Peso", "₱", 2),
		new("CVE", "Cape Verdean Escudo", "$", 2),
		new("CZK", "Czech Koruna", "Kč", 2),
		new("DJF", "Djiboutian Franc", "Fr", 0),
		new("DKK", "Danish Krone", "kr", 2),
		new("DOP", "Dominican Peso", "RD$", 2),
		new("DZD", "Algerian Dinar", "د.ج", 2),
		new("EGP", "Egyptian Pound", "£", 2),
		new("ERN", "Eritrean Nakfa", "Nfk", 2),
		new("ETB", "Ethiopian Birr", "Br", 2),
		new("EUR", "Euro", "€", 2),
		new("FJD", "Fijian Dollar", "FJ$", 2),
		new("FKP", "Falkland Islands Pound", "£", 2),
		new("FOK", "Faroese Króna", "kr", 2),
		new("GBP", "British Pound Sterling", "£", 2),
		new("GEL", "Georgian Lari", "₾", 2),
		new("GGP", "Guernsey Pound", "£", 2),
		new("GHS", "Ghanaian Cedi", "₵", 2),
		new("GIP", "Gibraltar Pound", "£", 2),
		new("GMD", "Gambian Dalasi", "D", 2),
		new("GNF", "Guinean Franc", "Fr", 0),
		new("GTQ", "Guatemalan Quetzal", "Q", 2),
		new("GYD", "Guyanaese Dollar", "$", 2),
		new("HKD", "Hong Kong Dollar", "$", 2),
		new("HNL", "Honduran Lempira", "L", 2),
		new("HRK", "Croatian Kuna", "kn", 2),
		new("HTG", "Haitian Gourde", "G", 2),
		new("HUF", "Hungarian Forint", "Ft", 2),
		new("IDR", "Indonesian Rupiah", "Rp", 2),
		new("ILS", "Israeli New Sheqel", "₪", 2),
		new("IMP", "Isle of Man Pound", "£", 2),
		new("INR", "Indian Rupee", "₹", 2),
		new("IQD", "Iraqi Dinar", "ع.د", 3),
		new("IRR", "Iranian Rial", "﷼", 2),
		new("ISK", "Icelandic Króna", "kr", 0),
		new("JEP", "Jersey Pound", "£", 2),
		new("JMD", "Jamaican Dollar", "J$", 2),
		new("JOD", "Jordanian Dinar", "د.ا", 3),
		new("JPY", "Japanese Yen", "¥", 0),
		new("KES", "Kenyan Shilling", "Sh", 2),
		new("KGS", "Kyrgystani Som", "с", 2),
		new("KHR", "Cambodian Riel", "៛", 2),
		new("KID", "Kiribati Dollar", "$", 2),
		new("KMF", "Comorian Franc", "Fr", 0),
		new("KPW", "North Korean Won", "₩", 2),
		new("KRW", "South Korean Won", "₩", 0),
		new("KWD", "Kuwaiti Dinar", "د.ك", 3),
		new("KYD", "Cayman Islands Dollar", "$", 2),
		new("KZT", "Kazakhstani Tenge", "₸", 2),
		new("LAK", "Laotian Kip", "₭", 2),
		new("LBP", "Lebanese Pound", "ل.ل", 2),
		new("LKR", "Sri Lankan Rupee", "₨", 2),
		new("LRD", "Liberian Dollar", "$", 2),
		new("LSL", "Lesotho Loti", "L", 2),
		new("LYD", "Libyan Dinar", "ل.د", 3),
		new("MAD", "Moroccan Dirham", "د.م.", 2),
		new("MDL", "Moldovan Leu", "L", 2),
		new("MGA", "Malagasy Ariary", "Ar", 2),
		new("MKD", "Macedonian Denar", "ден", 2),
		new("MMK", "Myanma Kyat", "Ks", 2),
		new("MNT", "Mongolian Tugrik", "₮", 2),
		new("MOP", "Macanese Pataca", "P", 2),
		new("MRU", "Mauritanian Ouguiya", "UM", 2),
		new("MUR", "Mauritian Rupee", "₨", 2),
		new("MVR", "Maldivian Rufiyaa", "ރ.", 2),
		new("MWK", "Malawian Kwacha", "MK", 2),
		new("MXN", "Mexican Peso", "$", 2),
		new("MYR", "Malaysian Ringgit", "RM", 2),
		new("MZN", "Mozambican Metical", "MT", 2),
		new("NAD", "Namibian Dollar", "$", 2),
		new("NGN", "Nigerian Naira", "₦", 2),
		new("NIO", "Nicaraguan Córdoba", "C$", 2),
		new("NOK", "Norwegian Krone", "kr", 2),
		new("NPR", "Nepalese Rupee", "₨", 2),
		new("NZD", "New Zealand Dollar", "$", 2),
		new("OMR", "Omani Rial", "ر.ع.", 3),
		new("PAB", "Panamanian Balboa", "B/.", 2),
		new("PEN", "Peruvian Nuevo Sol", "S/.", 2),
		new("PGK", "Papua New Guinean Kina", "K", 2),
		new("PHP", "Philippine Peso", "₱", 2),
		new("PKR", "Pakistani Rupee", "₨", 2),
		new("PLN", "Polish Złoty", "zł", 2),
		new("PYG", "Paraguayan Guarani", "₲", 0),
		new("QAR", "Qatari Rial", "ر.ق", 2),
		new("RON", "Romanian Leu", "lei", 2),
		new("RSD", "Serbian Dinar", "дин.", 2),
		new("RUB", "Russian Ruble", "₽", 2),
		new("RWF", "Rwandan Franc", "Fr", 0),
		new("SAR", "Saudi Riyal", "ر.س", 2),
		new("SBD", "Solomon Islands Dollar", "$", 2),
		new("SCR", "Seychellois Rupee", "₨", 2),
		new("SDG", "Sudanese Pound", "£", 2),
		new("SEK", "Swedish Krona", "kr", 2),
		new("SGD", "Singapore Dollar", "$", 2),
		new("SHP", "Saint Helena Pound", "£", 2),
		new("SLL", "Sierra Leonean Leone", "Le", 2),
		new("SOS", "Somali Shilling", "Sh", 2),
		new("SRD", "Surinamese Dollar", "$", 2),
		new("SSP", "South Sudanese Pound", "£", 2),
		new("STD", "São Tomé and Príncipe Dobra", "Db", 2),
		new("SVC", "Salvadoran Colón", "$", 2),
		new("SYP", "Syrian Pound", "ل.س", 2),
		new("SZL", "Swazi Lilangeni", "L", 2),
		new("THB", "Thai Baht", "฿", 2),
		new("TJS", "Tajikistani Somoni", "ЅМ", 2),
		new("TMT", "Turkmenistani Manat", "m", 2),
		new("TND", "Tunisian Dinar", "د.ت", 3),
		new("TOP", "Tongan Pa'anga", "T$", 2),
		new("TRY", "Turkish Lira", "₺", 2),
		new("TTD", "Trinidad and Tobago Dollar", "TT$", 2),
		new("TWD", "New Taiwan Dollar", "NT$", 2),
		new("TZS", "Tanzanian Shilling", "Sh", 2),
		new("UAH", "Ukrainian Hryvnia", "₴", 2),
		new("UGX", "Ugandan Shilling", "Sh", 0),
		new("USD", "United States Dollar", "$", 2),
		new("UYU", "Uruguayan Peso", "$", 2),
		new("UZS", "Uzbekistan Som", "сўм", 2),
		new("VES", "Venezuelan Bolívar", "Bs.", 2),
		new("VND", "Vietnamese Đồng", "₫", 0),
		new("VUV", "Vanuatu Vatu", "Vt", 0),
		new("WST", "Samoan Tala", "T", 2),
		new("XAF", "Central African CFA Franc", "Fr", 0),
		new("XCD", "East Caribbean Dollar", "$", 2),
		new("XOF", "West African CFA Franc", "Fr", 0),
		new("XPF", "CFP Franc", "Fr", 0),
		new("YER", "Yemeni Rial", "﷼", 2),
		new("ZAR", "South African Rand", "R", 2),
		new("ZMW", "Zambian Kwacha", "ZK", 2),
		new("ZWL", "Zimbabwean Dollar", "Z$", 2)
	};

	private readonly HashSet<string>? _allowedCodes;

	public IsoCurrencyProvider()
	{
	}

	/// <summary>
	/// Initializes a new <see cref="IsoCurrencyProvider"/> instance.
	/// </summary>
	/// <param name="allowedCodes">Only these codes are allowed</param>
	public IsoCurrencyProvider(IEnumerable<string> allowedCodes)
	{
		foreach (var code in allowedCodes)
		{
			if (_currencies.Any(c => c.Code == code)) continue;

			throw new ArgumentException($"The currency code '{code}' is not supported.");
		}

		_allowedCodes = allowedCodes.ToHashSet();
	}

	public bool TryGetCurrency(string code, [NotNullWhen(true)] out ICurrency? currency)
	{
		if (_allowedCodes is null || _allowedCodes.Contains(code))
		{
			currency = _currencies.FirstOrDefault(c => c.Code == code);
			return currency is not null;
		}

		currency = null;
		return false;
	}
}