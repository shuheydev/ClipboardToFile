using ClipboardToFileCore.Common;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClipboardToFileCore
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
                    var folderPath = Path.GetDirectoryName(filePath);
                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var ext = Path.GetExtension(filePath);

                    var newName = $"{fileName}_{postFix}";
                    var newPath = Path.Combine(folderPath, $"{newName}{ext}");
                    if (!File.Exists(newPath))
                        return newPath;

                    postFix++;
                }
            }

            //ファイル名にタイムスタンプを付加する。
            string AddTimeStampToFilePath(string filePath)
            {
                var folderPath = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var ext = Path.GetExtension(filePath);//ドットまで含まれるので注意。例：「.jpg」

                return Path.Combine(folderPath, $"{fileName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}{ext}");
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
                outputFilePath = AddTimeStampToFilePath(outputFilePath);

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
                outputFilePath = AddTimeStampToFilePath(outputFilePath);

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
                outputFilePath = AddTimeStampToFilePath(outputFilePath);

                File.WriteAllText(outputFilePath, data);

                return;
            }
        }
    }
}
