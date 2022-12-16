using System;
using Kompas6API5;
using Kompas6Constants3D;
using System.Runtime.InteropServices;

namespace Ashtray.Wrapper
{
    /// <summary>
    /// Класс для работы с Компас-3D.
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
        private ksDocument3D _document;

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
        /// Начало координат.
        /// /// </summary>
        private const int Origin = 0;

        /// <summary>
        /// Метод для запуска Компас-3D.
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
                if (_kompas != null) return;
                var kompasType = Type.GetTypeFromProgID("KOMPAS.Application.5");
                _kompas = (KompasObject)Activator.CreateInstance(kompasType);
                StartKompas();
                if (_kompas == null)
                {
                    throw new Exception("Не удается открыть Компас-3D.");
                }
            }
            catch (COMException)
            {
                _kompas = null;
                StartKompas();
            }
        }

        /// <summary>
        /// Метод создания файла в Компас-3D.
        /// </summary>
        public void CreateFile()
        {
            try
            {
                _document = (ksDocument3D)_kompas.Document3D();
                _document.Create();
                _document = (ksDocument3D)_kompas.ActiveDocument3D();
                _part = (ksPart)_document.GetPart((short)Part_Type.pTop_Part);
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
        /// Создание кривой.
        /// </summary>
        // TODO: параметры ЕСТЬ
        /// <param name="wallThickness">Толщина стенки.</param>
        /// <param name="upperDiameter">Диаметр верхней окружности.</param>
        /// <param name="height">Высота.</param>
        /// <param name="bottomThickness">Толщина дна.</param>
        /// <param name="lowerDiameter">Диаметр нижней окружности.</param>
        /// <return>Возвращает выдавленный эскиз.</return>
        public void CreateCurve(int wallThickness, int upperDiameter, int height,
            int bottomThickness, int lowerDiameter)
        {
            CreateSketch((short)Obj3dType.o3d_planeXOY);
            _sketchEdit = (ksDocument2D)_sketchDefinition.BeginEdit();
            _sketchEdit.ksLineSeg
                (Origin, Origin, Origin + lowerDiameter / 2, Origin, 1);
            _sketchEdit.ksLineSeg
                (Origin + lowerDiameter / 2, Origin, upperDiameter / 2, height, 1);
            _sketchEdit.ksLineSeg
                (upperDiameter / 2, height, upperDiameter / 2 - wallThickness, height, 1);
            _sketchEdit.ksLineSeg
                (upperDiameter / 2 - wallThickness, height,
                    lowerDiameter / 2 - wallThickness, bottomThickness, 1);
            _sketchEdit.ksLineSeg
                (lowerDiameter / 2 - wallThickness, bottomThickness, Origin, bottomThickness, 1);
            _sketchEdit.ksLineSeg
                (Origin, bottomThickness, Origin, Origin, 1);
            _sketchEdit.ksLineSeg
              (Origin, Origin, Origin, bottomThickness, 3);
            _sketchDefinition.EndEdit();
            ExtrudeByRotation(_entitySketch, (short)Direction_Type.dtNormal, true, 360);
        }

        /// <summary>
		/// Выбрать плоскость для скругления.
		/// </summary>
        /// <param name="bottomThickness">Толщина дна</param>
		/// <param name="lowerDiameter">Нижний диаметр</param>
        public void ChooseFillet(int bottomThickness, int lowerDiameter)
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
        /// Создание отверстий по концентрической сетке.
        /// </summary>
        /// <param name="height">Высота вырезания.</param>
        public void CreateHoles(int height)
        {
            var document = (ksDocument3D)_kompas.ActiveDocument3D();
            var part = (ksPart)document.GetPart((short)Part_Type.pTop_Part);
            const int countHoles = 4;
            const int radius = 10;
            const int styleLineBase = 1;
            const int coordinateX = 0;
            CreateSketch((short)Obj3dType.o3d_planeXOY);
            ksDocument2D sketchEdit = (ksDocument2D)_sketchDefinition.
                BeginEdit();
            sketchEdit.ksCircle(coordinateX, height, radius,
                styleLineBase);
            _sketchDefinition.EndEdit();
            ksEntity entityCutExtrusion = (ksEntity)part.NewEntity((short)
                Obj3dType.o3d_cutExtrusion);
            ksCutExtrusionDefinition cutExtrusionDefinition =
                (ksCutExtrusionDefinition)entityCutExtrusion.GetDefinition();
            cutExtrusionDefinition.directionType =
                (short)Direction_Type.dtNormal;
            cutExtrusionDefinition.SetSideParam(true,
                (short)End_Type.etThroughAll);
            cutExtrusionDefinition.SetSketch(_entitySketch);
            entityCutExtrusion.Create();
            CreateCircularCopy(countHoles, cutExtrusionDefinition);
        }

        /// <summary>
        /// Построение параллелепипедов.
        /// </summary>
        /// <param name="wallThickness">Толщина стенки.</param>
        /// <param name="lowerDiameter">Диаметр нижней окружности.</param>
        public void CreateParallelepipeds(int lowerDiameter, int wallThickness)
        {
            const int styleLineBase = 1;
            int offset = (lowerDiameter - wallThickness) / 2 - 10;
            double[,] cubeVertexes =
            {
                {offset - 5, offset - 5 },
                { offset, offset - 5 },
                {offset, offset },
                { offset - 5,offset },
                { offset - 5, offset - 5 }
            };
            CreateSketch((short)Obj3dType.o3d_planeXOZ);
            ksDocument2D sketchEdit = _sketchDefinition.BeginEdit();
            for (var i = 0; i < cubeVertexes.GetLength(0) - 1; i++)
            {
                sketchEdit.ksLineSeg(cubeVertexes[i, 0], cubeVertexes[i, 1],
                    cubeVertexes[i + 1, 0], cubeVertexes[i + 1, 1], styleLineBase);
                sketchEdit.ksLineSeg(-cubeVertexes[i, 0], cubeVertexes[i, styleLineBase],
                    -cubeVertexes[i + 1, 0], cubeVertexes[i + 1, 1], styleLineBase);
                sketchEdit.ksLineSeg(cubeVertexes[i, 0], -cubeVertexes[i, styleLineBase],
                    cubeVertexes[i + 1, 0], -cubeVertexes[i + 1, 1], styleLineBase);
                sketchEdit.ksLineSeg(-cubeVertexes[i, 0], -cubeVertexes[i, 1],
                    -cubeVertexes[i + 1, 0], -cubeVertexes[i + 1, 1], styleLineBase);
            }

            _sketchDefinition.EndEdit();
            ExtrudeSketch(_entitySketch);
        }

        /// <summary>
        /// Построение цилиндров.
        /// </summary>
        /// <param name="wallThickness">Толщина стенки.</param>
        /// <param name="lowerDiameter">Диаметр нижней окружности.</param>
        public void CreateCylinders(int lowerDiameter, int wallThickness)
        {
            const double radius = 2.5;
            const int styleLineBase = 1;
            int offset = (lowerDiameter - wallThickness) / 2 - 10;
            double[,] circleCenters =
            {
                {offset, offset},
                {offset, -offset},
                {-offset, -offset},
                {-offset, offset},
            };
            CreateSketch((short)Obj3dType.o3d_planeXOZ);
            ksDocument2D sketchEdit = _sketchDefinition.BeginEdit();
            for (int i = 0; i < circleCenters.GetLength(0); i++)
            {
                sketchEdit.ksCircle(circleCenters[i, 0], circleCenters[i, 1],
                    radius, styleLineBase);
            }

            _sketchDefinition.EndEdit();
            ExtrudeSketch(_entitySketch);
        }

        /// <summary>
        /// Построение полусфер.
        /// </summary>
        public void CreateHemispheres()
        {
            double[,] arcCenters =
            {
                {10, 12.5}
            };
            double[,] arcStartPoints =
            {
                {10, 10}
            };
            double[,] arcEndPoints =
            {
                {10, 15}
            };
            double[,,] points =
            {
                {
                    {10, 10}, {10, 15}
                }
            };
            const int style = 1;
            const short direction = 1;
            const double radius = 2.5;
            CreateSketch((short)Obj3dType.o3d_planeXOZ);
            ksDocument2D sketchEdit = _sketchDefinition.BeginEdit();
            sketchEdit.ksArcByPoint(arcCenters[0, 0], arcCenters[0, 1], radius,
                arcStartPoints[0, 0], arcStartPoints[0, 1],
                arcEndPoints[0, 0], arcEndPoints[0, 1], direction, style);
            sketchEdit.ksLineSeg(points[0, 0, 0], points[0, 0, 1],
                points[0, 1, 0], points[0, 1, 1], 3);
            _sketchDefinition.EndEdit();
            var definition = ExtrudeByRotation(_entitySketch,
                (short)Direction_Type.dtReverse, false, 180);
            CreateCircularCopy(4, definition);
        }

        /// <summary>
        /// Выдавливание эскиза вращением.
        /// </summary>
        /// <param name="sketch">Эскиз.</param>
        /// <param name="direction">Направление вращения.</param>
        /// <param name="side">Сторона вращения.</param>
        /// <param name="angle">Угол вращения.</param>
        /// <returns>Описание действия над эскизом.</returns>
        private ksBaseRotatedDefinition ExtrudeByRotation(ksEntity sketch, short direction,
            bool side, int angle)
        {
            ksEntity entity = _part.NewEntity((short)Obj3dType.o3d_baseRotated);
            ksBaseRotatedDefinition definition = entity.GetDefinition();
            definition.directionType = direction;
            definition.SetSideParam(side, angle);
            definition.SetSketch(sketch);
            entity.Create();
            return definition;
        }

        /// <summary>
        /// Выдавить эскиз.
        /// </summary>
        /// <param name="sketch">Выдавливаемый эскиз.</param>
        private void ExtrudeSketch(ksEntity sketch)
        {
            const int depth = 2;
            ksEntity entity = _part.NewEntity((short)Obj3dType.o3d_baseExtrusion);
            ksBaseExtrusionDefinition definition = entity.GetDefinition();
            definition.directionType = (short)Direction_Type.dtReverse;
            definition.SetSideParam(false, (short)End_Type.etBlind, depth);
            definition.SetSketch(sketch);
            entity.Create();
        }

        /// <summary>
        /// Построение массива по концентрической сетке.
        /// </summary>
        /// <param name="count">Количество элементов.</param>
        /// <param name="definition">Какое действие нужно повторить.</param>
        private void CreateCircularCopy(int count, object definition)
        {
            ksEntity entity = _part.NewEntity((short)Obj3dType.o3d_circularCopy);
            ksCircularCopyDefinition copyDefinition = entity.GetDefinition();
            copyDefinition.SetCopyParamAlongDir(count, 360,
                true, false);
            ksEntity baseAxisOz = _part.GetDefaultEntity((short)Obj3dType.o3d_axisOY);
            copyDefinition.SetAxis(baseAxisOz);
            ksEntityCollection entityCollection = copyDefinition.GetOperationArray();
            entityCollection.Add(definition);
            entity.Create();
        }
    }
}