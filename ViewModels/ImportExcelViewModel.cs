using ArizaAnaliz.Model;
using ArizaAnaliz.Properties;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Ribbon;

using LinqToExcel;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ArizaAnaliz.ViewModels
{
    public class ImportExcelViewModel : INotifyPropertyChanged
    {
        public static readonly ExcelQueryFactory ArizaImportedExcel = new ExcelQueryFactory(ArizaAnaliz.Properties.Settings.Default.ArizaExcelPath);
        public static readonly ExcelQueryFactory ModemListImportedExcel = new ExcelQueryFactory(ArizaAnaliz.Properties.Settings.Default.KoordinatAndModemExcelPath);
        readonly List<ArizaExcelModel> getImportArizaExcelModelList = ArizaImportedExcel.Worksheet<ArizaExcelModel>(@"Rapor Sonuçları - 1")
           .ToList();
        readonly List<ModemKoordinatExcelModel> getImportModemKoordinatList = ModemListImportedExcel.Worksheet<ModemKoordinatExcelModel>(@"Rapor Sonuçları - 1")
           .ToList();

        public ImportExcelViewModel()
        {
            arizaExcelPath = Settings.Default.ArizaExcelPath;
            modemKoordinatExcelPath = Settings.Default.KoordinatAndModemExcelPath;
            SembolDicList = new Dictionary<String, ArizaAnalizPivot>();
        }

        public static ImportExcelViewModel Create() { return new ImportExcelViewModel(); }

        public double Boyutkatsayi { get; set; } = 0.0001d;


        static Dictionary<String, ArizaAnalizPivot> sembolDicList;

        public static Dictionary<String, ArizaAnalizPivot> SembolDicList
        {
            get => sembolDicList;
            set => sembolDicList = value;
        }

        public List<ArizaAnalizPivot> GetArizaAnalizPivots { get { return SembolDicList.Values.ToList(); } }

        private IList<DataGridCellInfo> selectedGridCellCollection = new List<DataGridCellInfo>();

        public IList<DataGridCellInfo> SelectedGridCellCollection
        {
            get { return selectedGridCellCollection; }
            set
            {
                selectedGridCellCollection = value; //.Select(x => x.Item).Distinct();
                NotifyOfPropertyChange(() => SelectedGridCellCollection);
            }
        }

        public List<ObjectId> SelectedArizaAnalizPivots
        {
            get
            {
                return (List<ObjectId>)SelectedGridCellCollection.GroupBy(x => new
                {
                    ((ArizaAnalizPivot)x.Item).CircleObjectId
                })
                    .Select((x => x.Key.CircleObjectId))
                    .ToList();
            }

        }

        public void ShowingDraw()
        {
            if ((SelectedArizaAnalizPivots == null) || (SelectedArizaAnalizPivots.Count == 0))
            {
                return;
            }

            ArizaAnalizExApp.doc.Editor.SetImpliedSelection(SelectedArizaAnalizPivots.Select(x => x).ToArray());
            ExtentionMethods.ZoomObjects(SelectedArizaAnalizPivots.Select(x => x).ToList(), 2);
            // StaticCommon.HighlightObjects(SelectedArizaAnalizPivots.Select(x => x).ToList());
        }


        public void Import()
        {
            var AnalizList = GetImportArizaExcelModelList
                .GroupBy(x => new { x.ModemNumarasi, x.OzetLokasyon, BasTarihi = x.BaslangisTarihi.Hour.ToString("00") + ":" + x.BaslangisTarihi.Minute.ToString("00"), BitTarihi = x.BitisTarihi.Date.ToShortDateString() })
                .Select(x =>
                {
                    ArizaAnalizPivot newArizaAnalizPivot = new ArizaAnalizPivot();
                    newArizaAnalizPivot.ModemNumarasi = x.Key.ModemNumarasi;
                    newArizaAnalizPivot.OzetLokasyon = x.Key.OzetLokasyon;
                    newArizaAnalizPivot.ArizaSayisi = x.Count();
                    newArizaAnalizPivot.ArizaSuresi = x.Sum(s => s.KesintiSuresiSN);
                    newArizaAnalizPivot.BasTarihi = x.Key.BasTarihi;
                    newArizaAnalizPivot.BitTarihi = x.Key.BitTarihi;
                    return newArizaAnalizPivot;
                })
                .ToList();

            AnalizList.ForEach(x =>
            {
                var temp = new ModemKoordinatExcelModel();
                temp = GetImportModemKoordinatExcelModelList.FirstOrDefault(f => f.ModemNumarasi == x.ModemNumarasi);
                if (temp != null)
                {
                    x.xcoord = temp.xcoord;
                    x.ycoord = temp.ycoord;
                }
                else
                {
                    x.xcoord = 0;
                    x.ycoord = 0;
                }
            });



            foreach (var item in AnalizList)
            {
                int colorindex = (int)((int.Parse(item.BasTarihi.Substring(0, 2)) * 60 + (int)(int.Parse(item.BasTarihi.Substring(3, 2)))) / 6);
                MText mText;
                Circle circle;
                if (SembolDicList.ContainsKey(item.ModemNumarasi))
                {
                    using (Transaction tr = ArizaAnalizExApp.doc.Database.TransactionManager.StartTransaction())
                    {
                        if (!SembolDicList[item.ModemNumarasi].MtextObjectId.IsErased)
                        {
                            mText = (MText)tr.GetObject(SembolDicList[item.ModemNumarasi].MtextObjectId, OpenMode.ForRead) as MText;
                        }
                        else
                        {
                            mText = new MText();
                            mText.SetDatabaseDefaults();
                            LockAndExecute(() =>
                            {
                                mText.AddToCurrentSpace();
                            });
                        }

                        LockAndExecute(() =>
                        {
                            mText.UpgradeOpen();

                            mText.TextHeight = 1 * Boyutkatsayi;
                            mText.Contents = $"Modem Numarası       : {item.ModemNumarasi}\n" +
                                $"Ozet Lokasyon           : {item.OzetLokasyon} \n" +
                                $"Top.Arz Süresi/Sayısı : {item.ArizaSuresi} / {item.ArizaSayisi} \n" +
                                $"Saat:" + item.BasTarihi;

                            mText.ColorIndex = colorindex;
                            mText.Location = new Point3d(item.xcoord, item.ycoord, 0);
                        });

                        if (!SembolDicList[item.ModemNumarasi].CircleObjectId.IsErased)
                        {
                            circle = tr.GetObject(SembolDicList[item.ModemNumarasi].CircleObjectId, OpenMode.ForRead) as Circle;
                        }
                        else
                        {
                            circle = new Circle();
                            circle.SetDatabaseDefaults();
                            LockAndExecute(() =>
                            {
                                circle.AddToCurrentSpace();
                            });
                        }

                        LockAndExecute(() =>
                        {
                            circle.UpgradeOpen();
                            circle.Center = new Point3d(item.xcoord, item.ycoord, 0);
                            circle.Radius = item.ArizaSayisi * Boyutkatsayi;
                            circle.ColorIndex = colorindex;
                        });


                        item.MtextObjectId = mText.ObjectId;
                        item.CircleObjectId = circle.ObjectId;
                        AddToDistinctDic(SembolDicList, item);
                        tr.Commit();
                        circle.Dispose();
                        mText.Dispose();
                    }

                }
                else
                {
                    circle = new Circle(new Point3d(item.xcoord, item.ycoord, 0),
                                      Vector3d.ZAxis,
                                      item.ArizaSayisi * Boyutkatsayi);
                    circle.ColorIndex = colorindex;

                    mText = new MText();
                    mText.SetDatabaseDefaults();
                    mText.TextHeight = 1 * Boyutkatsayi;
                    mText.Contents = $"Modem Numarası       : {item.ModemNumarasi}\n" +
                        $"Ozet Lokasyon           : {item.OzetLokasyon} \n" +
                        $"Top.Arz Süresi/Sayısı : {item.ArizaSuresi} / {item.ArizaSayisi} \n" +
                                $"Saat:" + item.BasTarihi;

                    mText.ColorIndex = colorindex;
                    mText.Location = new Point3d(item.xcoord, item.ycoord, 0);

                    LockAndExecute(() =>
                    {
                        mText.AddToCurrentSpace();
                        circle.AddToCurrentSpace();
                    });

                    item.MtextObjectId = mText.ObjectId;
                    item.CircleObjectId = circle.ObjectId;

                    AddToDistinctDic(SembolDicList, item);
                }
                NotifyOfPropertyChange(() => GetArizaAnalizPivots);
            }
        }


        public static void LockAndExecute(Action action)
        {
            using (var doclock = ArizaAnalizExApp.doc.LockDocument())
            {
                action();
            }
        }

        private void AddToDistinctDic(Dictionary<string, ArizaAnalizPivot> DicList, ArizaAnalizPivot arizaAnalizPivot)
        {
            if (DicList.ContainsKey(arizaAnalizPivot.ModemNumarasi))
            {
                DicList[arizaAnalizPivot.ModemNumarasi] = arizaAnalizPivot;
            }
            else
            {
                DicList.Add(arizaAnalizPivot.ModemNumarasi, arizaAnalizPivot);
            }
        }

        string arizaExcelPath;

        public string ArizaExcelPath
        {
            get => arizaExcelPath;
            set
            {
                if (arizaExcelPath == value)
                {
                    return;
                }

                arizaExcelPath = value;
                NotifyOfPropertyChange(() => ArizaExcelPath);
                ((DelegateCommand)ImportCommand).RaiseCanExecuteChanged();
            }
        }

        string modemKoordinatExcelPath;

        public string ModemKoordinatExcelPath
        {
            get => modemKoordinatExcelPath;
            set
            {
                if (modemKoordinatExcelPath == value)
                {
                    return;
                }

                modemKoordinatExcelPath = value;
                NotifyOfPropertyChange(() => ModemKoordinatExcelPath);
                ((DelegateCommand)KaydetCommand).RaiseCanExecuteChanged();
            }
        }

        public List<ArizaExcelModel> GetImportArizaExcelModelList
        {
            get { return getImportArizaExcelModelList; }
        }

        public List<ModemKoordinatExcelModel> GetImportModemKoordinatExcelModelList
        {
            get { return getImportModemKoordinatList; }
        }


        private ICommand importCommand;

        public ICommand ImportCommand
        {
            get
            {
                if (null == importCommand)
                {
                    importCommand = new DelegateCommand(Import, CanImport);
                }
                return importCommand;
            }
        }


        private ICommand kaydetCommand;

        public ICommand KaydetCommand
        {
            get
            {
                if (null == kaydetCommand)
                {
                    kaydetCommand = new DelegateCommand(Kaydet, CanKaydet);
                }
                return kaydetCommand;
            }
        }


        private ICommand showingDrawCommand;

        public ICommand ShowingDrawCommand
        {
            get
            {
                if (null == showingDrawCommand)
                {
                    showingDrawCommand = new DelegateCommand(ShowingDraw, (SelectedArizaAnalizPivots.Count() > 0));
                }
                return showingDrawCommand;
            }
        }
        public AyarlarViewModel ayarlarViewModel = AyarlarViewModel.Create();

        private ICommand changeArizaExcelPathCommand;

        public ICommand ChangeArizaExcelPathCommand
        {
            get
            {
                if (null == changeArizaExcelPathCommand)
                {
                    changeArizaExcelPathCommand = new DelegateCommand(ArizaExcelPathChange, CanArizaExcelPathChange);
                }
                return changeArizaExcelPathCommand;
            }
        }


        public void ArizaExcelPathChange()
        {
            ArizaExcelPath = ayarlarViewModel.GetFilesDialog(false)[0];
            NotifyOfPropertyChange(() => ArizaExcelPath);
        }

        public bool CanArizaExcelPathChange(object parameter)
        {
            return true;
        }


        private ICommand changeModemKoordinatExcelPathCommand;

        public ICommand ChangeModemKoordinatExcelPathCommand
        {
            get
            {
                if (null == changeModemKoordinatExcelPathCommand)
                {
                    changeModemKoordinatExcelPathCommand = new DelegateCommand(ModemKoordinatExcelPathChange, CanModemKoordinatExcelPathChange);
                }
                return changeModemKoordinatExcelPathCommand;
            }
        }


        public void ModemKoordinatExcelPathChange()
        {
            ModemKoordinatExcelPath = ayarlarViewModel.GetFilesDialog(false)[0];
            NotifyOfPropertyChange(() => ModemKoordinatExcelPath);
        }

        public bool CanModemKoordinatExcelPathChange(object parameter)
        {
            return true;
        }




        public void Kaydet()
        {
            Settings.Default.ArizaExcelPath = arizaExcelPath;
            Settings.Default.KoordinatAndModemExcelPath = ModemKoordinatExcelPath;
            Settings.Default.Save();
        }
        public bool CanKaydet(object param)
        { return File.Exists(arizaExcelPath) & File.Exists(modemKoordinatExcelPath); }

        public bool CanImport(object param)
        { return File.Exists(arizaExcelPath) & File.Exists(modemKoordinatExcelPath); }

        #region Property Notification
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyOfPropertyChange<TValue>(Expression<Func<TValue>> propertySelector)
        {
            if (PropertyChanged != null)
            {
                var memberExpression = propertySelector.Body as MemberExpression;
                if (memberExpression != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(memberExpression.Member.Name));
                }
            }
        }
        #endregion
    }
}