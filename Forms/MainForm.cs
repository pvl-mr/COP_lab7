﻿using BusinessLogics.BindingModels;
using BusinessLogics.BusinessLogic;
using BusinessLogics.Config;
using BusinessLogics.Interfaces;
using BusinessLogics.ViewModels;
using PavComponents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Unity;

namespace Forms
{
    public partial class MainForm : Form
    {
        [Dependency]
        public new IUnityContainer Container { get; set; }
        private readonly BillLogic billLogic;
        private readonly TypeLogic waiterLogic;
        
        public MainForm(BillLogic billLogic, TypeLogic waiterLogic)
        {
            InitializeComponent();
            this.billLogic = billLogic;
            this.waiterLogic = waiterLogic;
        }

        /// <summary>
        /// Срабатывает при загрузке формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadData()
        {
            Queue<string> propertyNames = new Queue<string>();
            propertyNames.Enqueue("TypeName");
            propertyNames.Enqueue("Sum");
            propertyNames.Enqueue("Id");
            propertyNames.Enqueue("WaiterFullName");

            treeView.SetTreeСonfiguration(propertyNames);

            try
            {
                treeView.Clear();
                var list = billLogic.Read(null);
                foreach (var bill in list)
                {
                    treeView.AddItems(bill);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK,
               MessageBoxIcon.Error);
            }
        }

        private void FormBills_Load_1(object sender, EventArgs e)
        {
            LoadData();
        }
        private void CreateBill(object sender, EventArgs e)
        {
            var form = Container.Resolve<FormBill>();
            form.ShowDialog();
            LoadData();
        }

        private void создатьНовыйСчётToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<FormBill>();
            form.ShowDialog();
            LoadData();
        }

        private void редактироватьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<FormBill>();
            var node = treeView.GetSelectedItem<BillViewModel>();
            form.Elem = billLogic.Read(new BillBindingModel { Id = node.Id })[0];
            form.Id = node.Id;
            form.ShowDialog();
            LoadData();
        }

        private void удалитьЗаписьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedItem = treeView.GetSelectedItem<BillViewModel>();
            if (selectedItem is null) return;

            if (MessageBox.Show("Удалить запись", "Вопрос", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                int id = selectedItem.Id;
                try
                {
                    billLogic.Delete(new BillBindingModel() { Id = id });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                LoadData();
            }
        }

        private void создатьПростойДокументToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] bills = new string[6];
            var myList = billLogic.Read(new BillBindingModel
            {
                Sum = 0,
            });
            for (int i = 0; i < myList.Count; i++)
            {
                bills[i] = myList[i].WaiterFullName + " : " + myList[i].Info;
            }
            using (var d = new SaveFileDialog() { Filter = "xlsx|*.xlsx" })
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    excelText1.CreateExcel(d.FileName,
                    "Акционные счета", bills);
                }
            }

        }

        private void создатьДоктСНастраиваемойТаблицейToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new SaveFileDialog() { Filter = "docx|*.docx", FileName = "Bills" })
            {
                if (d.ShowDialog() == DialogResult.OK)
                {
                    var bills_data = billLogic.Read(null);

                    foreach (var element in bills_data)
                    {
                        if (element.Sum.Equals("0.00"))
                        {
                            element.Sum = "По акции";
                        }
                    }

                    int[] height_rows = new int[billLogic.Read(null).Count + 1];

                    for (int i = 0; i < billLogic.Read(null).Count + 1; i++) {
                        height_rows[i] = 20;
                    }


                    bool result = word_Custom_Table_Component1.CreateDoc(d.FileName, "Счета", height_rows, new List<WordTableColumn>
                    {
                        new WordTableColumn {Header = "ID", Width = 40, PropertyName = "Id"},
                    new WordTableColumn {Header = "Тип заказа", Width = 100, PropertyName = "TypeName"},
                    new WordTableColumn {Header = "Описание", Width = 180, PropertyName = "Info"},

                    new WordTableColumn {Header = "ФИО офицанта", Width = 100, PropertyName = "WaiterFullName"},
                        new WordTableColumn {Header = "Стоимость", Width = 80, PropertyName = "Sum"}
                    }, bills_data);
                    if (result == true)
                    {
                        MessageBox.Show("Отчёт создан", "Сообщение",
               MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void созданиеДоктаСДиаграммойToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var d = new SaveFileDialog() { Filter = "pdf|*.pdf", FileName = "Bills" })
            {
                if (d.ShowDialog() == DialogResult.OK)
                {

                    var listData = billLogic.Read(null);
                    Dictionary<string, int> result = new Dictionary<string, int>();

                    for (int i = 0; i < waiterLogic.Read(null).Count; i++)
                    {
                        result[waiterLogic.Read(null)[i].TypeName] = 0;
                    }

                    for (int i = 0; i < listData.Count; i++)
                    {
                        if (listData[i].Sum.Equals("0.00"))
                        {
                            result[listData[i].TypeName]++;
                        }
                    }

                    shPDFChart1.CreatePDF(
                         d.FileName,
                        "Акционные счета",
                        "Диаграмма счетов",
                        LegendLocation.Right, result

                    );
                    MessageBox.Show("Отчёт создан", "Сообщение",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void телеграмToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<SenderForm>();
            form.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = Container.Resolve<ReportForm>();
            form.ShowDialog();
        }
    }
}
