using       Ebcdic2Unicode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EbcdicConverter.Concrete;
using Ebcdic2UnicodeApp.Concrete;
using CommandLine;

namespace Ebcdic2UnicodeApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Options options = new Options();
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts =>
                {
                    EbcdicParser parser = new EbcdicParser();
                    KickstartDataRetriever retriever = new KickstartDataRetriever(opts.ServerName, opts.DatabaseName);
                    KickstartLineTemplate layout = retriever.RetrieveTemplate(opts.LayoutName);
                    parser.ParseAndWriteLines(layout, opts.SourceFile, opts.DestinationFile, chunkSize: layout.ChunkSize);
                })
                .WithNotParsed<Options>(opts =>
                {
                    throw new FormatException("Bad Parsing");
                });
        
            /*
            LineTemplate ddadscchrg = new LineTemplate(337, "DDADSCCHRG");
            ddadscchrg.AddFieldTemplates(new List<FieldTemplate>()
            {
                new FieldTemplate("DealID",FieldType.BinaryNum,0,4),
                new FieldTemplate("DealDiscountStatusID",FieldType.String,32,1)
            });
            parser.ParseAndWriteLines(ddadscchrg, @"W:\ASDA\ETL$\Deals\Decompressed\DDADSCCHRG06052018.dat", @"W:\ASDA\ETL$\Deals\Decompressed\DDADSCCHRG06052018.txt", chunkSize: 250000);
            
            
           
            LineTemplate dealComent = new LineTemplate(302, "DEALCOMENT");
            dealComent.AddFieldTemplates(new List<FieldTemplate>()
            {
                new FieldTemplate("DealID",FieldType.BinaryNum,0,4),
                new FieldTemplate("CommentTypeCode",FieldType.BinaryNum,4,2),
                new FieldTemplate("CreateTS",FieldType.DateString,6,19),
                new FieldTemplate("CreateUser",FieldType.String,32,10),
                new FieldTemplate("VendorViewInd",FieldType.String,42,1),
                new FieldTemplate("Unknown",FieldType.BinaryNum,43,2),
                new FieldTemplate("Comment",FieldType.String,45,256),
                new FieldTemplate("InvoicePrintInd",FieldType.String,301,1)
            });
            parser.ParseAndWriteLines(dealComent, @"W:\ASDA\ETL$\Deals\Decompressed\DEALCOMENT06052018.dat", @"W:\ASDA\ETL$\Deals\Decompressed\DEALCOMENT06052018.txt", chunkSize: 250000);
            
            
            LineTemplate vdrDealData = new LineTemplate(892, "VDRDEALDTA");
            vdrDealData.AddFieldTemplates(new List<FieldTemplate>()
            {
                new FieldTemplate("DealID",FieldType.BinaryNum,0,4),
                new FieldTemplate("DealDiscCode",FieldType.BinaryNum,4,2),
                new FieldTemplate("VendorID",FieldType.BinaryNum,6,4),
                new FieldTemplate("VendorDeptNo",FieldType.BinaryNum,10,2),
                new FieldTemplate("VendorSeqNo",FieldType.BinaryNum,12,2),
                new FieldTemplate("ContactName",FieldType.String,14,40),
                new FieldTemplate("TelNo",FieldType.String,54,20),
                new FieldTemplate("FaxNo",FieldType.String,74,20),
                new FieldTemplate("Email",FieldType.String,95,80),
                new FieldTemplate("BillDate",FieldType.DateString,176,10),
                new FieldTemplate("TermDueDayQty",FieldType.BinaryNum,187,2),
                new FieldTemplate("ExchRateCode",FieldType.BinaryNum,189,2),
                new FieldTemplate("ExchRate",FieldType.Packed,191,5,5),
                new FieldTemplate("Curr",FieldType.String,197,3),
                new FieldTemplate("BuyerName",FieldType.String,200,40),
                new FieldTemplate("BuyerTel",FieldType.String,240,18),
                new FieldTemplate("Autorenew",FieldType.String,258,1),
                new FieldTemplate("TotalAllowAmount",FieldType.Packed,259,8,2),
                new FieldTemplate("BaseDivNo",FieldType.BinaryNum,267,2),
                new FieldTemplate("SAMESCatNo",FieldType.BinaryNum,269,2),
                new FieldTemplate("BuyerEmailAddress",FieldType.String,272,80),
                new FieldTemplate("DiscTypeCode",FieldType.BinaryNum,353,2),
                new FieldTemplate("DealStatusCode",FieldType.BinaryNum,355,2),
                new FieldTemplate("PayMethod",FieldType.BinaryNum,357,2),
                new FieldTemplate("StoreAllocate",FieldType.String,359,1),
                new FieldTemplate("DealCreateDate",FieldType.DateString,360,10),
                new FieldTemplate("RetailLinkInd",FieldType.String,370,1),
                new FieldTemplate("F14ReportCode",FieldType.String,371,2),
                new FieldTemplate("AcctDivNo",FieldType.BinaryNum,373,2),
                new FieldTemplate("VendorDealEffectiveDate",FieldType.DateString,375,10),
                new FieldTemplate("VendorDealExpDate",FieldType.DateString,385,10),
                new FieldTemplate("DealStatusTimeStamp",FieldType.String,395,26),
                new FieldTemplate("DealStatusUserID",FieldType.String,421,10),
                new FieldTemplate("LevelTypeCode",FieldType.BinaryNum,431,2),
                new FieldTemplate("ParentDealID",FieldType.BinaryNum,433,4),
                new FieldTemplate("FollowOnDealID",FieldType.BinaryNum,438,4),
                new FieldTemplate("Unknown",FieldType.BinaryNum,443,4),
                new FieldTemplate("GeneralComment",FieldType.String,445,255),
                new FieldTemplate("StatCondCode",FieldType.BinaryNum,701,2),
                new FieldTemplate("AsdaBrandIn",FieldType.String,704,1),
                new FieldTemplate("MDSECatNo",FieldType.BinaryNum,706,2),
                new FieldTemplate("ApproveStatusCD",FieldType.BinaryNum,709,2),
                new FieldTemplate("NetxBillDate",FieldType.DateString,712,10),
                new FieldTemplate("AccumDealAmnt",FieldType.Packed,723,9,0),
                new FieldTemplate("TaxItemClasscode",FieldType.BinaryNum,733,2),
                new FieldTemplate("MarkupVal",FieldType.Packed,736,8),
                new FieldTemplate("MarkupFMTCD",FieldType.BinaryNum,745,2),
                new FieldTemplate("BrokerFeeValue",FieldType.Packed,748,8,0),
                new FieldTemplate("BrokerFeeFTMCD",FieldType.BinaryNum,757,2),
                new FieldTemplate("ReplaceDealID",FieldType.BinaryNum,760,4),
                new FieldTemplate("TaxPercent",FieldType.BinaryNum,765,2),
                new FieldTemplate("AllStoresInd",FieldType.String,769,1),
                new FieldTemplate("CoOpDealCode",FieldType.String,770,1),
                new FieldTemplate("Unknown_Field73",FieldType.String,771,1),
                new FieldTemplate("CoopPymtFreq_CD",FieldType.BinaryNum,772,2),
                new FieldTemplate("PRCPROTNDCAMT",FieldType.Packed,775,8,0)
            });

            parser.ParseAndWriteLines(vdrDealData, @"W:\ASDA\ETL$\Deals\Decompressed\VDRDEALDTA06052018.dat", @"W:\ASDA\ETL$\Deals\Decompressed\VDRDEALDTA06052018.txt", chunkSize:100000);
            
            LineTemplate dealItmData = new LineTemplate(131, "DEALITMDTA");
            dealItmData.AddFieldTemplates(new List<FieldTemplate>()
            {
                new FieldTemplate("DealID",FieldType.BinaryNum,0,4),
                new FieldTemplate("ItemNo",FieldType.BinaryNum,4,4),
                new FieldTemplate("MRKDISCCHRGFMT",FieldType.Packed,8,8,0),
                new FieldTemplate("BROKERFEEVALUE",FieldType.String,16,2),
                new FieldTemplate("BRKDISCCHRGFMT",FieldType.Packed,18,8,0),
                new FieldTemplate("BRACKETPRICEAMT",FieldType.String,26,2),
                new FieldTemplate("TAXPERCENT",FieldType.Packed,28,5,2),
                new FieldTemplate("ITEMOVERIDEIND",FieldType.String,36,1),
                new FieldTemplate("INCLUDEITEMIND",FieldType.String,42,1),
                new FieldTemplate("UOMFRIENDLYIND",FieldType.String,43,1),
                new FieldTemplate("ITEMINCLUDEDDATE",FieldType.DateString,44,10),
                new FieldTemplate("ITEMEXCLUDEDDATE",FieldType.DateString,54,10),
                new FieldTemplate("STATUSCONDCODE",FieldType.BinaryNum,72,2),
                new FieldTemplate("TAXITEMCLASCODE",FieldType.String,74,2),
                new FieldTemplate("Field18",FieldType.String,75,2),
                new FieldTemplate("ItemDesc",FieldType.String,77,25)
            });
            parser.ParseAndWriteLines(dealItmData, @"W:\ASDA\ETL$\Deals\Decompressed\DEALITMDAT06052018.dat", @"W:\ASDA\ETL$\Deals\Decompressed\DEALITMDAT06052018.txt",chunkSize:250000);
            */
        }
    }
}
