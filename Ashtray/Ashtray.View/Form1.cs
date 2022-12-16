using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Ashtray.Model;
using Ashtray.Wrapper;
// TODO: динамически изменять значения в label

namespace Ashtray.View
{
    /// <summary>
    /// Класс формы
    /// </summary>
    public partial class AshtrayForm : Form
    {
        /// <summary>
        /// Словарь Тип параметра-TextBox.
        /// </summary>
        private readonly Dictionary<ParameterType, TextBox> _parameterToTextBox;
        
        /// <summary>
        /// Параметры пепельницы.
        /// </summary>
        private readonly AshtrayParameters _ashtrayParameters;

        /// <summary>
        /// Объект для построения пепельницы.
        /// </summary>
        private readonly AshtrayBuilder _ashtrayBuilder;

        /// <summary>
        /// Конструктор основной формы.
        /// </summary>
        public AshtrayForm()
        {
            InitializeComponent();
            _ashtrayBuilder = new AshtrayBuilder();
            _ashtrayParameters = new AshtrayParameters();
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

            BottomThicknessTextBox.Text = _ashtrayParameters.Parameters[ParameterType.BottomThickness].Value.ToString();
            HeightTextBox.Text = _ashtrayParameters.Parameters[ParameterType.Height].Value.ToString();
            LowerDiametrTextBox.Text = _ashtrayParameters.Parameters[ParameterType.LowerDiameter].Value.ToString();
            UpperDiametrTextBox.Text = _ashtrayParameters.Parameters[ParameterType.UpperDiameter].Value.ToString();
            WallThicknessTextBox.Text = _ashtrayParameters.Parameters[ParameterType.WallThickness].Value.ToString();

            legsComboBox.Items.Add("Нет");
            legsComboBox.Items.Add("Круглые");
            legsComboBox.Items.Add("Квадратные");
            legsComboBox.Items.Add("Цилиндрические");
            legsComboBox.SelectedItem = legsComboBox.Items[0];
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

            _ashtrayParameters.SetParameters(BottomThicknessTextBox.Text, HeightTextBox.Text,
                LowerDiametrTextBox.Text, UpperDiametrTextBox.Text, WallThicknessTextBox.Text);
            UpdateLabels();
            foreach (var keyValue in _ashtrayParameters.Errors)
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
                if (keyValue.Value.Text == string.Empty || _ashtrayParameters.Errors.ContainsKey(keyValue.Key))
                {
                    if (_ashtrayParameters.Errors[keyValue.Key] != string.Empty)
                    {
                        keyValue.Value.BackColor = Color.LightPink;
                    }
                    else
                    {
                        counter += 1;
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
        private void BuildButton_Click(object sender, EventArgs e)
        {
            if (CheckEmptyTextBox())
            {
                if (_ashtrayParameters.Errors.Count > 0)
                {
                    var message = string.Empty;
                    foreach (var keyValue in _ashtrayParameters.Errors)
                    {
                        message += $"• {keyValue.Value} \n\n";
                    }

                    MessageBox.Show(message, @"Неверно введены данные!",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    _ashtrayBuilder.BuildAshtray(_ashtrayParameters.Parameters[ParameterType.BottomThickness].Value,
                        _ashtrayParameters.Parameters[ParameterType.Height].Value,
                        _ashtrayParameters.Parameters[ParameterType.LowerDiameter].Value,
                        _ashtrayParameters.Parameters[ParameterType.UpperDiameter].Value,
                        _ashtrayParameters.Parameters[ParameterType.WallThickness].Value,
                        legsComboBox.SelectedItem.ToString());
                }
            }
            else
            {
                MessageBox.Show(
                    @"Ошибка при построении! Проверьте введенные данные.",
                    @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Обновление диапазонов для параметров
        /// </summary>
        private void UpdateLabels()
        {
            UpperDiameterLabel.Text = (_ashtrayParameters.Parameters[ParameterType.LowerDiameter].Value + 20).ToString() 
                                      + " - " + (_ashtrayParameters.Parameters[ParameterType.LowerDiameter].Value + 30).ToString() + " мм.";
            HeightLabel.Text = (_ashtrayParameters.Parameters[ParameterType.BottomThickness].Value * 5).ToString()
                                          + " - " + (_ashtrayParameters.Parameters[ParameterType.BottomThickness].Value * 6).ToString() + " мм.";
            LowerDiameterLabel.Text = (_ashtrayParameters.Parameters[ParameterType.UpperDiameter].Value - 30).ToString()
                                  + " - " + (_ashtrayParameters.Parameters[ParameterType.UpperDiameter].Value - 20).ToString() + " мм.";
            var from = _ashtrayParameters.Parameters[ParameterType.Height].Value / 6;
            var to = _ashtrayParameters.Parameters[ParameterType.Height].Value / 5;
            BottomThicknessLabel.Text = from.ToString() + " - " + to.ToString() + " мм.";
        }
    }
}
