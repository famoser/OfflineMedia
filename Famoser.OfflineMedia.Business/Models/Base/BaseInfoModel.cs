using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Famoser.OfflineMedia.Business.Models.Base
{
    public class BaseInfoModel : BaseGuidModel
    {
        public string Name { get; set; }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set { Set(ref _isActive, value); }
        }
    }
}
