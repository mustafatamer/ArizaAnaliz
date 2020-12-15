using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ArizaAnaliz.Views;
using Autodesk.Windows;
using Kaynak = ArizaAnaliz.Properties.Resources;

namespace ArizaAnaliz
{
    public class ArizaAnalizRibbonMenu
    {
        private const string TabId = "ArizaAnalizMenu_TAB_ID";
        private const string Title = "ArizaAnaliz";
        private RibbonControl AcadRibbonControl = ComponentManager.Ribbon;
        private RibbonTab ArizaAnalizTab;//= new RibbonTab();
        public bool Loded { get; set; } = false;
        public static ArizaAnalizRibbonMenu Create()
        {
            return new ArizaAnalizRibbonMenu();
        }
        private ArizaAnalizRibbonMenu()
        {
            ArizaAnalizTab = new RibbonTab
            {
                Title = Title,
                Id = TabId
            };
        }

        public bool RibbonLoad()
        {
            try
            {

                #region Ribbon Load

                #region Ribbon Tab Load Control

                //RibbonTabLoadControl = false;

                RibbonTab fintab = AcadRibbonControl.FindTab(TabId);
                if (fintab == null)
                {
                    AcadRibbonControl.Tabs.Add(ArizaAnalizTab);
                    ArizaAnalizTab.IsActive = true;
                }
                else
                {
                    return true;
                }

                #endregion Ribbon Tab Load Control

                RibbonPanel RibPan1Promagis = new RibbonPanel();
               // RibbonPanel RibPan2Promagis = new RibbonPanel();
                RibbonRowPanel RibRowPan1Promagis = new RibbonRowPanel();
               // RibbonRowPanel RibRowPan2Promagis = new RibbonRowPanel();

                ArizaAnalizTab.Panels.Add(RibPan1Promagis); // Autocad Tab
               // ArizaAnalizTab.Panels.Add(RibPan2Promagis); // Autocad Tab
                RibbonPanelSource RibPanSrc1Promagis = new RibbonPanelSource();
                //RibbonPanelSource RibPanSrc2Promagis = new RibbonPanelSource();
                RibPanSrc1Promagis.Title = "Arıza Analiz";
                //RibPanSrc2Promagis.Title = "Ayarlar";
                RibPanSrc1Promagis.Items.Add(RibRowPan1Promagis);
                //RibPanSrc2Promagis.Items.Add(RibRowPan2Promagis);
                RibPan1Promagis.Source = RibPanSrc1Promagis;
                //RibPan2Promagis.Source = RibPanSrc2Promagis;

                #region Hat Ribbon Button Elemanının Tüm Özellikleri Ayarlanıyor

                PromagisRibbonButton RibBtn1 = new PromagisRibbonButton(true)
                {
                    Text = "Paneli Aç",
                    CommandParameter = "pp ",
                    Image = getBitmap(Kaynak.Settings),
                    LargeImage = getBitmap(Kaynak.Settings),
                    Description = "Aktarım Formunu ve Dosya Ayarları Formunu Açar",
                };
                RibBtn1.Clicked += RibBtn1_Clicked;
                RibRowPan1Promagis.Items.Add(RibBtn1);

                #endregion Hat Ribbon Button Elemanının Tüm Özellikleri Ayaralnıyor

                //#region Direk Ribbon Button Elemanının Tüm Özellikleri Ayarlanıyor

                //PromagisRibbonButton RibBtn2Direk = new PromagisRibbonButton(true);
                //RibBtn2Direk.Text = "Direk";
                //RibBtn2Direk.Image = getBitmap(Kaynak.Direk32);
                //RibBtn2Direk.LargeImage = getBitmap(Kaynak.Direk32);
                //RibBtn2Direk.Description = "Direk Çizim Formunu Açar";
                //RibBtn2Direk.Clicked += this.RibBtn2Direk_Clicked;
                //RibRowPan1Promagis.Items.Add(RibBtn2Direk);

                //#endregion Direk Ribbon Button Elemanının Tüm Özellikleri Ayarlanıyor

                //#region GPS Ribbon Button Elemanının Tüm Özellikleri Ayaralnıyor

                //PromagisRibbonButton RibBtn3GPS = new PromagisRibbonButton(true);
                //RibBtn3GPS.Text = "GPS";
                //RibBtn3GPS.Image = getBitmap(Kaynak.location);
                //RibBtn3GPS.LargeImage = getBitmap(Kaynak.location);
                //RibBtn3GPS.Description = "GPS Bağla";
                //RibBtn3GPS.Clicked += RibBtn3GPS_Clicked;
                //RibRowPan1Promagis.Items.Add(RibBtn3GPS);

                //#endregion GPS Ribbon Button Elemanının Tüm Özellikleri Ayaralnıyor

                //#region Ayarlar

                //PromagisRibbonButton RibBtn4Ayarlar = new PromagisRibbonButton(true);
                //RibBtn4Ayarlar.Text = "Ayarlar";
                //RibBtn4Ayarlar.CommandParameter = "AY ";
                //RibBtn4Ayarlar.Image = getBitmap(Kaynak.Settings);
                //RibBtn4Ayarlar.LargeImage = getBitmap(Kaynak.Settings);
                //RibBtn4Ayarlar.Description = "Ayarlar Formunu Açar";
                //RibBtn4Ayarlar.Clicked += RibBtn4Ayarlar_Clicked;
                //RibRowPan2Promagis.Items.Add(RibBtn4Ayarlar);


                //PromagisRibbonButton RibBtn5DurumTespit = new PromagisRibbonButton(true);
                //RibBtn5DurumTespit.Text = "DurumTespit";
                //RibBtn5DurumTespit.CommandParameter = "DR ";

                ////var imgs = DevExpress.Images.ImageResourceCache.Default.GetAllResourceKeys(); //.GetImage("images/actions/add_16x16.png");
                //// File.WriteAllLines("deneme.csv", imgs);
                //RibBtn5DurumTespit.LargeImage = GetBitmapImageFromStream(PromagisCad.Properties.PromagisCad_Images.images_reports_report_32x32_png); ; // getBitmap((Bitmap)DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/add_16x16.png"));
                //RibBtn5DurumTespit.Image = GetBitmapImageFromStream(PromagisCad.Properties.PromagisCad_Images.images_reports_report_16x16_png); ; // getBitmap((Bitmap)DevExpress.Images.ImageResourceCache.Default.GetImage("images/actions/add_16x16.png"));
                //RibBtn5DurumTespit.Description = "DurumTespit Formunu Açar";
                //RibBtn5DurumTespit.Clicked += RibBtn5DurumTespit_Clicked;
                //RibRowPan2Promagis.Items.Add(RibBtn5DurumTespit);
                //#endregion Ayarlar

                //RibbonItem rbitm1 = new RibbonItem();
                //rbitm1.Description = "deneme1";
                //rbitm1.ShowImage = true;
                //rbitm1.Text = "deneme1";
                //rbitm1.Image = getBitmap ( Kaynak.Direk32 );
                //rbitm1.ShowText = true;

                //RibbonCheckBox RibChcBx = new RibbonCheckBox();
                //RibChcBx.Description = "checkboxdeneme";
                //RibChcBx.ShowImage = true;
                //RibChcBx.Text = "checkboxdeneme";
                //RibChcBx.Image = getBitmap ( Kaynak.Kablo16 );
                //RibChcBx.ShowText = true;
                //RibChcBx.IsCheckable = true;
                //RibChcBx.IsChecked = true;

                //MyRibbonCombo RibCombo = new MyRibbonCombo();
                //RibCombo.Items.Add ( rbitm1 );
                //RibCombo.Items.Add ( RibChcBx );
                //RibCombo.Items.Add ( RibBtn1Hat );
                //RibCombo.Items.Add ( RibBtn2Direk );
                //RibCombo.Items.Add ( RibBtn3GPS );

                //RibbonListButton RibList = new   MyRibbonListButton();
                //RibList.Items.Add ( RibBtn1Hat );
                //RibList.Items.Add ( RibBtn2Direk );
                //RibList.Items.Add ( RibBtn3GPS );
                //RibList.Items.Add ( RibBtn4Ayarlar );
                //RibList.Width = 200;
                //RibList.Height = 200;
                //RibList.Text = "deneme";
                //RibList.ShowImage = true;
                //RibList.ShowText = true;
                //RibList.ListButtonStyle = Autodesk.Private.Windows.RibbonListButtonStyle.SplitButton;




                ArizaAnalizTab.IsActive = true;
                Loded = true;
                return true;
                #endregion Ribbon Load
            }
            catch (Exception)
            {
                Loded = false;
                return false;

            }
        }



        public void RibbonUnLoad()
        {
            ArizaAnalizTab = AcadRibbonControl.FindTab(TabId);
            if (ArizaAnalizTab != null)
            {
                AcadRibbonControl.Tabs.Remove(ArizaAnalizTab);
            }
        }

        private void RibBtn1_Clicked(object sender, EventArgs e)
        {
            ImportExcelView ımportExcelView = new ImportExcelView();

            Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(ımportExcelView);
            //if (Main.frmIletkenler2 != null)
            //{
            //    Main.frmIletkenler2.Activate();
            //    Main.frmIletkenler2.Visible = false;
            //    if (Main.frmIletkenler2.Visible == false)
            //    {
            //        Main.frmIletkenler2.Visible = true;
            //    }
            //}
            //else
            //{
            //    Main.frmIletkenler2 = new FrmIletkenler2();
            //    Main.frmIletkenler2.ShowDialog();
            //    //Application.ShowModelessDialog(Main.frmIletkenler2);
            //}
        }

        private void RibBtn2Direk_Clicked(object sender, EventArgs e)
        {
            //FrmDirekler frm = new FrmDirekler();
            //Application.ShowModalDialog(frm);
        }

        private void RibBtn3GPS_Clicked(object sender, EventArgs e)
        {
            //System.Windows.Forms.Form frmGPS = new FrmGPS();
            //Application.ShowModelessDialog(frmGPS);
        }

        private void RibBtn4Ayarlar_Clicked(object sender, EventArgs e)
        {
            //System.Windows.Forms.Form frmSettings = new FrmAyarlar();
            //Application.ShowModelessDialog(frmSettings);
        }
      //  static MainWindow mwMainWindow = null;
        //private void RibBtn5DurumTespit_Clicked(object sender, EventArgs e)
        //{
        //    if (mwMainWindow == null)
        //        mwMainWindow = new MainWindow();
        //    if (!mwMainWindow.IsLoaded)
        //        mwMainWindow = new MainWindow();

        //    if (!mwMainWindow.IsVisible)
        //    {
        //        Autodesk.AutoCAD.ApplicationServices.Application.ShowModelessWindow(mwMainWindow);
        //    }

        //}

        public class PromagisRibbonButton : RibbonButton, ICommand
        {
            public PromagisRibbonButton()
            {
                CommandHandler = this;
            }

            public PromagisRibbonButton(bool SetDefaultVlues)
            {
                CommandHandler = this;
                SetDefault();
            }
            public event EventHandler CanExecuteChanged;

            public event EventHandler Clicked;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Click()
            {
                InvokeClicked(EventArgs.Empty);
            }

            public void Execute(object parameter)
            {
                PromagisRibbonButton rbbtn = (PromagisRibbonButton)parameter;
                rbbtn.Click();
            }

            private void InvokeClicked(EventArgs e)
            {
                var handler = Clicked;
                if (handler != null)
                    handler(this, e);
            }

            public void SetDefault()
            {
                Width = 200;
                Height = 200;
                Orientation = System.Windows.Controls.Orientation.Vertical;
                ShowText = true;
                ShowImage = true;
                Size = RibbonItemSize.Large;
            }
        }

        public class PromagisRibbonCombo : RibbonCombo, ICommand
        {
            public PromagisRibbonCombo()
            {
                CommandHandler = this;

            }

            public event EventHandler CanExecuteChanged;

            public event EventHandler Clicked;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Click()
            {
                InvokeClicked(EventArgs.Empty);
            }

            public void Execute(object parameter)
            {
                PromagisRibbonCombo rbCombo = (PromagisRibbonCombo)parameter;
                rbCombo.Click();
            }

            private void InvokeClicked(EventArgs e)
            {

                var handler = Clicked;
                if (handler != null)
                    handler(this, e);
            }
        }

        public class PromagisRibbonListButton : RibbonListButton, ICommand
        {
            public PromagisRibbonListButton()
            {
                base.CommandHandler = this;
            }

            public event EventHandler CanExecuteChanged;

            public event EventHandler Clicked;

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Click()
            {
               this.InvokeClicked(EventArgs.Empty);
            }

            public void Execute(object parameter)
            {
                PromagisRibbonListButton rbListBtn = (PromagisRibbonListButton)parameter;
                rbListBtn.Click();
            }

            private void InvokeClicked(EventArgs e)
            {
                var handler = Clicked;
                if (handler != null)
                    handler(this, e);
            }
        }

        public static BitmapImage getBitmap(Bitmap image)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Png);
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();
            return bmp;
        }

        public static BitmapImage GetBitmapImageFromStream(UnmanagedMemoryStream stream)
        {
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = stream;
            bmp.EndInit();
            return bmp;
        }
    }
}
