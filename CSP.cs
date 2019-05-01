using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    public class CSP<V>
    {
        public List<V> variables;
        public List<V> domain;
        public List<int> constraints;
        public CSP()
        {
            variables = new List<V>();
            domain = new List<V>();
            constraints = new List<int>();
        }
    }
}
