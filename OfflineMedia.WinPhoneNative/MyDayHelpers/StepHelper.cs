using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Lumia.Sense;
using OfflineMedia.Common.Framework.Singleton;

namespace OfflineMedia.MyDayHelpers
{
    public class StepHelper : SingletonBase<StepHelper>
    {
        #region Init and Helpers
        private IStepCounter _stepCounter;

        private bool _isInitialized = false;

        private async Task<bool> CheckIfValid()
        {
            if (!_isInitialized)
            {
                var supp = await StepCounter.IsSupportedAsync();
                if (supp)
                {
                    await CallSensorcoreApiAsync(async () =>
                    {
                        _stepCounter = await StepCounter.GetDefaultAsync();
                        await _stepCounter.ActivateAsync();
                    });
                }
                _isInitialized = true;
            }

            return _stepCounter != null;
        }

        /// <summary> 
        /// Performs asynchronous SensorCore SDK operation and handles any exceptions 
        /// </summary> 
        /// <param name="action"></param> 
        /// <returns><c>true</c> if call was successful, <c>false</c> otherwise</returns> 
        private async Task<bool> CallSensorcoreApiAsync(Func<Task> action)
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
                        //dialog = new MessageDialog("The sensor is not supported on this device", "Information");
                        //await dialog.ShowAsync();
                        //new System.Threading.ManualResetEvent(false).WaitOne(500);
                        return false;
                    default:
                        //dialog = new MessageDialog("Failure: " + SenseHelper.GetSenseError(failure.HResult), "");
                        //await dialog.ShowAsync();
                        return false;
                }
            }

            return true;
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
                var steps = await _stepCounter.GetStepCountHistoryAsync(DateTime.Now.Date, DateTime.Now - DateTime.Now.Date);
                foreach (var stepCounterReading in steps)
                {
                    res += stepCounterReading.WalkingStepCount;
                    res += stepCounterReading.RunningStepCount;
                }
                return res;
            }
            return res;
        }
    }
}
