using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

namespace ArizaAnaliz
{
    public class ArizaAnalizExApp : IExtensionApplication
    {
        public static Document doc;
        public static Database db;
        public static Editor ed;
        public void Initialize()
        {
            doc = Application.DocumentManager.MdiActiveDocument;
            db = doc.Database;
            ed = doc.Editor;

            Commands.syncCtrl = new Control();
            Commands.syncCtrl.CreateControl();
            SetCultureTr();
            Application.Idle += Application_Idle;
        }


        public void Terminate()
        {
        }


        private static void SetCultureTr()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("tr-TR");
            // The following line provides localization for the application's user interface. 
            Thread.CurrentThread.CurrentUICulture = culture;

            // The following line provides localization for data formats. 
            Thread.CurrentThread.CurrentCulture = culture;
            // Set this culture as the default culture for all threads in this application. 
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        private void Application_Idle(object sender, EventArgs e)
        {

            if (ArizaAnalizRibbonMenu.Create().RibbonLoad() && Application.DocumentManager.Count > 0)
            {
                Application.Idle -= this.Application_Idle;
            }

        }
    }
}
