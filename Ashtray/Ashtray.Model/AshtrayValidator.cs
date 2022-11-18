using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Model
{
  // TODO: XML
  // TODO: Переименовать
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

        private void isParameterEmpty (String textParameter, ParameterType parameterType, string errorMessage)
        {
            if (textParameter != string.Empty)
            {
                Parameters[parameterType].Value = int.Parse(textParameter);
            } 
            else
            {
                Errors.Add(parameterType, errorMessage + " не должно быть пустым");
            }
        }

        /// <summary>
        /// Создает объект класса звездолета для построения.
        /// </summary>
        /// <param name="bottomThickness">Толщина дна</param>
        /// <param name="height">Высота</param>
        /// <param name="lowerDiametr">Нижний диаметр</param>
        /// <param name="upperDiametr">Верхний диаметр</param>
        /// <param name="wallThickness">Толщина стенок</param>
        public void SetParameters(string bottomThickness, string height, string lowerDiametr,
            string upperDiametr, string wallThickness)
        {
            Errors.Clear();
            isParameterEmpty(lowerDiametr, ParameterType.LowerDiametr, "Нижний диаметр");
            isParameterEmpty(upperDiametr, ParameterType.UpperDiametr, "Верхний диаметр");
            isParameterEmpty(bottomThickness, ParameterType.BottomThickness, "Толщина дна");
            isParameterEmpty(height, ParameterType.Height, "Высота");
            isParameterEmpty(wallThickness, ParameterType.WallThickness, "Толщина стенок");
            if (Errors.Count == 0)
            {
                CheckParametersRelationship(int.Parse(upperDiametr), int.Parse(lowerDiametr) + 20 , int.Parse(lowerDiametr) + 30, ParameterType.UpperDiametr, "Диаметр верхней части");
                if (!Errors.ContainsKey(ParameterType.UpperDiametr))
                {
                    Parameters[ParameterType.LowerDiametr].Value = int.Parse(lowerDiametr);
                }
                CheckParametersRelationship(int.Parse(height), int.Parse(bottomThickness) * 5, int.Parse(bottomThickness) * 6, ParameterType.UpperDiametr, "Высота");
                if (!Errors.ContainsKey(ParameterType.Height))
                {
                    Parameters[ParameterType.BottomThickness].Value = int.Parse(bottomThickness);
                }
            }
            else
            {
                return;
            }
            /*if (!isParameterEmpty(bottomThickness))
            {
                Parameters[ParameterType.BottomThickness].Value = int.Parse(bottomThickness);

            }
            if (!isParameterEmpty(height))
            {
                Parameters[ParameterType.Height].Value = int.Parse(height);

            }
            if (!isParameterEmpty(lowerDiametr))
            {
                Parameters[ParameterType.LowerDiametr].Value = int.Parse(lowerDiametr);

            }
            if (!isParameterEmpty(upperDiametr))
            {
                Parameters[ParameterType.UpperDiametr].Value = int.Parse(upperDiametr);

            }
            if (!isParameterEmpty(wallThickness))
            {
                Parameters[ParameterType.WallThickness].Value = int.Parse(wallThickness);

            }*/
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

        /// <summary>
        /// Проверка взаимосвязи параметров между собой.
        /// </summary>
        /// <param name="value">Значение введенного параметра.</param>
        /// <param name="mainParameter">Значение параметра, от которого зависимость.</param>
        /// <param name="parameterType">Тип параметра.</param>
        /// <param name="errorMessage">Сообщение об ошибке.</param>
        private void CheckParametersRelationship(int value, int mainParameterMin, int mainParameterMax,
            ParameterType parameterType, string errorMessage)
        {
            if (!Errors.ContainsKey(parameterType))
            {
                if ((value > mainParameterMin) && (value < mainParameterMax))
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
