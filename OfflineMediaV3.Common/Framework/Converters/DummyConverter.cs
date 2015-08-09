using System;

namespace OfflineMediaV3.Common.Framework.Converters
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
