namespace OfflineMediaV3.Common.Framework.Converters
{
    public class EnumConverter<T> : IEntityValueConverter
    {
        public object Convert(object val)
        {
            return (T)val;
        }

        public object ConvertBack(object val)
        {
            return (int)val;
        }
    }
}
