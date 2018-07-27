using Ebcdic2Unicode;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebcdic2UnicodeApp.Concrete
{
    public static class FieldTemplateMapper
    {
        static FieldType GetFieldType(string EbcdicType)
        {
            FieldType result;
            string[] ebcdicTypes = new string[] { "EBCDIC", "DATETIME", "PACKED", "BINARY", "NUMERIC" };
            string dataType = EbcdicType.ToUpper();

            if (!ebcdicTypes.Contains(dataType))
                throw new NotSupportedException($"Type '{EbcdicType}' is not a supported Ebcdic data type");

            switch (dataType)
            {
                case "EBCDIC":
                    result = FieldType.String;
                    break;
                case "DATETIME":
                    result = FieldType.DateString;
                    break;
                case "PACKED":
                    result = FieldType.Packed;
                    break;
                case "BINARY":
                    result = FieldType.BinaryNum;
                    break;
                case "NUMERIC":
                    result = FieldType.NumericString;
                    break;
                default:
                    result = FieldType.String;
                    break;
            }

            return result;
        }
        public static FieldTemplate GetFieldTemplate(IDataRecord record)
        {
            return new FieldTemplate(record["FieldName"].ToString(), GetFieldType(record["ACLDataTypeName"].ToString()), (int)record["StartPosition"], (int)record["FieldWidth"], (int)record["DecimalPlaces"]);
        }

    }
}
