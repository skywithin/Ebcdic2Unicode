﻿using Ebcdic2Unicode.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ebcdic2Unicode
{
    public class LineTemplate
    {
        public Dictionary<string, FieldTemplate> FieldTemplates = new Dictionary<string, FieldTemplate>();

        public string LineTemplateName { get; private set; }

        public int LineSize { get; protected set; }

        public int FieldsCount
        {
            get
            {
                return FieldTemplates.Count;
            }
        }

        //Constructor 1
        public LineTemplate(int lineSize, string templateName = "")
        {
            this.PopulateInitialObjectProperties(lineSize, templateName);
        }

        //Constructor 2
        public LineTemplate(string lineTemplateXmlPath)
        {
            using (TextReader tr = new StreamReader(lineTemplateXmlPath))
            {
                XElement lineTemplateXml = XElement.Load(tr);
                this.InitializeLineTemplateFromXml(lineTemplateXml);
            }
        }

        //Constructor 3
        public LineTemplate(XElement lineTemplateXml)
        {
            this.InitializeLineTemplateFromXml(lineTemplateXml);
        }


        private void PopulateInitialObjectProperties(int lineSize, string templateName)
        {
            //Console.WriteLine("Initiaising template '{0}'...", templateName);
            if (lineSize <= 0)
            {
                throw new ArgumentOutOfRangeException(Messages.LineLengthTooShort);
            }
            this.LineTemplateName = templateName;
            this.LineSize = lineSize;
        }

        private void InitializeLineTemplateFromXml(XElement lineTemplaeXml)
        {
            //Input XML:
            //<lineTemplate Name="RNA_RecType01" Length="1190">
            //  <fields>
            //    <fieldTemplate Name="RecordType" Type="AlphaNum" StartPosition="13" Size="2" DecimalPlaces="0" />
            //    <fieldTemplate Name="SourceInd" Type="AlphaNum" StartPosition="15" Size="1" DecimalPlaces="0" />
            //  </fields>
            //</lineTemplate>

            int lineSize = ParserUtilities.GetAttributeNumericValue(lineTemplaeXml, Fields.Length);
            string templateName = ParserUtilities.GetCompulsoryAttributeValue(lineTemplaeXml, Fields.Name);
            this.PopulateInitialObjectProperties(lineSize, templateName);

            foreach (XElement fieldXml in lineTemplaeXml.Element(Fields.XmlFields).Elements(Fields.XmlFieldTemplate))
            {
                FieldTemplate fieldTemplate = new FieldTemplate(fieldXml);
                this.AddFieldTemplate(fieldTemplate);
            }
        }

        public void AddFieldTemplate(FieldTemplate fieldTemplate)
        {
            if ((fieldTemplate.StartPosition + fieldTemplate.FieldSize) > this.LineSize)
            {
                throw new Exception(String.Format(Messages.FieldExceedsLineBoundary, fieldTemplate.FieldName));
            }
            this.FieldTemplates.Add(fieldTemplate.FieldName, fieldTemplate);
        }

        public void AddFieldTemplates(List<FieldTemplate> fieldTemplates)
        {
            fieldTemplates.ForEach(t =>
            {
                if ((t.StartPosition + t.FieldSize) > this.LineSize)
                {
                    throw new Exception(String.Format(Messages.FieldExceedsLineBoundary, t.FieldName));
                }
                this.FieldTemplates.Add(t.FieldName, t);
            });
        }

        public string[] GetFieldNames(bool addQuotes, string quoteCharacter)
        {
            var query = (from f in FieldTemplates.Values
                         select String.Format("{1}{0}{1}", f.FieldName, addQuotes ? quoteCharacter : "")).ToArray();

            return query;
        }

        public XElement GetLineTemplateXml()
        {
            //Output XML:
            //<lineTemplate Name="RNA_RecType01" Length="1190">
            //  <fields>
            //    <fieldTemplate Name="RecordType" Type="AlphaNum" StartPosition="13" Size="2" DecimalPlaces="0" />
            //    <fieldTemplate Name="SourceInd" Type="AlphaNum" StartPosition="15" Size="1" DecimalPlaces="0" />
            //  </fields>
            //</lineTemplate>

            string templateName;
            if (String.IsNullOrWhiteSpace(this.LineTemplateName))
            {
                templateName = "No_Name";
            }
            else
            {
                templateName = this.LineTemplateName;
            }

            XElement lineXml = new XElement(Fields.XmlLineTemplate);
            lineXml.Add(new XAttribute(Fields.Name, templateName));
            lineXml.Add(new XAttribute(Fields.Length, this.LineSize));

            XElement fields = new XElement(Fields.XmlFields);
            lineXml.Add(fields);

            foreach (FieldTemplate field in FieldTemplates.Values)
            {
                XElement fieldXml = field.GetFieldTemplateXml();
                fields.Add(fieldXml);
            }

            return lineXml;
        }

        public string GetLineTemplateXmlString()
        {
            return GetLineTemplateXml().ToString();
        }

        public void SaveLineTemplateXml(string outputXmlFilePath)
        {
            if (File.Exists(outputXmlFilePath))
            {
                throw new Exception(String.Format(Messages.FileAlreadyExists, outputXmlFilePath));
            }

            XElement templateXml = this.GetLineTemplateXml();

            using (StreamWriter sw = new StreamWriter(outputXmlFilePath))
            {
                templateXml.Save(sw);
            }

            Console.WriteLine("XML template created: {0}", outputXmlFilePath);
        }
    }
}
