using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows.Input;
using Famoser.FrameworkEssentials.Logging;
using Famoser.OfflineMedia.Business.Enums.Models;
using Famoser.OfflineMedia.Business.Models;
using Famoser.OfflineMedia.Business.Models.Configuration;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Business.Services;
using Famoser.OfflineMedia.Data.Enums;
using Famoser.OfflineMedia.View.Enums;
using Famoser.OfflineMedia.View.Helpers;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Famoser.OfflineMedia.View.ViewModels
{
    public class ArticlePageViewModel : ViewModelBase
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IPlatformCodeService _platformCodeService;

        public ArticlePageViewModel(ISettingsRepository settingsRepository, IArticleRepository articleRepository, IPlatformCodeService platformCodeService)
        {
            _settingsRepository = settingsRepository;
            _articleRepository = articleRepository;
            _platformCodeService = platformCodeService;

            _setDisplayState = new RelayCommand<DisplayState>(SetDisplayState);

            _makeFontBiggerCommand = new RelayCommand(MakeFontBigger, () => CanMakeFontBigger);
            _makeFontSmallerCommand = new RelayCommand(MakeFontSmaller, () => CanMakeFontSmaller);
            _favoriteCommand = new RelayCommand(Favorite);
            _openInBrowserCommand = new RelayCommand(OpenInBrowser, () => CanOpenInBrowser);
            _reloadArticleCommand = new RelayCommand(ReloadArticle, () => CanReloadArticle);

            _goToStartCommand = new RelayCommand(GoToStart, () => CanGoToStart);
            _goLeftCommand = new RelayCommand(GoLeft, () => CanGoLeft);
            _goRightCommand = new RelayCommand(GoRight, () => CanGoRight);

            _startCommand = new RelayCommand(Start, () => CanStart);

            _increaseSpeedCommand = new RelayCommand(IncreaseSpeed, () => CanIncreaseSpeed);
            _decreaseSpeedCommand = new RelayCommand(DecreaseSpeed, () => CanDecreaseSpeed);

            if (IsInDesignMode)
            {
                FontSize = 20;
                ReadingSpeed = 300;
                SelectArticle(_articleRepository.GetInfoArticle());
            }
            else
            {
                Initialize();
            }
        }

        public async void SelectArticle(ArticleModel am)
        {
            SetDisplayState(DisplayState.Article);
            Article = am;
            await _articleRepository.LoadFullArticleAsync(am);
            await _articleRepository.MarkArticleAsReadAsync(am);
            InitializeSpritz();
        }

        private async void Initialize()
        {
            var fontSizeSetting = (await _settingsRepository.GetSettingByKeyAsync(SettingKey.FontSize)) as IntSettingModel;
            FontSize = fontSizeSetting?.IntValue ?? 20;

            var wordsPerMinute = (await _settingsRepository.GetSettingByKeyAsync(SettingKey.WordsPerMinute)) as IntSettingModel;
            ReadingSpeed = wordsPerMinute?.IntValue ?? 20;
        }

        private ArticleModel _article;
        public ArticleModel Article
        {
            get { return _article; }
            set
            {
                var oldarticle = _article;
                if (Set(ref _article, value))
                {
                    if (_article != null)
                        _article.PropertyChanged += ArticleOnPropertyChanged;

                    if (oldarticle != null)
                        oldarticle.PropertyChanged -= ArticleOnPropertyChanged;

                    _reloadArticleCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(() => SelectedArticle);
                }
            }
        }

        public ArticleModel SelectedArticle
        {
            get { return null; }
            set
            {
                if (value != null)
                {
                    SelectArticle(value);
                }
            }
        }

        private void ArticleOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == "LoadingState")
                _reloadArticleCommand.RaiseCanExecuteChanged();
        }


        private DisplayState _displayState;
        public DisplayState DisplayState
        {
            get { return _displayState; }
            set
            {
                if (Set(ref _displayState, value))
                {
                    RaisePropertyChanged(() => DisplayStateIndex);
                }
            }
        }

        public int DisplayStateIndex
        {
            get { return (int)_displayState; }
            set
            {
                var newdisplaystatae = (DisplayState)value;
                if (Set(ref _displayState, newdisplaystatae))
                {
                    RaisePropertyChanged(() => DisplayState);
                }
            }
        }

        #region SetStateCommand
        private readonly RelayCommand<DisplayState> _setDisplayState;
        public ICommand SetDisplayStateCommand => _setDisplayState;

        private void SetDisplayState(DisplayState state)
        {
            DisplayState = state;
        }

        public DisplayState DisplayStateArticle => DisplayState.Article;

        public DisplayState DisplayStateSpritz => DisplayState.Spritz;

        public DisplayState DisplayStateInfo => DisplayState.Info;

        #endregion

        #region Article View

        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                if (Set(ref _fontSize, value))
                {
                    SafeFontSize();
                    _makeFontBiggerCommand.RaiseCanExecuteChanged();
                    _makeFontSmallerCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async void SafeFontSize()
        {
            var setting = await _settingsRepository.GetSettingByKeyAsync(SettingKey.FontSize) as IntSettingModel;
            if (setting != null)
            {
                setting.IntValue = FontSize;
                await _settingsRepository.SaveSettingsAsync();
            }
        }

        #region MakeFontBiggerCommand
        private readonly RelayCommand _makeFontBiggerCommand;
        public ICommand MakeFontBiggerCommand => _makeFontBiggerCommand;

        private bool CanMakeFontBigger => FontSize < 40;

        private void MakeFontBigger()
        {
            FontSize += 2;
        }
        #endregion

        #region MakeFontSmallerCommand
        private readonly RelayCommand _makeFontSmallerCommand;
        public ICommand MakeFontSmallerCommand => _makeFontSmallerCommand;

        private bool CanMakeFontSmaller => FontSize > 5;

        private void MakeFontSmaller()
        {
            FontSize -= 2;
        }
        #endregion

        #region FavoriteCommand
        private readonly RelayCommand _favoriteCommand;
        public ICommand FavoriteCommand => _favoriteCommand;

        private void Favorite()
        {
            _articleRepository.SetArticleFavoriteStateAsync(Article, !Article.IsFavorite);
        }
        #endregion

        #endregion

        #region Spritz View

        private int _readingSpeed;
        public int ReadingSpeed
        {
            get { return _readingSpeed; }
            set
            {
                if (Set(ref _readingSpeed, value))
                {
                    SafeReadingSpeed();
                    _decreaseSpeedCommand.RaiseCanExecuteChanged();
                    _increaseSpeedCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private async void SafeReadingSpeed()
        {
            var setting = await _settingsRepository.GetSettingByKeyAsync(SettingKey.WordsPerMinute) as IntSettingModel;
            if (setting != null)
            {
                setting.IntValue = ReadingSpeed;
                await _settingsRepository.SaveSettingsAsync();
            }
        }

        private string _beforeText = "S";
        public string BeforeText
        {
            get { return _beforeText; }
            set { Set(ref _beforeText, value); }
        }

        private char _middleText = 'p';
        public char MiddleText
        {
            get { return _middleText; }
            set { Set(ref _middleText, value); }
        }

        private string _afterText = "ritz";
        public string AfterText
        {
            get { return _afterText; }
            set { Set(ref _afterText, value); }
        }

        public void DisplayWord(SpritzWord sw)
        {
            _platformCodeService.CheckBeginInvokeOnUi(() =>
            {
                BeforeText = sw.Before;
                MiddleText = sw.Middle;
                AfterText = sw.After;
            });
        }

        private bool _isSpritzReady;

        private int _activeIndexSave;
        private int ActiveIndex
        {
            get { return _activeIndexSave; }
            set
            {
                if (Set(ref _activeIndexSave, value))
                {
                    RaisePropertyChanged(() => ActiveWord);
                }
            }
        }
        public int ActiveWord => _spritzWords != null ? _activeIndexSave + 1 : 0;

        public int TotalWords => _spritzWords?.Count ?? 0;

        private List<SpritzWord> _spritzWords;
        private void InitializeSpritz()
        {
            try
            {
                _isSpritzReady = false;
                ActiveIndex = 0;
                _spritzState = SpritzState.Ready;
                _spritzWords = new List<SpritzWord>();

                DisplayWord(new SpritzWord()
                {
                    After = "ding",
                    Before = "Lo",
                    Middle = 'a',
                    Lenght = 1000
                });
                _platformCodeService.CheckBeginInvokeOnUi(() =>
                {
                    _goLeftCommand.RaiseCanExecuteChanged();
                    _goRightCommand.RaiseCanExecuteChanged();
                    _goToStartCommand.RaiseCanExecuteChanged();
                    _startCommand.RaiseCanExecuteChanged();
                    RaisePropertyChanged(() => TotalWords);
                });

                //inititialize Wordlists
                if (Article.Content != null && Article.Content.Any())
                {
                    _spritzWords = SpritzHelper.GenerateList(Article.Content);

                    //prepare User Interface
                    if (_spritzWords != null && _spritzWords.Count > 0)
                    {
                        _platformCodeService.CheckBeginInvokeOnUi(() =>
                        {
                            _goLeftCommand.RaiseCanExecuteChanged();
                            _goRightCommand.RaiseCanExecuteChanged();
                            RaisePropertyChanged(() => TotalWords);
                        });

                        DisplayWord(_spritzWords[0]);

                        _isSpritzReady = true;
                        _platformCodeService.CheckBeginInvokeOnUi(() =>
                        {
                            _startCommand.RaiseCanExecuteChanged();
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Log(LogLevel.FatalError, "Spritz failed", this, ex);
            }
        }

        private async void DisplayNextWord()
        {
            if (_spritzWords == null)
                _spritzWords = SpritzHelper.GenerateList(Article.Content);

            int delay = (60 * 1000) / ReadingSpeed;

            if (SpritzState == SpritzState.Running && ActiveIndex < _spritzWords.Count)
            {
                if (++ActiveIndex < _spritzWords.Count)
                {
                    DisplayWord(_spritzWords[ActiveIndex]);
                    await Task.Delay(delay);
                    DisplayNextWord();
                }
                else
                {
                    SpritzState = SpritzState.Finished;
                }
            }
        }

        #region buttons

        #region Go ToStart Button
        private readonly RelayCommand _goToStartCommand;
        public ICommand GoToStartCommand => _goToStartCommand;

        private bool CanGoToStart => ActiveIndex > 0 && SpritzState != SpritzState.Running;

        private void GoToStart()
        {
            ActiveIndex = 0;
            DisplayWord(_spritzWords[ActiveIndex]);

            _goRightCommand.RaiseCanExecuteChanged();
            _goLeftCommand.RaiseCanExecuteChanged();
            _goToStartCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #region Go Left Button
        private readonly RelayCommand _goLeftCommand;
        public ICommand GoLeftCommand => _goLeftCommand;

        private bool CanGoLeft => ActiveIndex > 0 && SpritzState != SpritzState.Running;

        private void GoLeft()
        {
            if (--ActiveIndex > 0)
            {
                DisplayWord(_spritzWords[ActiveIndex]);
                _goRightCommand.RaiseCanExecuteChanged();
            }
            else
            {
                DisplayWord(_spritzWords[0]);
                _goLeftCommand.RaiseCanExecuteChanged();
                _goToStartCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Go Right Button
        private readonly RelayCommand _goRightCommand;
        public ICommand GoRightCommand => _goRightCommand;

        private bool CanGoRight => _spritzWords != null && ActiveIndex < _spritzWords.Count - 1 &&
                                   SpritzState != SpritzState.Running;

        private void GoRight()
        {
            if (++ActiveIndex < _spritzWords.Count - 1)
            {
                DisplayWord(_spritzWords[ActiveIndex]);
                _goLeftCommand.RaiseCanExecuteChanged();
                _goToStartCommand.RaiseCanExecuteChanged();
            }
            else
            {
                DisplayWord(_spritzWords[ActiveIndex]);
                _goRightCommand.RaiseCanExecuteChanged();
            }
        }
        #endregion

        #region Open In Browser Button
        private readonly RelayCommand _reloadArticleCommand;
        public ICommand ReloadArticleCommand => _reloadArticleCommand;

        private bool CanReloadArticle => Article.LoadingState != LoadingState.Loading;

        private void ReloadArticle()
        {
            _articleRepository.ActualizeArticleAsync(Article);
        }
        #endregion

        #region Open In Browser Button
        private readonly RelayCommand _openInBrowserCommand;
        public ICommand OpenInBrowserCommand => _openInBrowserCommand;

        private bool CanOpenInBrowser => Article != null && Article.PublicUri != null;

        private void OpenInBrowser()
        {
            _platformCodeService.OpenInBrowser(new Uri(Article.PublicUri));
        }
        #endregion

        #region Increase Speed Button
        private readonly RelayCommand _increaseSpeedCommand;
        public ICommand IncreaseSpeedCommand => _increaseSpeedCommand;

        private bool CanIncreaseSpeed => ReadingSpeed < 2000;

        private void IncreaseSpeed()
        {
            ReadingSpeed += 50;
        }
        #endregion

        #region Decrease Speed Button
        private readonly RelayCommand _decreaseSpeedCommand;
        public ICommand DecreaseSpeedCommand => _decreaseSpeedCommand;

        private bool CanDecreaseSpeed => ReadingSpeed > 50;

        private void DecreaseSpeed()
        {
            ReadingSpeed -= 50;

        }
        #endregion

        #region Start Button
        private readonly RelayCommand _startCommand;
        public ICommand StartCommand => _startCommand;

        private bool CanStart => _isSpritzReady;

        private SpritzState _spritzState;
        public SpritzState SpritzState
        {
            get { return _spritzState; }
            set { Set(ref _spritzState, value); }
        }

        private void Start()
        {
            if (SpritzState == SpritzState.Running)
            {
                SpritzState = SpritzState.Paused;
            }
            else if (SpritzState == SpritzState.Paused || SpritzState == SpritzState.Ready)
            {
                SpritzState = SpritzState.Running;
                DisplayNextWord();
            }
            else if (SpritzState == SpritzState.Finished)
            {
                SpritzState = SpritzState.Ready;
                ActiveIndex = 0;
                if (_spritzWords.Count > 0)
                    DisplayWord(_spritzWords[0]);
            }

            _goLeftCommand.RaiseCanExecuteChanged();
            _goToStartCommand.RaiseCanExecuteChanged();
            _goRightCommand.RaiseCanExecuteChanged();
        }
        #endregion

        #endregion

        #endregion

    }
}
