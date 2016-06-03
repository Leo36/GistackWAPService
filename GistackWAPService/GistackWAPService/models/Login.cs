using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using WAPWrapper;

namespace GistackWAPService.models
{
    [DataContract]
    public class Login
    {
        [DataMember]
        public string WAPServer { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public AuthType TenantAuthType { get; set; }

        [DataMember]
        public string SubscriptionID { get; set; }

        [DataMember]
        public Subscription[] SubscriptionList { get; set; }

        [DataMember]
        public string SessionID { get; set; }

        [DataMember]
        public bool IsValidUser { get; set; }
    }
    
    /// <summary>
    /// Model class to hold user Login list
    /// </summary>
    public class LoginList : List<Login>
    {
        private static LoginList loginList;

        /// <summary>
        /// Initializes a new instance of the Login class.
        /// </summary>
        public LoginList()
            : base()
        {
        }

        /// <summary>
        /// Instantiates a single-instance to hold login detail
        /// </summary>
        public static LoginList Instance
        {
            get
            {
                if (loginList == null)
                {
                    loginList = new LoginList();
                }
                return loginList;
            }
        }

        /// <summary>
        /// Initializes a new instance of the Login class.
        /// </summary>
        /// <param name="records">The records.</param>
        public LoginList(IEnumerable<Login> records)
            : base(records)
        {
        }

        /// <summary>
        /// Gets the user login detail per session id and user name
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Login GetLoginRecord(string sessionID, string userName)
        {
            Login login = null;
            var loginlist = this.FindAll(x => (x.SessionID == sessionID) && (x.UserName == userName));
            if (loginlist.Count > 0)
            {
                login = loginlist[0];
            }
            return login;
        }

        /// <summary>
        /// Adds the user login detail
        /// </summary>
        /// <param name="record"></param>
        public void InsertLoginRecord(Login record)
        {
            this.Add(record);
        }

        /// <summary>
        /// Updates the user login record
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public Login UpdateLoginRecord(Login record)
        {
            Login login = null;
            var loginlist = this.FindAll(x => (x.SessionID == record.SessionID) && x.UserName == record.UserName);
            if (loginlist.Count > 0)
            {
                login = loginlist[0];
                login.IsValidUser = record.IsValidUser;
                login.Password = record.Password;
                login.WAPServer = record.WAPServer;
                login.UserName = record.UserName;
                login.SubscriptionID = record.SubscriptionID;
                login.SubscriptionList = record.SubscriptionList;
            }
            return login;
        }

        /// <summary>
        /// Removes the user login record
        /// </summary>
        /// <param name="sessionID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Login RemoveLoginRecord(string sessionID, string userName)
        {
            Login login = null;
            var loginlist = this.FindAll(x => (x.SessionID == sessionID) && (x.UserName == userName));
            if (loginlist.Count > 0)
            {
               login = loginlist[0];
               loginlist.Remove(login);
            }
            return login;
        }
    }
}