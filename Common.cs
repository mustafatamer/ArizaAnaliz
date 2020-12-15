using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArizaAnaliz
{
    public class Common
    {
        public const string TextStyleName = "Arial";
        public DBText Text(string text, double height, Point3d position, double rotation = 0, bool centerAligned = false, string textStyle = TextStyleName)
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

            if (centerAligned) // todo: centerAligned=true makes DT vanished
            {
                dbText.HorizontalMode = TextHorizontalMode.TextCenter;
                dbText.VerticalMode = TextVerticalMode.TextVerticalMid;
            }

            return dbText;
        }


        public ObjectId GetTextStyleId(string textStyleName, bool createIfNotFound = false, Database db = null)
        {
            db = db ?? HostApplicationServices.WorkingDatabase;
            return GetSymbolTableRecord(
                symbolTableId: db.TextStyleTableId,
                name: textStyleName,
                create: () => new TextStyleTableRecord { Name = textStyleName },
                defaultValue: db.Textstyle);
        }

        public ObjectId GetSymbolTableRecord(ObjectId symbolTableId, string name, ObjectId? defaultValue = null, Func<SymbolTableRecord> create = null)
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

    }

    public class DataGridSelectedCellsBehavior
    {
        // Source : https://archive.codeplex.com/?p=datagridthemesfromsl
        // Credit to : T. Webster, https://stackoverflow.com/users/266457/t-webster

        public static IList<DataGridCellInfo> GetSelectedCells(DependencyObject obj)
        {
            return (IList<DataGridCellInfo>)obj.GetValue(SelectedCellsProperty);
        }
        public static void SetSelectedCells(DependencyObject obj, IList<DataGridCellInfo> value)
        {
            obj.SetValue(SelectedCellsProperty, value);
        }

        public static readonly DependencyProperty SelectedCellsProperty = DependencyProperty.RegisterAttached("SelectedCells", typeof(IList<DataGridCellInfo>), typeof(DataGridSelectedCellsBehavior), new UIPropertyMetadata(null, OnSelectedCellsChanged));

        static SelectedCellsChangedEventHandler GetSelectionChangedHandler(DependencyObject obj)
        {
            return (SelectedCellsChangedEventHandler)obj.GetValue(SelectionChangedHandlerProperty);
        }
        static void SetSelectionChangedHandler(DependencyObject obj, SelectedCellsChangedEventHandler value)
        {
            obj.SetValue(SelectionChangedHandlerProperty, value);
        }

        static readonly DependencyProperty SelectionChangedHandlerProperty = DependencyProperty.RegisterAttached("SelectedCellsChangedEventHandler", typeof(SelectedCellsChangedEventHandler), typeof(DataGridSelectedCellsBehavior), new UIPropertyMetadata(null));

        //d is MultiSelector (d as ListBox not supported)
        static void OnSelectedCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            if (GetSelectionChangedHandler(d) != null)
                return;

            if (d is DataGrid)//DataGrid
            {
                DataGrid datagrid = d as DataGrid;
                SelectedCellsChangedEventHandler selectionchanged = null;
                foreach (var selected in GetSelectedCells(d) as IList<DataGridCellInfo>)
                    datagrid.SelectedCells.Add(selected);

                selectionchanged = (sender, e) =>
                {
                    SetSelectedCells(d, datagrid.SelectedCells);
                };
                SetSelectionChangedHandler(d, selectionchanged);
                datagrid.SelectedCellsChanged += GetSelectionChangedHandler(d);
            }
            //else if (d is ListBox)
            //{
            //    ListBox listbox = d as ListBox;
            //    SelectionChangedEventHandler selectionchanged = null;

            //    selectionchanged = (sender, e) =>
            //    {
            //        SetSelectedCells(d, listbox.SelectedCells);
            //    };
            //    SetSelectionChangedHandler(d, selectionchanged);
            //    listbox.SelectionChanged += GetSelectionChangedHandler(d);
            //}
        }

    }
}