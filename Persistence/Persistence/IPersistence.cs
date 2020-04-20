using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    interface IPersistence
    {
        bool SaveSession(String messages);
        List<String> RetrieveSession(int startSessionID, int endSessionID);
        int DeleteSession(int startSessionID, int endSessionID);
    }
}
