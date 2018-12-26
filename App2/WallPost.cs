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
    public class WallPost
    {
        public BitmapImage GroupOrUserImage { get; set; }
        public string GroupOrUserName { get; set; }
        public string PostAuthorName { get; set; }
        public string DateText { get; set; }

        public TextAndAttachments TextAndAttachments { get; set; }

        public string LikesCountText { get; set; }
        public string RepostsCountText { get; set; }
        public string CommentsCountText { get; set; }
        public string ViewersCountText { get; set; }

        public WallPost(string name, Uri photo, NewsItem newspost = null, Post wallpost = null)
        {
            if (newspost == null && wallpost == null)
            {
                throw new Exception("Отсутствие данных для создания записи стены, источник: " + name);
            }
            else if (newspost != null)
            {
                GroupOrUserName = name;
                GroupOrUserImage = new BitmapImage(photo);

                DateText = newspost.Date.Value.ToLocalTime().ToString();
                LikesCountText = newspost.Likes.Count.ToString();
                RepostsCountText = newspost.Reposts.Count.ToString();
                CommentsCountText = newspost.Comments.Count.ToString();

                // PostAuthorName = post.SignerId; - про него в схеме API забыли =)

                // ViewersCountText = post.Views.Count.ToString(); - и про него забыли

                TextAndAttachments = new TextAndAttachments(newspost.Text, newspost.Attachments);
            }
            else if (wallpost != null)
            {
                GroupOrUserName = name;
                GroupOrUserImage = new BitmapImage(photo);

                DateText = wallpost.Date.Value.ToLocalTime().ToString();
                LikesCountText = wallpost.Likes.Count.ToString();
                RepostsCountText = wallpost.Reposts.Count.ToString();
                CommentsCountText = wallpost.Comments.Count.ToString();

                if (wallpost.Views == null)
                {
                    ViewersCountText = "N/A";
                }
                else
                {
                    ViewersCountText = wallpost.Views.Count.ToString();
                }
                
                if (wallpost.SignerId.HasValue)
                {
                    var user = VkAuth.Api.Users.Get(new List<long>() { wallpost.SignerId.Value }).FirstOrDefault();
                    PostAuthorName = "Автор записи: " + user.FirstName + " " + user.LastName;
                }

                TextAndAttachments = new TextAndAttachments(wallpost.Text, wallpost.Attachments, PostAuthorName);
            }
        }
    }
}
