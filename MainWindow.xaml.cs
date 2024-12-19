using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hoi4_file_replacer
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<string> FileCollection { get; set; } = new ObservableCollection<string>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OpenFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true, // Allow multiple file selection
                Filter = "All Files|*.*|Text Files|*.txt|Images|*.jpg;*.png;*.gif", // Example filters
                Title = "Select Files"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filePath in openFileDialog.FileNames)
                {
                    // Check if the file is already in the collection
                    if (!FileCollection.Contains(filePath))
                    {
                        FileCollection.Add(filePath);
                    }
                }
            }
        }
        private void FileSelectClick(object sender, RoutedEventArgs e) {
            OpenFiles();
        }

        private void LoadFilesContent()
        {
            foreach (string filePath in FileCollection)
            {
                try
                {
                    using(StreamReader reader  = new StreamReader(filePath))
                    {
                        List<string> fileLines = new List<string>();
                        string line;
                        string replace = CodeReplace.Text;
                        string replaceCode = "      " + ReplaceCode.Text;
                        while ((line = reader.ReadLine()) != null)
                        {
                            fileLines.Add(line);
                        }
                        for (int i = 0; i < fileLines.Count();i++)
                        {
                            if (fileLines[i].Contains(replace))
                            {
                                fileLines[i] = replaceCode;
                            }
                        }
                        reader.Close();
                        File.WriteAllLines(filePath, fileLines);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file {filePath}: {ex.Message}");
                }
            }
        }

        private void ReplaceClick (object sender, RoutedEventArgs e) {
            LoadFilesContent();
        }

        private void ReplaceMultipleClick (object sender, RoutedEventArgs e)
        {
            replaceMultipleLinesofCode();
        }

        private void replaceMultipleLinesofCode()
        {
            foreach (string filePath in FileCollection)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        List<string> fileLines = new List<string>();
                        string line;
                        bool his = false;
                        bool own = false;
                        bool cor = false;
                        bool cannotRemove = false;
                        bool vic = false;
                        int conCounter = 0;
                        string replaceCode = ReplaceCode.Text;
                        while ((line = reader.ReadLine()) != null)
                        {
                            fileLines.Add(line);
                        }
                        for (int i = 0; i < fileLines.Count(); i++)
                        {
                            if (vic)
                            {
                                cannotRemove = true;
                            }
                            else
                            {
                                cannotRemove = false;
                            }
                            if (fileLines[i].Contains("history"))
                            {
                                his = true;
                                cannotRemove = true;
                            }
                            if(his)
                            {
                                if (fileLines[i].Contains("victory_points"))
                                {
                                    vic = true;
                                    cannotRemove = true;
                                }
                                if (fileLines[i].Contains("{"))
                                {
                                    if (fileLines[i-1].Contains("history") && conCounter == 0)
                                    {
                                        cannotRemove = true;
                                    }
                                    conCounter++;
                                }
                                if (fileLines[i].Contains("}"))
                                {
                                    conCounter--;
                                    vic = false;
                                }
                                if (conCounter == 0 && !fileLines[i].Contains("history"))
                                {
                                    cannotRemove = true;
                                    his = false;
                                }
                                if (fileLines[i].Contains("owner") && !own){
                                    fileLines[i] = "      owner = " + replaceCode;
                                    own = true;
                                }
                                else if (fileLines[i].Contains("add_core_of") && !cor)
                                {
                                    fileLines[i] = "      add_core_of = " + replaceCode;
                                    cor = true;
                                }
                                else
                                {
                                    if (!cannotRemove)
                                    {
                                        fileLines.RemoveAt(i);
                                        i--;
                                    }
                                }
                            }
                        }
                        reader.Close();
                        File.WriteAllLines(filePath, fileLines);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error reading file {filePath}: {ex.Message}");
                }
            }
        }
    }
}
