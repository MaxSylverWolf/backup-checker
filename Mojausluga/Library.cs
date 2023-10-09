using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// -- Grzesiek
namespace Mojausluga
{
    public static class Library
    {
        public static void WriteErrorLog(Exception ex)
        {
            StreamWriter sw = null;
            try
            {
                // -- Dominika
                sw = new StreamWriter(ConfigurationSettings.AppSettings["sciezka_i_nazwa_pliku_z_monitoringiem_uslugi"], true);
                // -- END
                sw.WriteLine(DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + "; " + ex.Message.ToString().Trim());
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }

        }

        public static void WriteErrorLog(string Message)
        {
            StreamWriter sw = null;
            try
            {
                // -- Dominika
                sw = new StreamWriter(ConfigurationSettings.AppSettings["sciezka_i_nazwa_pliku_z_monitoringiem_uslugi"], true);
                // -- END
                sw.WriteLine(DateTime.Now.ToString() + ": " + Message);
                sw.Flush();
                sw.Close();
            }
            catch
            {
            }

        }

        public static void read(string msg)
        {
            




        }



    }

}
// -- END Grzesiek

