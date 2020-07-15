using Arriba.Types;
using System.Collections.Generic;
using System.Security.Principal;

namespace Arriba.Communication.Server.Application
{
    public interface IArribaManagementService
    {
        IEnumerable<string> GetTables();

        IDictionary<string, TableInformation> GetTablesForUser(IPrincipal user);
    }
}
