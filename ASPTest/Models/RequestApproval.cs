using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ASPTest.Models
{
    public class RequestApproval : DataMapObject
    {
        [DataColumnName("uid")]
        public override string GUID { get; set; }
        public override string TableName { get; set; } = "sibi_request_items_approvals";
        [DataColumnName("approval_note")]
        public string Note { get; set; }
        [DataColumnName("approval_type")]
        public string Type { get; set; }
        [DataColumnName("requestor_name")]
        public string RequestorName { get; set; }
        [DataColumnName("sibi_request_item_uid")]
        public string SibiRequestItemUID { get; set; }


        public bool PostSuccess { get; set; }


        public SibiRequestItem SibiRequestItemInfo { get; set; } = new SibiRequestItem();


        

        public RequestApproval(DataTable data) : base(data)
        {
        }
        
        public RequestApproval()
        {
            this.GUID = string.Empty;
            this.PostSuccess = false;
        }

        public RequestApproval(string requestID)
        {
            this.GUID = requestID;
            this.PostSuccess = false;
        }
        public RequestApproval(string requestID, bool success)
        {
            this.GUID = requestID;
            this.PostSuccess = success;
        }
    }
}
