using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotification
{
    class Program
    {
        
        static void Main(string[] args)
        {
            string conStr = ConfigurationManager.ConnectionStrings["sqlConString"].ConnectionString;
            Console.WriteLine("Starting dependency...");
            SqlDependency.Start(conStr);
            Console.WriteLine("Dependency started." + Environment.NewLine);
            RegisterNotification();
            
            Console.WriteLine("Waiting for change notification......");
            Console.ReadLine();

            SqlDependency.Stop(conStr);
        }

        public static void RegisterNotification()
        {
            string conStr = ConfigurationManager.ConnectionStrings["sqlConString"].ConnectionString;
            string sqlCommand = @"SELECT [ContactID],[ContactName],[ContactNo] from [dbo].[Contacts]";
             using (SqlConnection con = new SqlConnection(conStr))
            {
                SqlCommand cmd = new SqlCommand(sqlCommand, con); 
                if (con.State != System.Data.ConnectionState.Open)
                {
                    con.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += sqlDep_OnChange; 
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here now
                }
            }
        }


        static void sqlDep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency sqlDep = sender as SqlDependency;
                sqlDep.OnChange -= sqlDep_OnChange;
                Console.WriteLine("Something changed in database...."); 
                RegisterNotification();
            }
        }
    }

}
