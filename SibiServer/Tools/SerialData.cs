using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Runtime.Serialization;
using System.IO;



namespace SibiServer.Tools
{
    public static class SerialData
    {
        public static byte[] SerializeDataRow(DataRow row)
        {
            using (DataTable serialTable = row.Table.Clone())
            {
                serialTable.TableName = "serialTable";
                
                serialTable.Rows.Add(row.ItemArray);
                
                var serializer = new DataContractSerializer(typeof(DataTable));
                using (var memoryStream = new MemoryStream())
                {

                    serializer.WriteObject(memoryStream, serialTable);
                    return memoryStream.ToArray();
                }
            }
        }

        public static DataRow DeserializeDataRow(byte[] byteArray)
        {
            var deserializer = new DataContractSerializer(typeof(DataTable));
            using (var memoryStream = new MemoryStream(byteArray))
            {
                var serialTable = new DataTable();
                serialTable = (DataTable)deserializer.ReadObject(memoryStream);
                return serialTable.Rows[0];
            }

        }

    }
}
