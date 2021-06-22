﻿#pragma checksum "..\..\..\..\View\Overlay.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "17FA5D315E4FB5897E5E39680D42531CF55A08BF"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using PCon.View;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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
using Vlc.DotNet.Wpf;


namespace PCon.View {
    
    
    /// <summary>
    /// Overlay
    /// </summary>
    public partial class Overlay : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 341 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel ComplexPanel;
        
        #line default
        #line hidden
        
        
        #line 343 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label TimeShow;
        
        #line default
        #line hidden
        
        
        #line 346 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider VideoSlider;
        
        #line default
        #line hidden
        
        
        #line 350 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel PlayerPanel;
        
        #line default
        #line hidden
        
        
        #line 351 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Play;
        
        #line default
        #line hidden
        
        
        #line 352 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button Pause;
        
        #line default
        #line hidden
        
        
        #line 353 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Slider VolumeSlider;
        
        #line default
        #line hidden
        
        
        #line 361 "..\..\..\..\View\Overlay.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Vlc.DotNet.Wpf.VlcControl Player;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/PCon;V1.0.0.0;component/view/overlay.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\View\Overlay.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.8.1.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 11 "..\..\..\..\View\Overlay.xaml"
            ((PCon.View.Overlay)(target)).MouseEnter += new System.Windows.Input.MouseEventHandler(this.Overlay_OnMouseEnter);
            
            #line default
            #line hidden
            
            #line 11 "..\..\..\..\View\Overlay.xaml"
            ((PCon.View.Overlay)(target)).MouseLeave += new System.Windows.Input.MouseEventHandler(this.Overlay_OnMouseLeave);
            
            #line default
            #line hidden
            return;
            case 2:
            this.ComplexPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 3:
            this.TimeShow = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.VideoSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 347 "..\..\..\..\View\Overlay.xaml"
            this.VideoSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.Slider_ValueChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.PlayerPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.Play = ((System.Windows.Controls.Button)(target));
            
            #line 351 "..\..\..\..\View\Overlay.xaml"
            this.Play.Click += new System.Windows.RoutedEventHandler(this.Button_Play);
            
            #line default
            #line hidden
            return;
            case 7:
            this.Pause = ((System.Windows.Controls.Button)(target));
            
            #line 352 "..\..\..\..\View\Overlay.xaml"
            this.Pause.Click += new System.Windows.RoutedEventHandler(this.Button_Pause);
            
            #line default
            #line hidden
            return;
            case 8:
            this.VolumeSlider = ((System.Windows.Controls.Slider)(target));
            
            #line 354 "..\..\..\..\View\Overlay.xaml"
            this.VolumeSlider.ValueChanged += new System.Windows.RoutedPropertyChangedEventHandler<double>(this.VolumeSlider_OnValueChanged);
            
            #line default
            #line hidden
            return;
            case 9:
            this.Player = ((Vlc.DotNet.Wpf.VlcControl)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

