namespace OfflineMedia.Common.Framework.Converters
{
    public interface IEntityValueConverter
    {
        object Convert(object val);

        object ConvertBack(object val);
    }
}
