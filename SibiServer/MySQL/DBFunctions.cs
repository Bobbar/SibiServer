using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using AssetManager;
using System.Data.Common;

namespace SibiServer
{
    public static class DBFunctions
    {
        public static void PopApprovalData(ref Models.RequestApproval approval)
        {
            var approvalQuery = "SELECT * FROM " + approval.TableName + " WHERE " + SibiApprovalColumns.UID + " ='" + approval.GUID + "'";
            approval = new Models.RequestApproval(DBFactory.GetDatabase().DataTableFromQueryString(approvalQuery));


            //var itemsQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ApprovalID + " = '" + approval.GUID + "'";
            var approvalItemsQuery = "SELECT * FROM " + SibiApprovalItemsColumns.TableName + " WHERE " + SibiApprovalItemsColumns.ApprovalUID + " = '" + approval.GUID + "'";
            using (var approvalItemsTable = DBFactory.GetDatabase().DataTableFromQueryString(approvalItemsQuery))
            {

                if (approvalItemsTable.Rows.Count > 0)
                {
                    approval.ApprovalItems = new Models.ApprovalItem[approvalItemsTable.Rows.Count];
                    // approval.SibiRequestItems = new Models.SibiRequestItem[itemsTable.Rows.Count];
                    for (int i = 0; i < approvalItemsTable.Rows.Count; i++)
                    {
                        //approval.SibiRequestItems[i] = new Models.SibiRequestItem(approvalItemsTable.Rows[i]);
                        approval.ApprovalItems[i] = new Models.ApprovalItem(approvalItemsTable.Rows[i]);


                    }


                }

            }
            var requestQuery = "SELECT * FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.UID + " = '" + approval.SibiRequestUID + "'";
            using (var requestTable = DBFactory.GetDatabase().DataTableFromQueryString(requestQuery))
            {
                approval.SibiRequest = new Models.SibiRequest(requestTable);
            }



            //approval.MapClassProperties(DBFactory.GetDatabase().DataTableFromQueryString(itemsQuery));
            //approval.MapClassProperties(DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ItemUID + " = '" + approval.SibiRequestUID + "'"));

        }

        public static bool ApproveRequest(Models.RequestApproval request)
        {
            var database = DBFactory.GetDatabase();

            bool isApproved = false;

            var appVal = Convert.ToString(database.ExecuteScalarFromQueryString("SELECT " + SibiApprovalColumns.Status + " FROM " + request.TableName + " WHERE " + SibiApprovalColumns.UID + " ='" + request.GUID + "'"));
            isApproved = (appVal == ApprovalStatus.approved.ToString());

            if (!isApproved)
            {
                using (var trans = database.StartTransaction())
                {
                    try
                    {


                        request.ApprovalStatus = ApprovalStatus.approved.ToString();

                        int affectedRows = request.Update(trans);

                        if (affectedRows > 0)
                        {
                            if (!SetRequestItemsCurrent(request, trans))
                            {
                                return false;
                            }
                        }

                        AddNewNotification(NotificationType.ACCEPTED, trans, request.GUID);

                        database.CommitTransaction(trans);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        database.RollbackTransaction(trans);
                        return false;
                    }
                }
            }
            // The request is already approved or no rows were affected, return false for error.
            return false;


        }

        private static bool SetRequestItemsCurrent(Models.RequestApproval request, DbTransaction transaction)
        {
            var approvalItemsQuery = "SELECT " + SibiApprovalItemsColumns.RequestItemUID + " FROM " + SibiApprovalItemsColumns.TableName + " WHERE " + SibiApprovalItemsColumns.ApprovalUID + " = '" + request.GUID + "'";

            using (var approvalItems = DBFactory.GetDatabase().DataTableFromQueryString(approvalItemsQuery))
            {
                int affectedRows = 0;
                foreach (DataRow item in approvalItems.Rows)
                {
                    var itemsQry = "UPDATE " + SibiRequestItemsCols.TableName +
               " SET " + SibiRequestItemsCols.ModifyStatus + " ='" + ItemChangeStatus.MODCURR.ToString() + "' WHERE " + SibiRequestItemsCols.ItemUID + " = '" + item[SibiApprovalItemsColumns.RequestItemUID] + "'";

                    using (var cmd = DBFactory.GetDatabase().GetCommand(itemsQry))
                    {
                        affectedRows += DBFactory.GetDatabase().ExecuteQuery(cmd, transaction);
                    }


                }

                if (affectedRows > 0)
                {
                    return true;
                }
                return false;
            }

            //var itemsQry = "UPDATE " + request.TableName +
            //    " SET " + SibiRequestItemsCols.ModifyStatus + " ='" + ItemChangeStatus.MODCURR.ToString() + "' WHERE " + SibiRequestItemsCols.ItemUID + " = '" + request.GUID + "'";




            // int affectedRows = DBFactory.GetDatabase().ExecuteQuery(itemsQry);

            //if (affectedRows > 0)
            //{
            //    return true;
            //}
            //return false;
        }

        public static bool RejectRequest(Models.RequestApproval request)
        {
            var database = DBFactory.GetDatabase();
            bool isRejected = false;

            var appVal = Convert.ToString(database.ExecuteScalarFromQueryString("SELECT " + SibiApprovalColumns.Status + " FROM " + request.TableName + " WHERE " + SibiApprovalColumns.UID + " ='" + request.GUID + "'"));
            isRejected = (appVal == ApprovalStatus.rejected.ToString());

            if (!isRejected)
            {
                using (var trans = database.StartTransaction())
                {
                    try
                    {
                        request.ApprovalStatus = ApprovalStatus.rejected.ToString();

                        int affectedRows = request.Update(trans);
                        if (affectedRows < 1)
                        {
                            return false;
                        }

                        AddNewNotification(NotificationType.REJECTED, trans, request.GUID);
                        database.CommitTransaction(trans);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        database.RollbackTransaction(trans);
                        return false;
                    }
                }
            }
            // The request is already approved or no rows were affected, return false for error.
            return false;

        }

        //public static void SetNotifySent(string approvalID)
        //{
        //    var setSentQuery = "UPDATE " + SibiApprovalColumns.TableName + " SET " + SibiApprovalColumns.NotifySent + " = '1' WHERE " + SibiApprovalColumns.UID + " = '" + approvalID + "'";

        //    int affectedRows = DBFactory.GetDatabase().ExecuteQuery(setSentQuery);
        //    Console.WriteLine(affectedRows.ToString());
        //}

        public static void SetNotifySent(Emailer.Notification notification)
        {
            notification.Sent = true;
            var setSentQuery = "UPDATE " + NotificationColumns.TableName + " SET " + NotificationColumns.Sent + " = '1' WHERE " + NotificationColumns.UID + " = '" + notification.GUID + "'";
            int affectedRows = DBFactory.GetDatabase().ExecuteQuery(setSentQuery);

            // Console.WriteLine(affectedRows.ToString());
        }


        public static DataTable GetApprovalsTable(string approverId)
        {
            var approvalsQuery = "SELECT * FROM " + SibiApprovalColumns.TableName + " WHERE " + SibiApprovalColumns.ApproverID + " = '" + approverId + "'";

            return DBFactory.GetDatabase().DataTableFromQueryString(approvalsQuery);

        }



        private static void AddNewNotification(NotificationType type, DbTransaction transaction, string approvalId = "", string requestItemUID = "")
        {
            var newGUID = Guid.NewGuid().ToString();
            var emptyNotificationQry = "SELECT * FROM " + NotificationColumns.TableName + " LIMIT 0";
            using (var newNotification = DBFactory.GetDatabase().DataTableFromQueryString(emptyNotificationQry))
            {
                newNotification.Rows.Add();
                var newRow = newNotification.Rows[0];

                newRow[NotificationColumns.UID] = newGUID;

                switch (type)
                {
                    case NotificationType.APPROVAL:
                        newRow[NotificationColumns.Type] = NotificationType.APPROVAL.ToString();
                        newRow[NotificationColumns.ApprovalID] = approvalId;

                        break;

                    case NotificationType.ACCEPTED:
                        newRow[NotificationColumns.Type] = NotificationType.ACCEPTED.ToString();
                        newRow[NotificationColumns.ApprovalID] = approvalId;

                        break;

                    case NotificationType.REJECTED:
                        newRow[NotificationColumns.Type] = NotificationType.REJECTED.ToString();
                        newRow[NotificationColumns.ApprovalID] = approvalId;

                        break;

                    case NotificationType.CHANGE:
                        newRow[NotificationColumns.Type] = NotificationType.CHANGE.ToString();
                        newRow[NotificationColumns.RequestItemUID] = requestItemUID;

                        break;
                }

                DBFactory.GetDatabase().UpdateTable(emptyNotificationQry, newNotification, transaction);

            }
        }

    }

    public enum NotificationType
    {
        /// <summary>
        /// Notify approver of new approval.
        /// </summary>
        APPROVAL,
        /// <summary>
        /// Approver accepted. Notify approver and requestor.
        /// </summary>
        ACCEPTED,
        /// <summary>
        /// Approver rejected. Notify approver and requestor.
        /// </summary>
        REJECTED,
        /// <summary>
        /// Item status change. Only notify approver.
        /// </summary>
        CHANGE
    }

    public enum ItemChangeStatus
    {
        /// <summary>
        /// Is current. (Up-to-date)
        /// </summary>
        MODCURR,
        /// <summary>
        /// Is a new item.
        /// </summary>
        MODNEW,
        /// <summary>
        /// Is changed. Pending approval.
        /// </summary>
        MODCHAN,
        /// <summary>
        /// Status change. Only notify approver.
        /// </summary>
        MODSTCH
    }

    public enum ApprovalStatus
    {
        pending,
        approved,
        rejected
    }

}
