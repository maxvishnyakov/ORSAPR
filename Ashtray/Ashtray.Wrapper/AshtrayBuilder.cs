using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Wrapper
{
    public class AshtrayBuilder
    {
       public void BuildAshtray()
       {
            KompasWrapper kompasWrapper = new KompasWrapper();
            kompasWrapper.StartKompas();
            kompasWrapper.CreateFile();
            kompasWrapper.CreateDetail(true);
            kompasWrapper.FilletPlate();
            kompasWrapper.CreateArray(21, 10, 35, 4);
       }
    }
}
