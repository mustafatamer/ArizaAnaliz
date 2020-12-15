using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToExcel.Attributes;

namespace ArizaAnaliz.Model
{
    public class ArizaExcelModel
    {
        public ArizaExcelModel()
        {
        }
        [ExcelColumn("Modem Numarası")] public string ModemNumarasi { get; set; }
        [ExcelColumn("Modem Markası")] public string ModemMarkasi { get; set; }
        [ExcelColumn("Özet Lokasyon")] public string OzetLokasyon { get; set; }
        [ExcelColumn("Ünvan")] public string Unvan { get; set; }
        [ExcelColumn("Adres")] public string Adres { get; set; }
        [ExcelColumn("Başlangıç Tarihi")] public DateTime BaslangisTarihi { get; set; }
        [ExcelColumn("Bitiş Tarihi")] public DateTime BitisTarihi { get; set; }
        [ExcelColumn("Kesinti Süre (sn)")] public double KesintiSuresiSN { get; set; }
        [ExcelColumn("Kesinti Süresi")] public string KesintiSuresi { get; set; }
        [ExcelColumn("Lokasyon")] public string Lokasyon { get; set; }
        [ExcelColumn("x")] public double XCoord { get; set; }
        [ExcelColumn("y")] public double YCoord { get; set; }

    }
}
