using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ashtray.Model;

namespace Ashtray.UnitTest
{
    /// <summary>
    /// Класс тестирования полей класса пепельницы.
    /// </summary>
    [TestFixture]
    public class AshtrayTest
    {
        /// <summary>
        /// Объект класса пепельницы.
        /// </summary>
        private readonly Model.Ashtray _ashtray = new Model.Ashtray();

        /// <summary>
        /// Позитивный тест геттера Errors.
        /// </summary>
        [Test(Description = "Позитивный тест геттера Errors.")]
        public void TestErrorListGet_CorrectValue()
        {
            var expected = new Dictionary<ParameterType, string>();
            var actual = _ashtray.Errors;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Позитивный тест геттера Parameters.
        /// </summary>
        [Test(Description = "Позитивный тест геттера Parameters.")]
        public void TestParametersGet_CorrectValue()
        {
            var expected = new Dictionary<ParameterType, Parameter>()
               {
                { ParameterType.BottomThickness,
                new Parameter(7, 7, 10, "Толщина дна", ParameterType.BottomThickness, _ashtray.Errors) },
                { ParameterType.Height,
                new Parameter(42, 35, 60, "Высота", ParameterType.Height, _ashtray.Errors) },
                { ParameterType.LowerDiameter,
                new Parameter(50, 50, 70, "Диаметр дна снизу", ParameterType.LowerDiameter, _ashtray.Errors) },
                { ParameterType.UpperDiameter,
                new Parameter(80, 70, 100, "Диаметр верхней части", ParameterType.UpperDiameter, _ashtray.Errors) },
                { ParameterType.WallThickness,
                new Parameter(6, 5, 7, "Толщина стенок", ParameterType.WallThickness, _ashtray.Errors) },
            };
            var actual = _ashtray.Parameters;
            Assert.AreEqual(expected, actual);
        }
    }
}
