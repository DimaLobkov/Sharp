using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationManager
{
    public interface IConfigurationParser<out T>
    {
        T Parse();
    }
}
