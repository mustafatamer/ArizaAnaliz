using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Linq;

namespace ArizaAnaliz
{
    public class ArizaAnalizPivot
    {
        public ArizaAnalizPivot()
        {
        }
        public ArizaAnalizPivot(ObjectId circleObjectId, ObjectId mtextObjectId, string modemNumarasi, string ozetLokasyon, int arizaSayisi, double arizaSuresi, double xcoord, double ycoord)
        {
            this.CircleObjectId = circleObjectId;
            this.MtextObjectId = mtextObjectId;
            ModemNumarasi = modemNumarasi;
            OzetLokasyon = ozetLokasyon;
            ArizaSayisi = arizaSayisi;
            ArizaSuresi = arizaSuresi;
            this.xcoord = xcoord;
            this.ycoord = ycoord;
        }

        public ObjectId CircleObjectId  { get; set; }
        public ObjectId MtextObjectId   { get; set; }
        public string   ModemNumarasi   { get; set; }
        public string   OzetLokasyon    { get; set; }
        public int      ArizaSayisi     { get; set; }
        public double   ArizaSuresi     { get; set; }
        public double   xcoord          { get; set; }
        public double   ycoord          { get; set; }
        public string BasTarihi { get; set; }
        public string BitTarihi { get; set; }
    }
}
