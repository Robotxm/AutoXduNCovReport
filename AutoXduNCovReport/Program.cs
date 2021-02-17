using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoXduNCovReport.Repository;
using Cocona;

namespace AutoXduNCovReport
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CoconaApp.RunAsync<Program>(args, options => { options.EnableShellCompletionSupport = false; });
        }

        public async Task Report(
            [Option('u', Description = "Specify your student id number")]
            string username,
            [Option('p', Description = "Specify your password")]
            string password,
            [Option('k', Description = "Specify your Serverchan key")]
            string sckey = "")
        {
            try
            {
                Console.WriteLine("- Logging in...");
                var loginResult = await NCovRepository.Instance.Login(username, password);
                if (!loginResult.Item1)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to login ({loginResult.Item2}). Check your username and password.\n" +
                                      "If you are sure that your authentication is correct, contact the author for help.");
                    Console.ResetColor();
                    await SendNotification(sckey, "疫情通填写失败",
                        $"无法登录疫情通系统: {loginResult.Item2}。请检查用户名和密码。如果确认信息正确，请联系作者.");
                    return;
                }

                Console.WriteLine("- Checking...");
                var isReported = await NCovRepository.Instance.CheckIsReported();
                if (isReported)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("You have submitted your information today.");
                    Console.ResetColor();
                    return;
                }

                var oldInfo = await NCovRepository.Instance.GetOldInfo();
                if (oldInfo == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(
                        "Failed to parse your information submitted before. Contact the author for help.");
                    Console.ResetColor();
                    await SendNotification(sckey, "疫情通填写失败", "无法解析前一日所填信息。请联系作者。");
                    return;
                }

                // Set temperature
                if (oldInfo.ContainsKey("tw"))
                    oldInfo["tw"] = "3";
                else
                    oldInfo.Add("tw", "3");
                // Add params
                if (!oldInfo.ContainsKey("ismoved"))
                    oldInfo.Add("ismoved", "0");
                if (!oldInfo.ContainsKey("szgjcs"))
                    oldInfo.Add("szgjcs", "");
                if (!oldInfo.ContainsKey("mjry"))
                    oldInfo.Add("mjry", "0");
                if (!oldInfo.ContainsKey("csmjry"))
                    oldInfo.Add("csmjry", "0");
                if (!oldInfo.ContainsKey("zgfxdq"))
                    oldInfo.Add("zgfxdq", "0");
                // Parse the geolocation info submitted before
                var geolocationInfo = JsonDocument.Parse(oldInfo["geo_api_info"].ToString()).RootElement;
                var province = geolocationInfo.GetProperty("addressComponent").GetProperty("province").GetString();
                var city = geolocationInfo.GetProperty("addressComponent").GetProperty("city").GetString();
                var area =
                    $"{province} {city} {geolocationInfo.GetProperty("addressComponent").GetProperty("district").GetString()}";
                // Complete the information to be submitted
                var newInfo = oldInfo.ToDictionary(kv => kv.Key, kv => kv.Value.ToString());
                var municipalities = new[] {"北京市", "天津市", "上海市", "重庆市"};
                newInfo["province"] = province;
                newInfo["city"] = municipalities.Contains(province) ? province : city;
                newInfo["area"] = area;
                newInfo["date"] = $"{DateTimeOffset.Now:yyyyMMdd}";
                newInfo["created"] = $"{DateTimeOffset.Now.ToUnixTimeSeconds()}";

                Console.WriteLine("- Submitting...");
                var submitResult = await NCovRepository.Instance.Submit(newInfo);
                if (submitResult.Item1)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Submitted successfully!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Submitted unsuccessfully: {submitResult.Item2}\n" +
                                      "Contact the author for help.");
                    await SendNotification(sckey, "疫情通填写失败", $"信息提交失败: {loginResult.Item2}。请联系作者。");
                }

                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await SendNotification(sckey, "自动疫情通运行失败", "请自行检查填写状态。有关运行失败的信息，请联系作者。");
            }
        }

        private async Task SendNotification(string sckey, string title, string content)
        {
            if (sckey == "")
                return;

            await ServerchanRepository.Instance.SendMessage(sckey, title, content);
        }
    }
}