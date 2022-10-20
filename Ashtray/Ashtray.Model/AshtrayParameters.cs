using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Model
{
    public class AshtrayParameters
    {
        private Dictionary<ParameterType, String> _errors = new Dictionary<ParameterType, String>();

        private double _upperDiametr;
        
        private double _lowerDiametr;
        
        private double _height;
        
        private double _bottomThickness;
        
        private double _wallThickness;

        private ParameterValidator _parameterValidator = new ParameterValidator();

        public Dictionary<ParameterType, String> getErrorsMap()
        {
            return _errors;
        }

        public double UpperDiametr
        {
            get
            {
                return _upperDiametr;
            }
            set
            {
                if (value < 80 || value > 100)
                {
                    throw new ArgumentException("Верхний даиметр должен находиться в диапазоне от 80 до 100" + '\n');
                }
                this._upperDiametr = value;
            }
        }

        public double LowerDiametr
        {
            get
            {
                return _lowerDiametr;
            }
            set
            {
                if (value < 50 || value > 70)
                {
                    throw new ArgumentException("Нижний даиметр должен находиться в диапазоне от 50 до 75" + '\n');
                }
                this._lowerDiametr = value;
            }
        }

        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value < 35 || value > 65) 
                {
                    throw new ArgumentException("Высота должна находиться в диапазоне от 35 до 65" + '\n');
                }
                this._height = value;
            }
        }

        public double BottomThickness
        {
            get
            {
                return _bottomThickness;
            }
            set
            {
                if (value < 7 || value > 10)
                {
                    throw new ArgumentException("Толщина дна должна находиться в диапазоне от 7 до 10" + '\n');
                }
                this._bottomThickness = value;
            }
        }

        public double WallThickness
        {
            get
            {
                return _wallThickness;
            }
            set
            {
                if (value < 5 || value > 7)
                {
                    throw new ArgumentException("Толщина стенок должна находиться в диапазоне от 5 до 7" + '\n');
                }
                this._wallThickness = value;
            }
        }
    }
}
