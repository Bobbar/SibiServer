using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManager;
using System.Data;

namespace SibiServer.Emailer
{
    public class Notification : DataMapObject
    {


        public override string TableName { get; set; } = NotificationColumns.TableName;


        public Models.RequestApproval Approval { get; set; } = new Models.RequestApproval();
        public Models.SibiRequestItem RequestItem { get; set; } = new Models.SibiRequestItem();

        [DataColumnName(NotificationColumns.UID)]
        public override string GUID { get; set; }

        [DataColumnName(NotificationColumns.Type)]
        public string NotificationTypeString
        {
            set
            {
                notificationTypeString = value;

            }

        }

        public NotificationType Type
        {
            get
            {
                return (NotificationType)Enum.Parse(typeof(NotificationType), notificationTypeString);
            }
            set
            {
                notificationTypeString = value.ToString();
            }
        }
        private string notificationTypeString;




        [DataColumnName(NotificationColumns.ApprovalID)]
        public string ApprovalId
        {
            get
            {
                return approvalId;
            }
            set
            {
                approvalId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PopulateApproval(value);
                }
            }
        }
        private string approvalId;


        [DataColumnName(NotificationColumns.RequestItemUID)]
        public string RequestItemId
        {
            get
            {
                return requestItemId;

            }
            set
            {
                requestItemId = value;
                if (!string.IsNullOrEmpty(value))
                {
                    PopulateRequestItem(value);
                    this.approver = new Models.User(RequestItem.ApproverId);
                    this.requestor = new Models.User(RequestItem.RequestorId);
                }

            }
        }
        private string requestItemId;

        [DataColumnName(NotificationColumns.Sent)]
        public bool Sent { get; set; }


        public Models.User Approver
        {
            get
            {
                if (Type != NotificationType.CHANGE)
                {
                    return Approval.Approver;
                }
                else
                {
                    return approver;
                }
            }
        }
        private Models.User approver;

        
        public Models.User Requestor
        {
            get
            {
                if (Type != NotificationType.CHANGE)
                {
                    return Approval.Requestor;
                }
                else
                {
                    return requestor;
                }
            }
        }
        
        private Models.User requestor;

        



        public Notification() { }

        public Notification(NotificationType type, Models.RequestApproval approval)
        {
            Type = type;
            Approval = approval;
        }

        public Notification(DataTable data) : base(data) { }
        public Notification(DataRow data) : base(data) { }


        private void PopulateApproval(string approvalId)
        {
            //var approvalQuery = "SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.UID + " ='" + approvalId + "'";
            //this.Approval = new Models.RequestApproval(DBFactory.GetDatabase().DataTableFromQueryString(approvalQuery));
            var newApproval = new Models.RequestApproval(approvalId);
            DBFunctions.PopApprovalData(ref newApproval);
            this.Approval = newApproval;
        }

        private void PopulateRequestItem(string requestItemId)
        {
            var requestItemQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ItemUID + " ='" + requestItemId + "'";
            this.RequestItem = new Models.SibiRequestItem(DBFactory.GetDatabase().DataTableFromQueryString(requestItemQuery));
        }


    }


}
