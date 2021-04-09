using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Condition : Rule
    {

        public Func<bool> del;

        public Condition(string d) : base(d)
        {
        }
        public Condition(string d, int n) : base(d, n)
        {
            del = () => false;
        }

        public void setDel(Func<bool> d)
        {
            del = d;
        }

        public override bool validate()
        {
            if (validateTargets()) return del();
            return false;
        }
    }
}
