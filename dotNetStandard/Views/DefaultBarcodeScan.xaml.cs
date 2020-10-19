using Atomus.Page.Scanner.Barcode.ViewModel;
using Atomus.Scanner.Barcode;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Atomus.Page.Scanner.Barcode
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefaultBarcodeScan : ContentPage, IAction
    {
        //public event ReturnBarcodeScannedEventHandler ReturnBarcodeScannedEvent;
        private AtomusPageEventHandler beforeActionEventHandler;
        private AtomusPageEventHandler afterActionEventHandler;

        #region INIT
        public DefaultBarcodeScan()
        {
            this.BindingContext = new DefaultBarcodeScanViewModel(this);

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            this.BackgroundColor = ((string)Config.Client.GetAttribute("BackgroundColor")).ToColor();
        }
        #endregion

        #region IO
        object IAction.ControlAction(ICore sender, AtomusPageArgs e)
        {
            try
            {
                this.beforeActionEventHandler?.Invoke(this, e);


                //switch (e.Action)
                //{
                //    case "Back":
                //        Navigation.PopModalAsync();
                //        break;

                //    case "Barcode.Detected":
                //        this.afterActionEventHandler?.Invoke(this, e);
                //        break;
                //}

                return true;
            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    this.afterActionEventHandler?.Invoke(this, e);
                });

                //if (e.Action != "Barcode.Detected")
            }
        }
        #endregion

        #region Event
        event AtomusPageEventHandler IAction.BeforeActionEventHandler
        {
            add
            {
                this.beforeActionEventHandler += value;
            }
            remove
            {
                this.beforeActionEventHandler -= value;
            }
        }
        event AtomusPageEventHandler IAction.AfterActionEventHandler
        {
            add
            {
                this.afterActionEventHandler += value;
            }
            remove
            {
                this.afterActionEventHandler -= value;
            }
        }

        private void CameraView_OnDetected(object sender, DetectedEventArg e)
        {
            (this.BindingContext as DefaultBarcodeScanViewModel).BarcodeResult = e.BarcodeResults;
            (this.BindingContext as DefaultBarcodeScanViewModel).DetectedCommand.Execute(e);
        }
        #endregion
    }
}