using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AssetManager;
using System.Data;

namespace SibiServer.Models
{
    public class ApprovalItem : DataMapObject
    {
        [DataColumnName(SibiApprovalItemsColumns.UID)]
        public override string GUID { get; set; }
        public override string TableName { get; set; } = SibiApprovalItemsColumns.TableName;

        [DataColumnName(SibiApprovalItemsColumns.ApprovalUID)]
        public string ApprovalId { get; set; }

        [DataColumnName(SibiApprovalItemsColumns.RequestItemUID)]
        public string RequestItemUID { get; set; }

        [DataColumnName(SibiApprovalItemsColumns.ChangeType)]
        public string ChangeType { get; set; }



        public SibiRequestItem OldItemValues { get; set; } = new SibiRequestItem();

        public SibiRequestItem NewItemValues { get; set; } = new SibiRequestItem();

        public DataRow RequestItemNewValuesRaw { get; set; }

        [DataColumnName(SibiApprovalItemsColumns.ItemHistoryUID)]
        public string requestItemHistoryUID
        {
            set
            {
                PopulateHistoryicalItem(value);
            }
        }
       

        [DataColumnName(SibiApprovalItemsColumns.NewValues)]
        public byte[] requestItemNewValsBinary
        {
            set
            {
                RequestItemNewValuesRaw = Tools.SerialData.DeserializeDataRow(value);
                NewItemValues = new SibiRequestItem(RequestItemNewValuesRaw);
            }
        }

        public ApprovalItem() { }

        public ApprovalItem(DataTable data) : base(data) { }

        public ApprovalItem(DataRow data) : base(data) { }

        private void PopulateHistoryicalItem(string histItemUID)
        {
            var selectItemQuery = "SELECT * FROM " + SibiHistoricalItemsCols.TableName + " WHERE " + SibiHistoricalItemsCols.HistID + " = '" + histItemUID + "'";
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(selectItemQuery))
            {
                this.MapClassProperties(results);
            }
            
        }

    }
}
