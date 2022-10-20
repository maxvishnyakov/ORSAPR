using Ashtray.Model;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace Ashtray.View
{
    public partial class Form1 : Form
    {
        private AshtrayParameters _ashtrayParameters = new AshtrayParameters();

        /// <summary>
        /// Константа для корректного цвета. 
        /// </summary>
        private readonly Color _correctColor = Color.White;

        /// <summary>
        /// Константа для цвета ошибки.
        /// </summary>
        private readonly Color _errorColor = Color.LightPink;

        private String _validationError = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void clearErrorsMapByParameter(ParameterType type)
        {
            if (_ashtrayParameters.getErrorsMap().ContainsKey(type))
            {
                _ashtrayParameters.getErrorsMap().Remove(type);
            }
        }

        private void upperDiametr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clearErrorsMapByParameter(ParameterType.UpperDiametr);
                _ashtrayParameters.UpperDiametr = Convert.ToDouble(UpperDiametrTextBox.Text);
                UpperDiametrTextBox.BackColor = _correctColor;
            }
            catch(Exception exception)
            {
                UpperDiametrTextBox.BackColor = _errorColor;
                _ashtrayParameters.getErrorsMap().Add(ParameterType.UpperDiametr, exception.Message);
            }
        }

        private void height_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clearErrorsMapByParameter(ParameterType.Height);
                _ashtrayParameters.Height = Convert.ToDouble(HeightTextBox.Text);
                HeightTextBox.BackColor = _correctColor;
            }
            catch (Exception exception)
            {
                HeightTextBox.BackColor = _errorColor;
                _ashtrayParameters.getErrorsMap().Add(ParameterType.Height, exception.Message);
            }
        }

        private void lowerDiametr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clearErrorsMapByParameter(ParameterType.LowerDiametr);
                _ashtrayParameters.LowerDiametr = Convert.ToDouble(LowerDiametrTextBox.Text);
                LowerDiametrTextBox.BackColor = _correctColor;
            }
            catch (Exception exception)
            {
                LowerDiametrTextBox.BackColor = _errorColor;
                _ashtrayParameters.getErrorsMap().Add(ParameterType.LowerDiametr, exception.Message);
            }
        }

        private void bottomThickness_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clearErrorsMapByParameter(ParameterType.BottomThickness);
                _ashtrayParameters.BottomThickness = Convert.ToDouble(BottomThicknessTextBox.Text);
                BottomThicknessTextBox.BackColor = _correctColor;
            }
            catch (Exception exception)
            {
                BottomThicknessTextBox.BackColor = _errorColor;
                _ashtrayParameters.getErrorsMap().Add(ParameterType.BottomThickness, exception.Message);
            }
        }

        private void wallThickness_TextChanged(object sender, EventArgs e)
        {
            try
            {
                clearErrorsMapByParameter(ParameterType.WallThickness);
                _ashtrayParameters.WallThickness = Convert.ToDouble(WallThicknessTextBox.Text);
                WallThicknessTextBox.BackColor = _correctColor;
            }
            catch (Exception exception)
            {
                WallThicknessTextBox.BackColor = _errorColor;
                _ashtrayParameters.getErrorsMap().Add(ParameterType.WallThickness, exception.Message);
            }
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            if (CheckFormOnErrors())
            {

            }
        }

        /// <summary>
        /// Проверка на наличие ошибок в форме.
        /// </summary>
        private bool CheckFormOnErrors()
        {
            if (_ashtrayParameters.getErrorsMap().Count != 0)
            {
                foreach(var error in _ashtrayParameters.getErrorsMap())
                {
                    _validationError = _validationError + error.Value;
                }
                MessageBox.Show(_validationError);
                _validationError = "";
                return false;
            }
            else
            {
                _validationError = "";
                return true;
            }
        }
    }
}
