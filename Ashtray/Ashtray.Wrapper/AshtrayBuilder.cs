using Kompas6Constants3D;

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
        // TODO: Разделить логику с враппером   ЕСТЬ
        public void BuildAshtray(int bottomThickness, int height, int lowerDiameter, int upperDiameter, int wallThickness, string legName)
        {
            var kompasWrapper = new KompasWrapper();
            kompasWrapper.StartKompas();
            kompasWrapper.CreateFile();
            // Создание эскиза пепельницы.
            kompasWrapper.CreateCurve(wallThickness, upperDiameter, height, bottomThickness, lowerDiameter);
            // Создание скругления на пепельнице.
            kompasWrapper.ChooseFillet(bottomThickness, lowerDiameter);
            // Создание отверстий на пепельнице.
            kompasWrapper.CreateHoles(height);
            // Выбор ножек.
            if (!legName.Equals("Нет"))
            {
                if (legName.Equals("Круглые"))
                {
                    kompasWrapper.CreateHemispheres();
                } else if (legName.Equals("Прямоугольные"))
                {
                    kompasWrapper.CreateParallelepipeds(lowerDiameter, wallThickness);
                }
                else
                {
                    kompasWrapper.CreateCylinders(lowerDiameter, wallThickness);
                }
            }
        }
    }
}