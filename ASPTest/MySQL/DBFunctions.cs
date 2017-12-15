using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPTest
{
    public static class DBFunctions
    {
        public static void PopRequestData(ref Models.RequestApproval approval)
        {
            var approvalQuery = "SELECT * FROM " + approval.TableName + " WHERE uid ='" + approval.GUID + "'";
            approval = new Models.RequestApproval(DBFactory.GetDatabase().DataTableFromQueryString(approvalQuery));


            var itemsQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ApprovalID + " = '" + approval.GUID + "'";

            using (var itemsTable = DBFactory.GetDatabase().DataTableFromQueryString(itemsQuery))
            {

                if (itemsTable.Rows.Count > 0)
                {
                    approval.SibiRequestItems = new Models.SibiRequestItem[itemsTable.Rows.Count];
                    for (int i = 0; i < itemsTable.Rows.Count; i++)
                    {
                        approval.SibiRequestItems[i] = new Models.SibiRequestItem(itemsTable.Rows[i]);

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
            bool isApproved = false;

            var appVal = Convert.ToString(DBFactory.GetDatabase().ExecuteScalarFromQueryString("SELECT approval_status FROM " + request.TableName + " WHERE uid ='" + request.GUID + "'"));
            isApproved = (appVal == "accept");//Convert.ToBoolean(appVal);

            if (!isApproved)
            {
                var approveQry = "UPDATE " + request.TableName + " SET approval_status ='accept' WHERE uid ='" + request.GUID + "'";
                int affectedRows = DBFactory.GetDatabase().ExecuteQuery(approveQry);
                // If the command returned affected rows, return true for a success.
                if (affectedRows > 0)
                {
                    return true;
                }

            }
            // The request is already approved or no rows were affected, return false for error.
            return false;

        }

        public static bool RejectRequest(Models.RequestApproval request)
        {
            bool isRejected = false;

            var appVal = Convert.ToString(DBFactory.GetDatabase().ExecuteScalarFromQueryString("SELECT approval_status FROM " + request.TableName + " WHERE uid ='" + request.GUID + "'"));
            isRejected = (appVal == "reject");//Convert.ToBoolean(appVal);

            if (!isRejected)
            {
                var approveQry = "UPDATE " + request.TableName + " SET approval_status ='reject' WHERE uid ='" + request.GUID + "'";
                int affectedRows = DBFactory.GetDatabase().ExecuteQuery(approveQry);
                // If the command returned affected rows, return true for a success.
                if (affectedRows > 0)
                {
                    return true;
                }

            }
            // The request is already approved or no rows were affected, return false for error.
            return false;

        }

        public static void SetNotifySent(string approvalID)
        {
            var setSentQuery = "UPDATE sibi_request_items_approvals SET approval_sent = '1', approval_status = 'pending' WHERE uid = '" + approvalID + "'";
            int affectedRows = DBFactory.GetDatabase().ExecuteQuery(setSentQuery);
            Console.WriteLine(affectedRows.ToString());
        }


    }
}
