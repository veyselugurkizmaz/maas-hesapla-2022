namespace WebAPI.Entities;

public record YillikMaasHesaplaRequest
{
    public int Yil { get; set; }
    public decimal BrutUcret { get; set; }
}

