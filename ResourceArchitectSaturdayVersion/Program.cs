using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace ResourceArchitectSaturdayVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            var origilalResourceFiles = Directory.GetFiles(@"A:\Work\Source\Repos\onyx\Source\FUEL.Web.UI", "*resx", SearchOption.AllDirectories)
                         .Where(x => !x.Contains(".uk.resx")
                         && !x.Contains(".de.resx") &&
                         !x.Contains(".fr-FR.resx") &&
                         !x.Contains(".fr-CA.resx"))
                         .ToList();

            var engValues = File.ReadAllLines(@"A:\Work\eng.txt").ToList();
            var fraValues = File.ReadAllLines(@"A:\Work\fra.txt").ToList();
            var gerValues = File.ReadAllLines(@"A:\Work\ger.txt").ToList();
            var queValues = File.ReadAllLines(@"A:\Work\que.txt").ToList();

            var kvps = new List<KeyValuePair>();

            foreach (var engValue in engValues)
            {
                var index = engValues.IndexOf(engValue);

                foreach (var resFile in origilalResourceFiles)
                {
                    using (var sourceReader = new ResXResourceReader(resFile))
                    {
                        foreach (DictionaryEntry entry in sourceReader)
                        {
                            var value = entry.Value.ToString();
                            if (engValue == value)
                            {
                                var kvp = new KeyValuePair
                                {
                                    Key = entry.Key.ToString(),
                                    LineNumber = index,
                                    EngValue = engValue,
                                    FraValue = fraValues[index],
                                    GerValue = gerValues[index],
                                    QueValue = queValues[index],
                                    OriginalResFileName = resFile
                                };
                                kvps.Add(kvp);
                            }
                        }
                    }
                }
            }

            Console.WriteLine();
        }
    }
}
