namespace OfflineMediaV3.Common.Framework.Converters
{
    public interface IEntityValueConverter
    {
        object Convert(object val);

        object ConvertBack(object val);
    }
}
