using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp8
{
    public class Tnode
    {
        public List<double> branching = new List<double>();
        public List<double> decayConstant = new List<double>();
        public List<Tnode> son = new List<Tnode>();
        public bool hasParent;
        public int number;
        public Tnode(int n)
        {
            number = n;
            hasParent = false;
        }
        public static bool operator ==(Tnode c1, Tnode c2)
        {
            return c1.number == c2.number;
        }
        public static bool operator !=(Tnode c1, Tnode c2)
        {
            return c1.number != c2.number;
        }
    }


}
