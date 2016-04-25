namespace OfflineMedia.Data.Repository.Converters
{
    public interface IEntityValueConverter
    {
        object Convert(object val);

        object ConvertBack(object val);
    }
}
