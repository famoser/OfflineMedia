namespace OfflineMedia.Data.Repository.Converters
{
    public class StringBoolConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            var str = val as string;
            if (str != null)
                return System.Convert.ToBoolean(str);
            return false;
        }

        public object ConvertBack(object val)
        {
            var bolean = (bool)val;
            return bolean.ToString();
        }
    }
}
