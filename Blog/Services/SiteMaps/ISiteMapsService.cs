using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Blog.Services
{
    public interface ISiteMapsService
    {
        XmlDocument GenerateNewSiteMap();
        void Save(XmlDocument document);
    }
}
