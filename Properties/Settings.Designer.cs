﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Bu kod araç tarafından oluşturuldu.
//     Çalışma Zamanı Sürümü:4.0.30319.42000
//
//     Bu dosyada yapılacak değişiklikler yanlış davranışa neden olabilir ve
//     kod yeniden oluşturulursa kaybolur.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArizaAnaliz.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.8.1.0")]
    public sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Compiled\\ArızaAnaliz\\bin\\Debug\\CAD DATA\\okan_cesur_gunluk_aril_acma_raporu__(o" +
            "kan.cesur)_fgU7W.xlsx")]
        public string ArizaExcelPath {
            get {
                return ((string)(this["ArizaExcelPath"]));
            }
            set {
                this["ArizaExcelPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Compiled\\ArızaAnaliz\\bin\\Debug\\CAD DATA\\koordinat ve modem.xlsx")]
        public string KoordinatAndModemExcelPath {
            get {
                return ((string)(this["KoordinatAndModemExcelPath"]));
            }
            set {
                this["KoordinatAndModemExcelPath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0.0001")]
        public decimal SembolBoyutKatsayi {
            get {
                return ((decimal)(this["SembolBoyutKatsayi"]));
            }
            set {
                this["SembolBoyutKatsayi"] = value;
            }
        }
    }
}
