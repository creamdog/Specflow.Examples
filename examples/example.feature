Feature: Webservices
	In order to know what is going on
	As a developer
	I want to instantiate any arbitrary web service and invoke availible methods and assert their return values
	
@mytag
Scenario: Weather
	Given webservice named Weather with the wsdl url http://ws.cdyne.com/WeatherWS/Weather.asmx?wsdl
	When I call the method GetCityForecastByZIP with the parameter ZIP=36003
	Then I expect it to return an object matching content {City: 'Autaugaville', WeatherStationCity: 'Selma'}

@mytag
Scenario: Speed
	Given webservice named ConvertSpeeds with the wsdl url http://www.webservicex.net/ConvertSpeed.asmx?WSDL
	When I call the method ConvertSpeed with the parameters speed=100 and FromUnit=centimetersPersecond and ToUnit=feetPersecond
	Then I expect it to return the number 3.2808398950131235

@mytag
Scenario: Whois
	Given webservice named whois with the wsdl url http://www.webservicex.net/whois.asmx?WSDL
	When I call the method GetWhoIS with the parameters HostName=google.com
	Then I expect it to return a string containing TERMS OF USE: You are not authorized to access or query our Whois
	
@mytag
Scenario: country
	Given webservice named country with the wsdl url http://www.webservicex.net/country.asmx?WSDL
	When I call the method GetCountries with no parameters
	Then I expect it to return html containing <NewDataSet><Table><Name>Kiribati</Name></Table><Table><Name>Cocos (Keeling) Islands</Name></Table></NewDataSet>

@mytag
Scenario: GeoIPService
	Given webservice named GeoIPService with the wsdl url http://www.webservicex.net/geoipservice.asmx?WSDL
	When I call the method GetGeoIP with the parameters IPAddress='209.85.148.147'
	Then I expect it to return an object matching content {IP:'209.85.148.147', CountryName:'United States', CountryCode:'USA'}

@mytag
Scenario: BarCode
	Given webservice named BarCode with the wsdl url http://www.webservicex.net/genericbarcode.asmx?WSDL
	When I call the method GenerateBarCode with the parameters BarCodeText=123456 and BarCodeParam={Height:1,Width:1,Ratio:5,FontName:'Arial',FontSize:10,BarColor:'Black',BGColor:'White',barcodeOption:'Both', barcodeType: 'Code_2_5_interleaved', showTextPosition: 'BottomCenter', BarCodeImageFormat: 'PNG'}
	Then I expect it to return a base64 string matching iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAA1JREFUGFdjYGBg+A8AAQQBAHAgZQsAAAAASUVORK5CYIIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==
