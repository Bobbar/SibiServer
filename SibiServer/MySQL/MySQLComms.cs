﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace SibiServer
{
    public class MySQLDatabase : IDisposable, IDataBase
    {
        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            // TODO: uncomment the following line if Finalize() is overridden above.
            //    GC.SuppressFinalize(Me)
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    MySQLConnectString = null;
                }
                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        #endregion IDisposable Support

        #region Fields


        private string MySQLConnectString;

        #endregion Fields

        #region Constructors

        public MySQLDatabase()
        {
            // MySQLConnectString = "server=" + ServerInfo.MySQLServerIP + ";uid=asset_mgr_usr;pwd=" + SecurityTools.DecodePassword(EncMySqlPass) + ";ConnectionTimeout=5;TreatTinyAsBoolean=false;database=";
            MySQLConnectString = "server=10.10.0.89;port=3306;user=approval_user;password=password;database=";
        }

        #endregion Constructors

        #region Methods

        public MySqlConnection NewConnection()
        {
            return new MySqlConnection(GetConnectString());
        }

        public bool OpenConnection(MySqlConnection connection, bool overrideNoPing = false)
        {
            //if (!ServerInfo.ServerPinging) //Server not pinging.
            //{
            //    if (overrideNoPing) //Ignore server not pinging, try to open anyway.
            //    {
            //        return TryOpenConnection(connection);
            //    }
            //    else //Throw exception.
            //    {
            //        throw (new NoPingException());
            //    }
            //}
            //else //Server is pinging, try to open connection.
            //{
            //    return TryOpenConnection(connection);
            //}
            return TryOpenConnection(connection);
        }

        private bool TryOpenConnection(MySqlConnection connection)
        {
            if (ReferenceEquals(connection, null)) //Instantiate new connection.
            {
                connection = NewConnection();
            }
            if (connection.State != ConnectionState.Open) //Try to open connection.
            {
                connection.Open();
            }
            if (connection.State == ConnectionState.Open) //Check if connection is open.
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetConnectString()
        {
            return MySQLConnectString + "test_db";//ServerInfo.CurrentDataBase.ToString();
        }

        public MySqlDataAdapter ReturnMySqlAdapter(string sqlQry, MySqlConnection connection)
        {
            return new MySqlDataAdapter(sqlQry, connection);
        }

        #endregion Methods

        #region IDataBase

        public DbTransaction StartTransaction()
        {
            var conn = NewConnection();
            OpenConnection(conn);
            var trans = conn.BeginTransaction();
            return trans;
        }

        public void CommitTransaction(DbTransaction transaction)
        {
            var conn = transaction.Connection;
            transaction.Commit();
            conn.Close();
            conn.Dispose();
            transaction.Dispose();
        }

        public void RollbackTransaction(DbTransaction transaction)
        {
            var conn = transaction.Connection;
            transaction.Rollback();
            conn.Close();
            conn.Dispose();
            transaction.Dispose();
        }

        public DataTable DataTableFromQueryString(string query)
        {
            using (DataTable results = new DataTable())
            using (var da = new MySqlDataAdapter())
            using (var cmd = new MySqlCommand(query))
            using (var conn = NewConnection())
            {
                OpenConnection(conn);
                cmd.Connection = conn;
                da.SelectCommand = cmd;
                da.Fill(results);
                return results;
            }
        }

        public DataTable DataTableFromCommand(DbCommand command, DbTransaction transaction = null)
        {
            if (ReferenceEquals(transaction, null))
            {
                using (DbDataAdapter da = new MySqlDataAdapter())
                using (DataTable results = new DataTable())
                using (var conn = NewConnection())
                {
                    OpenConnection(conn);
                    command.Connection = conn;
                    da.SelectCommand = command;
                    da.Fill(results);
                    command.Dispose();
                    return results;
                }
            }
            else
            {
                var conn = (MySqlConnection)transaction.Connection;
                using (DbDataAdapter da = new MySqlDataAdapter())
                using (DataTable results = new DataTable())
                {
                    command.Connection = conn;
                    da.SelectCommand = command;
                    da.Fill(results);
                    command.Dispose();
                    return results;
                }
            }
        }

        //public DataTable DataTableFromParameters(string query, List<DBQueryParameter> @params)
        //{
        //    using (var cmd = GetCommandFromParams(query, @params))
        //    using (var results = DataTableFromCommand(cmd))
        //    {
        //        return results;
        //    }
        //}

        public object ExecuteScalarFromCommand(DbCommand command)
        {
            try
            {
                using (var conn = NewConnection())
                {
                    OpenConnection(conn);
                    command.Connection = conn;
                    return command.ExecuteScalar();
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        public object ExecuteScalarFromQueryString(string query)
        {
            using (var conn = NewConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                OpenConnection(conn);
                return cmd.ExecuteScalar();
            }
        }

        public int ExecuteQuery(string query)
        {
            using (var conn = NewConnection())
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                OpenConnection(conn);
                return cmd.ExecuteNonQuery();
            }
        }

        public int ExecuteQuery(DbCommand command, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = (MySqlConnection)transaction.Connection;
                command.Connection = conn;
                using (command)
                {
                    return command.ExecuteNonQuery();
                }

            }
            else
            {
                if (command.Connection == null)
                {
                    using (var conn = NewConnection())
                    using (command)
                    {
                        command.Connection = conn;
                        OpenConnection(conn);
                        return command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (command.Connection)
                    using (command)
                    {
                        return command.ExecuteNonQuery();
                    }
                }
            }
        }

        //public int InsertFromParameters(string tableName, List<DBParameter> @params, DbTransaction transaction = null)
        //{
        //    string SelectQuery = "SELECT * FROM " + tableName + " LIMIT 0";
        //    if (transaction != null)
        //    {
        //        var conn = (MySqlConnection)transaction.Connection;
        //        using (var cmd = new MySqlCommand(SelectQuery, conn, (MySqlTransaction)transaction))
        //        using (var Adapter = new MySqlDataAdapter(cmd))
        //        using (var Builder = new MySqlCommandBuilder(Adapter))
        //        {
        //            var table = DataTableFromQueryString(SelectQuery);
        //            table.Rows.Add();
        //            foreach (var param in @params)
        //            {
        //                table.Rows[0][param.FieldName] = param.Value;
        //            }
        //            return Adapter.Update(table);
        //        }
        //    }
        //    else
        //    {
        //        using (var conn = NewConnection())
        //        using (var Adapter = new MySqlDataAdapter(SelectQuery, conn))
        //        using (var Builder = new MySqlCommandBuilder(Adapter))
        //        {
        //            OpenConnection(conn);
        //            var table = DataTableFromQueryString(SelectQuery);
        //            table.Rows.Add();
        //            foreach (var param in @params)
        //            {
        //                table.Rows[0][param.FieldName] = param.Value;
        //            }
        //            return Adapter.Update(table);
        //        }
        //    }
        //}

        public int UpdateTable(string selectQuery, DataTable table, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = (MySqlConnection)transaction.Connection;
                using (var cmd = new MySqlCommand(selectQuery, conn, (MySqlTransaction)transaction))
                using (var Adapter = new MySqlDataAdapter(cmd))
                using (var Builder = new MySqlCommandBuilder(Adapter))
                {
                    return Adapter.Update(table);
                }
            }
            else
            {
                using (var conn = NewConnection())
                using (var Adapter = new MySqlDataAdapter(selectQuery, conn))
                using (var Builder = new MySqlCommandBuilder(Adapter))
                {
                    OpenConnection(conn);
                    return Adapter.Update(table);
                }
            }
        }

        public int UpdateValue(string tableName, string fieldIn, object valueIn, string idField, string idValue, DbTransaction transaction = null)
        {
            string sqlUpdateQry = "UPDATE " + tableName + " SET " + fieldIn + "=@ValueIN  WHERE " + idField + "='" + idValue + "'";
            if (transaction != null)
            {
                var conn = (MySqlConnection)transaction.Connection;
                using (var cmd = new MySqlCommand(sqlUpdateQry, conn, (MySqlTransaction)transaction))
                {
                    cmd.Parameters.AddWithValue("@ValueIN", valueIn);
                    return cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var conn = NewConnection())
                using (MySqlCommand cmd = new MySqlCommand(sqlUpdateQry, conn))
                {
                    OpenConnection(conn);
                    cmd.Parameters.AddWithValue("@ValueIN", valueIn);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public DbCommand GetCommand(string qryString = "")
        {
            return new MySqlCommand(qryString);
        }

        //public DbCommand GetCommandFromParams(string query, List<DBQueryParameter> @params)
        //{
        //    var cmd = new MySqlCommand();
        //    string ParamQuery = "";
        //    foreach (var param in @params)
        //    {
        //        if (param.Value is bool)
        //        {
        //            ParamQuery += " " + param.FieldName + "=@" + param.FieldName;
        //            cmd.Parameters.AddWithValue("@" + param.FieldName, Convert.ToInt32(param.Value));
        //        }
        //        else
        //        {
        //            if (param.IsExact)
        //            {
        //                ParamQuery += " " + param.FieldName + "=@" + param.FieldName;
        //                cmd.Parameters.AddWithValue("@" + param.FieldName, param.Value);
        //            }
        //            else
        //            {
        //                ParamQuery += " " + param.FieldName + " LIKE @" + param.FieldName;
        //                cmd.Parameters.AddWithValue("@" + param.FieldName, "%" + param.Value.ToString() + "%");
        //            }
        //        }
        //        //Add operator if we are not on the last entry
        //        if (@params.IndexOf(param) < @params.Count - 1)
        //        {
        //            ParamQuery += " " + param.OperatorString;
        //        }
        //    }
        //    cmd.CommandText = query + ParamQuery;
        //    return cmd;
        //}

        #endregion IDataBase
    }
}