using System.Windows;

namespace ArizaAnaliz.Views
{
    /// <summary>
    /// InportExcelView.xaml etkileşim mantığı
    /// </summary>
    public partial class ImportExcelView : Window
    {
        public ImportExcelView()
        {
            this.LoadViewFromUri("pack://application:,,,/ArizaAnaliz;component/views/importexcelview.xaml");
            //InitializeComponent();
        }
    }
}
