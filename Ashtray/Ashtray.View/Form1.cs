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

        private void upperDiametr_TextChanged(object sender, EventArgs e)
        {
            try
            {
                _ashtrayParameters.UpperDiametr = Convert.ToDouble(UpperDiametrTextBox.Text);
                UpperDiametrTextBox.BackColor = _correctColor;
            }
            catch(Exception exception)
            {
                UpperDiametrTextBox.BackColor = _errorColor;
                _validationError = _validationError + exception.Message;
            }
        }

        private void height_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void lowerDiametr_TextChanged(object sender, EventArgs e)
        {

        }

        private void bottomThickness_TextChanged(object sender, EventArgs e)
        {

        }

        private void wallThickness_TextChanged(object sender, EventArgs e)
        {

        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            if (CheckFormOnErrors())
            {

            }
        }

        /// <summary>
        /// Проверка на анличие ошибок в форме.
        /// </summary>
        private bool CheckFormOnErrors()
        {
            if (_validationError != "")
            {
                MessageBox.Show(_validationError);
                return false;
            }
            else
            {
                return true;
            }
            _validationError = "";
        }
    }
}
