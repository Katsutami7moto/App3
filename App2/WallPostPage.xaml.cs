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
using Windows.UI.Popups;
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
using Button = Windows.UI.Xaml.Controls.Button;
using Image = Windows.UI.Xaml.Controls.Image;
using Photo = VkNet.Model.Attachments.Photo;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class WallPostPage : Page
    {
        public WallPostPage()
        {
            this.InitializeComponent();
        }

        public WallPost GroupWallPost { get; set; }

        public ObservableCollection<WallPost> Posts = new ObservableCollection<WallPost>();
        
        private void WallPostTestButton_Click(object sender, RoutedEventArgs e)
        {
            WallPostTest();
        }
        
        private async void WallPostTest()
        {
            Posts.Clear();

            var groups = VkAuth.Api.Groups.Get(new GroupsGetParams() { Extended = true }).ToList();
            var randomizer = new Random();
            var group1 = groups[randomizer.Next(groups.Count)];
            try
            {
                var wall11 = VkAuth.Api.Wall.Get(new WallGetParams { OwnerId = -group1.Id, Count = 1, Filter = WallFilter.Owner });
            }
            catch
            {
                var alert = new MessageDialog("This wall is empty. Source: " + group1.Name);
                alert.Commands.Add(new UICommand("OK"));
                await alert.ShowAsync();
                return;
            }
            var wall1 = VkAuth.Api.Wall.Get(new WallGetParams { OwnerId = -group1.Id, Count = 1, Filter = WallFilter.Owner });
            Post post1 = wall1.WallPosts.FirstOrDefault();
            var groupimageuri = group1.Photo100;
            try
            {
                Posts.Add(new WallPost(group1.Name, group1.Photo50, wallpost: post1));
            }
            catch (Exception ex)
            {
                var alert = new MessageDialog(ex.Message);
                alert.Commands.Add(new UICommand("OK"));
                await alert.ShowAsync();
                return;
            }


            /*
            async void BClick(object sender, RoutedEventArgs e) {
                string uriToLaunch = @"http://www.vk.com/" + group1.ScreenName;
                var uri = new Uri(uriToLaunch);
                var success = await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            groupImageButton.Click -= new RoutedEventHandler(BClick);
            groupImageButton.Click += new RoutedEventHandler(BClick);
            */

            /*
             * 
            if (post1.Likes.CanLike)
            {
                if (post1.Likes.UserLikes)
                {
                    LikeButton.IsChecked = true;
                }
            }

            void Lchecked(object sender, RoutedEventArgs e)
            {
                if (post1.Likes.CanLike)
                {
                    if (!post1.Likes.UserLikes)
                    {
                        vk.Likes.Add(new LikesAddParams() { Type = LikeObjectType.Post, ItemId = (long)post1.Id, AccessKey = vk.Token });
                    }
                }
            }

            void Lunchecked(object sender, RoutedEventArgs e)
            {
                if (post1.Likes.CanLike)
                {
                    if (post1.Likes.UserLikes)
                    {
                        vk.Likes.Delete(LikeObjectType.Post, (long)post1.Id);
                    }
                }
            }

            LikeButton.Checked += new RoutedEventHandler(Lchecked);
            LikeButton.Unchecked += new RoutedEventHandler(Lunchecked);
            */


        }


        private static string PhotoUrl(long id, long owner_id)
        {
            string result = "http://vk.com/photo" + owner_id.ToString() + "_" + id.ToString();
            return result;
        }
        
        private void LikeButton_Checked(object sender, RoutedEventArgs e)
        {
            /*
            Uri likedUri = new Uri("Assets/liked_outline_24.svg");
            LikeButton.Content = new Image() { Source = new BitmapImage(likedUri) };
            */
        }

        private void LikeButton_Unchecked(object sender, RoutedEventArgs e)
        {
            /*
            Uri likeUri = new Uri("Assets/like_outline_24.svg");
            LikeButton.Content = new Image() { Source = new BitmapImage(likeUri) };
            */
        }
    }
}
