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
using System.Threading.Tasks;
using Windows.UI.Core;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class NewsFeedPage : Page
    {
        public ObservableCollection<WallPost> WallPosts = new ObservableCollection<WallPost>();

        public NewsFeedPage()
        {
            this.InitializeComponent();
        }

        private async void Refresh()
        {
            WallPosts.Clear();

            //NewsFeed news_feed = VkAuth.Api.NewsFeed.Get(new NewsFeedGetParams() { Filters = NewsTypes.Post, Count = 20 });
            NewsFeed news_feed = await VkAuth.Api.NewsFeed.GetAsync(new NewsFeedGetParams() { Filters = NewsTypes.Post, Count = 20 });

            var items = news_feed.Items;
            var users = news_feed.Profiles;
            var groups = news_feed.Groups;
            string name;
            Uri photo;
            
            foreach (var item in items)
            {
                var source_id = item.SourceId;
                if (source_id < 0)
                {
                    var group =
                        groups
                        .Where(x => x.Id == (0 - source_id))
                        .FirstOrDefault();
                    name = group.Name;
                    //photo = group.Photo50; - раньше было!
                    photo = VkAuth.Api.Groups.GetById(null, group.Id.ToString(), null).FirstOrDefault().Photo50;
                }
                else
                {
                    var user =
                        users
                        .Where(x => x.Id == source_id)
                        .FirstOrDefault();
                    name = user.FirstName + " " + user.LastName;
                    photo = user.Photo50;
                }

                WallPosts.Add(new WallPost(name, photo, newspost: item));
            }
        }

        private /*async*/ void StartTestButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();

            //await Refresh();

            //await Task.Run(() => Refresh());

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Refresh());
        }
    }
}
