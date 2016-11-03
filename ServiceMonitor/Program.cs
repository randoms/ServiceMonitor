using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Threading;
using System.Net.Http;

namespace ServiceMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> services = ConfigurationManager.AppSettings["Services"].Split(',').ToList();
            Dictionary<string, bool> serviceStatus = new Dictionary<string, bool>();
            string toEmail = ConfigurationManager.AppSettings["ToEmail"];
            while (true) {
                foreach(var service in services) {
                    if (!serviceStatus.ContainsKey(service))
                        serviceStatus.Add(service, true);
                    Task.Run(async () => {
                        using (var client = new HttpClient()) {
                            HttpResponseMessage res = null;
                            try {
                                res = await client.GetAsync("http://" + service);
                            }catch(Exception e) { Console.WriteLine(e.StackTrace); };
                            if (res.StatusCode != HttpStatusCode.OK && serviceStatus[service])
                            {
                                serviceStatus[service] = false;
                                await sendEmailAsync(toEmail, "Service Monitor", "服务:" + service + " 现在不可用" + Environment.NewLine
                                + "故障时间: " + DateTime.Now + Environment.NewLine
                                + "请及时修复");
                            }
                            else if (res.StatusCode == HttpStatusCode.OK && !serviceStatus[service])
                            {
                                serviceStatus[service] = true;
                                await sendEmailAsync(toEmail, "Service Monitor", "服务:" + service + " 已恢复可用" + Environment.NewLine
                                + "恢复时间: " + DateTime.Now + Environment.NewLine
                                + "祝贺祝贺");
                            }

                            if (res.StatusCode == HttpStatusCode.OK) {
                                Console.WriteLine("Time: " + DateTime.Now + "  Service: " + service + " Status: OK");
                            }else {
                                Console.WriteLine("Time: " + DateTime.Now + "  Service: " + service + " Status: Failed");
                            }
                        }
                    });
                }
                
                // sleep for 10s
                Thread.Sleep(10 * 1000);
            }
        }



        public static Task sendEmailAsync(string to, string title, string content) {
            return Task.Run(() => {
                string from = ConfigurationManager.AppSettings["FromEmail"];
                string password = ConfigurationManager.AppSettings["Password"];
                string domain = ConfigurationManager.AppSettings["Domain"];
                int port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                string username = ConfigurationManager.AppSettings["Username"];
                SmtpClient client = new SmtpClient(domain, port);

                client.Credentials = new NetworkCredential(username, password);
                MailAddress fromAddr = new MailAddress(from,
                "Service Monitor", Encoding.UTF8);
                MailAddress toAddr = new MailAddress(to);
                MailMessage message = new MailMessage(fromAddr, toAddr);
                message.Body = content;
                message.Body += Environment.NewLine;
                message.BodyEncoding = Encoding.UTF8;
                message.Subject = title;
                message.SubjectEncoding = Encoding.UTF8;
                client.EnableSsl = true;

                ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate,
                         X509Chain chain, SslPolicyErrors sslPolicyErrors)
                { return true; };
                client.Send(message);
            });
        }
    }
}
