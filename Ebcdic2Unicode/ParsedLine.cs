using Ebcdic2Unicode.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Ebcdic2Unicode
{
    public class ParsedLine
    {
        public LineTemplate Template
        {
            get;
        }

        public Dictionary<string, ParsedField> ParsedFields
        {
            get;
        }

        public string this[string fieldName]
        {
            get
            {
                return this.ParsedFields[fieldName].Text.Trim();
            }
        }


        //Constructor
        public ParsedLine(LineTemplate template, byte[] lineBytes)
        {
            this.ParsedFields = new Dictionary<string, ParsedField>(template.FieldsCount);
            this.Template = template;
            this.ParseLine(lineBytes, template);
        }


        private void ParseLine(byte[] lineBytes, LineTemplate lineTemplate)
        {
            this.ValidateInputParameters(lineBytes, lineTemplate);

            foreach (var fieldTemplate in lineTemplate.FieldTemplates)
            {
                var parsedField = new ParsedField(lineBytes, lineTemplate.FieldTemplates[fieldTemplate.Key]);
                this.ParsedFields.Add(fieldTemplate.Key, parsedField);
            }
        }

        private void ValidateInputParameters(byte[] lineBytes, LineTemplate template)
        {
            if (lineBytes == null)
            {
                throw new ArgumentNullException(Messages.LineBytesRequired);
            }
            if (template == null)
            {
                throw new ArgumentNullException(Messages.LineTemplateRequired);
            }
            if (template.FieldsCount == 0)
            {
                throw new Exception(Messages.FieldTemplatesNotDefined);
            }
        }

        public string[] GetFieldValues(bool addQuotes, string quoteCharacter)
        {
            var output = ParsedFields.Values
                .Select(f => string.Format("{1}{0}{1}", f.Text.Replace(quoteCharacter, "").Replace("\r","").Replace("\n"," ").Trim(), addQuotes ? quoteCharacter : string.Empty))
                .ToArray();

            return output;
        }

        public string GetField(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new Exception(Messages.InvalidFieldName);
            }
            return this.ParsedFields[fieldName].Text.Trim();
        }

        public string GetField(string fieldName, bool surroundWithQuotes, string quoteCharacter = "\"")
        {
            return $"{quoteCharacter}{this.GetField(fieldName)}{quoteCharacter}";
        }

        public string GetField(string fieldName, bool surroundWithQuotes, int paddedLength, string quoteCharacter = "\"")
        {
            if (paddedLength < 0)
            {
                paddedLength = 0;
            }

            if (surroundWithQuotes)
            {
                return $"{quoteCharacter}{this.GetField(fieldName).PadRight(paddedLength)}{quoteCharacter}";
            }
            else
            {
                return this.GetField(fieldName).PadRight(paddedLength);
            }
        }

        public XElement ToXml(bool includeSrcBytesInHex = false)
        {
            var lineEl = new XElement(Fields.XmlLine);
            lineEl.Add(new XAttribute(Fields.TemplateName, this.Template.LineTemplateName));

            var fields = new XElement(Fields.XmlFields);
            lineEl.Add(fields);

            foreach (var parsedField in this.ParsedFields.Values)
            {
                fields.Add(parsedField.ToXml(includeSrcBytesInHex));
            }

            return lineEl;
        }

        public string ToXmlString()
        {
            return this.ToXml().ToString();
        }

        public string ToCsvString(bool addQuotes = true, char separator = ',', char quoteCharacter = '"')
        {
            var sb = new StringBuilder();
            bool addSeparator = false;

            foreach (var parsedField in this.ParsedFields.Values)
            {
                if (addSeparator)
                {
                    sb.Append(separator);
                }
                if (addQuotes)
                {
                    sb.Append(quoteCharacter);
                }

                sb.Append(parsedField.Text.Trim());

                if (addQuotes)
                {
                    sb.Append(quoteCharacter);
                }

                addSeparator = true;
            }
            return sb.ToString();
        }
    }
}
