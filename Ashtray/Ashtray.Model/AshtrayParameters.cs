using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashtray.Model
{
    public class AshtrayParameters
    {
        private Dictionary<ParameterType, String> _errors;

        private double _upperDiametr;
        
        private double _lowerDiametr;
        
        private double _height;
        
        private double _bottomThickness;
        
        private double _wallThickness;

        private ParameterValidator _parameterValidator = new ParameterValidator();

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
                    throw new ArgumentException(
                        $"The upper diametr must be in range from 80 to 100"
                        + $" But was {value}");
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
                this._wallThickness = value;

            }
        }
    }
}
