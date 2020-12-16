﻿using Ebcdic2Unicode.Constants;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace Ebcdic2Unicode
{
    public class EbcdicParser
    {
        public ParsedLine[] ParsedLines { get; protected set; }

        #region Constructors

        //Constractor 1
        public EbcdicParser()
        {
            //Empty
        }

        //Constractor 2
        public EbcdicParser(string sourceFilePath, LineTemplate lineTemplate)
            : this(File.ReadAllBytes(sourceFilePath), lineTemplate)
        {
            //Read all file bytes and call 3rd constructor
        }

        //Constractor 3
        public EbcdicParser(byte[] allBytes, LineTemplate lineTemplate)
        {
            this.ParsedLines = this.ParseAllLines(lineTemplate, allBytes);
        }
        #endregion


        /// <summary>
        /// Parses multiple lines of binary data.
        /// </summary>
        /// <param name="lineTemplate">Template</param>
        /// <param name="allBytes">Source file bytes</param>
        /// <returns>Array of parsed lines</returns>
        public ParsedLine[] ParseAllLines(LineTemplate lineTemplate, byte[] allBytes)
        {
            Console.WriteLine("{0}: Parsing...", DateTime.Now);
            this.ValidateInputParameters(lineTemplate, allBytes, false);

            double expectedRows = (double)allBytes.Length / lineTemplate.LineSize;
            if (expectedRows % 1 == 0)
            {
                Console.WriteLine("{1}: Line count est {0:#,###.00}", expectedRows, DateTime.Now);
            }

            byte[] lineBytes = new byte[lineTemplate.LineSize];
            ParsedLine[] linesList = new ParsedLine[Convert.ToInt32(expectedRows)];
            ParsedLine parsedLine = null;
            int lineIndex = 0;

            for (int i = 0; i < allBytes.Length; i += lineTemplate.LineSize)
            {
                try
                {
                    Array.Copy(allBytes, i, lineBytes, 0, lineTemplate.LineSize);

                    parsedLine = this.ParseSingleLine(lineTemplate, lineBytes);

                    if (parsedLine != null)
                    {
                        linesList[lineIndex] = parsedLine;
                    }

                    lineIndex++;

                    if (lineIndex % 1000 == 0)
                    {
                        Console.Write(lineIndex + "\r");
                    }
                }
                catch (Exception ex)
                {
                    //Used for dubugging 
                    Console.WriteLine("Exception at line index {0}", lineIndex);
                    throw ex;
                }
            }
            Console.WriteLine("{1}: {0} line(s) have been parsed", linesList.Count(), DateTime.Now);
            return linesList;
        }

        /// <summary>
        /// Parses multiple lines of binary data.
        /// </summary>
        /// <param name="lineTemplate">Template</param>
        /// <param name="sourceFilePath">Source file path</param>
        /// <returns>Array of parsed lines</returns>
        public ParsedLine[] ParseAllLines(LineTemplate lineTemplate, string sourceFilePath)
        {
            Console.WriteLine("{1}: Reading {2}...", sourceFilePath, DateTime.Now);
            return this.ParseAllLines(lineTemplate, File.ReadAllBytes(sourceFilePath));
        }

        /// <summary>
        /// Parses single line of binary data.
        /// </summary>
        /// <param name="lineTemplate">Template</param>
        /// <param name="lineBytes">Source bytes</param>
        /// <returns>Single parsed line</returns>
        public ParsedLine ParseSingleLine(LineTemplate lineTemplate, byte[] lineBytes)
        {
            bool isSingleLine = true;
            this.ValidateInputParameters(lineTemplate, lineBytes, isSingleLine);
            ParsedLine parsedLine = new ParsedLine(lineTemplate, lineBytes);
            return parsedLine;
        }

        public ParsedLine ParseAndAddSingleLine(LineTemplate lineTemplate, byte[] lineBytes, int flushThreshhold)
        {
            bool isSingleLine = true;
            this.ValidateInputParameters(lineTemplate, lineBytes, isSingleLine);
            ParsedLine parsedLine = new ParsedLine(lineTemplate, lineBytes);
            ParsedLine[] parsed = this.ParsedLines;
            if (parsed == null)
            {
                parsed = new ParsedLine[1];
            }
            else
            {
                if (parsed.Length == flushThreshhold)
                {
                    Array.Resize(ref parsed, 1);
                }
                else
                {
                    Array.Resize(ref parsed, (parsed == null ? 0 : parsed.Length) + 1);
                }
            }
            parsed[parsed.Length -1] = parsedLine;
            this.ParsedLines = parsed;
            return parsedLine;
        }

        private bool ValidateInputParameters(LineTemplate lineTemplate, byte[] allBytes, bool isSingleLine)
        {
            if (allBytes == null)
            {
                throw new ArgumentNullException(Messages.DataNotProvided);
            }
            if (lineTemplate == null)
            {
                throw new ArgumentNullException(Messages.LineTemplateNotProvided);
            }
            if (lineTemplate.FieldsCount == 0)
            {
                throw new Exception(Messages.LineTemplateHasNoFields);
            }
            if (allBytes.Length > 0 && allBytes.Length < lineTemplate.LineSize)
            {
                throw new Exception(Messages.DataShorterThanExpected);
            }
            if (isSingleLine && allBytes.Length != lineTemplate.LineSize)
            {
                throw new Exception(Messages.DataLengthDifferentThanExpected);
            }

            double expectedRows = (double)allBytes.Length / lineTemplate.LineSize;

            if (expectedRows % 1 != 0) //Expected number of rows is not a whole number
            {
                string errMsg = String.Format(Messages.ExpectedNumberOfRows, allBytes.Length, lineTemplate.LineSize, expectedRows);
                throw new Exception(errMsg);
            }

            return true;
        }

        public bool SaveParsedLinesAsCsvFile(string outputFilePath, bool includeColumnNames = true, bool addQuotes = true, bool append = false)
        {
            return ParserUtilities.WriteParsedLineArrayToCsv(this.ParsedLines, outputFilePath, includeColumnNames, addQuotes, false);
        }

        public bool SaveParsedLinesAsTxtFile(string outputFilePath, string delimiter = "\t", bool includeColumnNames = true, bool addQuotes = true, string quoteCharacter = "\"", bool append = false)
        {
            return ParserUtilities.WriteParsedLineArrayToTxt(this.ParsedLines, outputFilePath, delimiter, includeColumnNames, addQuotes, quoteCharacter, append);
        }

        public bool SaveParsedLinesAsXmlFile(string outputFilePath)
        {
            return ParserUtilities.WriteParsedLineArrayToXml(this.ParsedLines, outputFilePath);
        }
    }
}
