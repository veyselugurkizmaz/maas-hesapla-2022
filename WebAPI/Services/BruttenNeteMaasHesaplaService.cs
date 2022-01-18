using WebAPI.Entities;

namespace WebAPI.Services;

public class BruttenNeteMaasHesaplaService
{
    private const decimal SgkIsciOran = 0.14m;
    private const decimal IssizlikIsciOran = 0.01m;
    private const decimal DamgaVergisiOran = 0.00759m;

    private static IList<GelirVergisiDilimDto> GelirVergisiDilimList { get; } = new List<GelirVergisiDilimDto>();
    private static IList<BrutAsgariUcretDto> AsgariUcretList { get; } = new List<BrutAsgariUcretDto>();
    private static IList<SgkMatrakTavanDto> SgkMatrakTavanList { get; } = new List<SgkMatrakTavanDto>();
    private static IList<AsgariUcretIstisnaDto> AsgariUcretIstisnaList { get; } = new List<AsgariUcretIstisnaDto>();

    static BruttenNeteMaasHesaplaService()
    {
        GelirVergisiDilimList.Add(new GelirVergisiDilimDto { Yil = 2022, AltDilim = 0, UstDilim = 32000, Oran = 0.15m });
        GelirVergisiDilimList.Add(new GelirVergisiDilimDto { Yil = 2022, AltDilim = 32000, UstDilim = 70000, Oran = 0.20m });
        GelirVergisiDilimList.Add(new GelirVergisiDilimDto { Yil = 2022, AltDilim = 70000, UstDilim = 250000, Oran = 0.27m });
        GelirVergisiDilimList.Add(new GelirVergisiDilimDto { Yil = 2022, AltDilim = 250000, UstDilim = 880000, Oran = 0.35m });
        GelirVergisiDilimList.Add(new GelirVergisiDilimDto { Yil = 2022, AltDilim = 880000, UstDilim = 9999999999999, Oran = 0.40m });

        AsgariUcretList.Add(new BrutAsgariUcretDto { Yil = 2022, Tutar = 5004 });

        SgkMatrakTavanList.Add(new SgkMatrakTavanDto { Yil = 2022, Tutar = 37350 });
    }

    public SgkIsciResponse HesaplaSgkIsci(decimal brutUcret, decimal devirMatrah1AyOnce, decimal devirMatrah2AyOnce, int yil)
    {
        decimal sgkMatrahTavan = GetSgkMatrahTavan(yil);
        if(devirMatrah2AyOnce >= sgkMatrahTavan)
        {
            return new SgkIsciResponse
            {
                SgkIsciTutar = (sgkMatrahTavan * SgkIsciOran).Round(),
                KalanMatrah = brutUcret,
                KalanMatrah1AyOnce = devirMatrah1AyOnce
            };
        }
        if(devirMatrah2AyOnce + devirMatrah1AyOnce >= sgkMatrahTavan)
        {
            return new SgkIsciResponse
            {
                SgkIsciTutar = (sgkMatrahTavan * SgkIsciOran).Round(),
                KalanMatrah = brutUcret,
                KalanMatrah1AyOnce = devirMatrah2AyOnce + devirMatrah1AyOnce - sgkMatrahTavan
            };
        }
        if(devirMatrah2AyOnce + devirMatrah1AyOnce + brutUcret >= sgkMatrahTavan)
        {
            return new SgkIsciResponse
            {
                SgkIsciTutar = (sgkMatrahTavan * SgkIsciOran).Round(),
                KalanMatrah = devirMatrah2AyOnce + devirMatrah1AyOnce + brutUcret - sgkMatrahTavan,
                KalanMatrah1AyOnce = 0
            };
        }
        return new SgkIsciResponse
        {
            SgkIsciTutar = ((devirMatrah2AyOnce + devirMatrah1AyOnce + brutUcret) * SgkIsciOran).Round(),
            KalanMatrah = 0,
            KalanMatrah1AyOnce = 0
        };
    }
    public decimal HesaplaIssizlikIsci(decimal brutUcret)
    {
        return (brutUcret * IssizlikIsciOran).Round();
    }
    public GelirVergisiResponse HesaplaGelirVergisi(decimal brutUcret, decimal sgkIsci, decimal issizlikIsci, decimal kumulatifGelirVergisiMatrahi, int yil)
    {
        decimal gelirVergisiMatrah = brutUcret - sgkIsci - issizlikIsci;

        GelirVergisiResponse response = new GelirVergisiResponse();

        decimal toplamMatrah = kumulatifGelirVergisiMatrahi + gelirVergisiMatrah;
        var gelirVergisiDilimList = GetGelirVergisiDilimList(yil);

        decimal suAnkiMatrah = kumulatifGelirVergisiMatrahi;
        decimal buAyKalanMatrah = gelirVergisiMatrah;
        foreach (var dilim in gelirVergisiDilimList)
        {
            if(dilim.AltDilim <= suAnkiMatrah && dilim.UstDilim > suAnkiMatrah)
            {
                if(suAnkiMatrah + buAyKalanMatrah <= dilim.UstDilim)
                {
                    response.GelirVergisiDilimList.Add(new GelirVergisiDilimResponse
                    {
                        GelirVergisiMatrahi = buAyKalanMatrah,
                        GelirVergisiOrani = dilim.Oran,
                        GelirVergisi = (buAyKalanMatrah * dilim.Oran).Round()
                    });
                    break;
                }
                else
                {
                    decimal dilimMatrah = dilim.UstDilim - suAnkiMatrah;
                    response.GelirVergisiDilimList.Add(new GelirVergisiDilimResponse
                    {
                        GelirVergisiMatrahi = dilimMatrah,
                        GelirVergisiOrani = dilim.Oran,
                        GelirVergisi = (dilimMatrah * dilim.Oran).Round()
                    });
                    suAnkiMatrah = dilim.UstDilim;
                    buAyKalanMatrah = kumulatifGelirVergisiMatrahi + gelirVergisiMatrah - suAnkiMatrah;
                }
            }
        }
        response.GelirVergisi = response.GelirVergisiDilimList.Sum(x => x.GelirVergisi);
        response.GelirVergisiMatrahi = response.GelirVergisiDilimList.Sum(x => x.GelirVergisiMatrahi);
        return response;
    }
    public decimal HesaplaDamgaVergisi(decimal brutUcret)
    {
        decimal damgaVergisiMatrah = brutUcret;
        return (damgaVergisiMatrah * DamgaVergisiOran).Round();
    }

    public MaasHesaplaResponse Hesapla(MaasHesaplaRequest request)
    {
        var ayList = request.AyList.OrderBy(x => x.Ay).ToList();
        decimal devirSgkMatrah1AyOnce = 0, devirSgkMatrah2AyOnce = 0;
        decimal kumulatifGelirVergisiMatrahi = 0, kumulatifAsgariUcretIstisnaGelirVergisiMatrahi = 0;
        MaasHesaplaResponse response = new MaasHesaplaResponse();
        foreach (var ay in ayList)
        {
            var sgkIsci = HesaplaSgkIsci(ay.BrutUcret, devirSgkMatrah1AyOnce, devirSgkMatrah2AyOnce, request.Yil);
            {
                devirSgkMatrah1AyOnce = sgkIsci.KalanMatrah;
                devirSgkMatrah2AyOnce = sgkIsci.KalanMatrah1AyOnce;
            }
            var issizlikIsci = HesaplaIssizlikIsci(ay.BrutUcret);
            var gelirVergisi = HesaplaGelirVergisi(ay.BrutUcret, sgkIsci.SgkIsciTutar, issizlikIsci, kumulatifGelirVergisiMatrahi, request.Yil);
            {
                kumulatifGelirVergisiMatrahi += gelirVergisi.GelirVergisiMatrahi;
            }
            var damgaVergisi = HesaplaDamgaVergisi(ay.BrutUcret);
            var asgariUcretIstisna = HesaplaAsgariUcretIstisna(request.Yil, kumulatifAsgariUcretIstisnaGelirVergisiMatrahi);
            kumulatifAsgariUcretIstisnaGelirVergisiMatrahi += asgariUcretIstisna.GelirVergisiMatrahi;
            var responseAy = new MaasHesaplaAyResponseDto
            {
                Ay = ay.Ay,
                BrutUcret = ay.BrutUcret,
                SgkIsci = sgkIsci.SgkIsciTutar,
                IssizlikIsci = issizlikIsci,
                KumulatifGelirVergisiMatrahi = kumulatifGelirVergisiMatrahi,
                GelirVergisiMatrahi = gelirVergisi.GelirVergisiMatrahi,
                GelirVergisi = gelirVergisi.GelirVergisi,
                DamgaVergisi = damgaVergisi,
                GelirVergisiList = gelirVergisi.GelirVergisiDilimList.Select(x=>new MaasHesaplaAyResponseGelirVergisiDto
                {
                    GelirVergisi = x.GelirVergisi,
                    GelirVergisiMatrahi = x.GelirVergisiMatrahi,
                    GelirVergisiOrani = x.GelirVergisiOrani
                }
                ).ToList(),
                GelirVergisiIstisnaTutar = asgariUcretIstisna.GelirVergisiIstisnaTutar,
                DamgaVergisiIstisnaTutar = asgariUcretIstisna.DamgaVergisiIstisnaTutar
            };
            response.AyList.Add(responseAy);
        }
        response.NetOrtalama = (response.AyList.Sum(x => x.NetEleGecenUcret) / (decimal)response.AyList.Count).Round();
        return response;
    }

    public decimal GetBrutAsgariUcret(decimal yil)
    {
        return AsgariUcretList.Where(x => x.Yil == yil).FirstOrDefault().Tutar;
    }
    public List<GelirVergisiDilimDto> GetGelirVergisiDilimList(decimal yil)
    {
        return GelirVergisiDilimList.Where(x => x.Yil == yil).OrderBy(x => x.AltDilim).ToList();
    }
    public decimal GetSgkMatrahTavan(decimal yil)
    {
        return SgkMatrakTavanList.Where(x => x.Yil == yil).FirstOrDefault().Tutar;
    }
    public AsgariUcretIstisnaDto HesaplaAsgariUcretIstisna(int yil, decimal kumulatifAsgariUcretIstisnaGelirVergisiMatrahToplami)
    {
        AsgariUcretIstisnaDto istisna = AsgariUcretIstisnaList.Where(x => x.Yil == yil)?.FirstOrDefault();
        if (istisna != null)
            return istisna;

        decimal brutUcret = GetBrutAsgariUcret(yil);
        var sgkIsci = HesaplaSgkIsci(brutUcret, 0, 0, yil);
        var issizlikIsci = HesaplaIssizlikIsci(brutUcret);
        var gelirVergisi = HesaplaGelirVergisi(brutUcret, sgkIsci.SgkIsciTutar, issizlikIsci, kumulatifAsgariUcretIstisnaGelirVergisiMatrahToplami, yil);
        var damgaVergisi = HesaplaDamgaVergisi(brutUcret);
        istisna = new AsgariUcretIstisnaDto
        {
            Yil = yil,
            GelirVergisiIstisnaTutar = gelirVergisi.GelirVergisi,
            DamgaVergisiIstisnaTutar = damgaVergisi,
            GelirVergisiMatrahi = gelirVergisi.GelirVergisiMatrahi
        };
        return istisna;
    }
}
