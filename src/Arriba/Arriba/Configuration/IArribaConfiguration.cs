using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arriba.Configuration
{
    public interface IArribaConfiguration
    {
        string ArribaTable { get; }
    }
}
