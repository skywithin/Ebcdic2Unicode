using System;
using EbcdicConverter.Concrete;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EbcdicConverterTests
{
    [TestClass]
    public class KickstartDataRetrieverTests
    {
        [TestMethod]
        public void Connection_String_Retrieval_Passes()
        {
            //Assign
            KickstartDataRetriever retriever = new KickstartDataRetriever("SQL04","KickstartDb");
            //Act
            string expected = "Provider=SQLNCLI11;Server=SQL04;Initial Catalog=KickstartDb;Integrated Security=SSPI;";
            string actual = retriever.GetConnectionString();
            //Assert
            Assert.AreEqual<string>(expected, actual);
        }
        [TestMethod]
        public void Set_Connection_String_Through_Constructor()
        {
            //Assign
            KickstartDataRetriever retriever = new KickstartDataRetriever("SQL04","KickstartDb");
            //Act
            string expected = "Provider=SQLNCLI11;Server=SQL04;Initial Catalog=KickstartDb;Integrated Security=SSPI;";
            string actual = retriever.GetConnectionString();
            //Assert
            Assert.AreEqual<string>(expected, actual);
        }
    }
}
