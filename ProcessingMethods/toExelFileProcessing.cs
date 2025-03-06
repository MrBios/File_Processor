using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OfficeOpenXml;

namespace Настолка.ProcessingMethods
{
    internal class toExelFileProcessing : abstractFileProcessing
    {
        public toExelFileProcessing(string name, string extension) : base(name, extension) { }

        public override void ProcessFile()
        {
            List<Person> persons = new List<Person>();

            using (StreamReader sr = new StreamReader(_path))
            {
                string[] strings = sr.ReadToEnd().Split('\n');
                for (int i = 0; i < strings.Length; i += 2)
                {
                    persons.Add(new Person(strings[i], strings[i + 1]));
                    Application.Current.Dispatcher.Invoke(() => { Process = i * 50 / strings.Length; });
                    Thread.Sleep(10);
                }
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var package = new ExcelPackage();

            var worksheet = package.Workbook.Worksheets.Add("Persons");

            worksheet.Cells[1, 1].Value = "Name";
            worksheet.Cells[1, 2].Value = "Surname";

            for(int i = 0;  i < persons.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = persons[i].name;
                worksheet.Cells[i + 2, 2].Value = persons[i].surname;
                Application.Current.Dispatcher.Invoke(() => { Process = (i * 50 / persons.Count + 50); });
                Thread.Sleep(10);
            }

            _result.value = package.GetAsByteArray();
            _ready = true;
        }

        private class Person
        {
            public string name;
            public string surname;

            public Person(string name, string surname)
            {
                this.name = name;
                this.surname = surname;
            }
        }
    }
}
