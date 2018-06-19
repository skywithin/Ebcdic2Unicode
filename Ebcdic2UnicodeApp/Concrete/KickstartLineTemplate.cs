using Ebcdic2Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ebcdic2UnicodeApp.Concrete
{
    public partial class KickstartLineTemplate : LineTemplate
    {
        public KickstartLineTemplate(int lineSize,string templateName="")
            :base(lineSize,templateName)
        {
            this.LayoutName = templateName;
        }
        public int LayoutID { get; set; }
        public string LayoutName { get; private set; }
        public int ChunkSize { get; set; }
    }
}
