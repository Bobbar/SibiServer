using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AssetManager;

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

        //public RequestApproval(string requestID, bool success)
        //{
        //    this.GUID = requestID;
        //    this.PostSuccess = success;
        //}


        public override string TableName { get; set; } = SibiApprovalColumns.TableName;

        [DataColumnName(SibiApprovalColumns.UID)]
        public override string GUID { get; set; }

        [DataColumnName(SibiApprovalColumns.Note)]
        public string Note { get; set; }

        [DataColumnName(SibiApprovalColumns.NotifySent)]
        public bool ApprovalSent { get; set; }

        [DataColumnName(SibiApprovalColumns.Status)]
        public string ApprovalStatus { get; set; }

        [DataColumnName(SibiApprovalColumns.RequestUID)]
        public string SibiRequestUID { get; set; }

        [DataColumnName(SibiApprovalColumns.ApproverID)]
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

        [DataColumnName(SibiApprovalColumns.RequestorID)]
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

        [DataColumnName(SibiApprovalColumns.Timestamp)]
        public DateTime DateStamp { get; set; }

        public User Approver { get; set; } = new User();
        public User Requestor { get; set; } = new User();

        public string ApprovalResponse { get; set; }

        public bool PostSuccess { get; set; }

        public SibiRequest SibiRequest { get; set; } = new SibiRequest();
        // public SibiRequestItem[] SibiRequestItems { get; set; }// = new SibiRequestItem[]();
        public ApprovalItem[] ApprovalItems { get; set; }


    }
}
