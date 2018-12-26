using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Photo = VkNet.Model.Attachments.Photo;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class DialogPage : Page
    {
        public DialogPage()
        {
            this.InitializeComponent();
        }

        private static long peer_id = 0;

        public ObservableCollection<Message> Messages = new ObservableCollection<Message>();
        
        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (peer_id != 0)
            {
                var txt = TextToSendBox.Text;
                VkAuth.Api.Messages.Send(new MessagesSendParams() { Message = txt, PeerId = peer_id });
                LoadMessages();
            }
        }

        private void LoadMessages()
        {
            Messages.Clear();
            var messages = VkAuth.Api.Messages.GetHistory(new MessagesGetHistoryParams() { Count = 10, PeerId = peer_id }).Messages;
            foreach (var message in messages)
            {
                Messages.Add(new Message(message));
            }
        }

        private void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
            var conversation = VkAuth.Api.Messages.GetConversations(new GetConversationsParams() { Count = 1, Extended = true }).Items[0].Conversation;
            peer_id = conversation.Peer.Id;

            if (!(conversation.ChatSettings == null))
            {
                DialogImage.Source = new BitmapImage(conversation.ChatSettings.Photo.Photo100);
                DialogNameTextBlock.Text = conversation.ChatSettings.Title;
                MembersCountTextBlock.Text = conversation.ChatSettings.MembersCount.ToString() + " участников";
            }
            else
            {
                var user = VkAuth.Api.Users.Get(new List<long>() { peer_id }, ProfileFields.All)[0];
                DialogImage.Source = new BitmapImage(user.Photo100);
                DialogNameTextBlock.Text = user.FirstName + " " + user.LastName;
                if (user.Online.Value)
                {
                    MembersCountTextBlock.Text = "Онлайн";
                }
                else
                {
                    var was_online = "";
                    if (user.Sex == Sex.Female)
                    {
                        was_online += "была";
                    }
                    else
                    {
                        was_online += "был";
                    }
                    was_online += " в сети ";
                    was_online += user.LastSeen.Time.Value.ToLocalTime().ToString();
                    MembersCountTextBlock.Text = was_online;
                }
                PlatformBlock.Text = "Платформа: ";
                switch (user.LastSeen.Platform)
                {
                    case "1":
                        PlatformBlock.Text += "Mobile";
                        break;
                    case "2":
                        PlatformBlock.Text += "iPhone app";
                        break;
                    case "3":
                        PlatformBlock.Text += "iPad app";
                        break;
                    case "4":
                        PlatformBlock.Text += "Android app";
                        break;
                    case "5":
                        PlatformBlock.Text += "Windows Phone app";
                        break;
                    case "6":
                        PlatformBlock.Text += "Windows 10 app";
                        break;
                    case "7":
                        PlatformBlock.Text += "Desktop";
                        break;
                }
            }
            LoadMessages();
        }
    }
}
