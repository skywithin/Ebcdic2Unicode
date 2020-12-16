using Ebcdic2Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebcdic2UnicodeApp.Concrete
{
    public class MultiFileTypeMeta
    {
        public MultiFileTypeMeta()
        {
            this.Parser = new EbcdicParser();
            this.AppendToFile = false;
        }
        public int DefinitionID { get; set; }
        public KickstartLineTemplate DefinitionTemplate { get; set; }
        public EbcdicParser Parser { get; private set; }
        public bool AppendToFile { get; set; }
    }
}

