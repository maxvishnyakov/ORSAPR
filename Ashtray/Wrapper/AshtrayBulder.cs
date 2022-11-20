using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrapper
{
    /// <summary>
    /// Класс для открытия Компас-3D, создания файла и построения модели
    /// </summary>
    class AshtrayBulder
    {
        public void Build()
        {
            KompasWrapper kompasWrapper = new KompasWrapper();
            kompasWrapper.StartKompas();
        }
    }
}
