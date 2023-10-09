using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

// -- Dominika
namespace Mojausluga
{
    public partial class Scheduler : ServiceBase
    {

        private System.Timers.Timer timer1 = null;
        private string timeString;
        public int getCallType;
        public Scheduler()
        {
            InitializeComponent();
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            

            int strTime = 2;
            getCallType = 1;

            if (getCallType == 1)
            {

                timer1 = new System.Timers.Timer();
                Double inter = (double) GetNextInterval();


                this.timer1.Elapsed += timer1_Tick;
                timer1.Interval = inter;
                Library.WriteErrorLog("Wznowienie Scheduler'a.");
            }
            else
            {
                timer1 = new System.Timers.Timer();
                timer1.Elapsed += new ElapsedEventHandler(timer1_Tick);
            }

        }

        protected override void OnStart(string[] args)
        {
            Library.WriteErrorLog("Inicjalizacja usługi.");
            Library.WriteErrorLog("Usługa została uruchomiona.");
            timer1.AutoReset = true;
            timer1.Enabled = true;



        }

        private Double GetNextInterval()
        {
            timeString = System.Configuration.ConfigurationManager.AppSettings["startRaportu"];
            DateTime t = DateTime.Parse(timeString);
            TimeSpan ts = new TimeSpan(24,0,0);

            ts = t - System.DateTime.Now;
            if (ts.TotalMilliseconds < 0)
            {
                ts = t.AddDays(1) - System.DateTime.Now;
            }
            return ts.TotalMilliseconds;
        }

        private void timerElapsed(object sender, ElapsedEventArgs e)
        {
            //timer1.Stop();
           // System.Threading.Thread.Sleep(6000);
            SetTimer();
        }

        private void SetTimer()
        {
            try
            {
                Double inter = (double)GetNextInterval();
                timer1.Interval = inter;
                timer1.Start();


                //    if (System.DateTime.Now == DateTime.Parse(ConfigurationSettings.AppSettings["startRaportu"]))
                //    {
                //    Library.WriteErrorLog("Wznowienie Scheduler'a.");
                //    timer1.Elapsed += new ElapsedEventHandler(timer1_Tick);
                if (System.DateTime.Now != DateTime.Parse(ConfigurationSettings.AppSettings["startRaportu"]))
                {

                    Thread.Sleep(10000);
                    SetTimer();
                }
                else
                {
                    this.timer1.Elapsed += timer1_Tick;
                    timer1.Interval = inter;
                }

                }

            
            catch (Exception ex)
            {
                Library.WriteErrorLog("Wystąpił błąd" + ex);
            }
        }

        private void timer1_Tick(object sender, ElapsedEventArgs e)
        {
            try
            {
                var yesterday1 = DateTime.Today.AddDays(0).ToString("yyyy-MM-dd");
                string searcherDate = DateTime.Today.AddDays(-1).ToString(ConfigurationSettings.AppSettings["format_daty"]);
                var yesterday2 = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
                string[] lastDirs = ConfigurationSettings.AppSettings["foldery_do_przeszukiwania_plikow"].Split(new string[] { ";" }, StringSplitOptions.None);
                if (lastDirs.Any())
                {
                    List<string> messageList = new List<string>();
                    List<string> errorList = new List<string>();
                    string email = yesterday1 + " Raport dzienny: \n";
                    string errorMail = yesterday1 + " ALERT Wykryto zagrożenie: \nRaport dzienny: \n";
                    for (int i = 0; i < lastDirs.Length; i++)
                    {
                        var dirs = Directory.GetFiles(@lastDirs[i], "*" + searcherDate + "*");

                        if (dirs.Any())
                        {
                            foreach (string dir in dirs)
                            {
                                FileInfo f = new FileInfo(dir);
                                long s1 = f.Length;
                                if (s1 == 0)
                                {
                                    messageList.Add("\nPlik kopii o scieżce: ''" + lastDirs[i] + "'' istnieje z " + yesterday2 + ". Plik nie ma rozmiaru!!");
                                    errorList.Add("\nPlik kopii o scieżce: ''" + lastDirs[i] + "'' istnieje z " + yesterday2 + ". Plik nie ma rozmiaru!!");

                                }
                                else
                                {
                                    messageList.Add("\nPlik kopii o scieżce: ''" + lastDirs[i] + "'' istnieje z " + yesterday2);

                                }
                            }
                        }

                        else
                        {
                            messageList.Add("\nPlik kopii o scieżce: ''" + lastDirs[i] + "'' nie istnieje z " + yesterday2);
                            errorList.Add("\nPlik kopii o scieżce: ''" + lastDirs[i] + "'' nie istnieje z " + yesterday2);

                        }
                    }

                    if (messageList.Any())
                    {
                        foreach (string msg in messageList)
                        {
                            Library.WriteErrorLog(msg);
                            email = email + msg;
                        }
                        try
                        {

                            SendMail(email);
                        }
                        catch (Exception eeelo)
                        {
                            Library.WriteErrorLog("Nie wysłano emaili: " + eeelo);
                        }
                        Library.WriteErrorLog("Wysłano emaile");
                    }
                    else
                    {
                        Library.WriteErrorLog("Wiadomość jest pusta");
                    }

                    if (errorList.Any())
                    {
                        foreach (string msg in errorList)
                        {
                            errorMail = errorMail + msg;
                        }
                        try
                        {

                            SendMailAdmin(errorMail);
                        }
                        catch (Exception eeelo)
                        {
                            Library.WriteErrorLog("Nie wysłano emaila do administratora: " + eeelo);
                        }
                        Library.WriteErrorLog("Wysłano emaila do administratora");
                    }
                    else
                    {
                        Library.WriteErrorLog("Nie wysłano emaila do administratora. Nie wystąpił żaden błąd");
                    }

                }
                else
                {
                    Library.WriteErrorLog("Nie podano katalogów do przeszukiwania!");
                }
            }

            catch (Exception elo)
            {
                Library.WriteErrorLog("Wystąpił błąd" + elo);
            }
            Library.WriteErrorLog("Zakończenie dzisiejszego raportu");
            

           
            timer1.Enabled = true;
         

            if (getCallType == 1)
            {
             //   timer1.Stop();
             //   System.Threading.Thread.Sleep(6000);
                SetTimer();
            }

        }

        protected override void OnStop()
        {
            timer1.AutoReset = false;
            timer1.Enabled = false;
            Library.WriteErrorLog("Usługa została wstrzymana.");
        }

        public static void SendMail(String msg)
        {
            using (SmtpClient client = new SmtpClient())
            {
                string[] emailList = ConfigurationSettings.AppSettings["adresy_Email_Do_Wysylania_Wiadomosci"].Split(new string[] { ";" }, StringSplitOptions.None);
                if (emailList.Any())
                {
                    MailMessage mail = new MailMessage();

                    for (int i = 0; i < emailList.Length; i++)
                    {
                        mail.To.Add(new MailAddress(emailList[i]));
                    }
                    client.Port = Convert.ToInt32(ConfigurationSettings.AppSettings["numer_portu"]);
                    client.Host = ConfigurationSettings.AppSettings["nazwa_Hosta"];
                    client.Timeout = 20000;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.ServicePoint.MaxIdleTime = 1;
                    client.Credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings["email_Wysylajacy_Wiadomosc"],
                        ConfigurationSettings.AppSettings["haslo_Do_Emaila_Wysylajacego_Wiadomosc"]);
                    mail.From = new MailAddress(ConfigurationSettings.AppSettings["email_Wysylajacy_Wiadomosc"]);
                    string today = DateTime.Today.AddDays(0).ToString("yyyy-MM-dd");
                    mail.Subject = "Firma: " + ConfigurationSettings.AppSettings["nazwa_firmy"] + ". Raport z dnia: " + today;
                    mail.Body = msg;
                    client.Send(mail);
                    mail.Dispose();
                }
                else
                {
                    Library.WriteErrorLog("Nie podano żadnego adresu email!");
                }

            }
        }

        public static void SendMailAdmin(String msg)
        {
            using (SmtpClient client = new SmtpClient())
            {
                string emailAdmin = ConfigurationSettings.AppSettings["adres_Email_Do_Wysylania_Bledow"];
                if (emailAdmin.Any())
                {
                    MailMessage mail = new MailMessage();
                    mail.To.Add(new MailAddress(emailAdmin));
                    client.Port = Convert.ToInt32(ConfigurationSettings.AppSettings["numer_portu"]);
                    client.Host = ConfigurationSettings.AppSettings["nazwa_Hosta"];
                    client.Timeout = 20000;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.ServicePoint.MaxIdleTime = 1;
                    client.Credentials = new System.Net.NetworkCredential(ConfigurationSettings.AppSettings["email_Wysylajacy_Wiadomosc"],
                        ConfigurationSettings.AppSettings["haslo_Do_Emaila_Wysylajacego_Wiadomosc"]);
                    mail.From = new MailAddress(ConfigurationSettings.AppSettings["email_Wysylajacy_Wiadomosc"]);
                    string today = DateTime.Today.AddDays(0).ToString("yyyy-MM-dd");
                    mail.Subject = "ALERT! Firma: " + ConfigurationSettings.AppSettings["nazwa_firmy"] + ". Raport z dnia: " + today;
                    mail.Body = msg;
                    client.Send(mail);
                    mail.Dispose();
                }
                else
                {
                    Library.WriteErrorLog("Nie podano emaila administratora!");
                }

            }
        }

        // -- END Dominika

    }
}
