using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using System.Management;

namespace audit
{
    class Program
    {
        static void Main(string[] args)
        {
            //gets current user and computer name
            string computerName;
            string userName;
            string osVersion;
            string model;
            string processor;
            int memory;

            SqlCommand oComm;
            SqlConnection oConn;
            string sSql = "";
            int id=0;

            try
            {
                computerName = Environment.MachineName;
                userName = Environment.UserName;
                osVersion = Environment.OSVersion.ToString();
                DateTime currentDate = DateTime.Now;
                model = "";
                processor = "";
                memory = 0;

                ManagementObjectSearcher query1 = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
                ManagementObjectCollection queryCollection1 = query1.Get();

                foreach (ManagementObject mo in queryCollection1)
                {
                    model = mo["model"].ToString();
                    memory = Convert.ToInt32(mo["totalphysicalmemory"]);
                }

                query1 = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
                queryCollection1 = query1.Get();

                foreach (ManagementObject mo in queryCollection1)
                {
                    processor = mo["Name"].ToString();
                }

                oConn = new SqlConnection("server=sqlserver;database=audit_data;uid=user;pwd=password;");
                oConn.Open();

                //looks to see if computer is in database already
                sSql = "SELECT COUNT(*) FROM computers WHERE computerName='" + computerName + "'";
                oComm = new SqlCommand(sSql, oConn);

                //if not in database, put in database with current user and such
                if (Convert.ToInt32(oComm.ExecuteScalar()) == 0)
                {
                    //Console.WriteLine("New entry");
                    sSql = "INSERT INTO computers (computerName, osVersion, lastAudit, model, totalMemory, processor) values "
                        + "('" + computerName + "', '"
                        + osVersion + "', '" 
                        + currentDate + "', '" 
                        + model + "', " 
                        + memory + ", '"
                        + processor + "')";
                    oComm = new SqlCommand(sSql, oConn);
                    oComm.ExecuteNonQuery();

                    sSql = "SELECT id FROM computers WHERE computerName='" + computerName + "'";
                    oComm = new SqlCommand(sSql, oConn);

                    id = Convert.ToInt32(oComm.ExecuteScalar());
                    //Console.WriteLine("Computer id:" + id);
                }
                else  //else it's an existing system, just get the system id and update the last logged in user and os version
                {
                    //Console.WriteLine("Existing system");
                    sSql = "SELECT id FROM computers WHERE computerName='" + computerName + "'";
                    oComm = new SqlCommand(sSql, oConn);

                    id = Convert.ToInt32(oComm.ExecuteScalar());
                    //Console.WriteLine("Computer id:" + id);

                    sSql = "UPDATE computers SET "
                        + "osVersion='" + osVersion 
                        + "', lastAudit='" + currentDate 
                        + "', model='" + model
                        + "', processor='" + processor
                        + "', totalMemory=" + memory
                        + " WHERE id=" + id;
                    oComm = new SqlCommand(sSql, oConn);
                    oComm.ExecuteNonQuery();
                }

                //updates the memory information
                updateMemory(id);

                //updates disk drive information
                updateDrives(id);

                //updates user tables
                updateUsers(userName, id);

                //deletes all the current software associated with the system
                sSql = "DELETE FROM software WHERE computerId=" + id;
                oComm = new SqlCommand(sSql, oConn);
                oComm.ExecuteNonQuery();

                //looks at uninstall folder in registry to get currently installed software
                RegistryKey regKey, regSubKey;

                regKey = Registry.LocalMachine;
                regKey = regKey.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Windows").OpenSubKey("CurrentVersion").OpenSubKey("Uninstall");

                string[] foo = regKey.GetSubKeyNames();
                for (int i = 0; i < foo.Length; i++)
                {
                    string bar = foo[i].ToString();
                    if (String.Compare(bar, 0, "KB", 0, 2) != 0)
                    {
                        regSubKey = regKey.OpenSubKey(foo[i]);
                        if (regSubKey.GetValue("DisplayName") != null)
                        {                            
                            string software = regSubKey.GetValue("DisplayName").ToString();
                            //Console.WriteLine("Trying to add software: " + software);
                            software = software.Replace("'", "''");
                            sSql = "INSERT INTO software (computerId, softwareName) values (" + id + ", '" + software + "')";
                            oComm = new SqlCommand(sSql, oConn);
                            oComm.ExecuteNonQuery();
                            //Console.WriteLine("Software added");
                        }
                    }
                }
                oConn.Close();
            }
            catch (Exception e)
            {
                //oConn.Close();
                Console.WriteLine("An error has occoured.  Error report follows:");
                Console.WriteLine(e);
            }           
        }

        static void updateMemory(int computerID)
        {
            SqlCommand oComm;
            SqlConnection oConn;
            string sSql = "";

            try
            {
                oConn = new SqlConnection("server=sqlserver;database=audit_data;uid=user;pwd=password;");
                oConn.Open();

                sSql = "DELETE FROM memory WHERE computerId=" + computerID;
                oComm = new SqlCommand(sSql, oConn);
                oComm.ExecuteNonQuery();

                ManagementObjectSearcher query1 = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMemory");
                ManagementObjectCollection queryCollection1 = query1.Get();

                foreach (ManagementObject mo in queryCollection1)
                {
                    sSql = "INSERT INTO memory (capacity, location, computerId) values ("
                        + mo["Capacity"] + ", '"
                        + mo["DeviceLocator"] + "', "
                        + computerID + ")";

                    oComm = new SqlCommand(sSql, oConn);
                    oComm.ExecuteNonQuery();
                }
                oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has occoured.  Report follows:");
                Console.WriteLine(e);
            }


        }

        static void updateDrives(int computerID)
        {
            SqlCommand oComm;
            SqlConnection oConn;
            string sSql = "";

            try
            {
                oConn = new SqlConnection("server=sqlserver;database=audit_data;uid=user;pwd=password;");
                oConn.Open();

                sSql = "DELETE FROM diskDrives WHERE computerId=" + computerID;
                oComm = new SqlCommand(sSql, oConn);
                oComm.ExecuteNonQuery();

                ManagementObjectSearcher query1 = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
                ManagementObjectCollection queryCollection1 = query1.Get();

                foreach (ManagementObject mo in queryCollection1)
                {
                    sSql = "INSERT INTO diskDrives (deviceID, description, freeSpace, size, computerId) values ('"
                        + mo["DeviceID"] + "', '"
                        + mo["Description"] + "', '"
                        + mo["Freespace"] + "', '"
                        + mo["Size"] + "', "
                        + computerID + ")";

                    oComm = new SqlCommand(sSql, oConn);
                    oComm.ExecuteNonQuery();
                }
                oConn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error has occoured.  Report follows:");
                Console.WriteLine(e);
            }
        }

        static void updateUsers(string userName, int computerID)
        {
            SqlCommand oComm;
            SqlConnection oConn;
            string sSql = "";

            try
            {
                oConn = new SqlConnection("server=sqlserver;database=audit_data;uid=user;pwd=password;");
                oConn.Open();

                sSql = "select count(*) from users where userName='" + userName + "' and computerId=" + computerID;
                oComm = new SqlCommand(sSql, oConn);

                if (Convert.ToInt32(oComm.ExecuteScalar()) == 0)
                {
                    sSql = "INSERT INTO users (computerId, userName) values ("
                        + computerID + ", '" 
                        + userName + "')";

                    oComm = new SqlCommand(sSql, oConn);
                    oComm.ExecuteNonQuery();                        
                }
                else
                {
                    sSql = "SELECT counter FROM users WHERE userName='" + userName + "' AND computerId=" + computerID;
                    oComm = new SqlCommand(sSql, oConn);
                    int foo = Convert.ToInt32(oComm.ExecuteScalar())+1;

                    sSql = "UPDATE users SET counter=" + foo + " WHERE userName='" + userName + "' AND computerId=" + computerID;
                    oComm = new SqlCommand(sSql, oConn);
                    oComm.ExecuteNonQuery();
                }

                sSql = "SELECT userName FROM users WHERE computerId=" + computerID + " ORDER BY counter DESC";
                oComm = new SqlCommand(sSql, oConn);
                string user = oComm.ExecuteScalar().ToString();

                sSql = "UPDATE computers SET userName='" + user + "' WHERE id=" + computerID;
                oComm = new SqlCommand(sSql, oConn);
                oComm.ExecuteNonQuery();

                oConn.Close();
            }

            catch (Exception e)
            {
                Console.WriteLine("An error has occoured.  Report follows:");
                Console.WriteLine(e);
            }

        }

        
    }
}
