using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Настолка.ProcessingMethods;

namespace Настолка
{
    public partial class MainWindow : Window
    {
        // Настройки

        /* Расширение входного файла
         *
         * Если программа принимает 1 тип файлов - "Название типа входного файла|*.расширение" (пример "Text File|*.txt")
         * Если программа принимает несколько типов файлов - "Название 1 типа входного файла|*.1 расширение|Название 2 типа входного файла|*.2 расширение" и до бесконечности (пример "Text File|*.txt|Exel File|*.xlsx" */
        const string inFileExt = "Text File|*.txt";

        /* Список экземпляров классов-обработчиков
         * 
         * в формате new названиеКлассаОбработчика("название выходного файла", "Название типа выходного файла|*.расширение") */
        abstractFileProcessing[] processingList = {
            new toTextFileProcessing("textResultFile", "Text File|*.txt"),
            new toExelFileProcessing("exelResultFile", "Exel File|*.xlsx")
        };


        OpenFileDialog openFileDialog = new OpenFileDialog();
        List<ResultFile> resultFiles = new List<ResultFile>();
        int processingCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            this.Icon = Imaging.CreateBitmapSourceFromHIcon( Properties.Resources.icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions() );
            openFileDialog.Filter = inFileExt;
            openFileDialog.Multiselect = false;
        }

        void processValueChanged(int value)
        {
            Loading_progressBar.Value = (value / processingList.Length) + (100 / processingList.Length * processingCount);
        }

        private async void Load_panel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            bool? result = openFileDialog.ShowDialog();
            if (result != null && result == true)
            {
                Load_panel.Visibility = Visibility.Collapsed;
                Loading_panel.Visibility = Visibility.Visible;
                await LoadFile(openFileDialog.FileName);
            }
        }

        private async void Load_panel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                Load_panel.Visibility = Visibility.Collapsed;
                Loading_panel.Visibility = Visibility.Visible;
                await LoadFile(files[0]);
            }
        }

        private async Task LoadFile(string path)
        {
            for (int i = 0; i < processingList.Length; i++)
            {
                processingList[i].setPath(path);
                processingList[i].ValueChanged += processValueChanged;
                await Task.Run(new Action(processingList[i].ProcessFile));
                processingList[i].ValueChanged -= processValueChanged;
                resultFiles.Add(processingList[i].getResult());
                processingCount++;
            }

            string saveInst = "Готовые файлы:\n\n";
            foreach (var i in resultFiles)
            {
                saveInst += i.name + "\n";
            }
            Save_label.Content = saveInst;

            Loading_panel.Visibility = Visibility.Collapsed;
            Save_panel.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (resultFiles.Count == 0)
            {
                Environment.Exit(0);
                return;
            }
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            List<ResultFile> notSaved = new List<ResultFile>();
            string saveInst = "Не сохраненные файлы:\n\n";

            foreach (var i in resultFiles)
            {
                saveFileDialog.Filter = i.ext;
                saveFileDialog.FileName = i.name;
                saveFileDialog.AddExtension = true;
                saveFileDialog.Title = "Сохранение " + i.name;
                bool? result = saveFileDialog.ShowDialog();
                if (result != null && result == true)
                {
                    File.WriteAllBytes(saveFileDialog.FileName, i.value);
                }
                else
                {
                    notSaved.Add(i);
                    saveInst += i.name + "\n";
                }
            }

            resultFiles = notSaved.ToArray().ToList();

            if (resultFiles.Count == 0)
            {
                Save_label.Content = "Все файлы сохранены";
                back.Background = new SolidColorBrush(Color.FromRgb(101, 224, 128));
                saveButton.Content = "Закрыть Программу";
            }
            else
                Save_label.Content = saveInst;
        }
    }
}
