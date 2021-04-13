using Microsoft.Toolkit.Uwp.UI.Controls;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace RentMyCPU.WebView.RuntimeComponent
{
    [AllowForWeb]
    public sealed class UwpBridgeService
    {
        public UwpBridgeService(CoreDispatcher dispatcher, Windows.UI.Xaml.Controls.WebView webview, ContentControl inAppNotification)
        {
            Webview = webview;
            InAppNotification = inAppNotification;
            Dispatcher = dispatcher;
        }

        public Windows.UI.Xaml.Controls.WebView Webview { get; }
        public ContentControl InAppNotification { get; }
        public CoreDispatcher Dispatcher { get; }

        public async void showNotification(string text)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                (InAppNotification as InAppNotification).Show(text, 5000);
            });
        }

        public string getDeviceInfos()
        {
            return JsonConvert.SerializeObject(new EasClientDeviceInformation());
        }

        public async void buyCredits(string apiKey)
        {
            var success = await InAppProductService.Instance.BuyAndFulfillCredits(apiKey);
            if (success)
            {
                await Webview.InvokeScriptAsync("buyResult", new string[] { });
            }
            else
            {
                await Webview.InvokeScriptAsync("buyFailure", new string[] { });
            }
        }
    }
}
