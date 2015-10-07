using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Lumia.Sense;
using OfflineMediaV3.Common.Framework.Singleton;

namespace OfflineMediaV3.InfoPage
{
    public class StepHelper : SingletonBase<StepHelper>
    {
        #region Init and Helpers
        private bool _isInitialized = false;
        private IStepCounter _stepCounter;

        public async Task Init()
        {
            _isSupported = await StepCounter.IsSupportedAsync();
            if (_isSupported)
            {
                _isSupported = (await CallSensorcoreApiAsync(async () =>
                {
                    if (_stepCounter == null)
                    {
                        // Get sensor instance if needed...
                        _stepCounter = await StepCounter.GetDefaultAsync();
                    }
                    else
                    {
                        // ... otherwise just activate it
                        await _stepCounter.ActivateAsync();
                    }
                }));
            }

            _isInitialized = true;
        }

        private bool _isSupported = true;
        private async Task<bool> CheckIfValid()
        {
            if (!_isInitialized)
                await Init();
            if (_isSupported)
            {
                return _stepCounter != null;
            }
            return false;
        }

        /// <summary> 
        /// Performs asynchronous SensorCore SDK operation and handles any exceptions 
        /// </summary> 
        /// <param name="action"></param> 
        /// <returns><c>true</c> if call was successful, <c>false</c> otherwise</returns> 
        private async Task<bool> CallSensorcoreApiAsync(Func<Task> action)
        {
            if (await CheckIfValid())
            {
                Exception failure = null;
                try
                {
                    await action();
                }
                catch (Exception e)
                {
                    failure = e;
                }
                if (failure != null)
                {
                    MessageDialog dialog;
                    switch (SenseHelper.GetSenseError(failure.HResult))
                    {
                        case SenseError.LocationDisabled:
                            dialog =
                                new MessageDialog(
                                    "Location has been disabled. Do you want to open Location settings now?",
                                    "Information");
                            dialog.Commands.Add(new UICommand("Yes",
                                async cmd => await SenseHelper.LaunchLocationSettingsAsync()));
                            dialog.Commands.Add(new UICommand("No"));
                            await dialog.ShowAsync();
                            new System.Threading.ManualResetEvent(false).WaitOne(500);
                            return false;
                        case SenseError.SenseDisabled:
                            dialog =
                                new MessageDialog(
                                    "Motion data has been disabled. Do you want to open Motion data settings now?",
                                    "Information");
                            dialog.Commands.Add(new UICommand("Yes",
                                async cmd => await SenseHelper.LaunchSenseSettingsAsync()));
                            dialog.Commands.Add(new UICommand("No"));
                            await dialog.ShowAsync();
                            new System.Threading.ManualResetEvent(false).WaitOne(500);
                            return false;
                        case SenseError.SensorNotAvailable:
                            dialog = new MessageDialog("The sensor is not supported on this device", "Information");
                            await dialog.ShowAsync();
                            new System.Threading.ManualResetEvent(false).WaitOne(500);
                            return false;
                        default:
                            dialog = new MessageDialog("Failure: " + SenseHelper.GetSenseError(failure.HResult), "");
                            await dialog.ShowAsync();
                            return false;
                    }
                }

                return true;
            }
            return false;
        }

        #endregion


        /// <summary>
        /// Check motion data settings
        /// </summary>
        public async Task<uint> GetStepsToday()
        {
            uint res = 0;
            if (await CheckIfValid())
            {
                var list = await _stepCounter.GetStepCountHistoryAsync(DateTimeOffset.Now, TimeSpan.FromDays(1));
                foreach (var stepCounterReading in list)
                {
                    res += stepCounterReading.WalkingStepCount;
                }
            }
            return res;
        }
    }
}
