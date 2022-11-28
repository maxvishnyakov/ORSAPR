using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Wrapper
{
    /// <summary>
    /// Класс для открытия Компас-3D, создания файла и построения модели
    /// </summary>
    public class AshtrayBuilder
    {
        /// <summary>
        /// Построение модели пепельницы.
        /// </summary>
        public void BuildAshtray(int bottomThickness, int height, int lowerDiameter, int upperDiameter, int wallThickness)
       {
            KompasWrapper kompasWrapper = new KompasWrapper();
            kompasWrapper.StartKompas();
            kompasWrapper.CreateFile();
            kompasWrapper.CreateDetail(bottomThickness, height, lowerDiameter, upperDiameter, wallThickness);
            kompasWrapper.FilletPlate(bottomThickness, lowerDiameter);
            kompasWrapper.CreateArray(height);
       }
    }
}
