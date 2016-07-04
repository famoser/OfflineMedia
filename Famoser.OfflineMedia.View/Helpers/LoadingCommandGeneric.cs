using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Services.Interfaces;
using Famoser.OfflineMedia.View.Enums;
using GalaSoft.MvvmLight.Command;

namespace Famoser.OfflineMedia.View.Helpers
{
    public class LoadingCommandGeneric<T> : IDisposable
    {
        private readonly RelayCommand<T> _relayCommand;
        private readonly Action<bool> _booleanSetter;
        private readonly IndeterminateProgressKey _progressKey;
        private readonly IProgressService _progressService;

        public LoadingCommandGeneric(RelayCommand<T> command, Action<bool> booleanSetter, IndeterminateProgressKey key, IProgressService progressService)
        {
            _relayCommand = command;
            _booleanSetter = booleanSetter;
            _progressKey = key;
            _progressService = progressService;
        }

        public void Start()
        {
            _progressService.StartIndeterminateProgress(_progressKey);
            _booleanSetter(true);
            _relayCommand.RaiseCanExecuteChanged();
        }

        public void Dispose()
        {
            _progressService.StopIndeterminateProgress(_progressKey);
            _booleanSetter(false);
            _relayCommand.RaiseCanExecuteChanged();
        }

    }
}
