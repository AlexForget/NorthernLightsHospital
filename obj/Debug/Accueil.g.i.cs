﻿#pragma checksum "..\..\Accueil.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "658BD81FF104F05CB69AF6DBDDA3246719073F3822A367933D95852456318236"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using NorthernLightsHospital;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace NorthernLightsHospital {
    
    
    /// <summary>
    /// Accueil
    /// </summary>
    public partial class Accueil : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal NorthernLightsHospital.Accueil Login;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image imgLogin;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblConnexion;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblNomUtilisateur;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblMotDePasse;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox txtNomUtilisateur;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox txtMotDePasse;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnConnexion;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\Accueil.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAnnuler;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/NorthernLightsHospital;component/accueil.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Accueil.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Login = ((NorthernLightsHospital.Accueil)(target));
            
            #line 8 "..\..\Accueil.xaml"
            this.Login.Loaded += new System.Windows.RoutedEventHandler(this.Login_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.imgLogin = ((System.Windows.Controls.Image)(target));
            return;
            case 3:
            this.lblConnexion = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblNomUtilisateur = ((System.Windows.Controls.Label)(target));
            return;
            case 5:
            this.lblMotDePasse = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.txtNomUtilisateur = ((System.Windows.Controls.TextBox)(target));
            return;
            case 7:
            this.txtMotDePasse = ((System.Windows.Controls.PasswordBox)(target));
            return;
            case 8:
            this.btnConnexion = ((System.Windows.Controls.Button)(target));
            
            #line 18 "..\..\Accueil.xaml"
            this.btnConnexion.Click += new System.Windows.RoutedEventHandler(this.btnConnexion_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnAnnuler = ((System.Windows.Controls.Button)(target));
            
            #line 19 "..\..\Accueil.xaml"
            this.btnAnnuler.Click += new System.Windows.RoutedEventHandler(this.btnAnnuler_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
