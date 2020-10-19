using Atomus.Scanner.Barcode;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Atomus.Page.Scanner.Barcode.ViewModel
{
    public class DefaultBarcodeScanViewModel : MVVM.ViewModel
    {
        #region Declare
        private ICore core;

        List<BarcodeResult> list;
        string scanResult;
        private bool isEnabledControl;
        #endregion

        #region Property
        public ICore Core
        {
            get
            {
                return this.core;
            }
            set
            {
                this.core = value;
            }
        }

        public List<BarcodeResult> BarcodeResult
        {
            get
            {
                return this.list;
            }
            set
            {
                this.list = value;
            }
        }
        public string ScanResult
        {
            get
            {
                this.scanResult = string.Empty;
                foreach (BarcodeResult barcodeResult in this.list)
                    this.scanResult += $"Type : {barcodeResult.BarcodeValueFormat}, Value : {barcodeResult.DisplayValue}{Environment.NewLine}";

                return this.scanResult;
            }
            set
            {
                this.scanResult = value;
            }
        }
        public bool IsEnabledControl
        {
            get
            {
                return this.isEnabledControl;
            }
            set
            {
                if (this.isEnabledControl != value)
                {
                    this.isEnabledControl = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand BackCommand { get; set; }
        public ICommand FlashlightCommand { get; set; }
        public ICommand DetectedCommand { get; set; }
        #endregion

        #region INIT
        public DefaultBarcodeScanViewModel()
        {
        }
        public DefaultBarcodeScanViewModel(ICore core) : this()
        {
            this.Core = core;

            this.list = new List<BarcodeResult>();
            this.isEnabledControl = true;

            this.BackCommand = new Command(() => this.BackProcess()
                                            , () => { return this.IsEnabledControl; });

            this.FlashlightCommand = new Command(async () => await this.FlashlightProcess()
                                            , () => { return this.IsEnabledControl; });


            this.DetectedCommand = new Command(() => this.DetectedProcess()
                                            , () => { return this.IsEnabledControl; });
        }
        #endregion

        #region IO
        internal void BackProcess()
        {
            this.IsEnabledControl = false;
            (this.BackCommand as Command).ChangeCanExecute();

            (this.core as IAction).ControlAction(this, new AtomusPageArgs("Back", null));

            this.IsEnabledControl = true;
            (this.BackCommand as Command).ChangeCanExecute();
        }
        private async Task FlashlightProcess()
        {
            try
            {
                this.IsEnabledControl = false;
                (this.FlashlightCommand as Command).ChangeCanExecute();

                DependencyService.Get<IBarCodeCamera>()?.ToggleFlashlight();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", ex.Message, "OK");
            }
            finally
            {
                this.IsEnabledControl = true;
                (this.FlashlightCommand as Command).ChangeCanExecute();
            }
        }
        internal void DetectedProcess()
        {
            this.IsEnabledControl = false;
            (this.DetectedCommand as Command).ChangeCanExecute();

            (this.core as IAction).ControlAction(this, new AtomusPageArgs("Barcode.Detected", this.list));
            //await ((NavigationPage)Application.Current.MainPage).PopAsync();

            this.IsEnabledControl = true;
            (this.DetectedCommand as Command).ChangeCanExecute();
        }
        #endregion

        #region ETC
        #endregion
    }
}