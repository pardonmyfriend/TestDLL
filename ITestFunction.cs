using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDLL
{
    public interface ITestFunction
    {
        string Name { get; set; }
        int Dim { get; set; }
        double[] Xmin { get; set; }
        double[] Xmax { get; set; }
        double Calculate(params double[] x);
    }
}
