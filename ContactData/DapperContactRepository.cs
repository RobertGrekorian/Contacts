using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactData
{
    public interface IDapperContactRepository {
        string GetContactName();
    }
    public class DapperContactRepository : IDapperContactRepository
    {
        public string GetContactName()
        {
            return "robert";
        }
    }
}
