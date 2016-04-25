using System;

namespace OfflineMedia.Data.Repository.Converters
{
    public class DummyConverter : IEntityValueConverter
    {
        public object Convert(object val)
        {
            throw new NotImplementedException();
        }

        public object ConvertBack(object val)
        {
            throw new NotImplementedException();
        }
    }
}
