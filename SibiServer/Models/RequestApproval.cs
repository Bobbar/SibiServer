using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace SibiServer.Models
{
    public class RequestApproval : DataMapObject
    {

        private string approverID;
        private string requestorID;


        public RequestApproval(DataTable data) : base(data)
        {
        }

        public RequestApproval(DataRow row) : base(row)
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


        public override string TableName { get; set; } = "sibi_request_items_approvals";

        [DataColumnName("uid")]
        public override string GUID { get; set; }

        [DataColumnName("approval_note")]
        public string Note { get; set; }

        [DataColumnName("approval_sent")]
        public bool ApprovalSent { get; set; }

        [DataColumnName("approval_status")]
        public string ApprovalStatus { get; set; }

        [DataColumnName("sibi_request_uid")]
        public string SibiRequestUID { get; set; }

        [DataColumnName("approver_id")]
        public string ApproverID
        {
            get
            {
                return approverID;
            }

            set
            {
                approverID = value;
                this.Approver = new User(approverID);
            }
        }

        [DataColumnName("requestor_id")]
        public string RequestorID
        {
            get
            {
                return requestorID;
            }
            set
            {
                requestorID = value;
                this.Requestor = new User(requestorID);
            }
        }

        public User Approver { get; set; } = new User();
        public User Requestor { get; set; } = new User();


        public bool PostSuccess { get; set; }

        public SibiRequest SibiRequest { get; set; } = new SibiRequest();
        public SibiRequestItem[] SibiRequestItems { get; set; }// = new SibiRequestItem[]();


    }
}
