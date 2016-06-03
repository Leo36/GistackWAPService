using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAPWrapper
{
    /// <summary>
    /// Type defining all the supported WAP Authentication
    /// </summary>
    public enum AuthType
    {
        WindowsAuth = 0,
        ASPAuth = 1,
        ADFSAuth = 2,
        ADASPAuth = 3,
    }
}
