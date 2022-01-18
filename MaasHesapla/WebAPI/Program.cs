using WebAPI.Entities;
using WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BruttenNeteMaasHesaplaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();


app.MapPost("/maashesaplayillik", (YillikMaasHesaplaRequest request, BruttenNeteMaasHesaplaService service) =>
{
    MaasHesaplaRequest maasHesaplaRequest = new MaasHesaplaRequest()
    {
        Yil = request.Yil
    };
    for (int i = 1; i <= 12; i++)
    {
        maasHesaplaRequest.AyList.Add(new MaasHesaplaAyRequestDto { Ay = i, BrutUcret = request.BrutUcret });
    }
    var response = service.Hesapla(maasHesaplaRequest);
    return response;
});
app.MapPost("/maashesaplaaylik", (MaasHesaplaRequest request, BruttenNeteMaasHesaplaService service) =>
{
    var response = service.Hesapla(request);
    return response;
});


app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}