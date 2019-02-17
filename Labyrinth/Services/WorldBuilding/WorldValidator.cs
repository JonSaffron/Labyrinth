using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Labyrinth.Services.WorldBuilding
    {
    internal class WorldValidator
        {
        private readonly List<string> _validationErrors;

        public WorldValidator()
            {
            this._validationErrors = new List<string>();
            }

        public void Validate(string xml, string pathToXsd)
            {
            this._validationErrors.Clear();

            using (Stream schema = File.OpenRead(pathToXsd))
                {
                using (var sr = new StringReader(xml))
                    {
                    var readerSettings = GetSettings(schema);
                    var vr = XmlReader.Create(sr, readerSettings);
                    
                    while (vr.Read())
                        {
                        }
                    }
                }

            if (this._validationErrors.Count == 0)
                return;

            var strings = new string[this._validationErrors.Count];
            this._validationErrors.CopyTo(strings, 0);
            var message = String.Join("\r\n", strings);
            throw new FormatException("Xml data is invalid:\r\n" + message);
            }

        private XmlReaderSettings GetSettings(Stream schema)
            {
            XmlReader schemaReader = new XmlTextReader(schema);

            var readerSettings = new XmlReaderSettings
                                        {
                                            CheckCharacters = true,
                                            ConformanceLevel = ConformanceLevel.Document,
                                            IgnoreComments = true,
                                            IgnoreProcessingInstructions = false,
                                            IgnoreWhitespace = false,
                                            ValidationType = ValidationType.Schema
                                        };
            readerSettings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
            readerSettings.Schemas.Add(null, schemaReader);
            readerSettings.ValidationEventHandler += ValidationCallBack;

            return readerSettings;
            }

        private void ValidationCallBack(object sender, ValidationEventArgs e)
            {
            _validationErrors.Add(e.Message);
            }
        }
    }
