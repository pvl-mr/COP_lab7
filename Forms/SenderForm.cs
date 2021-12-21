using BusinessLogics.BindingModels;
using BusinessLogics.Config;
using BusinessLogics.Interfaces;
using BusinessLogics.Managers;
using BusinessLogics.ViewModels;
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

namespace Forms
{
    public partial class SenderForm : Form
    {
        private readonly int ApiId = xxx;
        private readonly string ApiHash = "xxx";

        [Dependency]
        public new IUnityContainer Container { get; set; }

        private ISenderMessage _messenger;
        private SenderManager _manager;
        
        

        public SenderForm(SenderManager manager)
        {
            _manager = manager;          
            InitializeComponent();
            List<string> nameSenderPlugins = new List<string>();
            nameSenderPlugins.Add("Telegram");
            nameSenderPlugins.Add("Vk");
            nameSenderPlugins.Add("Viber");
            foreach (var plugin in manager.Plugins)
            {
                foreach (var pl in nameSenderPlugins)
                {
                    if (pl.Equals(plugin.Key)) {
                        comboBoxPlugins.Items.Add(plugin.Key);
                    }
                }  
            }
        }

        private async void buttonConnect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxNumber.Text))
            {
                MessageBox.Show("Введите номер телефона", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _messenger = _manager.Plugins[comboBoxPlugins.SelectedItem.ToString()];

            var number = textBoxNumber.Text;
            var code = textBoxCode.Text;
            try
            {
                var list = await _messenger.Connect(new SenderConfiguratorModel()
                {
                    ApiId = ApiId,
                    ApiHash = ApiHash,
                }, new AuthConfigurationModel()
                {
                    Code = code,
                    PhoneNumber = number
                });
                listBoxPeople.DataSource = list;
                listBoxPeople.DisplayMember = "Phone";
                listBoxPeople.ValueMember = "Id";
                buttonDisconnect.Enabled = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TelegramForm_Load(object sender, EventArgs e)
        {
            if (_manager.Headers is null || _manager.Headers.Count.Equals(0)) return;
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (_messenger is null) return;
            if (listBoxPeople.SelectedItem == null)
            {
                MessageBox.Show("Выберите номер получателя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBoxMessage.Text))
            {
                MessageBox.Show("Введите сообщение", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _messenger.SendMessage(new SendMessageModel()
            {
                Id = (int)listBoxPeople.SelectedValue,
                Message = textBoxMessage.Text
            });
            textBoxMessage.Text = string.Empty;
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            if (_messenger is null) return;
            _messenger.Disconnect();
            buttonDisconnect.Enabled = false;
            _messenger = null;
            listBoxPeople.DataSource = null;
        }
    }
}
