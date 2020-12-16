using Ebcdic2Unicode;
using Ebcdic2UnicodeApp.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbcdicConverter.Abstract
{
    public interface IDataRetriever<T> where T : LineTemplate
    {
        T GetTemplateByID(int layoutID);
        int GetParentLayoutIDByName(string layoutName);
    }
}
