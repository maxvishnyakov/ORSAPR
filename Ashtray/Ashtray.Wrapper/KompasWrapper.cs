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
        /// Построение стакана.
        /// </summary>
        /// <param name="parameters">Параметры стакана.</param>
        /// <param name="checkFaceted">Определяем, граненый ли стакан.</param>
        public void CreateDetail(bool checkFaceted)
        {
            if (_kompas != null)
            {
                _doc3D = (ksDocument3D)_kompas.Document3D();
                _doc3D.Create(false, true);
            }

            var wallwidth = 3;
            var highdiameter = 80;
            var height = 42;
            var bottomthickness = 7;
            var lowdiameter = 70;

            _doc3D = (ksDocument3D)_kompas.ActiveDocument3D();
            _part = (ksPart)_doc3D.GetPart((short)Part_Type.pTop_Part);

            CreateAshtraySketch(wallwidth, highdiameter, height, bottomthickness, lowdiameter);
        }

        /// <summary>
        /// Эскиз стакана.
        /// </summary>
        /// <param name="wallWidth">Толщина стенки.</param>
        /// <param name="highDiameter">Диаметр верхней окружности.</param>
        /// <param name="height">Высота.</param>
        /// <param name="bottomThicknes">Толщина дна.</param>
        /// <param name="lowDiameter">Диаметр нижней окружности.</param>
        /// <return>Возвращает выдавленный эскиз.</return>
        private void CreateAshtraySketch(double wallWidth, double highDiameter, double height,
            double bottomThicknes, double lowDiameter)
        {
            CreateSketch((short)Obj3dType.o3d_planeXOY);
            _sketchEdit = (ksDocument2D)_sketchDefinition.BeginEdit();
            _sketchEdit.ksLineSeg
                (origin, origin, origin + lowDiameter / 2, origin, 1);
            _sketchEdit.ksLineSeg
                (origin + lowDiameter / 2, origin, highDiameter / 2, height, 1);
            _sketchEdit.ksLineSeg
                (highDiameter / 2, height, highDiameter / 2 - wallWidth, height, 1);
            _sketchEdit.ksLineSeg
                (highDiameter / 2 - wallWidth, height, lowDiameter / 2 - wallWidth, bottomThicknes, 1);
            _sketchEdit.ksLineSeg
                (lowDiameter / 2 - wallWidth, bottomThicknes, origin, bottomThicknes, 1);
            _sketchEdit.ksLineSeg
                (origin, bottomThicknes, origin, origin, 1);
            _sketchEdit.ksLineSeg
              (origin, origin, origin, bottomThicknes, 3);
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
        public void FilletPlate()
        {
            ksEntityCollection faceCollection =
                _part.EntityCollection((short)ksObj3dTypeEnum.o3d_edge);
            const int y = 0;
            faceCollection.SelectByPoint(35, y, 0);

            ksEntity baseFace = faceCollection.First();
            CreateFillet(baseFace, 7);
        }

        /// <summary>
		/// Метод скругления
		/// </summary>
		/// <param name="face">Грань скругления</param>
		/// <param name="radius">Радиус скругления</param>
		private void CreateFillet(ksEntity face, double radius)
        {
            ksEntity fillet = _part.NewEntity((short)ksObj3dTypeEnum.o3d_fillet);

            ksFilletDefinition filletDefinition = fillet.GetDefinition();

            filletDefinition.radius = radius;
            filletDefinition.tangent = false;

            ksEntityCollection entityCollectionFillet = filletDefinition.array();
            entityCollectionFillet.Add(face);

            fillet.Create();
        }

        public void BuildHolesInThePlate(double count, double diameterPlate,
            double diameterStake)
        {
            var document =
                (ksDocument3D)_kompas.ActiveDocument3D();
            // Новый компонент
            var part =
                (ksPart)document.GetPart((short)Part_Type.pTop_Part);
            // TODO: RSDN
            if (part == null) return;
            var entitySketch =
                (ksEntity)part.NewEntity((short)Obj3dType.o3d_sketch);
            if (entitySketch == null) return;
            // Интерфейс свойств эскиза
            var sketchDef =
                (ksSketchDefinition)entitySketch.GetDefinition();
            if (sketchDef == null) return;
            // Получим интерфейс базовой плоскости XOY
            var basePlane =
                (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_planeXOY);
            if (basePlane == null) return;
            // Установим плоскость XOZ базовой для эскиза
            sketchDef.SetPlane(basePlane);
            // Создадим эскиз
            entitySketch.Create();

            // Интерфейс редактора эскиза
            var sketchEdit =
                (ksDocument2D)sketchDef.BeginEdit();
            if (sketchEdit == null) return;
            // Радиус тарелки со свдигом
            diameterPlate = -(diameterPlate / 2 - 10.5);
            // Радиус кола со сдвигом
            diameterStake = -diameterStake / 2 - 2;
            // Общая координата
            const double sharedCoordinate = 0.75;

            // TODO: можно попробовать обернуть в цикл
            sketchEdit.ksLineSeg
                (-sharedCoordinate, diameterPlate,
                -sharedCoordinate, diameterStake, 1);
            sketchEdit.ksLineSeg
                (-sharedCoordinate, diameterStake,
                sharedCoordinate, diameterStake, 1);
            sketchEdit.ksLineSeg
                (sharedCoordinate, diameterStake,
                sharedCoordinate, diameterPlate, 1);
            sketchEdit.ksLineSeg
                (sharedCoordinate, diameterPlate,
                -sharedCoordinate, diameterPlate, 1);
            // Завершение редактирования эскиза
            sketchDef.EndEdit();

            //Вырезать выдавливанием
            var entityCutExtr =
                (ksEntity)part.NewEntity((short)Obj3dType.o3d_cutExtrusion);
            if (entityCutExtr == null) return;
            var cutExtrDef =
                (ksCutExtrusionDefinition)entityCutExtr.GetDefinition();
            if (cutExtrDef != null)
            {
                // Глубина выреза
                const int thickness = 5;
                // Установим эскиз операции
                cutExtrDef.SetSketch(entitySketch);
                // Прямое направление
                cutExtrDef.directionType =
                    (short)Direction_Type.dtBoth;
                cutExtrDef.SetSideParam
                    (true, (short)End_Type.etBlind, thickness, 0, true);
                cutExtrDef.SetThinParam(false, 0, 0, 0);
            }
            // Создадим операцию вырезание выдавливанием
            entityCutExtr.Create();

            //Отверстия по концетрической сетке
            var circularCopyEntity =
                (ksEntity)part.NewEntity((short)Obj3dType.o3d_circularCopy);
            var circularCopyDefinition =
                (ksCircularCopyDefinition)circularCopyEntity.GetDefinition();
            circularCopyDefinition.SetCopyParamAlongDir
                (Convert.ToInt32(count), 90, true, false);
            var baseAxisOz =
                (ksEntity)part.GetDefaultEntity((short)Obj3dType.o3d_axisOZ);
            circularCopyDefinition.SetAxis(baseAxisOz);
            var entityCollection =
                (ksEntityCollection)circularCopyDefinition.GetOperationArray();
            entityCollection.Add(cutExtrDef);
            circularCopyEntity.Create();
        }

        /// <summary>
        /// Создание отверстий по концентрической сетке
        /// </summary>
        /// <param name="coordinate">Координата расположения</param>
        /// <param name="radius">Радиус отверстия</param>
        /// <param name="height">Высота вырезания</param>
        /// <param name="count">Количество отверстий</param>
        public void CreateArray(double coordinate, double radius,
            double height, int count)
        {
            var document = (ksDocument3D)_kompas.ActiveDocument3D();
            var part = (ksPart)document.GetPart((short)Part_Type.pTop_Part);
            const int coordinateX = 42;
            const int coordinateY = 42;
            const int styleLineBase = 1;
            const int styleLineAuxiliary = 6;
            const int stepCopy = 360;

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
            sketchEdit.ksCircle(coordinateX, coordinateY, coordinate,
                styleLineAuxiliary);
            sketchEdit.ksCircle(coordinate, coordinateY, radius,
                styleLineBase);
            sketchDefinition.EndEdit();

            ksEntity entityCutExtrusion = (ksEntity)part.NewEntity((short)
                Obj3dType.o3d_cutExtrusion);
            ksCutExtrusionDefinition cutExtrusionDefinition =
                (ksCutExtrusionDefinition)entityCutExtrusion.GetDefinition();
            cutExtrusionDefinition.directionType =
                (short)Direction_Type.dtMiddlePlane;
            cutExtrusionDefinition.SetSideParam(true,
                (short)End_Type.etUpToSurfaceTo, height);
            cutExtrusionDefinition.SetSketch(entitySketch);
            entityCutExtrusion.Create();

            ksEntity circularCopyEntity = (ksEntity)part.NewEntity((short)
                Obj3dType.o3d_circularCopy);
            ksCircularCopyDefinition circularCopyDefinition =
                (ksCircularCopyDefinition)circularCopyEntity.GetDefinition();
            circularCopyDefinition.SetCopyParamAlongDir(count, stepCopy,
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
