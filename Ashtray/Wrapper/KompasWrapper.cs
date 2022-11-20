using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KompasAPI7;
using Kompas6API5;
using Kompas6Constants;
using System.Runtime.InteropServices;

namespace Wrapper
{
    public class KompasWrapper
    {
        /// <summary>
        /// Объект Компас API
        /// </summary>
        private KompasObject _kompas = null;

        /// <summary>
        /// Начало координат
        /// /// </summary>
        private const int origin = 0;

        /// <summary>
        /// Стиль линий, где 1 - основная, 3 -вспомогательная
        /// </summary>
        private int[] styleLine = new int[2] { 1, 3 };

        /// <summary>
        /// Угол поворота
        /// </summary>
        const int degreeOfRotation = 360;

        /// <summary>
        /// Метод для запуска Компас-3D
        /// </summary>
        public void StartKompas()
        {
            try
            {
                if (_kompas != null)
                {
                    _kompas.Visible = true;
                    _kompas.ActivateControllerAPI();
                }

                if (_kompas == null)
                {
                    var kompasType = Type.GetTypeFromProgID
                        ("KOMPAS.Application.5");
                    _kompas = (KompasObject)Activator.CreateInstance
                        (kompasType);

                    StartKompas();

                    if (_kompas == null)
                    {
                        throw new Exception
                            ("Не удается открыть Koмпас-3D");
                    }
                }
            }
            catch (COMException)
            {
                _kompas = null;
                StartKompas();
            }
        }


        /// <summary>
        /// Метод создания файла в Компас-3D
        /// </summary>
        public void CreateFile()
        {
            try
            {
                var document = (ksDocument3D)_kompas.Document3D();
                document.Create();
            }
            catch
            {
                throw new ArgumentException
                    ("Не удается построить деталь");
            }
        }
    }
}
