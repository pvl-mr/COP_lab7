using BusinessLogics.BindingModels;
using BusinessLogics.Config;
using BusinessLogics.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogics.Interfaces
{
    // <summary>
    /// Рассылка сообщений через различные API
    /// </summary>
    public interface ISenderMessage
    {
        Task<List<UserModel>> Connect(SenderConfiguratorModel sconfig, AuthConfigurationModel aconfig);
        void SendMessage(SendMessageModel message);
        void Disconnect();
    }
}
