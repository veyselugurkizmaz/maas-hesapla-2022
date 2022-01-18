namespace WebAPI.Entities;

public record MaasHesaplaResponse
{
    public decimal NetOrtalama { get; set; }
    public IList<MaasHesaplaAyResponseDto> AyList { get; } = new List<MaasHesaplaAyResponseDto>();
}
public record MaasHesaplaAyResponseDto
{
    public int Ay { get; init; }
    public decimal BrutUcret { get; init; }
    public decimal SgkIsci { get; init; }
    public decimal IssizlikIsci { get; init; }
    public decimal KumulatifGelirVergisiMatrahi { get; init; }
    public decimal GelirVergisiMatrahi { get; init; }
    public decimal GelirVergisi { get; init; }
    public decimal DamgaVergisi { get; init; }
    public decimal KesintilerToplami
    {
        get { return SgkIsci + IssizlikIsci + GelirVergisi + DamgaVergisi; }
    }
    public decimal NetUcret
    {
        get { return BrutUcret - KesintilerToplami; }
    }
    public decimal GelirVergisiIstisnaTutar { get; init; }
    public decimal DamgaVergisiIstisnaTutar { get; init; }
    public decimal NetEleGecenUcret 
    { 
        get
        {
            return NetUcret + GelirVergisiIstisnaTutar + DamgaVergisiIstisnaTutar;
        }
    }

    public IList<MaasHesaplaAyResponseGelirVergisiDto> GelirVergisiList { get; set; } = new List<MaasHesaplaAyResponseGelirVergisiDto>();
}
public record MaasHesaplaAyResponseGelirVergisiDto
{
    public decimal GelirVergisiOrani { get; init; }
    public decimal GelirVergisiMatrahi { get; init; }
    public decimal GelirVergisi { get; init; }
}