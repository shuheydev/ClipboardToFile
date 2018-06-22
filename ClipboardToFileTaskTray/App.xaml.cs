using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClipboardToFileTaskTray
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        /*
         * こちらを参考にした。
         ・C#WPFでタスクトレイ常駐アプリの作り方
            https://garafu.blogspot.jp/2015/06/dev-tasktray-residentapplication.html            
             */

        public SettingWindow settingWindow;

        /// <summary>
        /// タスクトレイに表示するアイコン
        /// </summary>
        private NotifyIconWrapper notifyIcon;

        /// <summary>
        /// System.Windows.Application.Startup イベント を発生させます。
        /// </summary>
        /// <param name="e">イベントデータ を格納している StartupEventArgs</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();

            //クリップボードの内容に変更があった場合のイベントハンドラをセット
            ClipboardNotification.ClipboardUpdate += ClipboardNotification_ClipboardUpdate;

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
            this.notifyIcon.Dispose();
        }
    }
}
