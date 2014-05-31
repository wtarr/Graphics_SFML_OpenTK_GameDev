using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;

namespace CppTutorialPort
{
    class Program
    {
        public static void Main(string[] args)
        {
            //Loader l = new Loader();
            //l.Show();
            //TriangleBasic b = new TriangleBasic();
            //b.Execute();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Loader());
        }
    }
}
