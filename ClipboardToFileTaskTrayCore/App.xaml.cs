using System;
using System.Windows;
using System.Windows.Forms;

namespace ClipboardToFileTaskTrayCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        /*
         * こちらを参考にした。
         ・C#WPFでタスクトレイ常駐アプリの作り方
            https://garafu.blogspot.jp/2015/06/dev-tasktray-residentapplication.html            
             */

        private SettingWindow _settingWindow;

        /// <summary>
        /// タスクトレイに表示するアイコン
        /// </summary>
        private NotifyIcon _notifyIcon;

        /// <summary>
        /// System.Windows.Application.Startup イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this._notifyIcon = new NotifyIcon();

            //クリップボードの内容に変更があった場合のイベントハンドラをセット
            //ClipboardNotification.ClipboardUpdate += ClipboardNotification_ClipboardUpdate;

            _notifyIcon.Icon = new System.Drawing.Icon("./Resources/Icon/clipboard.ico");
            _notifyIcon.Visible = true;
            _notifyIcon.Text = "ClipboardToFile Core";

            var menu = new ContextMenuStrip();
            menu.Items.Add(new ToolStripMenuItem(text: "&終了",
                                                 image: null,
                                                 onClick: (s, e) => System.Windows.Application.Current.Shutdown()));
            menu.Items.Add(new ToolStripMenuItem(text: "&Settings",
                                                 image: null,
                                                 onClick: OpenSettingWindow));

            _notifyIcon.ContextMenuStrip = menu;
        }

        private void OpenSettingWindow(object sender, EventArgs e)
        {
            this._settingWindow = new ClipboardToFileTaskTrayCore.SettingWindow();
            this._settingWindow.ShowDialog();
        }

        private void ClipboardNotification_ClipboardUpdate(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// System.Windows.Application.Exit イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している ExitEventArgs</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this._notifyIcon.Dispose();
        }
    }
}
