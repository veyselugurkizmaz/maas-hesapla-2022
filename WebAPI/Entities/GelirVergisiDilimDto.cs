namespace WebAPI.Entities;

public record GelirVergisiDilimDto
{
    public decimal AltDilim { get; init; }
    public decimal UstDilim { get; init; }
    public decimal Oran { get; init; }
    public decimal Yil { get; init; }
}
