using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Labyrinth.Services.WorldBuilding
    {
    internal class WorldValidator
        {
        public void Validate(string xml, string pathToXsd)
            {
            var validationErrors = new List<string>();

            using (Stream schema = File.OpenRead(pathToXsd))
                {
                using (var sr = new StringReader(xml))
                    {
                    using (var schemaReader = new XmlTextReader(schema))
                        {
                        var readerSettings = GetSettings(schemaReader, ValidationCallBack);
                        var vr = XmlReader.Create(sr, readerSettings);

                        while (vr.Read())
                            {
                            }
                        }
                    }
                }

            if (validationErrors.Count == 0)
                return;

            var message = string.Join("\r\n", validationErrors);
            throw new FormatException($"Xml data is invalid:\r\n{message}");

            void ValidationCallBack(object? sender, ValidationEventArgs args) => validationErrors.Add(args.Message);
            }

        private static XmlReaderSettings GetSettings(XmlReader schemaReader, ValidationEventHandler validationCallBack)
            {
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
            readerSettings.ValidationEventHandler += validationCallBack;
            
            return readerSettings;
            }
        }
    }
