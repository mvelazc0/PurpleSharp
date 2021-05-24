
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Threading.Tasks;

namespace PurpleSharp
{

    public class User
    {
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public bool isMapped { get; set; }

        public User(string username)
        {
            UserName = username;
        }
        public User()
        {
        }
    }
    public class Computer
    {
        public string ComputerName { get; set; }
        public string Fqdn { get; set; }
        public string IPv4 { get; set; }

        public Computer(string hostname, string ip)
        {
            ComputerName = hostname;
            IPv4 = ip;
        }
        public Computer()
        {
            
        }

    }

    public class Ldap
    {
        public static List<User> GetADUsers(int count, Lib.Logger logger, string dc = "", bool Enabled=true)
        {
            try
            {
                DateTime dt = DateTime.Now.AddDays(-7);
                long ftAccountExpires = dt.ToFileTime();
                DirectorySearcher search = new DirectorySearcher();

                DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
                string defaultNamingContext = (String)rootDSE.Properties["defaultNamingContext"].Value;

                if (dc == "")
                {
                    List<Computer> Dcs = GetDcs();
                    Random random = new Random();
                    int index = random.Next(Dcs.Count);
                    logger.TimestampInfo(String.Format("Randomly Picked DC for LDAP queries {0}", Dcs[index].ComputerName));
                    DirectoryEntry searchRoot = new DirectoryEntry("LDAP://" + Dcs[index].Fqdn);
                    search = new DirectorySearcher(searchRoot);

                }
                else
                {
                    logger.TimestampInfo(String.Format("Using LogonServer {0} for LDAP queries", dc));
                    DirectoryEntry searchRoot = new DirectoryEntry("LDAP://" + dc);
                    search = new DirectorySearcher(searchRoot);

                }
                List<User> lstADUsers = new List<User>();
                string DomainPath = "LDAP://" + defaultNamingContext;

                // (!(userAccountControl:1.2.840.113556.1.4.803:=2)) - get active users only
                // badPwdCount<=3 minimize the risk of locking accounts

                if (Enabled) search.Filter = "(&(objectCategory=person)(objectClass=user)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
                //if (Enabled) search.Filter = "(&(objectCategory=person)(objectClass=user)(lastLogon>=" + ftAccountExpires.ToString() + ")(badPwdCount<=3)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
                //if (Enabled) search.Filter = "(&(objectCategory=person)(objectClass=user)(lastLogon>=" + ftAccountExpires.ToString() + ")(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
                else search.Filter = "(&(objectCategory=person)(objectClass=user)(userAccountControl:1.2.840.113556.1.4.803:=2))";
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("usergroup");
                search.PropertiesToLoad.Add("displayname");
                search.SizeLimit = count * 5;
                SearchResult result;

                if (Enabled) logger.TimestampInfo("Querying for active domain users with badPwdCount <= 3..");
                else logger.TimestampInfo("Querying for disabled domain users ..");

                SearchResultCollection resultCol = search.FindAll();
                
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        string UserNameEmailString = string.Empty;
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname") && result.Properties.Contains("displayname"))
                        {
                            User objSurveyUsers = new User();
                            objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
                            objSurveyUsers.DisplayName = (String)result.Properties["displayname"][0];
                            lstADUsers.Add(objSurveyUsers);
                        }
                    }
                }
                return lstADUsers.OrderBy(arg => Guid.NewGuid()).Take(count).ToList();


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;

            }
        }

        public static List<User> GetADAdmins(int count, Lib.Logger logger)
        {

            DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = (String)rootDSE.Properties["defaultNamingContext"].Value;

            List<User> lstDas = new List<User>();
            string DomainPath = "LDAP://" + defaultNamingContext;
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            //search.Filter = "(&(objectCategory=person)(objectClass=user)(adminCount=1)(badPwdCount<=3)(!samAccountName=krbtgt)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
            search.Filter = "(&(objectCategory=person)(objectClass=user)(adminCount=1)(!samAccountName=krbtgt)(!(userAccountControl:1.2.840.113556.1.4.803:=2)))";
            search.SizeLimit = count * 5;
            SearchResult result;
            logger.TimestampInfo("Querying for active administrative users (adminCount=1) ..");
            SearchResultCollection resultCol = search.FindAll();
            if (resultCol != null)
            {
                for (int counter = 0; counter < resultCol.Count; counter++)
                {
                    string UserNameEmailString = string.Empty;
                    result = resultCol[counter];
                    if (result.Properties.Contains("samaccountname"))
                    {
                        User objSurveyUsers = new User();
                        //Console.WriteLine((String)result.Properties["samaccountname"][0]);
                        objSurveyUsers.UserName = (String)result.Properties["samaccountname"][0];
                        //objSurveyUsers.DisplayName = (String)result.Properties["displayname"][0];
                        lstDas.Add(objSurveyUsers);
                    }
                }
            }
            return lstDas.OrderBy(arg => Guid.NewGuid()).Take(count).ToList();

        }

        public static List<User> GetDomainAdmins(Lib.Logger logger)
        {
            List<User> lstDas = new List<User>();
            PrincipalContext PC = new PrincipalContext(ContextType.Domain);
            logger.TimestampInfo("Querying for active Domain Admins ..");
            GroupPrincipal GP = GroupPrincipal.FindByIdentity(PC, "Domain Admins");
            foreach (UserPrincipal member in GP.Members)
            {
                if (member.Enabled == true)
                {
                    User user = new User();
                    user.UserName = member.SamAccountName;
                    user.DisplayName = member.DisplayName;
                    lstDas.Add(user);
                }

            }
            return lstDas;
        }
        public static List<Computer> GetADComputers(int count, Lib.Logger logger, string dc = "", string username="", string password="")
        {

            DateTime dt = DateTime.Now.AddDays(-1);
            long ftAccountExpires = dt.ToFileTime();
            DirectorySearcher search = new DirectorySearcher();

            if (dc == "")
            {
                List<Computer> Dcs = GetDcs();
                Random random = new Random();
                int index = random.Next(Dcs.Count);
                logger.TimestampInfo(String.Format("Randomly Picked DC for LDAP queries {0}", Dcs[index].ComputerName));
                DirectoryEntry searchRoot = new DirectoryEntry();
                if (!username.Equals("") && !password.Equals("")) searchRoot = new DirectoryEntry("LDAP://" + Dcs[index].Fqdn, username, password);
                else searchRoot = new DirectoryEntry("LDAP://" + Dcs[index].Fqdn);
                search = new DirectorySearcher(searchRoot);

            }
            else
            {
                logger.TimestampInfo(String.Format("Using {0} for LDAP queries", dc));
                DirectoryEntry searchRoot = new DirectoryEntry();
                if (!username.Equals("") && !password.Equals("")) searchRoot = new DirectoryEntry("LDAP://" + dc, username, password);
                else searchRoot = new DirectoryEntry("LDAP://" + dc);
                search = new DirectorySearcher(searchRoot);

            }
            List<Computer> lstADComputers = new List<Computer>();
            search.Filter = "(&(objectClass=computer)(lastLogon>=" + ftAccountExpires.ToString() + "))";
            search.PropertiesToLoad.Add("samaccountname");
            search.PropertiesToLoad.Add("Name");
            search.PropertiesToLoad.Add("DNSHostname");
            search.SizeLimit = count*5;
            SearchResult result;
            SearchResultCollection resultCol = search.FindAll();
            if (resultCol != null)
            {
                //Console.WriteLine("Initial query obtained " + resultCol.Count.ToString() + " records;");

                List<Task> tasklist = new List<Task>();

                for (int counter = 0; counter < resultCol.Count; counter++)
                {

                    try
                    {
                        result = resultCol[counter];
                        if (result.Properties.Contains("samaccountname") && result.Properties.Contains("DNSHostname"))
                        {
                            string Name = (String)result.Properties["Name"][0];
                            string dnshostname = (String)result.Properties["DNSHostname"][0];

                            if (!Name.ToUpper().Contains(Environment.MachineName.ToUpper()))
                            {
                                tasklist.Add(Task.Factory.StartNew(() =>
                                {
                                    //TODO: Firewalls may block ping. Instead of pinging, i should should resolve.  
                                    //Console.WriteLine("[+] Trying to ping {0}", dnshostname);
                                    string ipv4 = PurpleSharp.Lib.Networking.PingHost(dnshostname);
                                    if (ipv4 != "")
                                    {
                                        TimeSpan interval = TimeSpan.FromSeconds(3);
                                        if (PurpleSharp.Lib.Networking.OpenPort(ipv4, 445, interval))
                                        {
                                            Computer newhost = new Computer();
                                            newhost.ComputerName = Name;
                                            newhost.IPv4 = ipv4;
                                            newhost.Fqdn = dnshostname;
                                            lstADComputers.Add(newhost);
                                        }
                                    }
                                }));
                            }
                        }

                    }
                    catch
                    {
                        continue;
                    }

                }
                Task.WaitAll(tasklist.ToArray());

            }
            return lstADComputers.OrderBy(arg => Guid.NewGuid()).Take(count).ToList();

        }

        public static List<String> GetSPNs()
        {
            List<String> lstSpns = new List<String>();

            DirectoryEntry rootDSE = new DirectoryEntry("LDAP://RootDSE");
            string defaultNamingContext = (String)rootDSE.Properties["defaultNamingContext"].Value;

            List<User> lstDas = new List<User>();
            string DomainPath = "LDAP://" + defaultNamingContext;
            DirectoryEntry searchRoot = new DirectoryEntry(DomainPath);
            DirectorySearcher search = new DirectorySearcher(searchRoot);
            search.Filter = "(&(samAccountType=805306368)(servicePrincipalName=*)(!(userAccountControl:1.2.840.113556.1.4.803:=2))(!samAccountName=krbtgt))";
            SearchResultCollection users = search.FindAll();

            foreach (SearchResult user in users)
            {
                string samAccountName = user.Properties["samAccountName"][0].ToString();
                string distinguishedName = user.Properties["distinguishedName"][0].ToString();
                string servicePrincipalName = user.Properties["servicePrincipalName"][0].ToString();
                lstSpns.Add(samAccountName + "#" + servicePrincipalName);
            }

            return lstSpns;

        }

        public static List<Computer> GetDcs()
        {
            List<Computer> lstDcs = new List<Computer>();
            Domain domain = Domain.GetCurrentDomain();
            foreach (DomainController dc in domain.FindAllDiscoverableDomainControllers())
            {
                Computer retdc = new Computer();

                retdc.ComputerName = dc.Name;
                retdc.IPv4 = dc.IPAddress;
                retdc.Fqdn = dc.Name;
                lstDcs.Add(retdc);
            }
            return lstDcs;
        }



    }

}