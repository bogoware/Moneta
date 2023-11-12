namespace Bogoware.Money.Conversion;

// https://github.com/JavaMoney/jsr354-ri/blob/master/moneta-convert/moneta-convert-base/src/main/asciidoc/userguide.adoc
/*
 * # ResourceLoader-Configuration (optional)
   # ECB Rates
   load.ECBCurrentRateProvider.type=SCHEDULED
   load.ECBCurrentRateProvider.period=03:00
   load.ECBCurrentRateProvider.resource=org/javamoney/moneta/convert/ecb/defaults/eurofxref-daily.xml
   load.ECBCurrentRateProvider.urls=https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml
   
   load.ECBHistoric90RateProvider.type=SCHEDULED
   load.ECBHistoric90RateProvider.period=03:00
   #load.ECBHistoric90RateProvider.at=12:00
   load.ECBHistoric90RateProvider.resource=org/javamoney/moneta/convert/ecb/defaults/eurofxref-hist-90d.xml
   load.ECBHistoric90RateProvider.urls=https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist-90d.xml
   
   load.ECBHistoricRateProvider.type=SCHEDULED
   load.ECBHistoricRateProvider.period=24:00
   load.ECBHistoricRateProvider.delay=01:00
   load.ECBHistoricRateProvider.at=07:00
   load.ECBHistoricRateProvider.resource=org/javamoney/moneta/convert/ecb/defaults/eurofxref-hist.xml
   load.ECBHistoricRateProvider.urls=https://www.ecb.europa.eu/stats/eurofxref/eurofxref-hist.xml
   
   # IMF Rates
   load.IMFRateProvider.type=SCHEDULED
   load.IMFRateProvider.period=06:00
   #load.IMFRateProvider.delay=12:00
   #load.IMFRateProvider.at=12:00
   load.IMFRateProvider.resource=/java-money/defaults/IMF/rms_five.tsv
   load.IMFRateProvider.urls=https://www.imf.org/external/np/fin/data/rms_five.aspx?tsvflag=Y
 */
public class ExchangeRateProvider
{
	
}