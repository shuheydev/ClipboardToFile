using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardToFileTaskTray
{
    public partial class NotifyIconWrapper : Component
    {
        public NotifyIconWrapper()
        {
            InitializeComponent();

            toolStripMenuItem_Config.Click += ToolStripMenuItem_Config_Click;
            toolStripMenuItem_Exit.Click += ToolStripMenuItem_Exit_Click;
        }

        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private readonly SettingWindow settingWindow = new SettingWindow();
        private void ToolStripMenuItem_Config_Click(object sender, EventArgs e)
        {
            settingWindow.Show();
            settingWindow.Activate();
        }



        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

    }
}
