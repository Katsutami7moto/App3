using System;
using System.IO;
using VkNet;
using VkNet.Model;
using VkNet.Enums.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VkNet.NLog.Extensions.Logging;
using VkNet.NLog.Extensions.Logging.Extensions;

namespace App2
{
    static class VkAuth
    {
        public static VkApi Api { get; private set; }
        private static readonly ulong APPID = 6361882;
        private static readonly string TOKEN = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "access_token.txt");
        private static readonly string USERID = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "user_id.txt");

        static VkAuth()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Trace);
                builder.AddNLog(new NLogProviderOptions
                {
                    CaptureMessageProperties = true,
                    CaptureMessageTemplates = true
                });
            });
            NLog.LogManager.LoadConfiguration("nlog.config");
            Api = new VkApi(services);
        }

        static public void ByToken()
        {
            Api.Authorize(new ApiAuthParams
            {
                AccessToken = File.ReadAllText(TOKEN),
                UserId = Convert.ToInt64(File.ReadAllText(USERID))
            });
            Api.Stats.TrackVisitor();
        }

        static public void ByLogin(string currentLogin, string currentPassword)
        {
            Api.Authorize(new ApiAuthParams
            {
                ApplicationId = APPID,
                Login = currentLogin,
                Password = currentPassword,
                Settings = Settings.All
            });
            Api.Stats.TrackVisitor();
            File.WriteAllText(TOKEN, Api.Token);
            File.WriteAllText(USERID, Api.UserId.Value.ToString());
        }
    }
}
