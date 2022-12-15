using System;
using System.Collections.Generic;
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
        private readonly AshtrayParameters _ashtrayParameters = new AshtrayParameters();

        /// <summary>
        /// Позитивный тест геттера Errors.
        /// </summary>
        [Test(Description = "Позитивный тест геттера Errors.")]
        public void TestErrorListGet_CorrectValue()
        {
            var expected = new Dictionary<ParameterType, string>();
            var actual = _ashtrayParameters.Errors;
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
                new Parameter(7, 7, 10, "Толщина дна",
                ParameterType.BottomThickness, _ashtrayParameters.Errors) },
                { ParameterType.Height,
                new Parameter(42, 35, 60, "Высота",
                    ParameterType.Height, _ashtrayParameters.Errors) },
                { ParameterType.LowerDiameter,
                new Parameter(50, 50, 70, "Диаметр дна снизу",
                    ParameterType.LowerDiameter, _ashtrayParameters.Errors) },
                { ParameterType.UpperDiameter,
                new Parameter(80, 70, 100, "Диаметр верхней части",
                    ParameterType.UpperDiameter, _ashtrayParameters.Errors) },
                { ParameterType.WallThickness,
                new Parameter(6, 5, 7, "Толщина стенок",
                    ParameterType.WallThickness, _ashtrayParameters.Errors) },
            };
            var actual = _ashtrayParameters.Parameters;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Позитивный и негативный тест сеттера Parameters.
        /// </summary>
        [Test(Description = "Позитивный и негативный тест сеттера Parameters.")]
        [TestCase("80", "50", 0, Description = "Позитивный тест сеттера Parameters.")]
        [TestCase("110", "50", 1, Description = "Негативный тест сеттера Parameters.")]
        [TestCase("", "50", 1, Description = "Негативный тест сеттера Parameters.")]
        [TestCase("80", "70", 1, Description = "Негативный тест сеттера Parameters.")]
        public void TestParametersSet_CorrectValue(string upperDiameter, string lowerDiameter, int expected)
        {
            const string bottomThickness = "7";
            const string height = "42";
            const string wallThickness = "5";
            _ashtrayParameters.SetParameters(bottomThickness, height, lowerDiameter, upperDiameter, wallThickness);
            var actual = _ashtrayParameters.Errors.Count;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Позитивный тест геттера Value.
        /// </summary>
        [Test(Description = "Позитивный тест геттера Value.")]
        public void TestValueGet_CorrectValue()
        {
            var parameter = new Parameter(10, 1, 20,
                "Value", ParameterType.UpperDiameter,
                new Dictionary<ParameterType, string>());
            const int expected = 10;
            var actual = parameter.Value;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Негативный тест геттера Value, когда Value < MinValue. />
        /// </summary>
        [Test(Description = "Негативный тест геттера Value, когда Value < MinValue.")]
        public void TestValueMixValueGet_IncorrectValue()
        {
            var parameter = new Parameter(-10, 1, 20,
                "Value", ParameterType.UpperDiameter,
                new Dictionary<ParameterType, string>());
            const int expected = 0;
            var actual = parameter.Value;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Негативный тест геттера Value, когда Value > MaxValue.
        /// </summary>
        [Test(Description = "Негативный тест геттера Value, когда Value > MaxValue.")]
        public void TestValueMaxValueGet_IncorrectValue()
        {
            var parameter = new Parameter(25, 1, 20,
                "Value", ParameterType.UpperDiameter,
                new Dictionary<ParameterType, string>());
            const int expected = 0;
            var actual = parameter.Value;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Позитивный тест метода Equals.
        /// </summary>
        [Test(Description = "Позитивный тест метода Equals.")]
        public void TestEquals_CorrectValue()
        {
            var expected = new Parameter(5, 0, 10, "string.Empty",
                ParameterType.UpperDiameter, new Dictionary<ParameterType, string>());
            var actual = expected;
            Assert.AreEqual(expected, actual);
        }
    }
}