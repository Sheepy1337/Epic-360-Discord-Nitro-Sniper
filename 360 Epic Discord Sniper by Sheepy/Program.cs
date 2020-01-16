using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using System.Threading;
using Discord.WebSocket;
using System.Net;
using System.IO;

namespace _360_Epic_Discord_Sniper_by_Sheepy
{
    class Program
    {
        private static DiscordSocketClient _client;
        private static string _token;

        static void Main(string[] args)
        {
            string token = "";
            Console.Title = "360 Epic Discord Sniper by Sheepy";
            if ((Properties.Settings.Default.Token == "null") || (Properties.Settings.Default.Token == "")) 
            {
                Console.Write("Token: ");
                token = Console.ReadLine();
                Properties.Settings.Default.Token = _token;
                Properties.Settings.Default.Save();

            }
            else
            {
                token = Properties.Settings.Default.Token;
            }
            _token = token;

            Console.Clear();

            _client = new DiscordSocketClient();

            _client.MessageReceived += _client_MessageReceived;
            
            _client.LoginAsync(TokenType.User, token);
            _client.Ready += ReadyAsync;
            _client.Log += _client_Log;
            _client.StartAsync();
            
            Thread.Sleep(-1);
        }

        private static Task _client_MessageReceived(SocketMessage arg)
        {
            
            if (arg.Content.Contains("https://discord.gift/") || arg.Content.Contains("http://discord.gift/") || arg.Content.Contains("discord.gift/"))
            {
                string code = arg.Content.Split(new[] { @"https://discord.gift/", @"http://discord.gift/", @"discord.gift/" }, StringSplitOptions.None)[1].Split(' ')[0];

                CookieContainer cookieContainer = new CookieContainer();


                try
                {

                    string payload = "";

                    var request2 = (HttpWebRequest)WebRequest.Create($"https://discordapp.com/api/v6/entitlements/gift-codes/{code}/redeem");
                    request2.Accept = "application/json, text/javascript, */*; q=0.01";

                    var postData = payload;
                    var data = Encoding.ASCII.GetBytes(postData);
                    request2.Headers["authorization"] = _token;
                    request2.Method = "POST";
                    request2.ContentType = "application/json";
                    request2.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.75 Safari/537.36";
                    request2.ContentLength = data.Length;
                    using (var stream = request2.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    var response = (HttpWebResponse)request2.GetResponse();
                    string responsestring = new StreamReader(response.GetResponseStream()).ReadToEnd();


                    if ((int)response.StatusCode == 200)
                    {
                        Log($"Successfully redeemed code {code}", ConsoleColor.Green);
                    }
                }
                catch (WebException e)
                {
                    var errresp = (HttpWebResponse)e.Response;
                    string respstring = new StreamReader(errresp.GetResponseStream()).ReadToEnd();
                    if ((int)errresp.StatusCode == 404)
                    {
                        Log($"Failed to redeem code {code}", ConsoleColor.Red);
                    }
                    else if (((int)errresp.StatusCode == 400)|| ((int)errresp.StatusCode == 429))
                    {
                        Log($"Expired code {code}", ConsoleColor.Red);
                    }
                }
                catch
                {
                }
            }
            return Task.CompletedTask;
        }

        private static Task _client_Log(LogMessage arg)
        {
            Log(arg.Message, ConsoleColor.Red);
            return Task.CompletedTask;
        }
        private static void Log(string message, ConsoleColor consoleColor)
        {
            Console.Write("[");
            Console.ForegroundColor = consoleColor;
            Console.Write(DateTime.Now.ToString("h:mm:ss"));
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"] {message}\n");

        }
        private static Task ReadyAsync()
        {
            Console.Title = $"360 Epic Discord Sniper by Sheepy | {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}";
            
            return Task.CompletedTask;
        }
    }
}
