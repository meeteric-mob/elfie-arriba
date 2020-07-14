using Arriba.Model;
using Arriba.Model.Column;
using Arriba.Server.Hosting;
using Arriba.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arriba.Communication.Server.Application
{
    public class ArribaManagementService : IArribaManagementService
    {
        private readonly SecureDatabase _database;

        public ArribaManagementService(DatabaseFactory databaseFactory)
        {
            _database = databaseFactory.GetDatabase();
        }

        
        public IEnumerable<string> GetTables()
        {
            return _database.TableNames;
        }
        
        public Dictionary<string, TableInformation> GetAllBasic()
        {
            bool hasTables = false;

            Dictionary<string, TableInformation> allBasics = new Dictionary<string, TableInformation>();
            foreach (string tableName in _database.TableNames)
            {
                hasTables = true;

                //if (HasTableAccess(tableName, ctx.Request.User, PermissionScope.Reader))
                //{
                    allBasics[tableName] = GetTableBasics(tableName);
                //}
            }

            // If you didn't have access to any tables, return a distinct result to show Access Denied in the browser
            // but not a 401, because that is eaten by CORS.
            if (allBasics.Count == 0 && hasTables)
            {
                return null;
            }

            return allBasics;
        }

        private TableInformation GetTableBasics(string tableName)
        {
            var table = _database[tableName];

            TableInformation ti = new TableInformation();
            ti.Name = tableName;
            ti.PartitionCount = table.PartitionCount;
            ti.RowCount = table.Count;
            ti.LastWriteTimeUtc = table.LastWriteTimeUtc;
            ti.CanWrite = true;//HasTableAccess(tableName, ctx.Request.User, PermissionScope.Writer);
            ti.CanAdminister = true;// HasTableAccess(tableName, ctx.Request.User, PermissionScope.Owner);

            IList<string> restrictedColumns = null;//_database.GetRestrictedColumns(tableName, (si) => this.IsInIdentity(ctx.Request.User, si));
            if (restrictedColumns == null)
            {
                ti.Columns = table.ColumnDetails;
            }
            else
            {
                List<ColumnDetails> allowedColumns = new List<ColumnDetails>();
                foreach (ColumnDetails column in table.ColumnDetails)
                {
                    if (!restrictedColumns.Contains(column.Name)) allowedColumns.Add(column);
                }
                ti.Columns = allowedColumns;
            }

            return ti;
        }

        
    }
}
