using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArizaAnaliz.Model
{
    public class ModemKoordinatExcelModel
    {
        public ModemKoordinatExcelModel()
        {
        }
        [ExcelColumn("GWY - Seri Numarası")] public string ModemNumarasi { get; set; }
        [ExcelColumn("GWY - Enlem")] public double ycoord { get; set; }
        [ExcelColumn("GWY - Boylam")] public double xcoord { get; set; }
        [ExcelColumn("GWY - Özet Lokasyon Bilgisi")] public string OzetLokasyon { get; set; }
        [ExcelColumn("GWY - Grup Bilgisi")] public string GrupBilgisi { get; set; }

    }
}
