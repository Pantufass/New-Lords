using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewNpc2
{
    public class Condition : Rule
    {

        public Func<List<dynamic>,bool> del;

        public Condition(string s, Func<List<dynamic>, bool> d = null) : base(s)
        {
            del = d;
        }

        public void setDel(Func<List<dynamic>,bool> d)
        {
            del = d;
        }

        public override bool validate(List<dynamic> d = null)
        {
            return del(d);
        }
    }
}
