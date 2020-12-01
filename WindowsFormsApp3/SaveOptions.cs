using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    [Serializable]
    public class SaveOptions
    {
        public List<string> Path { get; set; }
        public List<string> NameFile { get; set; }
    }
}
