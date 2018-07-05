using Ebcdic2Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EbcdicConverter.Concrete;
using Ebcdic2UnicodeApp.Concrete;
using CommandLine;
using EbcdicConverter.Abstract;

namespace Ebcdic2UnicodeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            /*CommandLine.Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options => RunCodeAndReturnExitCode(options),
                    _ => 1
                );
            */

            //Create parser
            MultiFileTypeParser parser = new MultiFileTypeParser();
            IDataRetriever<KickstartLineTemplate> retriever = new KickstartDataRetriever("SQL04", "KickStartDb");

            //Get Defs
            KickstartLineTemplate template = retriever.GetTemplate("EIWMTINVCE");
            Dictionary<string,KickstartLineTemplate> children = new Dictionary<string, KickstartLineTemplate>();
            template.ChildLayoutNames.ForEach(r =>
            {
                KickstartLineTemplate t = retriever.GetTemplate(r);
                children.Add(t.LayoutName, t);
            });
            parser.Parse(@"W:\ASDA\ETL$\EDI\Decompressed\EIWMTINVCE01032018.dat", template, children);


        }
        static int RunCodeAndReturnExitCode(Options options)
        {
            BulkParser parser = new BulkParser();
            IDataRetriever<KickstartLineTemplate> retriever = new KickstartDataRetriever(options.ServerName, options.DatabaseName);
            KickstartLineTemplate layout = retriever.GetTemplate(options.LayoutName);
            parser.ParseAndWriteLines(layout, options.SourceFile, options.DestinationFile, chunkSize: layout.ChunkSize);
            return 0;
        }
    }
}
