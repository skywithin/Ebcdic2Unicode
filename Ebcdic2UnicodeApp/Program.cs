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
            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    options => RunCodeAndReturnExitCode(options),
                    _ => 1
                );
        }
        static int RunCodeAndReturnExitCode(Options options)
        {
            IDataRetriever<KickstartLineTemplate> retriever = new KickstartDataRetriever(options.ServerName, options.DatabaseName);
            KickstartLineTemplate layout = retriever.GetTemplate(options.LayoutName);

            if (layout.MultiFileTypeFile)
            {
                MultiFileTypeParser parser = new MultiFileTypeParser();
                List<MultiFileTypeMeta> children = new List<MultiFileTypeMeta>();
                layout.ChildLayoutNames.ForEach(r =>
                {
                    MultiFileTypeMeta m = new MultiFileTypeMeta()
                    {
                        DefinitionName = r,
                        DefinitionTemplate = retriever.GetTemplate(r)
                    };
                    children.Add(m);
                });
                parser.Parse(options.SourceFile, layout, children);
            } else
            {
                BulkParser parser = new BulkParser();
                parser.ParseAndWriteLines(layout, options.SourceFile, options.DestinationFile, chunkSize: layout.ChunkSize);
            }
            return 0;
        }
    }
}
