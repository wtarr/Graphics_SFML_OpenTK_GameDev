using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using OpenTK;

namespace CppTutorialPort
{
    public partial class Loader : Form
    {
        private List<Type> _types;

        public Loader()
        {
            InitializeComponent();
            
        }

        private void Loader_Load(object sender, EventArgs e)
        {
            
            _types = new List<Type>();
            var type = typeof (IExample);
            _types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && !p.IsInterface
                && p.IsSubclassOf(typeof(GameWindow))).ToList();

            foreach (var instance in _types)
            {
                Lbox_Examples.Items.Add(instance.FullName);
            }

            Lbox_Examples.SetSelected(0, true);

        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            var type = _types.ElementAt(Lbox_Examples.SelectedIndex);
            using (Toolkit.Init())
            {
                using (var instance = (GameWindow)Activator.CreateInstance(type))
                {
                    MethodInfo mi = type.GetMethod("Execute");
                    mi.Invoke(instance, null);
                }
            }
        }
    }
}
