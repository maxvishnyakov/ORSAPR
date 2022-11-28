using System;
using Kompas6API5;
using Kompas6Constants3D;
using System.Runtime.InteropServices;

namespace Ashtray.Wrapper
{
    /// <summary>
    /// Класс для работы с Компас-3D
    /// </summary>
    public class KompasWrapper
    {
        /// <summary>
        /// Объект Компас API
        /// </summary>
        private KompasObject _kompas;

        /// <summary>
        ///  Указатель на интерфейс документа.
        /// </summary>
        private ksDocument3D _doc3D;

        /// <summary>
        ///  Указатель на интерфейс компонента.
        /// </summary>
        private ksPart _part;

        /// <summary>
        ///  Указатель на интерфейс сущности.
        /// </summary>
        private ksEntity _entitySketch;

        /// <summary>
        ///  Указатель на интерфейс параметров эскиза.
        /// </summary>
        private ksSketchDefinition _sketchDefinition;

        /// <summary>
        ///  Указатель на эскиз.
        /// </summary>
        private ksDocument2D _sketchEdit;

        /// <summary>
        /// Начало координат
        /// /// </summary>
        private const int origin = 0;

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

        /// <summary>
        /// Метод, создающий эскиз.
        /// </summary>
        /// <param name="plane">Плоскость, эскиз которой будет создан.</param>
        private void CreateSketch(short plane)
        {
            var currentPlane = (ksEntity)_part.GetDefaultEntity(plane);

            _entitySketch = (ksEntity)_part.NewEntity((short)Obj3dType.o3d_sketch);
            _sketchDefinition = (ksSketchDefinition)_entitySketch.GetDefinition();
            _sketchDefinition.SetPlane(currentPlane);
            _entitySketch.Create();
        }

        /// <summary>
        /// Построение пепельницы.
        /// </summary>
        /// <param name="parameters">Параметры пепельницы.</param>
        public void CreateDetail(int bottomThickness, int height, int lowerDiameter, int upperDiameter, int wallThickness)
        {
            if (_kompas != null)
            {
                _doc3D = (ksDocument3D)_kompas.Document3D();
                _doc3D.Create(false, true);
            }

            _doc3D = (ksDocument3D)_kompas.ActiveDocument3D();
            _part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);

            CreateAshtraySketch(wallThickness, upperDiameter, height, bottomThickness, lowerDiameter);
        }

        /// <summary>
        /// Эскиз пепельницы.
        /// </summary>
        /// <param name="wallThickness">Толщина стенки.</param>
        /// <param name="upperDiameter">Диаметр верхней окружности.</param>
        /// <param name="height">Высота.</param>
        /// <param name="bottomThickness">Толщина дна.</param>
        /// <param name="lowerDiameter">Диаметр нижней окружности.</param>
        /// <return>Возвращает выдавленный эскиз.</return>
        private void CreateAshtraySketch(int wallThickness, int upperDiameter, int height,
            int bottomThickness, int lowerDiameter)
        {
            CreateSketch((short)Obj3dType.o3d_planeXOY);
            _sketchEdit = (ksDocument2D)_sketchDefinition.BeginEdit();
            _sketchEdit.ksLineSeg
                (origin, origin, origin + lowerDiameter / 2, origin, 1);
            _sketchEdit.ksLineSeg
                (origin + lowerDiameter / 2, origin, upperDiameter / 2, height, 1);
            _sketchEdit.ksLineSeg
                (upperDiameter / 2, height, upperDiameter / 2 - wallThickness, height, 1);
            _sketchEdit.ksLineSeg
                (upperDiameter / 2 - wallThickness, height, lowerDiameter / 2 - wallThickness, bottomThickness, 1);
            _sketchEdit.ksLineSeg
                (lowerDiameter / 2 - wallThickness, bottomThickness, origin, bottomThickness, 1);
            _sketchEdit.ksLineSeg
                (origin, bottomThickness, origin, origin, 1);
            _sketchEdit.ksLineSeg
              (origin, origin, origin, bottomThickness, 3);
            _sketchDefinition.EndEdit();
            RotateSketch();
        }

        /// <summary>
        /// Метод для выдавливания вращением осовного эскиза.
        /// </summary>
        /// <return> Возвращает выдавленный эскиз.</return>
        private ksEntity RotateSketch()
        {
            var entityRotated =
                (ksEntity)_part.NewEntity((short)Obj3dType.o3d_baseRotated);
            var entityRotatedDefinition =
                (ksBaseRotatedDefinition)entityRotated.GetDefinition();

            entityRotatedDefinition.directionType = 0;
            entityRotatedDefinition.SetSideParam(true, 360);
            entityRotatedDefinition.SetSketch(_entitySketch);
            entityRotated.Create();

            return entityRotated;
        }

        /// <summary>
		/// Метод скругления
		/// </summary>
        /// <param name="bottomThickness">Толщина дна</param>
		/// <param name="lowerDiameter">Нижний диаметр</param>
        public void FilletPlate(int bottomThickness, int lowerDiameter)
        {
            ksEntityCollection faceCollection =
                _part.EntityCollection((short)ksObj3dTypeEnum.o3d_edge);
            
            //Точка, через которую проходит нижняя грань пепельницы. 
            faceCollection.SelectByPoint(lowerDiameter / 2, 0, 0);

            ksEntity baseFace = faceCollection.First();
            CreateFillet(baseFace, bottomThickness);
        }

        /// <summary>
		/// Метод скругления
		/// </summary>
		/// <param name="face">Грань скругления</param>
		/// <param name="radius">Радиус скругления</param>
		private void CreateFillet(ksEntity face, int radius)
        {
            ksEntity fillet = _part.NewEntity((short)ksObj3dTypeEnum.o3d_fillet);

            ksFilletDefinition filletDefinition = fillet.GetDefinition();

            filletDefinition.radius = radius;
            filletDefinition.tangent = false;

            ksEntityCollection entityCollectionFillet = filletDefinition.array();
            entityCollectionFillet.Add(face);

            fillet.Create();
        }

        /// <summary>
        /// Создание отверстий по концентрической сетке
        /// </summary>
        /// <param name="height">Высота вырезания</param>
        public void CreateArray(int height)
        {
            var document = (ksDocument3D)_kompas.ActiveDocument3D();
            var part = (ksPart)document.GetPart((short)Part_Type.pTop_Part);
            const int countHoles = 4;
            const int radius = 10;
            const int styleLineBase = 1;
            const int stepCopy = 360;
            const int coordinateX = 0;
            int coordinateY = height;

            ksEntity entitySketch = (ksEntity)part.NewEntity((short)
                Obj3dType.o3d_sketch);
            ksSketchDefinition sketchDefinition = (ksSketchDefinition)
                entitySketch.GetDefinition();
            ksEntity basePlane = (ksEntity)part.GetDefaultEntity((short)
                Obj3dType.o3d_planeXOY);
            sketchDefinition.SetPlane(basePlane);
            entitySketch.Create();
            ksDocument2D sketchEdit = (ksDocument2D)sketchDefinition.
                BeginEdit();
            sketchEdit.ksCircle(coordinateX, coordinateY, radius,
                styleLineBase);
            sketchDefinition.EndEdit();

            ksEntity entityCutExtrusion = (ksEntity)part.NewEntity((short)
                Obj3dType.o3d_cutExtrusion);
            ksCutExtrusionDefinition cutExtrusionDefinition =
                (ksCutExtrusionDefinition)entityCutExtrusion.GetDefinition();
            cutExtrusionDefinition.directionType =
                (short)Direction_Type.dtNormal;
            cutExtrusionDefinition.SetSideParam(true,
                (short)End_Type.etThroughAll);
            cutExtrusionDefinition.SetSketch(entitySketch);
            entityCutExtrusion.Create();

            ksEntity circularCopyEntity = (ksEntity)part.NewEntity((short)
                Obj3dType.o3d_circularCopy);
            ksCircularCopyDefinition circularCopyDefinition =
                (ksCircularCopyDefinition)circularCopyEntity.GetDefinition();
            circularCopyDefinition.SetCopyParamAlongDir(countHoles, stepCopy,
                true, false);
            ksEntity baseAxisOZ = (ksEntity)part.GetDefaultEntity((short)
                Obj3dType.o3d_axisOY);
            circularCopyDefinition.SetAxis(baseAxisOZ);
            ksEntityCollection entityCollection = (ksEntityCollection)
                circularCopyDefinition.GetOperationArray();
            entityCollection.Add(cutExtrusionDefinition);
            circularCopyEntity.Create();
        }
    }
}
