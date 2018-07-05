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
        public void Parse(string filePath, KickstartLineTemplate parentLayout, Dictionary<string, KickstartLineTemplate> definitionMap)
        {
            //Create a collection of parsers
            EbcdicParser parentParser = new EbcdicParser();
            Dictionary<string, EbcdicParser> parserCollection = new Dictionary<string, EbcdicParser>();
            foreach(KeyValuePair<string,KickstartLineTemplate> kv in definitionMap)
            {
                parserCollection.Add(kv.Key, new EbcdicParser());
            }

            using (FileStream reader = File.OpenRead(filePath))
            {
                int fsBytes = (int)reader.Length;
                int bytesRead = 0;

                while(bytesRead < fsBytes)
                {
                    byte[] bHeader = new byte[parentLayout.LineSize];
                    reader.Read(bHeader, 0,parentLayout.LineSize);
                    reader.Position = reader.Position - parentLayout.LineSize;
                    ParsedLine p = parentParser.ParseSingleLine(parentLayout, bHeader);

                    string recordType = p.ParsedFields.Where(f => f.Key == "RecordType").Select(f => f.Value.Text).First();
                    int recordLength = int.Parse(p.ParsedFields.Where(f => f.Key == "RecordLength").Select(f => f.Value.Text).First());

                    EbcdicParser parser = parserCollection.Where(pc => pc.Key == recordType).Select(pc => pc.Value).First();
                    KickstartLineTemplate template = definitionMap.Where(d => d.Key == recordType).Select(d => d.Value).First();

                    if (template.VariableWidth)
                    {
                        template.ChangeLineSize(recordLength + template.Offset);
                    }

                    byte[] child = new byte[template.LineSize];
                    bytesRead += reader.Read(child, 0, template.LineSize);
                    
                    if (template.FieldsCount > 0)
                    {
                        ParsedLine pl = parser.ParseAndAddSingleLine(template, child);
                    }
                }
                Parallel.ForEach<KeyValuePair<string, EbcdicParser>>(parserCollection, kv =>
                {
                    kv.Value.SaveParsedLinesAsTxtFile($"{parentLayout.LayoutName}_{kv.Key}.txt", "|", true, true, "¬", true);
                });

                //foreach (KeyValuePair<string, EbcdicParser> kv in parserCollection)
                //{
                //    kv.Value.SaveParsedLinesAsTxtFile($"{parentLayout.LayoutName}_{kv.Key}.txt", "|", true, true, "¬", true);
                //}
            }
        }
    }
}
