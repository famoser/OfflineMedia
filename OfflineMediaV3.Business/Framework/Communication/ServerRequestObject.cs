using System.Collections.Generic;

namespace OfflineMediaV3.Business.Framework.Communication
{
    public class ServerRequest
    {
        public string InstallationId { get; set; }
        public List<ServerRequestEntry> Entries { get; set; }
    }

    public class ServerRequestEntry
    {
        public string Guid { get; set; }
        public bool Value { get; set; }
    }
}
