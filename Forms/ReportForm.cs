using BusinessLogics.Config;
using BusinessLogics.Interfaces;
using BusinessLogics.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Unity;
using static BusinessLogics.Config.ChartConfigModel;

namespace Forms
{
    public partial class ReportForm : Form
    {
        [Dependency]
        public new IUnityContainer Container { get; set; }
        private IReportBuilder _reporter;
        private ReportManager _manager;
        public ReportForm(ReportManager manager)
        {
            InitializeComponent();
            List<string> nameReportPlugins = new List<string>();
            nameReportPlugins.Add("Excel");
            nameReportPlugins.Add("Pdf");
            nameReportPlugins.Add("Word");
            foreach (var plugin in manager.Plugins)
            {
                foreach (var pl in nameReportPlugins)
                {
                    if (pl.Equals(plugin.Key))
                    {
                        comboBoxPlugins.Items.Add(plugin.Key);
                    }
                }
            }
            _manager = manager;
        }

        private void buttonCreateReport_Click(object sender, EventArgs e)
        {
            _reporter = _manager.Plugins[comboBoxPlugins.SelectedItem.ToString()];
            _reporter.OpenFile();
            string path = "C:\\Users\\pavlo\\source\\repos\\COP\\";
            _reporter.AddImage(new ImageConfigModel()
            {

                Path = path + "nature.jpg",
                Coordinates = new int[] {100, 100, 150, 200 }
            }); ;
            List<DiagramSeries> thatlist = new List<DiagramSeries>();
            thatlist.Add(new DiagramSeries
            {
                Name = "скорость",
                Values = new double[] { 100, 120, 195, 127, 211 }
            });
            thatlist.Add(new DiagramSeries
            {
                Name = "быстрота",
                Values = new double[] { 130, 16, 18.2, 21.72 }
            });
            _reporter.AddChart(new ChartConfigModel()
            {
                DiagTitle = "Диаграмма",
                Data = thatlist
            });
            _reporter.AddTable(new TableConfigModel()
            {
                TitleName = "Таблица",
                Text = new List<string> { "Слова о чём-то 1", "Слова о чём-то 2", "Слова о чём-то 3", "Слова о чём-то 4", "Слова о чём-то 5" }
            });
            _reporter.AddParagraph(new ParagraphConfigModel()
            {
                Text = "Новый абзац",
                Cells = new int[] { 3, 1 }
            });

            var fbd = new SaveFileDialog();
            fbd.FileName = "Report.xls";
            fbd.Filter = "xls file | *.xls";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _reporter.SaveDocument(fbd.FileName);
                    {
                        MessageBox.Show("Успех", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
