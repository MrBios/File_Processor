using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Настолка
{
    abstract class abstractFileProcessing
    {
        /* Правила создания новых классов-обработчиков:
         * 
         * 1. Наследовать от этого класса с реализацией через override метода ProcessFile (сама обработка данных)
         * 2. В ProcessFile вести процент выполнения кода, записывая число (максимум 100) в Process
         * 3. В Process записывать значения через Application.Current.Dispatcher.Invoke(() => { Process = value; });
         * 4. В конце выполнения обработки данных в методе ProcessFile итоговое значения данных (для записи в файл) записать в переменную _result.value в формате byte[]
         * 5. Последней строчкой в методе ProcessFile указать _ready = true;
         * 6. Добавить класс в MainWindow.xaml.cs в список классов обработчиков (начало namespace) */

        private protected int _process;
        public event Action<int> ValueChanged;
        private protected bool _ready = false;
        private protected string _path = "";
        private protected ResultFile _result = new ResultFile();

        public abstractFileProcessing(string name, string extension)
        {
            _result.name = name;
            _result.ext = extension;
        }

        public int Process
        {
            get => _process;
            set
            {
                ValueChanged?.Invoke(value);
                _process = value;
            }
        }

        public abstract void ProcessFile();

        public ResultFile getResult() => _result;
        public bool isReady() => _ready;
        public void setPath(string path) => _path = path;
    }
}
