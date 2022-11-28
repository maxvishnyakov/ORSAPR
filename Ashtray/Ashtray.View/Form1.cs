using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Ashtray.Model;
using Ashtray.Wrapper;
using System.Linq;

namespace Ashtray.View
{
	// TODO: XML - Done.
    /// <summary>
    /// Класс формы
    /// </summary>
    public partial class AshtrayForm : Form
    {
        /// <summary>
        /// Словарь Тип параметра-TextBox.
        /// </summary>
        private readonly Dictionary<ParameterType, TextBox> _parameterToTextBox;

        private readonly Model.Ashtray _ashtray;

        private readonly AshtrayBuilder ashtrayBuilder;

        /// <summary>
        /// Конструктор основной формы.
        /// </summary>
        public AshtrayForm()
        {
            InitializeComponent();
            ashtrayBuilder = new AshtrayBuilder();
            _ashtray = new Model.Ashtray();
            _parameterToTextBox = new Dictionary<ParameterType, TextBox>
            {
                {ParameterType.BottomThickness, BottomThicknessTextBox},
                {ParameterType.Height, HeightTextBox},
                {ParameterType.LowerDiameter, LowerDiametrTextBox},
                {ParameterType.UpperDiameter, UpperDiametrTextBox},
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

            BottomThicknessTextBox.Text = _ashtray.Parameters[ParameterType.BottomThickness].Value.ToString();
            HeightTextBox.Text = _ashtray.Parameters[ParameterType.Height].Value.ToString();
            LowerDiametrTextBox.Text = _ashtray.Parameters[ParameterType.LowerDiameter].Value.ToString();
            UpperDiametrTextBox.Text = _ashtray.Parameters[ParameterType.UpperDiameter].Value.ToString();
            WallThicknessTextBox.Text = _ashtray.Parameters[ParameterType.WallThickness].Value.ToString();
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

	        _ashtray.SetParameters(BottomThicknessTextBox.Text, HeightTextBox.Text,
		        LowerDiametrTextBox.Text, UpperDiametrTextBox.Text, WallThicknessTextBox.Text);

	        foreach (var keyValue in _ashtray.Errors)
	        {
		        _parameterToTextBox[keyValue.Key].BackColor = Color.LightPink;
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
                else if (_ashtray.Errors.ContainsKey(keyValue.Key))
                {
                    if (_ashtray.Errors[keyValue.Key] != string.Empty)
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

        //TODO: rename button
        /// <summary>
        /// Построение по введенным параметрам пепельницы.
        /// </summary>
        /// <param name="sender">Кнопка.</param>
        /// <param name="e">Нажатие на кнопку.</param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckEmptyTextBox())
            {
                if (_ashtray.Errors.Count > 0)
                {
                    var message = string.Empty;
                    foreach (var keyValue in _ashtray.Errors)
                    {
                        // TODO: Селать в одну строку $
	                    message += "• " + keyValue.Value + "\n" + "\n";
                    }

                    MessageBox.Show(message, @"Неверно введены данные!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    ashtrayBuilder.BuildAshtray(_ashtray.Parameters[ParameterType.BottomThickness].Value, _ashtray.Parameters[ParameterType.Height].Value,
                        _ashtray.Parameters[ParameterType.LowerDiameter].Value, _ashtray.Parameters[ParameterType.UpperDiameter].Value,
                        _ashtray.Parameters[ParameterType.WallThickness].Value);
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
