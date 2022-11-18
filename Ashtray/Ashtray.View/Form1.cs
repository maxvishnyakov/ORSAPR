using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Ashtray.Model;
using System.Linq;

namespace Ashtray.View
{
	// TODO: XML
    public partial class Form1 : Form
    {
        /// <summary>
        /// Словарь Тип параметра-TextBox.
        /// </summary>
        private readonly Dictionary<ParameterType, TextBox> _parameterToTextBox;

        private readonly AshtrayValidator _ashtrayValidator;

        /// <summary>
        /// Конструктор основной формы.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            _ashtrayValidator = new AshtrayValidator();
            _parameterToTextBox = new Dictionary<ParameterType, TextBox>
            {
                {ParameterType.BottomThickness, BottomThicknessTextBox},
                {ParameterType.Height, HeightTextBox},
                {ParameterType.LowerDiametr, LowerDiametrTextBox},
                {ParameterType.UpperDiametr, UpperDiametrTextBox},
                {ParameterType.WallThickness, WallThicknessTextBox}
            };

            // Добавление всем TextBox события, когда изменятся текст.

            BottomThicknessTextBox.KeyPress += BanCharacterInput;
            HeightTextBox.KeyPress += BanCharacterInput;
            LowerDiametrTextBox.KeyPress += BanCharacterInput;
            UpperDiametrTextBox.KeyPress += BanCharacterInput;
            WallThicknessTextBox.KeyPress += BanCharacterInput;

            BottomThicknessTextBox.TextChanged += FindError;
            HeightTextBox.KeyPress += FindError;
            LowerDiametrTextBox.KeyPress += FindError;
            UpperDiametrTextBox.KeyPress += FindError;
            WallThicknessTextBox.KeyPress += FindError;


            BottomThicknessTextBox.Text = _ashtrayValidator.Parameters[ParameterType.BottomThickness].Value.ToString();
            HeightTextBox.Text = _ashtrayValidator.Parameters[ParameterType.Height].Value.ToString();
            LowerDiametrTextBox.Text = _ashtrayValidator.Parameters[ParameterType.LowerDiametr].Value.ToString();
            UpperDiametrTextBox.Text = _ashtrayValidator.Parameters[ParameterType.UpperDiametr].Value.ToString();
            WallThicknessTextBox.Text = _ashtrayValidator.Parameters[ParameterType.WallThickness].Value.ToString();

            
        }

        /// <summary>
        /// Проверка введенных значений в режиме реального времени.
        /// </summary>
        /// <param name="sender">TextBox.</param>
        /// <param name="e">Изменение текста в TextBox.</param>
        private void FindError(object sender, EventArgs e)
        {
	        foreach (var keyValue in _parameterToTextBox)
	        {
		        keyValue.Value.BackColor = Color.White;
	        }

	        _ashtrayValidator.SetParameters(BottomThicknessTextBox.Text, HeightTextBox.Text,
		        LowerDiametrTextBox.Text, UpperDiametrTextBox.Text, WallThicknessTextBox.Text);

	        foreach (var keyValue in _ashtrayValidator.Errors)
	        {
		        _parameterToTextBox[keyValue.Key].BackColor = Color.LightPink;
	        }


	        /*catch (Exception ex)
	        {
	            Console.WriteLine(ex);
	            CheckEmptyTextBox();
	        }*/
        }

        // TODO: Нужен? Убрать
        private void checkParameterIsEmptyAndUpdateState(String textParameter, ParameterType parameterType, int value, string message)
        {
            string error = " не должно быть пустым";
            if (textParameter != string.Empty)
            {
                _ashtrayValidator.Errors[parameterType] = "Поле " + message + error;
            } else
            {
                value = int.Parse(BottomThicknessTextBox.Text);
            }
        }

        /// <summary>
        /// Поиск пустых TextBox.
        /// </summary>
        /// <returns>Возвращает true, если нет пустых ячеек,
        /// возвращает false в обратном случае.</returns>
        private bool CheckEmptyTextBox()
       {
            var counter = 0;
            foreach (var keyValue in _parameterToTextBox)
            {
	            // TODO: Можно под одно условие
                if (keyValue.Value.Text == string.Empty)
                {
                    counter += 1;
                    keyValue.Value.BackColor = Color.LightPink;
                }
                else if (_ashtrayValidator.Errors.ContainsKey(keyValue.Key))
                {
                    if (_ashtrayValidator.Errors[keyValue.Key] != string.Empty)
                    { 
                        keyValue.Value.BackColor = Color.LightPink;
                    }
                }
            }
            return counter == 0;
        }

        /// <summary>
        ///  Запрет ввода символов и больше одной точки в число.
        /// </summary>
        /// <param name="sender">TextBox.</param>
        /// <param name="e">Нажатие на клавишу клавиатуры.</param>
        private static void BanCharacterInput(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)
                && !((e.KeyChar == ',') &&
                (((TextBox)sender).Text.IndexOf
                    (",", StringComparison.Ordinal) == -1)))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// Построение по введенным параметрам пепельницы.
        /// </summary>
        /// <param name="sender">Кнопка.</param>
        /// <param name="e">Нажатие на кнопку.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckEmptyTextBox())
            {
                if (_ashtrayValidator.Errors.Count > 0)
                {
                    var message = string.Empty;
                    foreach (var keyValue in _ashtrayValidator.Errors)
                    {
                        // TODO: Селать в одну строку $
	                    message += "• " + keyValue.Value + "\n" + "\n";
                    }

                    MessageBox.Show(message, @"Неверно введены данные!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {

                }
            }
            else
            {
                MessageBox.Show(
                    @"Ошибка при построении! Проверьте введенные данные.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
