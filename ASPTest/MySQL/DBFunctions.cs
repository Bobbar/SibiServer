using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPTest
{
    public static class DBFunctions
    {
        public static void PopRequestData(ref Models.RequestApproval request)
        {
            var query = "SELECT * FROM " + request.TableName + " WHERE uid ='" + request.GUID + "'";
            request = new Models.RequestApproval(DBFactory.GetDatabase().DataTableFromQueryString(query));
            request.MapClassProperties(DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ItemUID + " = '" + request.SibiRequestItemUID + "'"));

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



    }
}
