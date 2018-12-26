using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using VkNet;
using VkNet.Categories;
using VkNet.Enums;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Exception;
using VkNet.Model.RequestParams;
using VkNet.Utils;
using System.Diagnostics;
using VkNet.Model;
using Windows.Graphics.Imaging;

namespace App2
{
    public class Message
    {
        public string UserName { get; set; }
        public string UserImagePath { get; set; }
        public string DateText { get; set; }
        public TextAndAttachments TextAndAttachments { get; set; }

        public Message(VkNet.Model.Message message)
        {
            var from_id = message.FromId.Value;
            if (from_id < 0)
            {
                var group = VkAuth.Api.Groups.GetById(null, (0 - from_id).ToString(), null).FirstOrDefault();
                UserImagePath = group.Photo50.ToString();
                UserName = group.Name;
            }
            else
            {
                var user = VkAuth.Api.Users.Get(new List<long>() { from_id }, ProfileFields.Photo50).FirstOrDefault();
                UserImagePath = user.Photo50.ToString();
                UserName = user.FirstName + " " + user.LastName;
            }
            DateText = message.Date.Value.ToLocalTime().ToString();
            TextAndAttachments = new TextAndAttachments(message.Text, message.Attachments);
        }
    }
}
