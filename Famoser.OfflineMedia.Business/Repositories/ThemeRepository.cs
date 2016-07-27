using System.Linq;
using System.Threading.Tasks;
using Famoser.OfflineMedia.Business.Managers;
using Famoser.OfflineMedia.Business.Models.NewsModel;
using Famoser.OfflineMedia.Business.Repositories.Base;
using Famoser.OfflineMedia.Business.Repositories.Interfaces;
using Famoser.OfflineMedia.Data.Entities.Database;
using Famoser.OfflineMedia.Data.Entities.Database.Relations;
using Famoser.SqliteWrapper.Repositories;
using Famoser.SqliteWrapper.Services.Interfaces;
using Nito.AsyncEx;

namespace Famoser.OfflineMedia.Business.Repositories
{
    public class ThemeRepository : BaseRepository, IThemeRepository
    {
        private readonly GenericRepository<ThemeModel, ThemeEntity> _themeGenericRepository;
        private ISqliteService _sqliteService;

        public ThemeRepository(ISqliteService sqliteService)
        {
            _sqliteService = sqliteService;

            _themeGenericRepository = new GenericRepository<ThemeModel, ThemeEntity>(_sqliteService);
        }

        private bool _isInitialized;
        private AsyncLock _asyncLock = new AsyncLock();
        private Task Initialize()
        {
            return ExecuteSafe(async () =>
            {
                using (await _asyncLock.LockAsync())
                {
                    if (_isInitialized)
                        return;

                    _isInitialized = true;
                    ThemeManager.AddThemes(await _themeGenericRepository.GetAllAsync());
                }
            });
        }

        public async Task AddThemeToArticleAsync(ArticleModel article, string theme)
        {
            await Initialize();

            await ExecuteSafe(async () =>
            {
                var normalizedTheme = NormalizeThemeName(theme);
                var themeModel = ThemeManager.TryGetSimilarTheme(normalizedTheme);
                if (themeModel == null)
                {
                    themeModel = new ThemeModel()
                    {
                        NormalizedName = normalizedTheme,
                        Name = theme
                    };
                    //concurrency: soem other thread may have added the same theme
                    var tm = ThemeManager.TryAddTheme(themeModel);
                    if (tm == themeModel)
                        await _themeGenericRepository.AddAsync(themeModel);
                }
                if (article.Themes.Contains(themeModel))
                    return;

                article.Themes.Add(themeModel);
                await _sqliteService.Add(new ThemeArticleRelations()
                {
                    ArticleId = article.GetId(),
                    ThemeId = themeModel.GetId()
                });
            });
        }

        private string NormalizeThemeName(string str)
        {
            return str.ToLowerInvariant();
        }

        public async Task AddRelatedThemesArticlesAsync(ArticleModel article)
        {
            await Initialize();

            await ExecuteSafe(async () =>
            {
                var aId = article.GetId();
                var relations = await _sqliteService.GetByCondition<ThemeArticleRelations>(a => a.ArticleId == aId, null, false, 0, 0);
                var ids = relations.Select(q => q.ThemeId);
                foreach (var source in ThemeManager.GetAllThemes().Where(d => ids.Any(id => id == d.GetId())))
                {
                    article.Themes.Add(source);
                }
            });
        }

        public async Task LoadArticleThemesAsync(ArticleModel am)
        {
            await Initialize();

            await ExecuteSafe(async () =>
            {
                var id = am.GetId();
                var cm = await _sqliteService.GetByCondition<ThemeArticleRelations>(d => d.ArticleId == id, null, false, 0, 0);
                foreach (var entry in cm.ToArray())
                {
                    var theme = ThemeManager.GetAllThemes().FirstOrDefault(t => t.GetId() == entry.ThemeId);
                    if (theme != null)
                    {
                        am.Themes.Add(theme);
                    }
                }
            });
        }
    }
}
