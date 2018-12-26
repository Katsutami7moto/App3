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
using Photo = VkNet.Model.Attachments.Photo;
using Image = Windows.UI.Xaml.Controls.Image;
using Attachment = VkNet.Model.Attachments.Attachment;

namespace App2
{
    public class TextAndAttachments
    {
        public string Text { get; set; }
        public ObservableCollection<Image> ImageAttachments = new ObservableCollection<Image>();
        public string AuthorName { get; set; }
        public Visibility TextVisibility { get; set; }
        public Visibility AttachmentsVisibility { get; set; }
        public Visibility AuthorVisibility { get; set; }

        public TextAndAttachments(string text, IEnumerable<Attachment> attachments, string author = "")
        {
            if (text == "")
            {
                TextVisibility = Visibility.Collapsed;
            }
            else
            {
                TextVisibility = Visibility.Visible;
                Text = text;
            }

            if (author == "")
            {
                AuthorVisibility = Visibility.Collapsed;
            }
            else
            {
                AuthorVisibility = Visibility.Visible;
                AuthorName = author;
            }

            if (attachments == null)
            {
                AttachmentsVisibility = Visibility.Collapsed;
            }
            else
            {
                var photourls =
                    attachments
                    .Where(x => x.Type == typeof(Photo))
                    .Select(p => (Photo)p.Instance)
                    .Select(x => x.Sizes[0].Url) // 0 для 130px, 2 для 200px
                    .ToList();

                if (photourls.Count == 0)
                {
                    AttachmentsVisibility = Visibility.Collapsed;
                }
                else
                {
                    AttachmentsVisibility = Visibility.Visible;
                    foreach (var url in photourls)
                    {
                        ImageAttachments.Add(new Image { Source = new BitmapImage(url) });
                    }
                }
            }
        }
    }
}
