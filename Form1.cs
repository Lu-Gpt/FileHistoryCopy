using FileHistoryCopy.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileHistoryCopy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.icon;

            textBox1.Text = "E:\\FileHistory\\user";
            textBox2.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            textBox3.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            textBox4.Text = "E:\\FileHistory\\user\\DESKTOP-XXXXXX\\Data\\C\\Users\\user\\file.txt";

            richTextBox1.Text = "第一步:選擇FileHistory路徑\r\n第二步:選擇貼上路徑\r\n第三步:點擊Transport開始複製" +
            "\r\nStep 1: Select the FileHistory path\r\nStep 2: Select Paste Path\r\nStep 3: Click Transport to start copying";
        }
        #region Directory
        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                textBox1.Text = path;
            }
            else
            {
                MessageBox.Show("請指定Folder路徑!");
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                textBox2.Text = path;
            }
            else
            {
                MessageBox.Show("請指定Folder路徑!");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != string.Empty && Directory.Exists(textBox1.Text) &&
                textBox2.Text != string.Empty && Directory.Exists(textBox2.Text))
            {
                IEnumerable<string> d1 = Directory.EnumerateFiles(textBox1.Text, "*", SearchOption.AllDirectories);

                var latestFiles = GetFileList(d1).GroupBy(f => f)
                                    .Select(g => g.OrderByDescending(f => f.TimeStamp).First())
                                    .ToList();

                #region
                /*
                // 1. 這是你當初掃描的起始點 (備份來源的根目錄)
                string backupRoot = textBox1.Text;
                string destinationRoot = textBox2.Text;

                if(!destinationRoot.EndsWith(Path.GetFileName(backupRoot)))
                {
                    destinationRoot = Path.Combine(destinationRoot,Path.GetFileName(backupRoot));
                }

                foreach (FileDetail fd in latestFiles)
                {
                    // 2. 計算相對路徑 (重要！)
                    // 例如: fd.DirectoryPath 是 "D:\BackupSource\FolderA\SubFolderB"
                    // 得到的 relativeDir 就是 "\FolderA\SubFolderB"
                    string relativeDir = fd.DirectoryPath.Replace(backupRoot, "");

                    // 3. 組合出目標資料夾的完整路徑
                    // 結果: "E:\Restore\FolderA\SubFolderB"
                    string targetFolder = Path.Combine(destinationRoot, relativeDir.TrimStart(Path.DirectorySeparatorChar));

                    // 4. 確保多層資料夾一次建立
                    if (!Directory.Exists(targetFolder))
                    {
                        // CreateDirectory 很強大，它會自動建立中間缺少的每一層資料夾
                        Directory.CreateDirectory(targetFolder);
                    }

                    // 5. 組合最終檔案路徑並複製
                    string destinationPath = Path.Combine(targetFolder, fd.FileName);
                    File.Copy(fd.FullFileName, destinationPath, true);
                }
                */
                #endregion
                #region
                // 1. 這是你當初掃描的起始點 (備份來源的根目錄)
                string backupRoot = textBox1.Text.TrimEnd(Path.DirectorySeparatorChar);
                string destinationRoot = textBox2.Text;

                // 1. 確保目標資料夾內包含來源資料夾的名稱
                string backupDirName = Path.GetFileName(backupRoot);
                if (!destinationRoot.TrimEnd(Path.DirectorySeparatorChar).EndsWith(backupDirName, StringComparison.OrdinalIgnoreCase))
                {
                    destinationRoot = Path.Combine(destinationRoot, backupDirName);
                }

                richTextBox1.Text +=  Environment.NewLine;
                foreach (FileDetail fd in latestFiles)
                {
                    // 2. 用長度擷取相對路徑，比 Replace 更安全
                    string relativeDir = fd.DirectoryPath.Length > backupRoot.Length
                                         ? fd.DirectoryPath.Substring(backupRoot.Length).TrimStart(Path.DirectorySeparatorChar)
                                         : "";

                    // 3. 組合目標資料夾
                    string targetFolder = Path.Combine(destinationRoot, relativeDir);

                    // 4. 建立資料夾
                    if (!string.IsNullOrEmpty(targetFolder) && !Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    // 5. 複製檔案
                    string destinationPath = Path.Combine(targetFolder, fd.FileName);
                    File.Copy(fd.FullFileName, destinationPath, true);
                    richTextBox1.Text += $"{destinationPath},Size:{fd.FileLength}Byte" + Environment.NewLine;
                }
                #endregion
                MessageBox.Show("success");
            }
            else
            {
                MessageBox.Show("找不到路徑");
            }
        }
        #endregion
        #region SingleFile
        private void button6_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All (*.*)|*.*";
            openFileDialog1.Multiselect = false;
            openFileDialog1.Title = "SelectFile";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox4.Text = openFileDialog1.FileName;
            }
            else
            {
                MessageBox.Show("請指定File路徑!");
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = folderBrowserDialog1.SelectedPath;
                textBox3.Text = path;
            }
            else
            {
                MessageBox.Show("請指定Folder路徑!");
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != string.Empty && Directory.Exists(textBox3.Text) &&
                textBox4.Text != string.Empty && File.Exists(textBox4.Text))
            {
                IEnumerable<string> f1 = new string[] { textBox4.Text };
                var ff = GetFileList(f1).FirstOrDefault();
                if(ff != null)
                {
                    string gold = Path.Combine(textBox3.Text, ff.FileName);
                    File.Copy(ff.FullFileName, gold, true);
                    richTextBox1.Text += $"{Environment.NewLine}{gold},Size:{ff.FileLength}Byte" + Environment.NewLine;
                    MessageBox.Show("success");
                }
                else
                {
                    MessageBox.Show("error");
                }
            }
            else
            {
                MessageBox.Show("找不到路徑");
            }
        }
        #endregion
        private IEnumerable<FileDetail> GetFileList(IEnumerable<string> pathlist)
        {
            if (pathlist != null && pathlist.Any())
            {
                List<FileDetail> list = new List<FileDetail>();
                foreach (string path in pathlist)
                {
                    if (File.Exists(path))
                    {
                        using (FileStream file1 = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            FileDetail fd = new FileDetail();
                            fd.FullFileName = file1.Name;
                            fd.FileLength = file1.Length;
                            fd.LastWriteTime = File.GetLastWriteTimeUtc(path);

                            string pattern = @"\s\((?<timestamp>\d{4}_\d{2}_\d{2}\s\d{2}_\d{2}_\d{2})\sUTC\)$";

                            Match match = Regex.Match(Path.GetFileNameWithoutExtension(path), pattern);
                            if (match.Success)
                            {
                                string rawTimestamp = match.Groups["timestamp"].Value;
                                //須注意Kind要是UTC無偏移
                                fd.TimeStamp = DateTime.ParseExact(rawTimestamp, "yyyy_MM_dd HH_mm_ss",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                            }

                            string nameWithoutExt = Path.GetFileNameWithoutExtension(file1.Name);

                            string cleanMainName = Regex.Replace(nameWithoutExt, pattern, "");

                            fd.FileName = cleanMainName + Path.GetExtension(file1.Name);
                            list.Add(fd);
                        }
                    }
                }
                return list;
            }
            return Enumerable.Empty<FileDetail>();
        }
    }
}
