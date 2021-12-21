using BusinessLogics.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogics.Interfaces
{
    public interface IReportBuilder
    {
        string PluginType { get; }
        void OpenFile();
        void SaveDocument(string filepath);
        void AddParagraph(ParagraphConfigModel config);
        void AddImage(ImageConfigModel config);
        void AddTable(TableConfigModel config);
        void AddChart(ChartConfigModel config);
    }
}
