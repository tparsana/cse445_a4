using System;
using System.Xml.Schema;
using System.Xml;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 **/

namespace ConsoleApp1
{
    public class Program
    {
        // Q1.1, Q1.2, Q1.3 -> Updated with your GitHub Pages links
        public static string xmlURL = "https://tparsana.github.io/cse445_a4/Hotels.xml";        // 10 hotels
        public static string xmlErrorURL = "https://tparsana.github.io/cse445_a4/HotelsErrors.xml"; 
        public static string xsdURL = "https://tparsana.github.io/cse445_a4/Hotels.xsd";

        public static void Main(string[] args)
        {
            // Q3 (1): Validate correct XML
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);  // "No Error" if valid

            // Q3 (2): Validate error XML
            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);  // Should list the 5 mutations

            // Q3 (3): Convert correct XML to JSON
            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1 - Validates the XML file against the XSD
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings
                {
                    DtdProcessing = DtdProcessing.Ignore,
                    XmlResolver = null,
                    ValidationType = ValidationType.Schema
                };
                settings.Schemas.Add(null, xsdUrl);

                string errors = "";
                settings.ValidationEventHandler += (sender, e) =>
                {
                    errors += $"Line {e.Exception.LineNumber}, Pos {e.Exception.LinePosition}: {e.Message}\n";
                };

                // Read entire document for validation
                using (XmlReader reader = XmlReader.Create(xmlUrl, settings))
                {
                    while (reader.Read()) { }
                }

                return string.IsNullOrEmpty(errors) ? "No Error" : errors.Trim();
            }
            catch (Exception ex)
            {
                return $"Exception: {ex.Message}";
            }
        }

        // Q2.2 - Converts valid XML into JSON
        public static string Xml2Json(string xmlUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(xmlUrl);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    string xmlContent = reader.ReadToEnd();

                    // Fix unescaped '&' or other invalid entity references
                    xmlContent = Regex.Replace(xmlContent, "&(?!amp;|lt;|gt;|quot;|apos;|#\\d+;)", "&amp;");

                    // Load into XmlDocument
                    XmlDocument doc = new XmlDocument();
                    XmlReaderSettings rdSettings = new XmlReaderSettings
                    {
                        DtdProcessing = DtdProcessing.Ignore,
                        XmlResolver = null
                    };

                    using (StringReader sr = new StringReader(xmlContent))
                    using (XmlReader xmlReader = XmlReader.Create(sr, rdSettings))
                    {
                        doc.Load(xmlReader);
                    }

                    // Convert to JSON. Third param 'true' => attributes appear as '@hotelID', etc.
                    string jsonText = JsonConvert.SerializeXmlNode(doc, Formatting.Indented, true);
                    return jsonText;
                }
            }
            catch (Exception ex)
            {
                return $"Exception during XML to JSON conversion: {ex.Message}";
            }
        }
    }
}
