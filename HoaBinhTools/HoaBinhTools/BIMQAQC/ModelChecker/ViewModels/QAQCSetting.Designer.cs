﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HoaBinhTools.BIMQAQC.ModelChecker.ViewModels {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.2.0.0")]
    internal sealed partial class QAQCSetting : global::System.Configuration.ApplicationSettingsBase {
        
        private static QAQCSetting defaultInstance = ((QAQCSetting)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new QAQCSetting())));
        
        public static QAQCSetting Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsAutoRun {
            get {
                return ((bool)(this["IsAutoRun"]));
            }
            set {
                this["IsAutoRun"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool IsGGsheet {
            get {
                return ((bool)(this["IsGGsheet"]));
            }
            set {
                this["IsGGsheet"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TimeTrigers {
            get {
                return ((string)(this["TimeTrigers"]));
            }
            set {
                this["TimeTrigers"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string DatesApps {
            get {
                return ((string)(this["DatesApps"]));
            }
            set {
                this["DatesApps"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TimeApps {
            get {
                return ((string)(this["TimeApps"]));
            }
            set {
                this["TimeApps"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> ListFileAutoRun {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["ListFileAutoRun"]));
            }
            set {
                this["ListFileAutoRun"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool IsFileSelect {
            get {
                return ((bool)(this["IsFileSelect"]));
            }
            set {
                this["IsFileSelect"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime ExecuteTime {
            get {
                return ((global::System.DateTime)(this["ExecuteTime"]));
            }
            set {
                this["ExecuteTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string FolderResult {
            get {
                return ((string)(this["FolderResult"]));
            }
            set {
                this["FolderResult"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> Q_Category {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["Q_Category"]));
            }
            set {
                this["Q_Category"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> Q_Criteria {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["Q_Criteria"]));
            }
            set {
                this["Q_Criteria"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> Q_Property {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["Q_Property"]));
            }
            set {
                this["Q_Property"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> Q_Condition {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["Q_Condition"]));
            }
            set {
                this["Q_Condition"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Collections.Generic.List<System.String> Q_Value {
            get {
                return ((global::System.Collections.Generic.List<System.String>)(this["Q_Value"]));
            }
            set {
                this["Q_Value"] = value;
            }
        }
    }
}