using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licert
{
    public class SelectedFile
    {
        public bool IsSelected { get; set; } = true;
        public string FileName { get; set; }

        public SelectedFile(string fileName)
        {
            FileName = fileName;
        }
    }

}
