using Ebcdic2Unicode;
using EbcdicConverter.Abstract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebcdic2UnicodeApp.Concrete
{
    public class MultiFileTypeParser
    {
        public MultiFileTypeParser()
        {

        }    
        public void Parse(string filePath, KickstartLineTemplate parentLayout, List<MultiFileTypeMeta> MetaData)
        {
            //Get File Information
            FileInfo info = new FileInfo(filePath);
            string fileName = info.FullName.Replace(info.Extension, "");

            //Create a collection of parsers
            EbcdicParser parentParser = new EbcdicParser();

            //Open File and Action
            using (FileStream reader = File.OpenRead(filePath))
            {
                int fsBytes = (int)reader.Length;
                int bytesRead = 0;
                byte[] bHeader = new byte[parentLayout.LineSize];
                byte[] child;

                while (bytesRead < fsBytes)
                {
                    reader.Read(bHeader, 0,parentLayout.LineSize);
                    reader.Position = reader.Position - parentLayout.LineSize;
                    ParsedLine p = parentParser.ParseSingleLine(parentLayout, bHeader);

                    string recordType = p.ParsedFields.Where(f => f.Key == "RecordType").Select(f => f.Value.Text).First();
                    int recordLength = int.Parse(p.ParsedFields.Where(f => f.Key == "RecordLength").Select(f => f.Value.Text).First());

                    MultiFileTypeMeta meta = MetaData.Where(md => md.DefinitionName == recordType).First();

                    if (meta.DefinitionTemplate.VariableWidth)
                    {
                        meta.DefinitionTemplate.ChangeLineSize(recordLength + meta.DefinitionTemplate.Offset);
                        child = new byte[recordLength + meta.DefinitionTemplate.Offset];
                    } else
                    {
                        child = new byte[meta.DefinitionTemplate.LineSize];
                    }

                    bytesRead += reader.Read(child, 0, meta.DefinitionTemplate.LineSize);
                    
                    if (meta.DefinitionTemplate.FieldsCount > 0)
                    {       
                        ParsedLine pl = meta.Parser.ParseAndAddSingleLine(meta.DefinitionTemplate, child, meta.DefinitionTemplate.ChunkSize);
                        if (meta.Parser.ParsedLines.Length >= meta.DefinitionTemplate.ChunkSize)
                        {
                            meta.Parser.SaveParsedLinesAsTxtFile($"{fileName}_{recordType}.txt", "|", true, true, "¬", meta.AppendToFile);
                            meta.AppendToFile = true;
                        }
                    }
                }
                MetaData.ForEach(m => m.Parser.SaveParsedLinesAsTxtFile($"{fileName}_{m.DefinitionName}.txt", "|", true, true, "¬", m.AppendToFile));
            }
        }
    }
}
