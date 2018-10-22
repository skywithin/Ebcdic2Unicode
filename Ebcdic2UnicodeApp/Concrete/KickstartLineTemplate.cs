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
        public int Offset { get; set; }
        public bool VariableWidth { get; set; }
        public bool MultiFileTypeFile { get; set; }
        public bool Import { get; set; }
        public List<string> ChildLayoutNames { get; set; }

        public void ChangeLineSize(int newLineSize)
        {
            if (this.VariableWidth == true)
            {
                this.LineSize = newLineSize;
            } else
            {
                throw new InvalidOperationException("You cannot change the line size property on a file which is not variable width!");
            }
        }
    }
}
