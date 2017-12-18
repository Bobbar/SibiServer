using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace SibiServer.Models
{
    public class User : DataMapObject
    {

        public User() { }

        public User(string id)
        {
            GetUserFromDB(id);
        }
        public User(DataTable data) : base(data) { }
        public User(DataRow data) : base(data) { }

        [DataColumnName("fullname")]
        public string FullName { get; set; }

        [DataColumnName("username")]
        public string UserName { get; set; }

        [DataColumnName("email")]
        public string Email { get; set; }

        public override string TableName { get; set; } = "approval_users";

        [DataColumnName("id")]
        public override string GUID { get; set; }


        private void GetUserFromDB(string id)
        {
            var userQuery = "SELECT * FROM approval_users WHERE id = '" + id + "'";
            this.MapClassProperties(DBFactory.GetDatabase().DataTableFromQueryString(userQuery));
        }
    }
}
