using System;
using System.IO;
using System.Windows;

namespace ClipboardToFileTaskTray
{
    /// <summary>
    /// SettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            //前回の設定情報を復元
            CheckBox_AddToContextMenu.IsChecked = Properties.Settings.Default.AddToContextMenuSetting;
            CheckBox_ExecuteAtStartup.IsChecked = Properties.Settings.Default.ExecuteAtStartupSetting;

        }


        private readonly string appContextMenuRegKeyTree = @"Software\Classes\Directory\Background\shell\ClipboardToFile";
        private readonly string appContextMenuCommandRegKeyTree =
            @"Software\Classes\Directory\Background\shell\ClipboardToFile\command";

        private readonly string executeAtStartupKeyTree = @"Software\Microsoft\Windows\CurrentVersion\Run";

        private readonly string clipboardToFileExeName = "ClipboardToFile.exe";
        private readonly string contextMenuLabel = "Clipboard To File";
        private readonly string applicationName = "ClipboardToFile";



        /// <summary>
        /// 設定ウィンドウの閉じるボタンが押されたとき、
        /// closeしないで非表示とする。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null)
                e.Cancel = true;

            this.Visibility = Visibility.Hidden;
        }



        private void Button_ApplySetting_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AddToContextMenuSetting = (bool)CheckBox_AddToContextMenu.IsChecked;
            Properties.Settings.Default.ExecuteAtStartupSetting = (bool)CheckBox_ExecuteAtStartup.IsChecked;
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.AddToContextMenuSetting)
            {
                var regkey =
                    Microsoft.Win32.Registry.CurrentUser.CreateSubKey(appContextMenuRegKeyTree);
                regkey?.SetValue(null, contextMenuLabel);
                regkey.Close();


                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var clipboardToFileExePath = Path.Combine(Path.GetDirectoryName(assembly.Location), clipboardToFileExeName);

                var regkey_command =
                    Microsoft.Win32.Registry.CurrentUser.CreateSubKey(appContextMenuCommandRegKeyTree);
                regkey_command?.SetValue(null, $"{clipboardToFileExePath} \"%V\"");
                regkey_command.Close();
            }
            else
            {
                //対象のレジストリキーとその配下を削除する。
                //第2パラメータでfalseと指定することで、もし存在しなかった場合に例外がでないようにする。
                Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(appContextMenuRegKeyTree, false);
            }

            if (Properties.Settings.Default.ExecuteAtStartupSetting)
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();

                var regkey =
                    Microsoft.Win32.Registry.CurrentUser.OpenSubKey(executeAtStartupKeyTree, true);
                regkey?.SetValue(applicationName, assembly.Location);
                regkey.Close();
            }
            else
            {
                var regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(executeAtStartupKeyTree, true);
                regkey?.DeleteValue(applicationName, false);
                regkey.Close();
            }


            this.Visibility = Visibility.Hidden;
        }

        private void Button_CancelSetting_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}
