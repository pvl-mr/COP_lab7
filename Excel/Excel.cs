using System;
using System.ComponentModel.Composition;
using System.Linq;
using BusinessLogics.Config;
using BusinessLogics.Interfaces;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Office.Interop.Excel;

namespace Excel
{
    [Export(typeof(IReportBuilder))]
    class Excel : IReportBuilder
    {
        Application excel;
        Workbook workBook;
        Worksheet sheet;
        public string PluginType => "Report";
        public void OpenFile()
        {
            excel = new Application { SheetsInNewWorkbook = 1, Visible = false, DisplayAlerts = false };
            workBook = excel.Workbooks.Add(Type.Missing);
            sheet = (Worksheet)excel.Worksheets.get_Item(1);

        }
        public void AddChart(ChartConfigModel config)
        {
            Chart excelchart = (Chart)excel.Charts.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            excelchart.Activate();
            excelchart.Select(Type.Missing);
            excel.ActiveChart.ChartType = XlChartType.xlLineMarkers;
            excel.ActiveChart.HasTitle = true;
            excel.ActiveChart.ChartTitle.Text = config.DiagTitle;
            excel.ActiveChart.ChartTitle.Font.Size = 14;
            excel.ActiveChart.ChartTitle.Font.Color = 255;
            ((Axis)excel.ActiveChart.Axes(XlAxisType.xlCategory,
                XlAxisGroup.xlPrimary)).HasTitle = true;
            ((Axis)excel.ActiveChart.Axes(XlAxisType.xlCategory,
                XlAxisGroup.xlPrimary)).AxisTitle.Text = "Ox";
            ((Axis)excel.ActiveChart.Axes(XlAxisType.xlValue,
                XlAxisGroup.xlPrimary)).HasTitle = true;
            ((Axis)excel.ActiveChart.Axes(XlAxisType.xlValue,
                XlAxisGroup.xlPrimary)).AxisTitle.Text = "Oy";
            excel.ActiveChart.HasLegend = true;
            excel.ActiveChart.Legend.Position = XlLegendPosition.xlLegendPositionBottom;
            SeriesCollection seriesCollection = (SeriesCollection)excel.ActiveChart.SeriesCollection(Type.Missing);
            int index = 1;
            foreach (var element in config.Data)
            {
                var this_series = seriesCollection.NewSeries();
                this_series.Name = config.Data.ElementAt(index - 1).Name;
                this_series.Values = config.Data.ElementAt(index - 1).Values;
                index++;
            }

            excel.ActiveChart.Location(XlChartLocation.xlLocationAsObject, "Лист1");
            sheet.Shapes.Item(1).Height = 450;
            sheet.Shapes.Item(1).Width = 500;
        }

        public void AddImage(ImageConfigModel config)
        {
            sheet.Shapes.AddPicture(config.Path, Microsoft.Office.Core.MsoTriState.msoFalse, Microsoft.Office.Core.MsoTriState.msoCTrue, config.Coordinates[0], config.Coordinates[1], config.Coordinates[2], config.Coordinates[3]);
        }

        public void AddParagraph(ParagraphConfigModel config)
        {
            string text = sheet.Cells[config.Cells[0], config.Cells[1]].Text;
            sheet.Cells[config.Cells[0], config.Cells[1]] = text + "\n" + config.Text;
        }

        public void AddTable(TableConfigModel config)
        {
            sheet.Cells[1, 1] = config.TitleName;
            int index = 3;
            foreach (var element in config.Text)
            {
                sheet.Cells[index, 1] = element;
                index++;
            }
        }

        public void SaveDocument(string filepath)
        {
            excel.Application.ActiveWorkbook.SaveAs(filepath, XlSaveAsAccessMode.xlNoChange);
            excel.Quit();
        }
    }
}
