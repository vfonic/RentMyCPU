using Newtonsoft.Json;
using RentMyCPU.WebView.RuntimeComponent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RentMyCPU.WebView
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public string ApiAddress => "https://rentmycpu.azurewebsites.net/";
        public UwpBridgeService UwpBridge { get; private set; }
        public MainPage()
        {
            this.InitializeComponent();
            UwpBridge = new UwpBridgeService(Dispatcher, MainWebView, MainAppNotification);
        } 

        private void MainWebView_NavigationStarting(Windows.UI.Xaml.Controls.WebView sender, WebViewNavigationStartingEventArgs args)
        {
            this.MainWebView.AddWebAllowedObject("uwpbridge", UwpBridge);
        }

        private void MainWebView_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            this.MainWebView.Visibility = Visibility.Visible;
        }
    }
}
