namespace WebAPI.Entities;

public record GelirVergisiResponse
{
    public decimal GelirVergisiMatrahi { get; set; }
    public decimal GelirVergisi { get; set; }
    public IList<GelirVergisiDilimResponse> GelirVergisiDilimList { get; set; } = new List<GelirVergisiDilimResponse>();
}
public record GelirVergisiDilimResponse
{
    public decimal GelirVergisiOrani { get; init; }
    public decimal GelirVergisiMatrahi { get; init; }
    public decimal GelirVergisi { get; init; }
}
