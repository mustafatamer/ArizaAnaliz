using ArizaAnaliz.Model;
using ArizaAnaliz.Views;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.Internal.Windows;
using LinqToExcel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using System.Threading;
using System.Windows.Media.Animation;

namespace ArizaAnaliz
{
    public class Commands
    {

        public static Control syncCtrl;
        public static readonly ExcelQueryFactory ImportedExcel = new ExcelQueryFactory(ArizaAnaliz.Properties.Settings.Default.ArizaExcelPath);
        private static PaletteSet ArizaImportPaletset = new PaletteSet("Ariza Analiz");
        public Commands()
        {
            arizaSembolDicList = new Dictionary<String, ObjectId>();
            arizaTextlDicList = new Dictionary<String, ObjectId>();
        }

        static Dictionary<String, ObjectId> arizaSembolDicList;
        public static Dictionary<String, ObjectId> ArizaSembolDicList
        {
            get => arizaSembolDicList;
            set => arizaSembolDicList = value;
        }

        static Dictionary<String, ObjectId> arizaTextlDicList;
        public static Dictionary<String, ObjectId> ArizaTextDicList
        {
            get => arizaTextlDicList;
            set => arizaTextlDicList = value;
        }

        public const double Boyutkatsayi = 0.0001d;
        [CommandMethod("ImportExcel")]
        public void ImportExcel()
        {
            List<ArizaExcelModel> getImportExcelModelList = ImportedExcel.Worksheet<ArizaExcelModel>(@"Rapor Sonuçları - 1").ToList();
            var AnalizList = getImportExcelModelList
                .GroupBy(x => new { x.ModemNumarasi, x.XCoord, x.YCoord, x.OzetLokasyon })
                .Select(x =>
                {
                    ArizaAnalizPivot newArizaAnalizPivot = new ArizaAnalizPivot();
                    newArizaAnalizPivot.ModemNumarasi = x.Key.ModemNumarasi;
                    newArizaAnalizPivot.OzetLokasyon = x.Key.OzetLokasyon;
                    newArizaAnalizPivot.ArizaSayisi = x.Count();
                    newArizaAnalizPivot.ArizaSuresi = x.Sum(s => s.KesintiSuresiSN);
                    newArizaAnalizPivot.xcoord = x.Key.XCoord;
                    newArizaAnalizPivot.ycoord = x.Key.YCoord;
                    return newArizaAnalizPivot;
                }).ToList();

            foreach (var item in AnalizList)
            {
                var circle = new Circle(new Point3d(item.xcoord, item.ycoord, 0), Vector3d.ZAxis, item.ArizaSayisi * Boyutkatsayi);
                circle.ColorIndex = item.ArizaSayisi;
                //var text1 =  Text(item.ModemNumarasi, 0.1, new Point3d(item.xcoord, item.ycoord, 0), 0, false);
                //text1.ColorIndex = item.ArizaSayisi;

                MText mText = new MText();
                mText.SetDatabaseDefaults();
                mText.TextHeight = 1 * Boyutkatsayi;
                mText.Contents = $"Modem Numarası       : {item.ModemNumarasi}\n"
                               + $"Ozet Lokasyon           : {item.OzetLokasyon} \n"
                               + $"Top.Arz Süresi/Sayısı : {item.ArizaSuresi} / {item.ArizaSayisi}";

                mText.ColorIndex = item.ArizaSayisi;
                mText.Location = new Point3d(item.xcoord, item.ycoord, 0);
                mText.AddToCurrentSpace();
                //text1.AddToCurrentSpace();
                circle.AddToCurrentSpace();

                AddToDistinctDic(ArizaSembolDicList, item.ModemNumarasi, circle.ObjectId);
                AddToDistinctDic(ArizaTextDicList, item.ModemNumarasi, mText.ObjectId);
            }

        }

        private void AddToDistinctDic(Dictionary<string, ObjectId> DicList, string ModemNo, ObjectId objectId)
        {
            if (DicList.ContainsKey(ModemNo))
            {
                DicList[ModemNo] = objectId;
            }
            else
            {
                DicList.Add(ModemNo, objectId);
            }
        }

       


        [CommandMethod("ProcessInBackground")]
        public void ProcessBackground()
        {
            //Diyelim ki çizimden bazı veriler aldık ve
            // şimdi bunu bir arka plan iş parçacığında işlemek istiyoruz
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Tick += Timer_Tick;
            System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(BackgroundProcess));
            thread.Start();
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("Arka plan işlemeye başladı. " + "Her zamanki gibi çalışmaya devam edebilirsiniz.\n");
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ArizaAnalizExApp.ed.Regen();
        }

        public void BackgroundProcess()
        {
            // Bu, arka plan sürecini temsil etmektir
            System.Threading.Thread.Sleep(5000);
            // Şimdi ana iş parçacığına yapılan çağrıyı sıralamamız gerekiyor
            // Bunun bu bağlamda nasıl yanlış olabileceğini görmüyorum,
            // ama yine de kontrol ediyorum

            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            {
                ObjectId mtextId;
                using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
                {
                    BlockTable bt = (BlockTable)tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead);
                    BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                    Line line = new Line(new Point3d(0, 0, 0), new Point3d(10, 10, 0));
                    MText mText = new MText();
                    mText.SetDatabaseDefaults();
                    mText.Contents = "deneme1";
                    ms.AppendEntity(line);
                    mtextId = ms.AppendEntity(mText);
                    tr.AddNewlyCreatedDBObject(mText, true);
                    tr.AddNewlyCreatedDBObject(line, true);
                    tr.Commit();

                }

                using (Transaction tr = doc.Database.TransactionManager.StartOpenCloseTransaction())
                {
                    MText mtext = tr.GetObject(mtextId, OpenMode.ForRead) as MText;
                    mtext.UpgradeOpen();
                    if (mtext == null) return;
                    for (int i = 0; i < 10000; i++)
                    {
                        mtext.Contents = "deneme" + i;
                        // mtext.Draw();

                        //doc.Editor.UpdateScreen();
                        //doc.Editor.WriteMessage(mtext.Contents);
                        System.Threading.Thread.Sleep(100);
                    }
                    tr.Commit();
                    timer.Stop();
                }
            }


            if (syncCtrl.InvokeRequired)
                syncCtrl.Invoke(new FinishedProcessingDelegate(FinishedProcessing));
            else
                FinishedProcessing();
        }


        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        delegate void FinishedProcessingDelegate();


        void FinishedProcessing()
        {


            // Veritabanını değiştirmek istiyorsak, session/application bağlamında olduğumuz için belgeyi kilitlememiz gerekir
            //Document doc = Application.DocumentManager.MdiActiveDocument;
            //using (doc.LockDocument())
            //{
            //    ObjectId mtextId;
            //    using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            //    {
            //        BlockTable bt = (BlockTable)tr.GetObject(doc.Database.BlockTableId, OpenMode.ForRead);
            //        BlockTableRecord ms = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
            //        Line line = new Line(new Point3d(0, 0, 0), new Point3d(10, 10, 0));
            //        MText mText = new MText();
            //        mText.SetDatabaseDefaults();
            //        mText.Contents = "deneme1";
            //        ms.AppendEntity(line);
            //        mtextId = ms.AppendEntity(mText);
            //        tr.AddNewlyCreatedDBObject(mText, true);

            //        tr.AddNewlyCreatedDBObject(line, true);
            //        tr.Commit();

            //    }

            //    using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            //    {
            //        MText mtext = tr.GetObject(mtextId, OpenMode.ForRead) as MText;
            //        mtext.UpgradeOpen();
            //        if (mtext == null) return;
            //        for (int i = 0; i < 5000; i++)
            //        {
            //            mtext.Contents = "deneme" + i;
            //            ArizaAnalizExApp.ed.Regen();
            //          //  Thread.Sleep(500);
            //        }
            //        tr.Commit();
            //    }
            //}

            //Ayrıca komut satırına bir mesaj yazın
            // Not: Otomatik CAD bildirim balonları kullanmak
            // daha güzel bir çözüm :)
            // TrayItem/TrayItemBubbleWindow
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
            ed.WriteMessage("Arka plan işlemini tamamladı!\n");
        }
    }
}
