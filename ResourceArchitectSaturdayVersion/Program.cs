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
    enum Language
    {
        Fra,
        Ger,
        Que
    }
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

            foreach (var lang in Enum.GetValues(typeof(Language)).Cast<Language>())
            {
                M(kvps, lang);
            }

            Console.WriteLine("Finished");
        }

        static void M(IEnumerable<KeyValuePair> kvps, Language language)
        {
            foreach (var kvp in kvps)
            {
                var fileName = GetFileName(language, kvp.OriginalResFileName);

                var entries = new List<DictionaryEntry>();

                using (var resourceReader = new ResXResourceReader(fileName))
                {
                    entries = resourceReader.Cast<DictionaryEntry>().ToList();
                    var existingResource = entries.Where(r => r.Key.ToString() == kvp.Key).FirstOrDefault();
                    var kvpValue = GetValue(kvp, language);
                    if (existingResource.Key == null && existingResource.Value == null)
                    {
                        entries.Add(new DictionaryEntry() { Key = kvp.Key, Value = GetValue(kvp, language) });
                    }
                    //else if(existingResource.Value.ToString()!=kvpValue)
                    //{
                    //    var modifiedResx = new DictionaryEntry() { Key = existingResource.Key, Value = kvpValue };
                    //    entries.Remove(existingResource);
                    //    entries.Add(modifiedResx);
                    //}
                }

                using (var writer = new ResXResourceWriter(fileName))
                {
                    entries.ForEach(r =>
                    {
                        writer.AddResource(r.Key.ToString(), r.Value.ToString());
                    });
                    writer.Generate();
                }
            }
        }

        static string GetFileName(Language language, string original)
        {
            string res = "";
            switch (language)
            {
                case Language.Fra:
                    res = original.Replace(".resx", ".fr-FR.resx");
                    break;
                case Language.Ger:
                    res = original.Replace(".resx", ".de.resx");
                    break;
                case Language.Que:
                    res = original.Replace(".resx", ".fr-CA.resx");
                    break;                   
            }

            if (string.IsNullOrEmpty(res))
            {
                throw new ArgumentException();
            }
            return res;
        }

        static string GetValue(KeyValuePair kvp, Language language)
        {

            string res = "";
            switch (language)
            {
                case Language.Fra:
                    res = kvp.FraValue;
                    break;
                case Language.Ger:
                    res = kvp.GerValue;
                    break;
                case Language.Que:
                    res = kvp.QueValue;
                    break;
            }
            if (string.IsNullOrEmpty(res))
            {
                throw new ArgumentException();
            }
            return res;
        }
    }
}
