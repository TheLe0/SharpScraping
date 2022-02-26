using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Scrapping;
using Scrapping.Models;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"appsettings.json");

var config = builder.Build();
var seleniumConfig = new SeleniumConfig();
new ConfigureFromConfigurationOptions<SeleniumConfig>(
    config.GetSection("SeleniumConfigurations"))
        .Configure(seleniumConfig);

Console.WriteLine("Started the Web Scrapping!");


var page = new WebServerExport(seleniumConfig);

page.InitExport();
page.Dispose();

Console.WriteLine("Press enter to close...");
Console.ReadLine();