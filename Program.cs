using Blazored.LocalStorage;
using KPServices.BlazorGeoLoc;
using KPServices.IPAPI;
using KPServices.OpenWeatherAPI;
using KPServices.ClockAbstract;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http;
using SiteWasm;
using KPServices.IPBlazorWasm;

// var apiWeather= builder.Configuration.GetSection("ConnectionStrings").GetValue<string>("WeatherAPI"); 
var apiWeather= "be0e71519c7cbf8fb043bd5c35d1f8c3"; 

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.Services.AddBlazoredLocalStorage();

    builder.Services.AddHttpClient("IP",(options) => {
      options.BaseAddress = new Uri("https://jsonip.com");
    });
    builder.Services.AddHttpClient<IPClient>();
    builder.Services.AddHttpClient<WeatherClient>();
    builder.Services.AddScoped<IIPBlazorWasm,IPBlazorWasm>();
    builder.Services.AddScoped<IClockAbstract, ClockAbstract>();
    builder.Services.AddScoped<IGeoLService, GeoLService>();
    builder.Services.AddScoped<IipService, ipService>();
    if(apiWeather == null)
        throw new ArgumentNullException("ERROR: EL API-WEATHER TOKEN NO HA SIDO ENCONTRADO.");
    else
        /* ***PASANDO EL API TOKEN AL CONSTRUCTOR DEL SERVICIO MEDIANTE Microsoft.Extensions.DependencyInjection.ActivatorUtilities*** */
        builder.Services.AddScoped<IWeatherService>(p =>  
            ActivatorUtilities.CreateInstance<WeatherService>(p, apiWeather)
        );

    // if(NTPSer == null)
    //     throw new ArgumentNullException("ERROR: EL SERVIDOR NTP NO HA SIDO ENCONTRADO.");
    // // else
    //     builder.Services.AddSingleton<KPServices.NTPClient.INTPClient>( p =>
    //         ActivatorUtilities.CreateInstance<KPServices.NTPClient.NTPClient>(p,"mx.pool.ntp.org")
    //     );
    // builder.Services.AddSingleton<KPServices.NTPClient.INTPClient, KPServices.NTPClient.NTPClient>();

await builder.Build().RunAsync();
