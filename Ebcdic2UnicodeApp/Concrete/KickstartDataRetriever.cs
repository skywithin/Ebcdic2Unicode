using Ebcdic2Unicode;
using EbcdicConverter.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using EbcdicConverter.Concrete;
using Ebcdic2UnicodeApp.Extensions;
using Ebcdic2UnicodeApp.Concrete;
using System.Data;

namespace EbcdicConverter.Concrete
{
    public class KickstartDataRetriever : IDataRetriever<KickstartLineTemplate>
    {
        private string ServerName { get; set; }
        private string DatabaseName { get; set; }

        public KickstartDataRetriever(string ServerName,string DatabaseName)
        {
            this.ServerName = ServerName;
            this.DatabaseName = DatabaseName;
        }

        private string GetConnectionString() {
            if (string.IsNullOrWhiteSpace(this.ServerName))
                throw new FormatException("Server name must be set!");
            if (string.IsNullOrWhiteSpace(this.DatabaseName))
                throw new FormatException("Database name must be set!");

            return $"server={this.ServerName};database={this.DatabaseName};Trusted_Connection=True;";
        }

        public int GetParentLayoutIDByName(string layoutName)
        {
            int result;
            using (SqlConnection cnxn = new SqlConnection(this.GetConnectionString()))
            {
                cnxn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnxn;
                    cmd.CommandText = $@"SELECT LayoutID
                                         FROM ACLLayout al
                                         WHERE al.LayoutName='{layoutName}'
                                         AND al.ParentLayoutID IS NULL";
                    result = int.Parse(cmd.ExecuteScalar().ToString());
                }
            }
            return result;
        }

        public KickstartLineTemplate GetTemplateByID(int layoutID)
        {
            KickstartLineTemplate result;
            using(SqlConnection cnxn = new SqlConnection(this.GetConnectionString()))
            {
                cnxn.Open();
                using(SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cnxn;
                    cmd.CommandText = $@"SELECT LayoutID, LayoutName, FileWidth, ChunkSize, Offset, VariableWidth, CAST(CASE WHEN COALESCE(c.ChildCount,0) > 0 THEN 1 ELSE 0 END AS BIT) AS MultiFileTypeFile, Import
                                         FROM ACLLayout al
                                         OUTER APPLY(
                                            SELECT COUNT(1) AS ChildCount
                                            FROM ACLLayout al2
                                            WHERE al2.ParentLayoutID = al.LayoutID
                                         ) c
                                         WHERE al.LayoutID='{layoutID}'";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            result = reader.Enumerate().Select(r => new KickstartLineTemplate((int)r["FileWidth"], r["LayoutName"].ToString())
                            {
                                LayoutID = (int)r["LayoutID"],
                                ChunkSize = (int)r["ChunkSize"],
                                Offset = (int)r["Offset"],
                                VariableWidth = (bool)r["VariableWidth"],
                                MultiFileTypeFile = (bool)r["MultiFileTypeFile"],
                                Import = (bool)r["Import"]
                            }).First();
                        } catch (InvalidOperationException ex)
                        {
                            throw new InvalidOperationException($"The Layout with ID:'{layoutID} does not exist!", ex);
                        }
                    }

                    cmd.CommandText = $@"SELECT LayoutID FROM ACLLayout WHERE ParentLayoutID = {result.LayoutID}";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        result.ChildLayoutIDs = reader.Enumerate().Select(r => int.Parse(r["LayoutID"].ToString())).ToList();
                    }

                    cmd.CommandText = $@"SELECT FieldName,dt.ACLDataTypeName,StartPosition,FieldWidth,DecimalPlaces
                                        FROM ACLLayoutDetail ld
                                        INNER JOIN ACLDataType dt on dt.ACLDataTypeID = ld.ACLDataTypeID
                                        INNER JOIN ACLLayout al on al.LayoutID = ld.LayoutID
                                        WHERE al.LayoutID='{layoutID}'";
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        result.AddFieldTemplates(reader.Enumerate().Select(r => FieldTemplateMapper.GetFieldTemplate(r)).ToList());
                    }
                }
            }
            return result;           
        }
    }
}
