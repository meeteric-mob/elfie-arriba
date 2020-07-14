using Arriba.Types;
using System.Collections.Generic;

namespace Arriba.Communication.Server.Application
{
    public interface IArribaManagementService
    {
        IEnumerable<string> GetTables();

        Dictionary<string, TableInformation> GetAllBasic();
    }
}
