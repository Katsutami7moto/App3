using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VkNet.Exception;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=234238

namespace App2
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VkAuth.ByLogin(loginBox.Text, passwordBox.Password);
            }
            catch (VkApiAuthorizationException error)
            {
                errorBlock.Text = error.ToString();
            }
            Frame.Navigate(typeof(TestsPage));
        }
    }
}
