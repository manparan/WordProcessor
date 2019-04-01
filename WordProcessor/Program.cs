using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WordProcessor
{
    class Program
    {
        static void Main(string[] args)
        { 
            //Input is assumed in csv format with no quotes or double quotes around fields
            var words = SplitCSV(Console.ReadLine());
            ProcessText(words);

            Console.ReadLine();
        }

        private static void ProcessText(string[] words)
        {
            var Dates = new List<DateTime>();
            var PhoneNumbers = new List<string>();
            var Numbers = new List<double>();
            var Words = new List<string>();
            foreach (var word in words)
            {
                DateTime dateTime;
                double number;
                if (DateTime.TryParse(word, out dateTime))
                {
                    Dates.Add(dateTime);
                }
                else if (IsPhoneNumber(word))
                {
                    PhoneNumbers.Add(word);
                }
                else if (double.TryParse(word, out number))
                {
                    Numbers.Add(number);
                }
                else
                {
                    Words.Add(word);
                }
            }

            Parallel.Invoke(
                () => { Dates = Dates.Distinct().OrderBy(n => n).ToList(); },
                () => { PhoneNumbers = PhoneNumbers.Distinct().OrderBy(n => n).ToList(); },
                () => { Numbers = Numbers.Distinct().OrderBy(n => n).ToList(); },
                () => { Words = Words.Distinct().OrderBy(n => n).ToList(); }
                );

            var result = JsonConvert.SerializeObject(new object[] { Dates, PhoneNumbers, Numbers, Words });
            Console.WriteLine(result);

            //Output will be saved in Result.json file in output directory
            File.WriteAllText("Result.json", result);
        }

        private static bool IsPhoneNumber(string number)
        {
            //Phone numbers formats supported by regex used:
            //(123) 456 7899
            //(123).456.7899
            //(123)-456-7899
            //123-456-7899
            //123 456 7899
            //1234567899
            return Regex.Match(number, @"\(?([0-9]{3})\)?([ .-]?)([0-9]{3})\2([0-9]{4})").Success;
        }

        private static string[] SplitCSV(string input)
        {
            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] Fields = CSVParser.Split(input);

            for (int i = 0; i < Fields.Length; i++)
            {
                Fields[i] = Fields[i].TrimStart(' ', '"');
                Fields[i] = Fields[i].TrimEnd('"');
            }

            return Fields;
        }
    }
}
