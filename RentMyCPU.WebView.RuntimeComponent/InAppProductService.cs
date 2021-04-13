using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Storage;

namespace RentMyCPU.WebView.RuntimeComponent
{
    public sealed class InAppProductService
    {
        private static InAppProductService _instance;
        public static InAppProductService Instance => _instance ?? (_instance = new InAppProductService());

        public IAsyncOperation<bool> BuyAndFulfillCredits(string token)
        {
            return AsyncInfo.Run(async cancellationToken =>
            {
                try
                {
#if DEBUG
                    StorageFile proxyFile = await Package.Current.InstalledLocation.GetFileAsync("in-app-purchases-list.xml");
                    await CurrentAppSimulator.ReloadSimulatorAsync(proxyFile);
                    PurchaseResults purchaseResults = await CurrentAppSimulator.RequestProductPurchaseAsync("Credits");
#else
                    PurchaseResults purchaseResults = await CurrentApp.RequestProductPurchaseAsync("Credits");
#endif
                    switch (purchaseResults.Status)
                    {
                        case ProductPurchaseStatus.Succeeded:
                            return await GrantFeatureInternal(purchaseResults, token)
                                && await FulfillProductInternal(purchaseResults.TransactionId);
                        case ProductPurchaseStatus.NotFulfilled:
                            var result = true;
                            if (!await IsGrantedInternal(purchaseResults.TransactionId, token))
                            {
                                result = await GrantFeatureInternal(purchaseResults, token);
                            }
                            return result && await FulfillProductInternal(purchaseResults.TransactionId);
                        case ProductPurchaseStatus.NotPurchased:
                            return false;
                        default:
                            return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }


        private IAsyncOperation<bool> FulfillProductInternal(Guid transactionId)
        {
            return AsyncInfo.Run(async cancellationToken =>
            {
                try
                {
#if DEBUG
                    FulfillmentResult result = await CurrentAppSimulator.ReportConsumableFulfillmentAsync("Credits", transactionId);
#else
                    FulfillmentResult result = await CurrentApp.ReportConsumableFulfillmentAsync("Credits", transactionId);
                    Analytics.TrackEvent("credits.fulfillement", new Dictionary<string, string> { ["result"] = result.ToString() });
#endif
                    switch (result)
                    {
                        case FulfillmentResult.Succeeded:
                            return true;
                        case FulfillmentResult.NothingToFulfill:
                            return true;
                        case FulfillmentResult.PurchasePending:
                            return true;
                        case FulfillmentResult.PurchaseReverted:
                            return true;
                        case FulfillmentResult.ServerError:
                            return true;
                        default:
                            return false;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }

        private IAsyncOperation<bool> GrantFeatureInternal(PurchaseResults purchaseResults, string token)
        {
            return AsyncInfo.Run(async cancellationToken =>
            {
                try
                {
                    var result = await ApiService.Instance.AddCredits(purchaseResults.TransactionId, purchaseResults.ReceiptXml, token);
                    return result.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            });
        }

        private IAsyncOperation<bool> IsGrantedInternal(Guid transactionId, string token)
        {
            return AsyncInfo.Run(async cancellationToken =>
            {
                try
                {
                    var result = await ApiService.Instance.IsPurchaseFulfilled(transactionId, token);
                    return result.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            });
        }
    }
}
