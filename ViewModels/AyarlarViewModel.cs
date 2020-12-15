using ArizaAnaliz.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArizaAnaliz
{

    public class AyarlarViewModel
    {
        public Settings ArizaAnalizSettings { get { return Settings.Default; } set { } }
        public static AyarlarViewModel Create()
        {
            return new AyarlarViewModel();
        }

        private AyarlarViewModel()
        {
        }

        public void SaveConfig()
        {
            ArizaAnalizSettings.Save();
        }

        public void ResetConfig()
        {
            ArizaAnalizSettings.Reset();
        }

        public void ReloadConfig()
        {
            ArizaAnalizSettings.Reload();
        }

        public string PachSelect()
        {
            DialogResult result;
            string Path;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                result = dialog.ShowDialog();
                Path = dialog.SelectedPath;
            }
            if (result == DialogResult.OK)
                return Path;
            return "";
        }


        public StringCollection GetFilesDialog(bool multiselect, string FilterString = ".XLSX | *.XLSX")
        {
            StringCollection FileNameCollection = new StringCollection();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = multiselect;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = FilterString;
            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                try
                {
                    FileNameCollection.AddRange(openFileDialog.FileNames);
                    // openFileDialog.FileNames.ForEach(x =>
                    // {
                    ////if (x.Contains("DURUMTESPIT"))
                    ////{
                    //  FileNameCollection.Add(x);
                    ////};
                    // });
                }
                catch (SecurityException ex)
                {
                    MessageBox.Show("Güvelik Kısıtlaması. Detaylar Için Lütfen Sistem Yöneticinizle Irtibata Geçiniz.\n\n" +
                        "Hata Mesajı: " + ex.Message + "\n\n" +
                        "Detaylar :\n\n" + ex.StackTrace
                    );
                    return null;
                }
                // GetDurumTespitList  = PromagisStockRoomMainModel.DurumTespit.Worksheet<DurumTespit>(@"DURUM TESPİT").ToList();
            }
            return FileNameCollection;
        }

        public void SetImportExcelPath()
        {
            try
            {
                string dosyaYolu = GetFilesDialog(false, "Excel Dosyası | *.xlsx;*.xls")[0];
                if (File.Exists(dosyaYolu))
                {
                    ArizaAnalizSettings.ArizaExcelPath = dosyaYolu;
                }
                else
                {
                    MessageBox.Show("Seçilen Dosya Yok." ,"Dosya Yok",MessageBoxButtons.OK,MessageBoxIcon.Information);
                }
                return;
            }
            catch (Exception)
            {

            }
        }

    }
}
