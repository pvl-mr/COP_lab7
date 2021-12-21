using BusinessLogics.BindingModels;
using BusinessLogics.Config;
using BusinessLogics.Interfaces;
using BusinessLogics.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TeleSharp.TL;
using TLSharp.Core;


namespace Tel
{
    [Export(typeof(ISenderMessage))]
    public class Telegram : ISenderMessage
    {
        private TelegramClient _client;
        private string _hashCode;
        public string PluginType => "Telegram";

        public IEnumerable<(string Title, string Message)> Errors => throw new NotImplementedException();

        public async Task<List<UserModel>> Connect(SenderConfiguratorModel config, AuthConfigurationModel aconfig = null)
        {    
            if (string.IsNullOrEmpty(aconfig.Code) && !string.IsNullOrEmpty(aconfig.PhoneNumber))
            {
                _client = new TelegramClient(config.ApiId, config.ApiHash);
                await _client.ConnectAsync();
                _hashCode = await _client.SendCodeRequestAsync(aconfig.PhoneNumber);
                return null;           
            } else if (!string.IsNullOrEmpty(aconfig.Code) && !string.IsNullOrEmpty(aconfig.PhoneNumber))
            {
                await _client.MakeAuthAsync(aconfig.PhoneNumber, _hashCode, aconfig.Code);
            }       
            var list = new List<UserModel>();
            var result = await _client.GetContactsAsync();
            if (result != null)
            {
                list = result.Users
                    .Where(x => x.GetType() == typeof(TLUser))
                    .Cast<TLUser>().Select(x => new UserModel()
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Phone = x.Phone,
                        Username = x.Username
                    }).ToList();
            }

            return list;

        }

        public void Disconnect()
        {
            var path = "C:\\Users\\pavlo\\source\\repos\\COP\\Forms\\bin\\Debug\\session.dat";
            if (File.Exists(path)) File.Delete(path);
        }

        public async void SendMessage(SendMessageModel config)
        {
            if (_client is null ||
                config is null ||
                (config.Id).Equals(default) ||
                string.IsNullOrEmpty(config.Message)) return;

            await _client.SendMessageAsync(new TLInputPeerUser() { UserId = config.Id }, config.Message);
        }
    }
}
