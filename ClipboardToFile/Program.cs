using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using CommonLibrary;

namespace ClipboardToFile
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            if (args.Length != 2)//パスと画像出力形式の二つ
                return;

            var outputFolderPath = args[0];
            var outputImageType = args[1];

            if (!Directory.Exists(outputFolderPath))
                return;

            //クリップボードの情報を取得
            var cbData = Clipboard.GetDataObject();
            if (cbData == null)
                return;

            OutputClipboardContent(cbData, outputFolderPath, outputImageType);

        }

        /// <summary>
        /// クリップボードから取得したデータと、出力先のフォルダのパスを指定すると、
        /// そこにファイルが作られる。
        /// </summary>
        /// <param name="cbData"></param>
        /// <param name="outputFolderPath"></param>
        private static void OutputClipboardContent(IDataObject cbData, string outputFolderPath, string outputImageType)
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
                //Bitmap形式で変数に突っ込む
                var data = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);

                var extension = OutputImageType.JPEG;
                System.Drawing.Imaging.ImageFormat imageFormat = null;
                switch (outputImageType)
                {
                    case OutputImageType.JPEG:
                        extension = "jpg";
                        imageFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                        break;
                    case OutputImageType.PNG:
                        extension = "png";
                        imageFormat = System.Drawing.Imaging.ImageFormat.Png;
                        break;
                    case OutputImageType.Bitmap:
                        extension = "bmp";
                        imageFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                        break;
                }

                var outputFilePath = Path.Combine(outputFolderPath, $"clipboard.{extension}");
                outputFilePath = getNewFilePath(outputFilePath);

                //保存時に形式を指定できる。わーお
                data.Save(outputFilePath, imageFormat);

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
