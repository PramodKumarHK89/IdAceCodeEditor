using Microsoft.Graph;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace IdAceCodeEditor
{
    public class ReplaceService
    {      
        public bool ReplaceManualSettings(string clonedPath, List<ReplacementField> replacementFields)
        {
            foreach (var item in replacementFields)
            {

                if (item.ReplacementType.Equals("JSON", StringComparison.InvariantCultureIgnoreCase))
                {

                    var obj = UpdateJsonwithvalues(Path.Combine(clonedPath, item.FileName),
                                                item.Section,                           
                                                item.Destination,
                                                item.Value);
                }
                else if (item.ReplacementType.Equals("TXT", StringComparison.InvariantCultureIgnoreCase))
                {
                    FindAndReplace(Path.Combine(clonedPath, item.FileName),
                                                   item.Destination,
                                                   item.Value);
                }
            }
            return true;
        }
        private void FindAndReplace(string fileName,
                                   string Dest,
                                   string value)
        {
            string text = System.IO.File.ReadAllText(fileName);
            text = text.Replace(Dest, value);
            System.IO.File.WriteAllText(fileName, text);
        }
        private bool UpdateJsonwithvalues(string jsonFileName,
                                                      string section,
                                                      string dest,
                                                      string value)
        {
            string jsonString = System.IO.File.ReadAllText(jsonFileName);


            JObject? jObject = JsonConvert.DeserializeObject<JObject>(jsonString);

            if (jObject?[section] is JObject p)
            {
                p[dest] = value;
            }
            else
                jObject[dest] = value;

            string newJson = JsonConvert.SerializeObject(jObject);


            var writerOptions = new JsonWriterOptions
            {
                Indented = true
            };

            var documentOptions = new JsonDocumentOptions
            {
                CommentHandling = JsonCommentHandling.Skip
            };

            using FileStream fs = System.IO.File.Create(jsonFileName);
            using var writer = new Utf8JsonWriter(fs, options: writerOptions);
            using JsonDocument document = JsonDocument.Parse(newJson, documentOptions);

            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Object)
            {
                writer.WriteStartObject();
            }
            foreach (JsonProperty property in root.EnumerateObject())
            {
                property.WriteTo(writer);
            }

            writer.WriteEndObject();

            writer.Flush();
            return true;
        }
    }
}
