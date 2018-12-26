using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
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
    public class Dialog
    {
        public BitmapImage Image { get; set; }
        public string Name { get; set; }
        public string LastMessage { get; set; }
        public string UnreadCount { get; set; }
        public string LastDate { get; set; }
        public Conversation Conversation { get; set; }

        public Dialog(ConversationAndLastMessage dialog)
        {
            Conversation = dialog.Conversation;
            VkNet.Model.Message last_message = dialog.LastMessage;
            long peer_id = 0 - Conversation.Peer.Id;
            long last_message_id = 0 - last_message.FromId.Value;

            string last_user_name = "";
            if (last_message_id < 0 && last_message_id != peer_id)
            {
                if ((0 - last_message_id) == VkAuth.Api.UserId.Value)
                {
                    last_user_name = "Вы: ";
                }
                else
                {
                    var last_user = VkAuth.Api.Users.Get(new List<long>() { 0 - last_message_id }).FirstOrDefault();
                    last_user_name = last_user.FirstName + ": ";
                }
            }

            BitmapImage image = new BitmapImage();
            string name = "";
            if (Conversation.ChatSettings == null)
            {
                if (peer_id < 0)
                {
                    var user = VkAuth.Api.Users.Get(new List<long>() { 0 - peer_id }, ProfileFields.All).FirstOrDefault();
                    image = new BitmapImage(user.Photo50);
                    name = user.FirstName + " " + user.LastName;
                }
                else
                {
                    var group = VkAuth.Api.Groups.GetById(null, peer_id.ToString(), null).FirstOrDefault();
                    image = new BitmapImage(group.Photo50);
                    name = group.Name;
                }
            }
            else
            {
                image = new BitmapImage(Conversation.ChatSettings.Photo.Photo50);
                name = Conversation.ChatSettings.Title;
            }

            Image = image;
            Name = name;
            LastMessage = last_user_name + last_message.Text;
            var unread_count = Conversation.UnreadCount;
            if (unread_count == 0)
            {
                UnreadCount = "";
            }
            else
            {
                UnreadCount = unread_count.ToString();
            }
            LastDate = last_message.Date.Value.ToLocalTime().ToString();
        }
    }
}
