using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Model
{
  
    public class AshtrayValidator
    {
        public Dictionary<ParameterType, Parameter> Parameters { get; set; }

        /// <summary>
        /// Список ошибок введенного параметра.
        /// </summary>
        public Dictionary<ParameterType, string> Errors { get; set; }

        public AshtrayValidator()
        {
            Errors = new Dictionary<ParameterType, string>();
            Parameters = new Dictionary<ParameterType, Parameter>()
            {
                { ParameterType.BottomThickness,
                new Parameter(7, 7, 10, "Толщина дна", ParameterType.BottomThickness, Errors) },
                { ParameterType.Height,
                new Parameter(35, 35, 60, "Высота", ParameterType.Height, Errors) },
                { ParameterType.LowerDiametr,
                new Parameter(50, 50, 70, "Диаметр дна снизу", ParameterType.LowerDiametr, Errors) },
                { ParameterType.UpperDiametr,
                new Parameter(80, 80, 100, "Диаметр верхней части", ParameterType.UpperDiametr, Errors) },
                { ParameterType.WallThickness,
                new Parameter(6, 5, 7, "Толщина стенок", ParameterType.WallThickness, Errors) },
            };
        }



        /// <summary>
        /// Создает объект класса звездолета для построения.
        /// </summary>
        /// <param name="bottomThickness">Толщина дна</param>
        /// <param name="height">Высота</param>
        /// <param name="lowerDiametr">Нижний диаметр</param>
        /// <param name="upperDiametr">Верхний диаметр</param>
        /// <param name="wallThickness">Толщина стенок</param>
        public void SetParameters(int bottomThickness, int height, int lowerDiametr,
            int upperDiametr, int wallThickness)
        {
            Errors.Clear();
            Parameters[ParameterType.BottomThickness].Value = bottomThickness;
            Parameters[ParameterType.Height].Value = height;
            Parameters[ParameterType.LowerDiametr].Value = lowerDiametr;
            Parameters[ParameterType.UpperDiametr].Value = upperDiametr;
            Parameters[ParameterType.WallThickness].Value = wallThickness;
            /*CheckParametersRelationship(bottomThickness, 100, ParameterType.BottomThickness, "Толщина дна");
            CheckParametersRelationship(height, 100, ParameterType.Height, "Высота");
            CheckParametersRelationship(lowerDiametr, 100, ParameterType.LowerDiametr, "Диаметр дна снизу");
            CheckParametersRelationship(upperDiametr, 100, ParameterType.UpperDiametr, "Диаметр верхней части");
            CheckParametersRelationship(wallThickness, 100, ParameterType.WallThickness, "Толщина стенок");*/
            /*CheckParametersRelationship(height, bottomThickness * 6, ParameterType.Height,
                "Высота пепельницы должна быть больше толщины дна не более чем в 6 раз.");
            
                CheckParametersRelationship(bottomThickness, height/5, ParameterType.BottomThickness,
                    "Толщина дна должна быть меньше высоты минимум в 5 раз.");
            
            CheckParametersRelationship(upperDiametr, lowerDiametr + 30, ParameterType.UpperDiametr,
                "Верхний диаметр должен быть больше нижнего не более чем на 30");
               CheckParametersRelationship(lowerDiametr, upperDiametr - 20, ParameterType.BottomThickness,
                    "Нижний диметр должен быть меньше верхнего не менее чем на 20.");*/
        }

        private bool CheckRange(int value, int minValue, int maxValue, ParameterType parameterType, string errorMessage)
        {
            string minErrorMessage = errorMessage + " не может быть менее " +
                              minValue + " мм.";
            string maxErrorMessage = errorMessage + " не может быть более " +
                              maxValue + " мм.";
            if (value < minValue)
            {
                Errors.Add(parameterType, minErrorMessage);
                return false;
            }
            if (!(value > maxValue)) return true;
            Errors.Add(parameterType, maxErrorMessage);
            return false;
        }

        /// <summary>
        /// Проверка взаимосвязи параметров между собой.
        /// </summary>
        /// <param name="value">Значение введенного параметра.</param>
        /// <param name="mainParameter">Значение параметра, от которого зависимость.</param>
        /// <param name="parameterType">Тип параметра.</param>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        private void CheckParametersRelationship(int value, int mainParameter,
            ParameterType parameterType, string errorMessage)
        {
            Parameters[parameterType].Value = value;
            if (!Errors.ContainsKey(parameterType))
            {
                if (value > mainParameter)
                {
                    Errors.Add(parameterType, errorMessage);
                }
                else
                {
                    Parameters[parameterType].Value = value;
                }
            }
        }
    }
}
