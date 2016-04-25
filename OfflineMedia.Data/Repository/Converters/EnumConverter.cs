namespace OfflineMedia.Data.Repository.Converters
{
    public class EnumConverter<T> : IEntityValueConverter
    {
        public object Convert(object val)
        {
            if (val == null)
                return default(T);
            return (T)val;
        }

        public object ConvertBack(object val)
        {
            if (val == null)
                return 0;
            return (int)val;
        }
    }
}
