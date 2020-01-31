using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace WorldStressMapTool
{
    public class WSMCities
    {
        public WSMCities(string csvStr)
        {
            Read(csvStr);
        }

        public List<NamedCoordinate> Collection { get; set; } = new List<NamedCoordinate>();

        public void Read(string csvStr)
        {
            Collection.Clear();
            System.IO.StringReader stream = new System.IO.StringReader(csvStr);
            using (TextFieldParser parser = new TextFieldParser(stream))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    // Processing row
                    string[] fields = parser.ReadFields();

                    if (fields != null)
                    {
                        string name = fields[0];
                        double lat, lon;
                        if (!double.TryParse(fields[1], out lat)) continue;
                        if (!double.TryParse(fields[2], out lon)) continue;
                        NamedCoordinate city = new NamedCoordinate(name, lat, lon);
                        Collection.Add(city);
                    }
                }
            }
        }



    }


}