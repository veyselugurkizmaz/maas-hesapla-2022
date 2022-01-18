namespace WebAPI.Entities;

public record MaasHesaplaRequest
{
    public int Yil { get; set; }
    public IList<MaasHesaplaAyRequestDto> AyList { get; set; } = new List<MaasHesaplaAyRequestDto>();
}
public record MaasHesaplaAyRequestDto
{
    public int Ay { get; set; }
    public decimal BrutUcret { get; set; }
}
