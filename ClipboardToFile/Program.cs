using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;


namespace ClipboardToFile
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length == 0)
                return;
            var outputFolderPath = args[0];

            if (!Directory.Exists(outputFolderPath))
                return;

            //クリップボードの情報を取得
            var cbData = Clipboard.GetDataObject();
            if (cbData == null)
                return;

            OutputClipboardContent(cbData, outputFolderPath);

        }

        /// <summary>
        /// クリップボードから取得したデータと、出力先のフォルダのパスを指定すると、
        /// そこにファイルが作られる。
        /// </summary>
        /// <param name="cbData"></param>
        /// <param name="outputFolderPath"></param>
        private static void OutputClipboardContent(IDataObject cbData, string outputFolderPath)
        {
            if (cbData == null) return;


            foreach (var fmt in cbData.GetFormats())
            {
                Console.WriteLine(fmt);
            }


            //同名のファイルが存在していた場合に、
            //ファイル名に番号を付加する。
            string getNewFilePath(string filePath)
            {
                if (!File.Exists(filePath))
                    return filePath;

                var postFix = 1;
                while (true)
                {
                    var folder = Path.GetDirectoryName(filePath);
                    var name = Path.GetFileNameWithoutExtension(filePath);
                    var ext = Path.GetExtension(filePath);

                    var newName = $"{name}_{postFix}";
                    var newPath = Path.Combine(folder, $"{newName}{ext}");
                    if (!File.Exists(newPath))
                        return newPath;

                    postFix++;
                }
            }


            if (cbData.GetDataPresent(DataFormats.Rtf))
            {
                var data = (string)cbData.GetData(DataFormats.Rtf);

                if (string.IsNullOrEmpty(data))
                    return;

                var outputFilePath = Path.Combine(outputFolderPath, "clipboard.rtf");
                outputFilePath = getNewFilePath(outputFilePath);

                File.WriteAllText(outputFilePath, data);

                return;
            }
            else if (cbData.GetDataPresent(DataFormats.Html))
            {
                var data = (string)cbData.GetData(DataFormats.Html);

                if (string.IsNullOrEmpty(data))
                    return;

                var outputFilePath = Path.Combine(outputFolderPath, "clipboard.html");
                outputFilePath = getNewFilePath(outputFilePath);

                File.WriteAllText(outputFilePath, data);

                return;
            }
            else if (cbData.GetDataPresent(DataFormats.Bitmap))
            {
                var data = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);

                var outputFilePath = Path.Combine(outputFolderPath, "clipboard.bmp");
                outputFilePath = getNewFilePath(outputFilePath);

                data.Save(outputFilePath);

                return;
            }
            else if (cbData.GetDataPresent(DataFormats.OemText))
            {
                var data = (string)cbData.GetData(DataFormats.OemText);

                if (string.IsNullOrEmpty(data))
                    return;

                var outputFilePath = Path.Combine(outputFolderPath, "clipboard.txt");
                outputFilePath = getNewFilePath(outputFilePath);

                File.WriteAllText(outputFilePath, data);

                return;
            }
        }
    }
}
