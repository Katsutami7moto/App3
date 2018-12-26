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

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class DialogMiniaturePage : Page
    {
        public DialogMiniaturePage()
        {
            this.InitializeComponent();
        }

        private void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogs = VkAuth.Api.Messages.GetConversations(new GetConversationsParams() { Count = 1, Extended = true });
            var last_dialog = dialogs.Items[0];
            var conversation = last_dialog.Conversation;
            var last_message = last_dialog.LastMessage;
            var last_user = VkAuth.Api.Users.Get(new List<long>() { (long)last_message.FromId })[0];

            DialogImage.Source = new BitmapImage(conversation.ChatSettings.Photo.Photo100);
            DialogNameTextBlock.Text = conversation.ChatSettings.Title;
            PeopleNameTextBlock.Text = last_user.FirstName;
            MessageBeginTextBlock.Text = last_message.Text;
            UnreadCountTextBlock.Text = conversation.UnreadCount.ToString();
        }
    }
}
