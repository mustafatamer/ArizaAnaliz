using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace ArizaAnaliz
{
    public static class ExtentionMethods
    {
        public static void LoadViewFromUri(this UserControl userControl, string baseUri)
        {
            try
            {
                var resourceLocater = new Uri(baseUri, UriKind.RelativeOrAbsolute);
                var exprCa = (PackagePart)typeof(System.Windows.Application).GetMethod("GetResourceOrContentPart", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { resourceLocater });
                var stream = exprCa.GetStream();
                var uri = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), resourceLocater);
                var parserContext = new ParserContext
                {
                    BaseUri = uri

                };
                typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { stream, parserContext, userControl, true });
            }
            catch (Exception ex)
            {
                //log
            }
        }
        public static void LoadViewFromUri(this Window window, string baseUri)
        {
            try
            {
                var resourceLocater = new Uri(baseUri, UriKind.RelativeOrAbsolute);
                var exprCa = (PackagePart)typeof(System.Windows.Application).GetMethod("GetResourceOrContentPart", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { resourceLocater });
                var stream = exprCa.GetStream();
                var uri = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), resourceLocater);
                var parserContext = new ParserContext
                {
                    BaseUri = uri

                };
                typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { stream, parserContext, window, true });
            }
            catch (Exception ex)
            {
                //log
            }
        }
        public static void QForEach(this IEnumerable<ObjectId> ids, Action<object> action)
        {
            Database database = ids.Select(id => id.Database).Distinct().Single();
            using (var trans = database.TransactionManager.StartTransaction())
            {
                ids.Select(id => trans.GetObject(id, OpenMode.ForWrite)).ToList().ForEach(action);
                trans.Commit();
            }
        }
        public static ObjectId AddToCurrentSpace(this Entity entity, Database db = null)
        {
            db = db ?? HostApplicationServices.WorkingDatabase;
            using (var trans = db.TransactionManager.StartTransaction())
            {
                var currentSpace = (BlockTableRecord)trans.GetObject(db.CurrentSpaceId, OpenMode.ForWrite, false);
                var id = currentSpace.AppendEntity(entity);
                trans.AddNewlyCreatedDBObject(entity, true);
                trans.Commit();
                return id;
            }
        }
        public const string TextStyleName = "Arial";
        public static  DBText Text(string text, double height, Point3d position, double rotation = 0, bool centerAligned = false, string textStyle = TextStyleName)
        {
            var textStyleId = GetTextStyleId(textStyle);
            var style = textStyleId.QOpenForRead<TextStyleTableRecord>();
            var dbText = new DBText
            {
                TextString = text,
                Position = position,
                Rotation = rotation,
                TextStyleId = textStyleId,
                Height = height,
                Oblique = style.ObliquingAngle,
                WidthFactor = style.XScale
            };

            if (centerAligned) // todo: centerAligned=true DT'yi yok eder
            {
                dbText.HorizontalMode = TextHorizontalMode.TextCenter;
                dbText.VerticalMode = TextVerticalMode.TextVerticalMid;
            }

            return dbText;
        }
        public static ObjectId GetTextStyleId(string textStyleName, bool createIfNotFound = false, Database db = null)
        {
            db = db ?? HostApplicationServices.WorkingDatabase;
            return GetSymbolTableRecord(
                symbolTableId: db.TextStyleTableId,
                name: textStyleName,
                create: () => new TextStyleTableRecord { Name = textStyleName },
                defaultValue: db.Textstyle);
        }
        public static ObjectId GetSymbolTableRecord(ObjectId symbolTableId, string name, ObjectId? defaultValue = null, Func<SymbolTableRecord> create = null)
        {
            using (var trans = symbolTableId.Database.TransactionManager.StartTransaction())
            {
                var table = (SymbolTable)trans.GetObject(symbolTableId, OpenMode.ForRead);
                if (table.Has(name))
                {
                    return table[name];
                }

                if (create != null)
                {
                    var record = create();
                    table.UpgradeOpen();
                    var result = table.Add(record);
                    trans.AddNewlyCreatedDBObject(record, true);
                    trans.Commit();
                    return result;
                }
            }

            return defaultValue.Value;
        }
        public static DBObject QOpenForRead(this ObjectId id)
        {
            using (var trans = id.Database.TransactionManager.StartOpenCloseTransaction())
            {
                return trans.GetObject(id, OpenMode.ForRead);
            }
        }
        public static T QOpenForRead<T>(this ObjectId id) where T : DBObject
        {
            return id.QOpenForRead() as T;
        }
        public static void QOpenForWrite(this IEnumerable<ObjectId> ids, Action<DBObject[]> action) 
        {
            using (var trans = ArizaAnalizExApp.db.TransactionManager.StartTransaction())
            {
                var list = ids.Select(id => trans.GetObject(id, OpenMode.ForWrite)).ToArray();
                action(list);
                trans.Commit();
            }
        }



        public static Extents3d GetExtents(this IEnumerable<ObjectId> entityIds)
        {
            return GetExtents(entityIds.QOpenForRead<Entity>());
        }
        public static Extents3d GetExtents(this IEnumerable<Entity> entities)
        {
            var extent = entities.First().GeometricExtents;
            foreach (var ent in entities)
            {
                extent.AddExtents(ent.GeometricExtents);
            }
            return extent;
        }

        public static T[] QOpenForRead<T>(this IEnumerable<ObjectId> ids) where T : DBObject // newly 20130122
        {
            using (var trans = GetDatabase(ids).TransactionManager.StartTransaction())
            {
                return ids.Select(id => trans.GetObject(id, OpenMode.ForRead) as T).ToArray();
            }
        }

        internal static Database GetDatabase(IEnumerable<ObjectId> objectIds)
        {
            return objectIds.Select(id => id.Database).Distinct().Single();
        }

        public static void HighlightObjects(IEnumerable<ObjectId> entityIds)
        {
            entityIds.QForEachForRead<Entity>(entity => entity.Highlight());
        }

        public static void QForEachForRead<T>(this IEnumerable<ObjectId> ids, Action<T> action) where T : DBObject // newly 20130520
        {
            using (var trans = GetDatabase(ids).TransactionManager.StartTransaction())
            {
                ids.Select(id => trans.GetObject(id, OpenMode.ForRead) as T).ToList().ForEach(action);
                trans.Commit();
            }
        }

        public static void QOpenForWrite(this ObjectId id, Action<DBObject> action)
        {
            using (var trans = id.Database.TransactionManager.StartTransaction())
            {
                action(trans.GetObject(id, OpenMode.ForWrite));
                trans.Commit();
            }
        }
        public static void ZoomObjects(IEnumerable<ObjectId> entityIds, int factor = 10)
        {
            if (entityIds.Count() > 0)
            {
                var extent = entityIds.GetExtents();
                ZoomView(extent, factor);
            }
        }

        public static void ZoomView(Extents3d extents, int factor = 10)
        {
            Zoom(extents.MinPoint, extents.MaxPoint, new Point3d(), factor);
        }

        internal static void Zoom(Point3d min, Point3d max, Point3d center, double factor)
        {
            // Mevcut belgeyi ve veritabanını alın
            var document = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument;
            var database = document.Database;

            int currentViewport = Convert.ToInt32(Autodesk.AutoCAD.ApplicationServices.Application.GetSystemVariable("CVPORT"));

            // Mevcut boşluğun kapsamını alın nokta yok veya sadece bir merkez noktası sağlanmış Model uzayının güncel olup olmadığını kontrol edin
            if (database.TileMode)
            {
                if (min.Equals(new Point3d()) && max.Equals(new Point3d()))
                {
                    min = database.Extmin;
                    max = database.Extmax;
                }
            }
            else
            {
                //Kağıt alanının mevcut olup olmadığını kontrol edin
                if (currentViewport == 1)
                {
                    // Kağıt alanının kapsamını alın
                    if (min.Equals(new Point3d()) && max.Equals(new Point3d()))
                    {
                        min = database.Pextmin;
                        max = database.Pextmax;
                    }
                }
                else
                {
                    // Model alanının kapsamlarını öğrenin
                    if (min.Equals(new Point3d()) && max.Equals(new Point3d()))
                    {
                        min = database.Extmin;
                        max = database.Extmax;
                    }
                }
            }

            // Bir işlem başlatın
            using (var trans = database.TransactionManager.StartTransaction())
            {
                // Mevcut görünümü alın
                using (var currentView = document.Editor.GetCurrentView())
                {
                    Extents3d extents;

                    // WCS koordinatlarını DCS'ye çevir
                    var matWCS2DCS = Matrix3d.PlaneToWorld(currentView.ViewDirection);
                    matWCS2DCS = Matrix3d.Displacement(currentView.Target - Point3d.Origin) * matWCS2DCS;
                    matWCS2DCS = Matrix3d.Rotation(
                        angle: -currentView.ViewTwist,
                        axis: currentView.ViewDirection,
                        center: currentView.Target) * matWCS2DCS;

                    /* Bir merkez noktası belirtilmişse, minimum ve maks.
                     kapsam noktası
                     Merkez ve Ölçek modları için*/
                    if (center.DistanceTo(Point3d.Origin) != 0)
                    {
                        min = new Point3d(center.X - (currentView.Width / 2), center.Y - (currentView.Height / 2), 0);
                        max = new Point3d((currentView.Width / 2) + center.X, (currentView.Height / 2) + center.Y, 0);
                    }

                    // Bir çizgi kullanarak bir kapsam nesnesi oluşturun
                    using (Line line = new Line(min, max))
                    {
                        extents = new Extents3d(line.Bounds.Value.MinPoint, line.Bounds.Value.MaxPoint);
                    }

                    // Calculate the ratio between the width and height of the current view
                    double viewRatio = currentView.Width / currentView.Height;

                    // Tranform the extents of the view
                    matWCS2DCS = matWCS2DCS.Inverse();
                    extents.TransformBy(matWCS2DCS);

                    double width;
                    double height;
                    Point2d newCenter;

                    // Check to see if a center point was provided (Center and Scale modes)
                    if (center.DistanceTo(Point3d.Origin) != 0)
                    {
                        width = currentView.Width;
                        height = currentView.Height;

                        if (factor == 0)
                        {
                            center = center.TransformBy(matWCS2DCS);
                        }

                        newCenter = new Point2d(center.X, center.Y);
                    }
                    else // Working in Window, Extents and Limits mode
                    {
                        // Calculate the new width and height of the current view
                        width = extents.MaxPoint.X - extents.MinPoint.X;
                        height = extents.MaxPoint.Y - extents.MinPoint.Y;

                        // Get the center of the view
                        newCenter = new Point2d(
                            ((extents.MaxPoint.X + extents.MinPoint.X) * 0.5),
                            ((extents.MaxPoint.Y + extents.MinPoint.Y) * 0.5));
                    }

                    // Check to see if the new width fits in current window
                    if (width > (height * viewRatio))
                    {
                        height = width / viewRatio;
                    }

                    // Resize and scale the view
                    if (factor != 0)
                    {
                        currentView.Height = height * factor;
                        currentView.Width = width * factor;
                    }

                    // Set the center of the view
                    currentView.CenterPoint = newCenter;

                    // Set the current view
                    document.Editor.SetCurrentView(currentView);
                }

                // Commit the changes
                trans.Commit();
            }
        }
    }
}
