using System;
using System.Collections.Generic;
using System.Data;

namespace Rac.TestAutomation.Common.DBDriver
{
    public interface IDatabase : IDisposable
    {
        IDataReader ExecuteQuery(string query, Dictionary<string, string> queryParams);
    }
}
