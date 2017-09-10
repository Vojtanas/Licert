

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Licert
{
    public class MainViewModel : ObservableObject
    {
        private string _licenseText;
        public string LicenseText
        {
            get { return _licenseText; }
            set
            {
                if (value != _licenseText)
                {
                    _licenseText = value;
                    OnPropertyChanged("LicenseText");
                }
            }
        }

        private string _programName;
        public string ProgramName
        {
            get { return _programName; }
            set
            {
                if (value != _programName)
                {
                    _programName = value;
                    OnPropertyChanged("ProgramName");
                    LicenseText = GetLicenseText();
                }
            }
        }

        private string _selectedFolderPath;
        public string SelectedFolderPath
        {
            get { return _selectedFolderPath; }
            set
            {
                if (value != _licenseText)
                {
                    _selectedFolderPath = value;
                    OnPropertyChanged("SelectedFolderPath");
                }
            }
        }

        private ObservableCollection<SelectedFile> _selectedFiles;
        public ObservableCollection<SelectedFile> SelectedFiles
        {
            get { return _selectedFiles; }
            set
            {
                if (value != _selectedFiles)
                {
                    _selectedFiles = value;
                    OnPropertyChanged("SelectedFiles");
                }
            }
        }

        private MainWindow _window;
        public MainViewModel(MainWindow window)
        {
            _window = window;
            LicenseText = GetLicenseText();
        }



        private string GetLicenseText()
        {
            return string.Format(@"This file is part of {0}. 

{0} is free software: you can redistribute it and/or modify 
it under the terms of the GNU General Public License as published by 
the Free Software Foundation, either version 3 of the License, or 
(at your option) any later version. 

{0} is distributed in the hope that it will be useful, 
but WITHOUT ANY WARRANTY; without even the implied warranty of 
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
GNU General Public License for more details. 

You should have received a copy of the GNU General Public License 
along with {0}. If not, see <http://www.gnu.org/licenses/>.", ProgramName);
        }



        private ObservableCollection<SelectedFile> FindAllFiles(string path)
        {
            ObservableCollection<SelectedFile> list = new ObservableCollection<SelectedFile>();

            // .cs, .xaml
            var files = Directory.GetFileSystemEntries(path, "*", SearchOption.AllDirectories)
                             .Where(s => s.EndsWith(".cs", StringComparison.OrdinalIgnoreCase) || s.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase));
           
            foreach (var item in files)
            {
                list.Add(new SelectedFile(item));
            }

            return list;
        }

        #region Commands

        private CommandHandler _insertLicenseCommand;
        public CommandHandler InsertLicenseCommand
        {
            get
            {
                if (_insertLicenseCommand == null)
                {
                    _insertLicenseCommand = new CommandHandler(
                        param => InsertLicense(),
                        param => CanInsertLicenseCommand()
                    );
                }
                return _insertLicenseCommand;
            }
        }

        private bool CanInsertLicenseCommand()
        {
            return (SelectedFiles != null) && (SelectedFiles.Count > 0) ? true : false;
        }

        internal void InsertLicense()
        {
            try
            {
                string insertedLicenseCS = "/* \r\n" + LicenseText + "\r\n */ \r\n\r\n";
                string insertedLicenseXAML = "<!-- \r\n" + LicenseText + "\r\n --> \r\n\r\n";
                StringBuilder sb;

                foreach (var item in SelectedFiles.Where(x => x.IsSelected))
                {
                    sb = new StringBuilder();
                    if (item.FileName.Contains(".cs"))
                    {
                        sb.Append(insertedLicenseCS);
                    }
                    else
                    {
                        sb.Append(insertedLicenseXAML);
                    }                    

                    using (StreamReader sr = new StreamReader(item.FileName))
                    {
                        sb.Append(sr.ReadToEnd());
                    }

                    File.WriteAllText(item.FileName, sb.ToString());
                }

                MessageBox.Show("License succesfully inserted!");
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong! :(");
            }
        }


        private CommandHandler _selectFolderCommand;
        public CommandHandler SelectFolderCommand
        {
            get
            {
                if (_selectFolderCommand == null)
                {
                    _selectFolderCommand = new CommandHandler(
                        param => SelectFolder(),
                        null
                    );
                }
                return _selectFolderCommand;
            }
        }

        internal void SelectFolder()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedFolderPath = dialog.SelectedPath;
                    SelectedFiles = FindAllFiles(SelectedFolderPath);

                    if (SelectedFiles.Count > 0)
                        InsertLicenseCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

    }
}