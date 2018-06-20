using Ebcdic2Unicode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebcdic2UnicodeApp.Concrete
{
    public class BulkParser : EbcdicParser
    {
        public BulkParser()
        {
            
        }
        /// <summary>
        /// Parses multiple lines of binary data then writes it out to a specified destination in chunks.
        /// </summary>
        /// <param name="lineTemplate">Template</param>
        /// <param name="sourceFilePath">Source file path</param>
        /// <param name="writeOutputType">Output file type</param>
        /// <param name="outputFilePath">Output file path</param>
        /// <param name="chunkSize">Threshold in which to write output</param>
        /// <param name="includeColumnNames">Include column names in file</param>
        /// <param name="addQuotes">Add text quotes as part of output</param>
        /// <returns>boolean on completion</returns>
        public bool ParseAndWriteLines(LineTemplate lineTemplate, string sourceFilePath, string outputFilePath, WriteOutputType writeOutputType = WriteOutputType.Txt, bool includeColumnNames = true, bool addQuotes = true, int chunkSize = -1)
        {
            try
            {

                using (FileStream reader = File.OpenRead(sourceFilePath))
                {
                    int fsBytes = (int)reader.Length;
                    int chunk = chunkSize == -1 ? fsBytes : (lineTemplate.LineSize * chunkSize);
                    int loop = (int)(Math.Ceiling(((decimal)fsBytes / chunk)));
                    bool append = false;
                    int bytesRead = 0;

                    for (int i = 1; i <= loop; i++)
                    {
                        byte[] b = new byte[0];

                        Console.WriteLine($"Handling Batch {i} of {loop}");

                        if (bytesRead + chunk > fsBytes)
                        {
                            chunk = fsBytes - bytesRead;
                        }

                        b = new byte[chunk];
                        bytesRead += reader.Read(b, 0, chunk);

                        this.ParsedLines = ParseAllLines(lineTemplate, b);

                        switch (writeOutputType)
                        {
                            case WriteOutputType.Csv:
                                SaveParsedLinesAsCsvFile(outputFilePath, includeColumnNames, addQuotes, append);
                                break;
                            case WriteOutputType.Txt:
                                SaveParsedLinesAsTxtFile(outputFilePath, "|", includeColumnNames, addQuotes, "¬", append);
                                break;
                            case WriteOutputType.XML:
                                throw new NotImplementedException("XML Exporting in batches has not yet been implemented");
                        }
                        append = true;
                        Console.WriteLine($"--------------------------------------");
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
