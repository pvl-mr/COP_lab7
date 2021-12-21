using BusinessLogics.Config;
using BusinessLogics.Interfaces;
using BusinessLogics.ViewModels;
using System;
using System.Collections.Generic;
using TLSharp.Core;
using System.Threading.Tasks;
using BusinessLogics.BindingModels;

namespace TelegramPlugin
{
    public class Telegram : ISenderMessage
    {
        private TelegramClient _client;
        private string _hashCode;

        public string PluginType => "Telegram";

        public async Task Connect(SenderConfiguratorModel config)
        {
            _client = new TelegramClient(config.apiId, config.apiHash);
            await _client.ConnectAsync();
            /*var hash = await client.SendCodeRequestAsync("<user_number>");
            var code = "<code_from_telegram>"; // you can change code in debugger
            var user = await client.MakeAuthAsync("<user_number>", hash, code);
            var result = await client.GetContactsAsync();

            //find recipient in contacts
            return result.Users;*/
        }

        public async void SendCodeRequestAsync(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return;
            _hashCode = await _client.SendCodeRequestAsync(phoneNumber);
        }

        public async void SendMessage(SenderConfigModel config)
        {
            if (_client is null ||
                config is null ||
                (config.Id).Equals(default) ||
                string.IsNullOrEmpty(config.Message)) return;

            await _client.SendMessageAsync(new TLInputPeerUser() { UserId = config.Id }, config.Message);
        }

        public void SendMessage(SendMessageModel message)
        {
            throw new NotImplementedException();
        }

        public async Task MakeAuthAsync(AuthConfigurationModel config)
        {
            if (config is null ||
                string.IsNullOrEmpty(config.Code) ||
                string.IsNullOrEmpty(config.PhoneNumber) ||
                _hashCode is null) return;

            await _client.MakeAuthAsync(config.PhoneNumber, _hashCode, config.Code);
        }
    }
}
