using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebcdic2UnicodeApp.Concrete
{
    class Options
    {
        [Option('l', "Layout", Required = true, HelpText = "Please Enter The Layout Name")]
        public string LayoutName { get; set; }

        [Option('f', "SourceFile", Required = true, HelpText = "Please Enter The Source File Path")]
        public string SourceFile { get; set; }

        [Option('o', "OutputPath", Required = true, HelpText = "Please Enter The Output File Path")]
        public string DestinationFile { get; set; }

        [Option('s', "Server", Default = "SQL04", Required = false, HelpText = "Please Enter The Server Name")]
        public string ServerName { get; set; }

        [Option('d', "Database", Default = "KickStartDb", Required = false, HelpText = "Please Enter The Kickstart Database Name")]
        public string DatabaseName { get; set; }

    }
}
