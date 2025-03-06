using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Настолка.ProcessingMethods
{
    internal class toTextFileProcessing : abstractFileProcessing
    {
        public toTextFileProcessing(string name, string extension) : base(name, extension) { }

        public override void ProcessFile()
        {
            string textResult = "";
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

            for (int i = 0; i < persons.Count; i++)
            {
                textResult += $"Name: {persons[i].name}\nSurname: {persons[i].surname}\n\n";
                Application.Current.Dispatcher.Invoke(() => { Process = (i * 50 / persons.Count) + 50; });

                Thread.Sleep(10);
            }
            _result.value = Encoding.UTF8.GetBytes(textResult);
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

