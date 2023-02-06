using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adhoc.Common
{
    public class CommandLineArguments
    {
        public string[] args { get; private set; }

        public CommandLineArguments(string[] args)
        {
            this.args = args;
        }
    }
}
