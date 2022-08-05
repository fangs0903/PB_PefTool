using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PB_PefTool
{
    public partial class Form1 : Form
    {
        OpenFileDialog FileOpen = new OpenFileDialog();
        SaveFileDialog FileSave = new SaveFileDialog();
        //public string[] fileType = { "i3Pack", "i3s", "pef", "i3a", "I3CHR", "i3CharaEditor", "i3wrd", "i3Light", "i3Obj", "i3ObjectEditor", "i3Evt", "i3Path", "i3Game", "i3GL", "i3font", "env", "i3UIs", "gui", "guiNode", "i3LevelDesign", "i3AI", "i3UILib" };
        public PBFiles TempOpenFile;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            FileOpen.Filter = "All Types (*.*)|*.*|Pef Files (*.pef)|*.pef|PefDec Files (*.pefdec)|*.pefdec";
            //FileOpen.Filter = "All Types (*.*)|*.*|I3 Engine Files (*.*)|" + string.Join(";", Array.ConvertAll(fileType, a => string.Format("*.{0}", a)));
            //FileOpen.Filter += "|" + string.Join("|", Array.ConvertAll(fileType, a => string.Format("{0} (*.{0})|*.{0}", a)));
            //ReadCommandLineArgs(Environment.GetCommandLineArgs());
            //ReadDirectoryList(false);
            //ClearTempFiles();
        }

        void ClearTempFiles()
        {
            var path = Path.Combine(Path.GetTempPath(), "PBToolDecEnc");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (FileOpen.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = FileOpen.FileName;
                //ShowPBFiles();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Encrypt
            if(textBox1.Text != String.Empty && Path.GetExtension(FileOpen.FileName).ToLower() != ".pef")
            {
                TempOpenFile = new PBFiles(File.ReadAllBytes(FileOpen.FileName), FileOpen.FileName, 0);
                if (TempOpenFile != null && TempOpenFile.PEFEnc.Length > 0)
                {
                    if (TempOpenFile.FilePath.Length > 0)
                    {
                        FileSave.InitialDirectory = Path.GetDirectoryName(TempOpenFile.FilePath);
                    }
                    FileSave.FileName = Path.GetFileNameWithoutExtension(TempOpenFile.FileName);
                    FileSave.Filter = "Pef Files (*.pef)|*.pef";
                    if (FileSave.ShowDialog() == DialogResult.OK)
                    {
                        //UpdateI3S();
                        File.WriteAllBytes(Path.ChangeExtension(FileOpen.FileName, ".pef"), TempOpenFile.PEFEnc);
                        MessageBox.Show("Attempt to encrypt complete!");
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Decrypt
            if (textBox1.Text != String.Empty && Path.GetExtension(FileOpen.FileName).ToLower() != ".pefdec")
            {
                TempOpenFile = new PBFiles(File.ReadAllBytes(FileOpen.FileName), FileOpen.FileName, 1);
                if (TempOpenFile != null && TempOpenFile.PEFDec.Length > 0)
                {
                    if (TempOpenFile.FilePath.Length > 0)
                    {
                        FileSave.InitialDirectory = Path.GetDirectoryName(TempOpenFile.FilePath);
                    }
                    FileSave.FileName = Path.GetFileNameWithoutExtension(TempOpenFile.FileName);
                    FileSave.Filter = "PefDec Files (*.pefdec)|*.pefdec";
                    if (FileSave.ShowDialog() == DialogResult.OK)
                    {
                        //UpdateI3S();
                        File.WriteAllBytes(Path.ChangeExtension(FileOpen.FileName, ".pefdec"), TempOpenFile.PEFDec);
                        MessageBox.Show("Attempt to decrypt complete!");
                    }
                }
            }
        }
        
        public void ShowPBFiles()
        {
            //lv1.BeginUpdate();
            try
            {
            //tssl1.Text = tempFile.name + tempFile.extension;
            //tssl2.Text = "Size: " + String.Format(new FileSizeFormatProvider(), "{0:fs}", tempFile.backUp.Length);
            //tssl3.Text = "Header: " + tempFile.text.Length.ToString() + "/" + tempFile.textLines.Length.ToString();
            //tssl4.Text = "Content: " + tempFile._block.Count.ToString();

            //lv1.Items.Clear();

            foreach (var element in TempOpenFile._ContentDic)
            {
              var item = new ListViewItem(element.Value.Item()) { Checked = false };
              //lv1.Items.Add(item);
            }
        }
        catch (NullReferenceException)
        {

        }
        //lv1.EndUpdate();
        //lv2.EndUpdate();
        //lv3.EndUpdate();
        }
    }
}
