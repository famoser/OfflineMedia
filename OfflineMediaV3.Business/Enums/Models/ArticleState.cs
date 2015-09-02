namespace OfflineMediaV3.Business.Enums.Models
{
    public enum ArticleState
    {
        New = 0,
        Loading = 1,
        Loaded = 2,
        Read = 3,

        WrongSourceFaillure = 100,
        RetrieveArticleFaillure = 101,
        EvaluateArticleFaillure = 102,
        WritePropertiesFaillure = 103,

    }
}
