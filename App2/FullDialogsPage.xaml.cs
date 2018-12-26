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
using Windows.UI.Core;
using System.Threading.Tasks;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class FullDialogsPage : Page
    {
        public FullDialogsPage()
        {
            this.InitializeComponent();
            DialogsListBox.SelectionChanged += LoadDialog;
        }

        private static long peer_id = 0;
        private static Dialog CurrentDialog;

        public ObservableCollection<Message> Messages = new ObservableCollection<Message>();
        public ObservableCollection<Dialog> Dialogs = new ObservableCollection<Dialog>();

        private async void SendMessageButton_Click(object sender, RoutedEventArgs e) // переделать!
        {
            if (peer_id != 0)
            {
                VkAuth.Api.Messages.Send(new MessagesSendParams() { Message = TextToSendBox.Text, PeerId = peer_id });
                TextToSendBox.Text = "";
                await LoadMessages();
                await DialogsListLoad();
            }
        }

        private async Task LoadMessages()
        {
            Messages.Clear();
            var messages = (await VkAuth.Api.Messages.GetHistoryAsync(new MessagesGetHistoryParams() { PeerId = peer_id, Count = 10 })).Messages;
            foreach (var message in messages)
            {
                Messages.Add(new Message(message));
            }
        }

        private async void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
            await DialogsListLoad();

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => DialogsListLoad());
        }

        private async Task DialogsListLoad()
        {
            Dialogs.Clear();
            DialogSpaceClear();

            var dialogs = (await VkAuth.Api.Messages.GetConversationsAsync(new GetConversationsParams() { Extended = true, Count = 10 })).Items;
            foreach (var dialog in dialogs)
            {
                Dialogs.Add(new Dialog(dialog));
            }
        }

        private void DialogSpaceClear()
        {
            Messages.Clear();
            DialogImage.Source = null;
            DialogNameTextBlock.Text = "";
            MembersCountTextBlock.Text = "";
            PlatformBlock.Text = "";
        }

        private async void LoadDialog(object sender, SelectionChangedEventArgs e)
        {
            if (DialogsListBox.SelectedItems.SingleOrDefault() is Dialog dialog)
            {
                CurrentDialog = dialog;
            }
            else if (DialogsListBox.SelectedItems.Count == 0)
            {
                DialogSpaceClear();
                return;
            }

            var conversation = CurrentDialog.Conversation;
            peer_id = conversation.Peer.Id;

            if (!(conversation.ChatSettings == null))
            {
                DialogImage.Source = new BitmapImage(conversation.ChatSettings.Photo.Photo100);
                DialogNameTextBlock.Text = conversation.ChatSettings.Title;
                MembersCountTextBlock.Text = conversation.ChatSettings.MembersCount.ToString() + " участников";
                PlatformBlock.Text = "";
            }
            else if (peer_id < 0)
            {
                var group = (await VkAuth.Api.Groups.GetByIdAsync(null, (0 - peer_id).ToString(), null)).FirstOrDefault();
                DialogImage.Source = new BitmapImage(group.Photo100);
                DialogNameTextBlock.Text = group.Name;
                MembersCountTextBlock.Text = "";
                PlatformBlock.Text = "";
            }
            else
            {
                var user = (await VkAuth.Api.Users.GetAsync(new List<long>() { peer_id }, ProfileFields.All)).FirstOrDefault();
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
            await LoadMessages();
        }
    }
}
