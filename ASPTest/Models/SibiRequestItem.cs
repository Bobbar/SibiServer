using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;

namespace ASPTest.Models
{
    public class SibiRequestItem : DataMapObject
    {

        public SibiRequestItem(DataTable data) : base(data) { }
        public SibiRequestItem(DataRow data) : base(data) { }
        public SibiRequestItem() { }

        [DataColumnName(SibiRequestItemsCols.ItemUID)]
        public override string GUID { get; set; }
        [DataColumnName(SibiRequestItemsCols.RequestUID)]
        public string RequestGUID { get; set; }
        [DataColumnName(SibiRequestItemsCols.User)]
        public string User { get; set; }
        [DataColumnName(SibiRequestItemsCols.Description)]
        public string Description { get; set; }
        [DataColumnName(SibiRequestItemsCols.Location)]
        public string Location { get; set; }
        [DataColumnName(SibiRequestItemsCols.Status)]
        public string Status { get; set; }
        [DataColumnName(SibiRequestItemsCols.ReplaceAsset)]
        public string ReplaceAsset { get; set; }
        [DataColumnName(SibiRequestItemsCols.ReplaceSerial)]
        public string ReplaceSerial { get; set; }
        [DataColumnName(SibiRequestItemsCols.NewAsset)]
        public string NewAsset { get; set; }
        [DataColumnName(SibiRequestItemsCols.NewSerial)]
        public string NewSerial { get; set; }
        [DataColumnName(SibiRequestItemsCols.OrgCode)]
        public string OrgCode { get; set; }
        [DataColumnName(SibiRequestItemsCols.ObjectCode)]
        public string ObjCode { get; set; }
        [DataColumnName(SibiRequestItemsCols.Qty)]
        public string Qty { get; set; }
        [DataColumnName(SibiRequestItemsCols.Timestamp)]
        public string TimeStamp { get; set; }
        [DataColumnName(SibiRequestItemsCols.BudgetLineNo)]
        public string BudgetLineNo { get; set; }
        [DataColumnName(SibiRequestItemsCols.ChangeType)]
        public string ChangeType { get; set; }


        public override string TableName { get; set; } = SibiRequestItemsCols.TableName;
    }
}
