using System;
using TwitchLib;
using TwitchLib.Api;
using System.Threading.Tasks;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace StreamClone
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static TwitchAPI API;
        private static LiveStreamMonitorService Monitor;
        private static System.Diagnostics.Process proc;
        private static Config AppConfig;

        static void Main(string[] args)
        {
            log.Info("Hello World!");

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("config.json")
                .AddJsonFile("config.dev.json")
                .Build();
            var section = config.GetSection(nameof(Config));
            AppConfig = section.Get<Config>();



            API = new TwitchAPI();

            API.Settings.ClientId = AppConfig.ClientID;
            API.Settings.AccessToken = AppConfig.AccessToken;

            Monitor = new LiveStreamMonitorService(API, AppConfig.CheckPeriod) ; // check every 5 seconds


            Monitor.SetChannelsByName(new List<string>() { AppConfig.Source });

            Monitor.OnStreamOnline += Monitor_OnStreamOnline;
            Monitor.OnStreamOffline += Monitor_OnStreamOffline;

            Monitor.OnServiceTick += Monitor_OnServiceTick;


            Monitor.Start(); 

            while (true)
            {
                string input = Console.ReadLine();
                if (input.Length > 0)
                {
                    
                }
            }


        }

        private static void Monitor_OnServiceTick(object sender, OnServiceTickArgs e)
        {
            log.Info($"Tick..");
        }

        private static void Monitor_OnStreamOnline(object sender, OnStreamOnlineArgs e)
        {
            log.Info($"Stream up: {e.Channel}");
            log.Info($"Starting stream clone...");

            string strCmdText;
            strCmdText = $"/c streamlink\\streamlink.bat -v twitch.tv/{AppConfig.Source} best -O | streamlink\\ffmpeg\\ffmpeg.exe -loglevel repeat+level+verbose -i pipe:0 -c copy -f flv {AppConfig.Destination}";
            proc = System.Diagnostics.Process.Start("CMD.exe", strCmdText);
        }


        private static void Monitor_OnStreamOffline(object sender, OnStreamOfflineArgs e)
        {
            //Log or something. Probably don't need to do anything.
            log.Info($"stream down {e.Channel}");
            log.Info($"Killing proc...");
            proc.Kill();
        }
    }
}
