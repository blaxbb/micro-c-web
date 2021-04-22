using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microcweb.Client.Components
{
    public class ScanResultEventArgs
    {
        public string Text { get; set; }
        public ScanResultEventArgs(string text)
        {
            Text = text;
        }
    }
}
