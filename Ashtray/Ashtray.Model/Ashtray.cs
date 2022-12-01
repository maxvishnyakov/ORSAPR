using System;
using System.Collections.Generic;

namespace Ashtray.Model
{
    // TODO: XML - Done.
    // TODO: Переименовать - Done.
    /// <summary>
    /// Класс пепельницы
    /// </summary>
    public class Ashtray
    {
        /// <summary>
        /// Словарь с параметрами пепельницы.
        /// </summary>
        public Dictionary<ParameterType, Parameter> Parameters { get; set; }

        /// <summary>
        /// Список ошибок введенного параметра.
        /// </summary>
        public Dictionary<ParameterType, string> Errors { get; set; }

        public Ashtray()
        {
            Errors = new Dictionary<ParameterType, string>();
            Parameters = new Dictionary<ParameterType, Parameter>()
            {
                { ParameterType.BottomThickness,
                new Parameter(7, 7, 10, "Толщина дна", ParameterType.BottomThickness, Errors) },
                { ParameterType.Height,
                new Parameter(42, 35, 60, "Высота", ParameterType.Height, Errors) },
                { ParameterType.LowerDiameter,
                new Parameter(50, 50, 70, "Диаметр дна снизу", ParameterType.LowerDiameter, Errors) },
                { ParameterType.UpperDiameter,
                new Parameter(80, 70, 100, "Диаметр верхней части", ParameterType.UpperDiameter, Errors) },
                { ParameterType.WallThickness,
                new Parameter(6, 5, 7, "Толщина стенок", ParameterType.WallThickness, Errors) },
            };
        }

        /// <summary>
        /// Создает объект класса пепельницы для построения.
        /// </summary>
        /// <param name="textParameter">Параметр из текст бокса</param>
        /// <param name="parameterType">Тип параметра</param>
        /// <param name="errorMessage">Сообщение об ошибке</param>
        private void CheckParameterEmpty (String textParameter, ParameterType parameterType, string errorMessage)
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
        /// Создает объект класса пепельницы для построения.
        /// </summary>
        /// <param name="bottomThickness">Толщина дна</param>
        /// <param name="height">Высота</param>
        /// <param name="lowerDiametr">Нижний диаметр</param>
        /// <param name="upperDiametr">Верхний диаметр</param>
        /// <param name="wallThickness">Толщина стенок</param>
        public void SetParameters (string bottomThickness, string height, string lowerDiametr,
            string upperDiametr, string wallThickness)
        {
            Errors.Clear();
            CheckParameterEmpty(lowerDiametr, ParameterType.LowerDiameter, "Нижний диаметр");
            CheckParameterEmpty(upperDiametr, ParameterType.UpperDiameter, "Верхний диаметр");
            CheckParameterEmpty(bottomThickness, ParameterType.BottomThickness, "Толщина дна");
            CheckParameterEmpty(height, ParameterType.Height, "Высота");
            CheckParameterEmpty(wallThickness, ParameterType.WallThickness, "Толщина стенок");
            if (Errors.Count == 0)
            {
                CheckParametersRelationship(int.Parse(upperDiametr), int.Parse(lowerDiametr) + 20 , int.Parse(lowerDiametr) + 30, 
                    ParameterType.UpperDiameter, "Диаметр верхней части должен быть больше нижнего диаметра не менее чем на 20 и не более чем 30 мм");
                if (!Errors.ContainsKey(ParameterType.UpperDiameter))
                {
                    Parameters[ParameterType.LowerDiameter].Value = int.Parse(lowerDiametr);
                }
                CheckParametersRelationship(int.Parse(height), int.Parse(bottomThickness) * 5, int.Parse(bottomThickness) * 6, 
                    ParameterType.Height, "Высота должна быть больше толщины дна не менее чем в 5 раз и не более чем в 6 раз");
                if (!Errors.ContainsKey(ParameterType.Height))
                {
                    Parameters[ParameterType.BottomThickness].Value = int.Parse(bottomThickness);
                }
            }
            else
            {
                return;
            }
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
                if ((value >= mainParameterMin) && (value <= mainParameterMax))
                {
                    Parameters[parameterType].Value = value;
                }
                else
                {
                    Errors.Add(parameterType, errorMessage);
                }
            }
        }
    }
}
