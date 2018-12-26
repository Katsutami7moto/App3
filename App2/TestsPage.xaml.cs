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
using LemmaSharp;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class TestsPage : Page
    {
        public TestsPage()
        {
            this.InitializeComponent();
        }

        #region Categorization

        private List<List<double>> Covering(List<List<double>> docs)
        {
            List<List<double>> result = new List<List<double>>();
            for (int i = 0; i < docs.Count; i++)
            {
                List<double> new_line = new List<double>();
                double alpha = 1 / docs[i].Sum();
                for (int j = 0; j < docs.Count; j++)
                {
                    List<double> s = new List<double>();
                    for (int k = 0; k < docs[0].Count; k++)
                    {
                        List<double> s1 = new List<double>();
                        for (int p = 0; p < docs.Count; p++)
                        {
                            s1.Add(docs[p][k]);
                        }
                        double beta = 1 / s1.Sum();
                        s.Add(docs[i][k] * docs[j][k] * beta);
                    }
                    new_line.Add(alpha * s.Sum());
                }
                result.Add(new_line);
            }
            return result;
        }

        private int ClustersCounting(List<List<double>> covers)
        {
            List<double> delta = new List<double>();
            for (int i = 0; i < covers.Count; i++)
            {
                delta.Add(covers[i][i]);
            }
            return (int)Math.Ceiling(delta.Sum());
        }

        private List<Tuple<double, int>> Priming(List<List<double>> docs, List<List<double>> covers)
        {
            List<Tuple<double, int>> result = new List<Tuple<double, int>>();
            for (int i = 0; i < docs.Count; i++)
            {
                List<double> s = new List<double>();
                for (int j = 0; j < docs[i].Count; j++)
                {
                    s.Add(docs[i][j]);
                }
                result.Add(new Tuple<double, int>(covers[i][i] * (1 - covers[i][i]) * s.Sum(), i));
            }
            return result;
        }

        private int ArgMax(IEnumerable<double> source)
        {
            return !source.Any() ? -1 :
                source
                .Select((value, index) => new { Value = value, Index = index })
                .Aggregate((a, b) => (a.Value >= b.Value) ? a : b)
                .Index;
        }

        private List<List<int>> CCCM(List<List<double>> docs)
        {
            List<List<double>> cover_coefficients = Covering(docs);
            int clusters_number = ClustersCounting(cover_coefficients);
            List<Tuple<double, int>> priming_coefficients = Priming(docs, cover_coefficients);

            List<List<int>> clusters = new List<List<int>>();
            for (int i = 0; i < clusters_number; i++)
            {
                clusters.Add(new List<int>());
            }

            priming_coefficients.OrderByDescending(x => x.Item1);
            SortedSet<int> sorted_docs = new SortedSet<int>();

            List<List<double>> rearranged_docs = new List<List<double>>();
            for (int i = 0; i < docs.Count; i++)
            {
                rearranged_docs.Add(docs[priming_coefficients[i].Item2]);
            }
            double tau = 0.001;
            List<int> s = new List<int>();

            for (int j = 0; j < clusters_number; j++)
            {
                if (j != 0 && priming_coefficients[j - 1].Item1 == priming_coefficients[j].Item1)
                {
                    priming_coefficients.RemoveRange(j, 1);
                }
            }
            for (int j = 0; j < clusters_number; j++)
            {
                if (j == 0 || (priming_coefficients[j - 1].Item1 - priming_coefficients[j].Item1) > tau)
                {
                    s.Add(priming_coefficients[j].Item2);
                }
            }

            for (int i = 0; i < rearranged_docs.Count; i++)
            {
                for (int j = 0; j < s.Count; j++)
                {
                    int number = ArgMax(cover_coefficients[s[j]]) + 1;
                    if (!sorted_docs.Contains(number))
                    {
                        clusters[j].Add(number);
                        sorted_docs.Add(number);
                    }
                    cover_coefficients[s[j]][number - 1] = -1;
                }
            }
            return clusters;
        }

        private List<List<double>> CreateDocsTermsMatrix(List<SortedSet<string>> docs, SortedSet<string> terms)
        {
            List<List<double>> result = new List<List<double>>();

            foreach (var doc in docs)
            {
                List<double> new_line = new List<double>();
                foreach (var term in terms)
                {
                    if (doc.Contains(term))
                    {
                        new_line.Add(1);
                    }
                    else
                    {
                        new_line.Add(0);
                    }
                }
                result.Add(new_line);
            }

            return result;
        }

        private HashSet<string> Lemmatize(SortedSet<string> txt)
        {
            ILemmatizer lmtz = new LemmatizerPrebuiltCompact(LanguagePrebuilt.Russian);
            var result = txt.Select(lmtz.Lemmatize).ToHashSet();

            return result;
        }

        #endregion

        #region Tests
        
        private List<List<int>> CreateClusters(List<Group> groups)
        {
            List<SortedSet<string>> docs = new List<SortedSet<string>>();
            SortedSet<string> allterms = new SortedSet<string>();

            SortedSet<string> stopwords = new SortedSet<string>();
            stopwords.UnionWith(File.ReadAllText("StopWords.txt").Split().ToHashSet());
            var symbols = ",\\/[]{}()=\"*+«»|.!?&^%~`'<>;@№$:-0123456789".ToHashSet();
            var alphabet = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNMйцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ".ToHashSet();

            foreach (var group in groups)
            {
                try
                {
                    var wall = VkAuth.Api.Wall.Get(new WallGetParams { OwnerId = -group.Id, Count = 50, Filter = WallFilter.Owner });

                    SortedSet<string> doc = new SortedSet<string>();
                    foreach (var post in wall.WallPosts)
                    {
                        SortedSet<string> txt = new SortedSet<string>(post.Text.Split().ToHashSet());
                        txt.RemoveWhere(x => (x.Length < 4));
                        txt.RemoveWhere(x => (x.ToHashSet().Overlaps(symbols)));
                        txt.RemoveWhere(x => (!x.ToHashSet().Overlaps(alphabet) && !x.Contains("#")));
                        txt.ExceptWith(stopwords);
                        var lemmatized = Lemmatize(txt);
                        doc.UnionWith(lemmatized);
                        allterms.UnionWith(lemmatized);
                    }
                    docs.Add(doc);
                }
                catch (CannotBlacklistYourselfException) { }
            }

            List<List<double>> DocsTermsMatrix = CreateDocsTermsMatrix(docs, allterms);
            List<List<int>> Clusters = CCCM(DocsTermsMatrix);

            return Clusters;
        }

        private void ClustersTest()
        {
            var groups = VkAuth.Api.Groups.Get(new GroupsGetParams()).ToList();

            var rnd = new Random();
            var rands = new SortedSet<int>();
            var groups1 = new List<Group>();

            while (rands.Count < 6)
            {
                rands.Add(rnd.Next(groups.Count));
            }

            foreach (var num in rands)
            {
                groups1.Add(groups[num]);
            }

            var result = CreateClusters(groups1);

            clustersStackPanel.Children.Add(new TextBlock { Text = result.Count.ToString() });
            foreach (List<int> line in result)
            {
                clustersStackPanel.Children.Add(new TextBlock { Text = "=====" });
                foreach (int doc in line)
                {
                    clustersStackPanel.Children.Add(new TextBlock { Text = doc.ToString() });
                }
            }
        }

        private void WordsTest()
        {
            var groups = VkAuth.Api.Groups.Get(new GroupsGetParams()).ToList();
            var rnd = new Random();
            var group1 = groups[rnd.Next(groups.Count)];
            var wall1 = VkAuth.Api.Wall.Get(new WallGetParams { OwnerId = -group1.Id, Count = 50, Filter = WallFilter.Owner });
            SortedSet<string> group1terms = new SortedSet<string>();
            SortedSet<string> stopwords = new SortedSet<string>();
            stopwords.UnionWith(File.ReadAllText("StopWords.txt").ToLower().Split().ToHashSet());
            var symbols = ",\\/[]{}()=\"*+«»|.!?&^%~`'<>;@№$:-0123456789".ToHashSet();

            foreach (var post in wall1.WallPosts)
            {
                var txt = new SortedSet<string>(post.Text.ToLower().Split().ToHashSet());
                txt.RemoveWhere(x => (x.Length < 4));
                txt.RemoveWhere(x => (x.ToHashSet().Overlaps(symbols)));
                txt.ExceptWith(stopwords);
                var lemmatized = Lemmatize(txt); // test
                group1terms.UnionWith(lemmatized);
            }

            foreach (string term in group1terms)
            {
                group1TermsStackPanel.Children.Add(new TextBlock { Text = term });
            }
        }

        private void AllTermsTest()
        {
            var groups = VkAuth.Api.Groups.Get(new GroupsGetParams()).ToList();
            HashSet<string> allterms = new HashSet<string>();
            HashSet<string> stopwords = new HashSet<string>();
            stopwords.UnionWith(File.ReadAllText("StopWords.txt").Split().ToHashSet());
            var symbols = ",\\/[]{}()=\"*+«»|.!?&^%~`'<>;@№$:-0123456789".ToHashSet();
            var alphabet = "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNMйцукенгшщзхъфывапролджэячсмитьбюЙЦУКЕНГШЩЗХЪФЫВАПРОЛДЖЭЯЧСМИТЬБЮ".ToHashSet();

            foreach (var group in groups)
            {
                try
                {
                    var wall = VkAuth.Api.Wall.Get(new WallGetParams { OwnerId = -group.Id, Count = 50, Filter = WallFilter.Owner });

                    foreach (var post in wall.WallPosts)
                    {
                        var txt = post.Text.Split().ToHashSet();
                        txt.RemoveWhere(x => (x.Length < 4));
                        txt.RemoveWhere(x => (x.ToHashSet().Overlaps(symbols)));
                        txt.RemoveWhere(x => (!x.ToHashSet().Overlaps(alphabet) && !x.Contains("#")));
                        txt.ExceptWith(stopwords);
                        // Lemmatize(txt);
                        allterms.UnionWith(txt);
                    }
                }
                catch (VkNet.Exception.CannotBlacklistYourselfException error)
                {
                    errorBlock.Text = error.ToString();
                }
            }
            errorBlock.Text = allterms.Count.ToString();
        }

        private List<string> GetOnlineFriends()
        {
            var friendsOnlineIDs = VkAuth.Api.Friends.GetOnline(new FriendsGetOnlineParams
            {
                Order = FriendsOrder.Hints
            });

            List<string> onlineFriendsNames = new List<string>();
            var onlineFriends = VkAuth.Api.Users.Get(friendsOnlineIDs.Online);
            foreach (var friend in onlineFriends)
            {
                onlineFriendsNames.Add(friend.FirstName + " " + friend.LastName);
            }

            return onlineFriendsNames;
        }

        #endregion

        #region Button Clicks

        private void FriendsOnlineButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> friendsOnline = new List<string>();

            try
            {
                friendsOnline = GetOnlineFriends();
            }
            catch (VkApiAuthorizationException error)
            {
                errorBlock.Text = error.ToString();
            }

            friendsOnlineStackPanel.Children.Clear();

            foreach (string friend in friendsOnline)
            {
                friendsOnlineStackPanel.Children.Add(new TextBlock { Text = friend });
            }
        }

        private void WordsTestButton_Click(object sender, RoutedEventArgs e)
        {
            group1TermsStackPanel.Children.Clear();
            WordsTest();
        }

        private void ClustersTestButton_Click(object sender, RoutedEventArgs e)
        {
            ClustersTest();
        }

        private void WallPostTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WallPostPage));
        }

        private void DialogMiniuatureTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DialogMiniaturePage));
        }

        private void DialogTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DialogPage));
        }

        private void VariableGridTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(VariableGridPage));
        }

        private void DialogsListTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DialogsListPage));
        }

        private void FullDialogsTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(FullDialogsPage));
        }

        private void NewsFeedTestButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewsFeedPage));
        }

        #endregion
    }
}
