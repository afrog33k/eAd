﻿#pragma checksum "..\..\..\..\Menu\RFIDDetected.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A0D207FFCAF4FB4FA814E8C7A1C4E47B"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace DesktopClient.Menu {
    
    
    /// <summary>
    /// RFIDDetected
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
    public partial class RFIDDetected : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\..\Menu\RFIDDetected.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal DesktopClient.Menu.RFIDDetected Control;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\Menu\RFIDDetected.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.Storyboard FormFade;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\..\Menu\RFIDDetected.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.DoubleAnimation FormFadeAnimation;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Menu\RFIDDetected.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.Storyboard FormFadeOut;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Menu\RFIDDetected.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.Animation.DoubleAnimation FormFadeOutAnimation;
        
        #line default
        #line hidden
        
        
        #line 41 "..\..\..\..\Menu\RFIDDetected.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid gameplayLayoutRoot;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/DesktopClient;component/menu/rfiddetected.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Menu\RFIDDetected.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.Control = ((DesktopClient.Menu.RFIDDetected)(target));
            return;
            case 2:
            this.FormFade = ((System.Windows.Media.Animation.Storyboard)(target));
            return;
            case 3:
            this.FormFadeAnimation = ((System.Windows.Media.Animation.DoubleAnimation)(target));
            return;
            case 4:
            this.FormFadeOut = ((System.Windows.Media.Animation.Storyboard)(target));
            
            #line 30 "..\..\..\..\Menu\RFIDDetected.xaml"
            this.FormFadeOut.Completed += new System.EventHandler(this.FormFadeOut_Completed);
            
            #line default
            #line hidden
            return;
            case 5:
            this.FormFadeOutAnimation = ((System.Windows.Media.Animation.DoubleAnimation)(target));
            return;
            case 6:
            this.gameplayLayoutRoot = ((System.Windows.Controls.Grid)(target));
            return;
            case 7:
            
            #line 43 "..\..\..\..\Menu\RFIDDetected.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
