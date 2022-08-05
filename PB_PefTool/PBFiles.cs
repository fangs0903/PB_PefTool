using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PB_PefTool
{
    public class PBFiles
    {
        public string FileName; //檔名
        public string FileExtension; //副檔名
        public string FilePath; //檔案完整路徑
        public byte[] parent = new byte[0];
        public byte[] backUp = new byte[0];
        public byte[] Header = new byte[0]; //文件頭
        public byte[] HeaderTitle = new byte[0]; //文件頭標題
        public byte[] FileInfo = new byte[0]; //文件信息
        public string[] HeaderTitleLines;
        public byte[] PEFEnc; //檔案加密
        public byte[] PEFDec; //檔案解密
        public Dictionary<int, PBFiles.ContentDic> _ContentDic = new Dictionary<int, PBFiles.ContentDic>();
        public Dictionary<int, PBFiles.ContentDic> _ContentD = new Dictionary<int, PBFiles.ContentDic>();
        public class ContentDic
        {
            public PBFiles Root;
            public int ID;
            public int CTypeIndex;
            public int Target;
            public string Type;
            public int DataStartPoint;
            public byte[] Data;

            public ContentDic(int ID, int line, int target, byte[] data, PBFiles root)
            {
                this.ID = ID;
                this.CTypeIndex = line;
                this.Target = target;
                this.Type = root.HeaderTitleLines[line];
                this.Data = data;
                this.Root = root;
            }

            public ContentDic()
            {

            }
            public string[] Item()
            {
                string[] Item = new string[6];
                Item[0] = Type;
                Item[1] = ID.ToString();
                Item[2] = /*Comment()*/"";
                Item[3] = DataStartPoint.ToString();
                Item[4] = Data.Length.ToString();
                return Item;
            }
        }
        public PBFiles(byte[] FileData, string FPath, int type)
        {
            FileName = Path.GetFileNameWithoutExtension(FPath); //檔名
            FileExtension = Path.GetExtension(FPath); //副檔名
            FilePath = FPath; //檔案完整路徑
            //Encrypt
            if (type == 0)
            {
                if (FileExtension.ToLower() == ".pefdec")
                {
                    if (Encoding.Default.GetString(FileData, 0, 4) == "I3R2" || Encoding.Default.GetString(FileData, 1, 3) == "3R2")
                    {
                        PEFEnc = EncDec.Encrypt(FileData, 5);
                        //File.WriteAllBytes(Path.ChangeExtension(FPath, ".pef"), PEFEnc);
                    }
                    else
                    {
                        MessageBox.Show("File not decrypted or unknown file!");
                    }
                }
            }

            //Decrypt
            if (type == 1)
            {
                if (FileExtension.ToLower() == ".pef")
                {
                    if (Encoding.Default.GetString(FileData, 0, 4) != "I3R2" || Encoding.Default.GetString(FileData, 1, 3) != "3R2")
                    {
                        PEFDec = EncDec.Decrypt(FileData, 3);
                        //File.WriteAllBytes(Path.ChangeExtension(FPath, ".pefdec"), PEFDec);
                    }
                    else
                    {
                        MessageBox.Show("File not encrypted or unknown file!");
                    }
                }
            }
            /*
            if (Encoding.Default.GetString(FileData, 1, 3) == "3R2")
            {
                int ContentLinesCount = 0;
                using (var FileDataBR = new BinaryReader(new MemoryStream(FileData)))
                {
                    //ReadInt16     從當前流中讀取 2 位元組帶符號整數
                    //ReadInt32     從當前流中讀取 4 位元組帶符號整數
                    //ReadInt64     從當前流中讀取 8 位元組帶符號整數
                    //ReadByte      從當前流中讀取下一個字節
                    //ReadBytes     從當前流中讀取指定的位元組數以寫入位元組陣列中
                    //ReadSByte     從當前流中讀取一個有符號位元組
                    //ReadSingle    從當前流中讀取 4 位元組浮點值
                    //ReadString    從當前流中讀取一個字串。字串有長度前綴，一次 7 位地被編碼為整數。
                    //ReadUInt16    使用 Little-Endian 編碼從當前流中讀取 2 位元組無符號整數
                    //ReadUInt32    從當前流中讀取 4 位元組無符號整數
                    //ReadUInt64    從當前流中讀取 8 位元組無符號整數

                    //以上會使流的當前位置上升到第 n 個位元組。
                    //ex: 使用 ReadInt32 會上升到第四個位元組

                    var Read1 = FileDataBR.ReadInt32(); // I3R2 //3R2  -- 4 位元組
                    var Read2 = FileDataBR.ReadInt32(); // 0     -- 4 位元組
                    var Read3 = FileDataBR.ReadInt16(); // 1     -- 2 位元組
                    var Read4 = FileDataBR.ReadInt16(); // 8     -- 2 位元組
                    var Read5 = FileDataBR.ReadInt32(); // 行數   -- 4 位元組

                    //以上當前流讀取到第 16 個位元組 並且後4個位元組為 Header 行數
                    var HeaderSize = FileDataBR.ReadInt64();


                    //擷取 FileData (Header 位元組大小為 184 個位元組) 放入 Header
                    long StartPoint = 0; //要擷取的 data 起始點位置
                    Header = new byte[HeaderSize];
                    Array.Copy(FileData, StartPoint, Header, 0, Header.Length);

                    //擷取 FileData (依照 Header 內容字數 來指定位元組大小) 放入 text -- 從第 184 個位元組開始放入
                    StartPoint = HeaderSize; //要擷取的 data 起始點位置 //讀取 8 位元組 此為 Header 位元組數量(184) -- 當前流讀取到第 24 個位元組
                    var HeaderCS = FileDataBR.ReadInt64(); //讀取 8 位元組 此為 Header 內容位元組數量  -- 當前流讀取到第 32 個位元組
                    HeaderTitle = new byte[HeaderCS];
                    Array.Copy(FileData, StartPoint, HeaderTitle, 0, HeaderTitle.Length);

                    //擷取 FileData (依照 Header 內容字數 來指定位元組大小) 放入 info
                    ContentLinesCount = FileDataBR.ReadInt32(); // 讀取 4 位元組 此為內容行數 -- 當前流讀取到第 36 個位元組 
                    StartPoint = FileDataBR.ReadInt64();
                    //要擷取的 FileData 起始點位置 //讀取 8 位元組 此為 Header 開頭位元組數量(184) + Header 內容位元組數量(等於信息開始位置) -- 當前流讀取到第 44 個位元組
                    var InfoSize = FileDataBR.ReadInt64(); //讀取 8 位元組 此為 信息大小 -- 當前流讀取到第 52 個位元組
                    FileInfo = new byte[InfoSize];
                    Array.Copy(FileData, StartPoint, FileInfo, 0, FileInfo.Length);
                }

                var HeaderTitleDec = Encoding.Default.GetString(HeaderTitle, 0, HeaderTitle.Length - 2);
                HeaderTitleLines = HeaderTitleDec.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                
                using (var FileInfoBR = new BinaryReader(new MemoryStream(FileInfo)))
                {

                    for (int i = 0; i < ContentLinesCount; i++)
                    {
                        var content = new PBFiles.ContentDic();
                        content.Root = this;
                        content.CTypeIndex = FileInfoBR.ReadInt32();    //讀取 4 位元組 //往後指定為TYPE編號
                        content.ID = FileInfoBR.ReadUInt16();     //讀取 2 位元組 //列表ID
                        content.Target = FileInfoBR.ReadUInt16(); //讀取 2 位元組 //???

                        if (content.Target != 0)
                        {
                            content.Target -= 32768;
                        }

                        int unknown = FileInfoBR.ReadInt32();   //讀取 4 位元組 //???
                        if (unknown != 0 && unknown != 64)
                        {
                            MessageBox.Show("???");
                        }

                        //擷取 FileData (依照 FileInfot紀錄的內容位元組大小) 放入 content.data
                        content.DataStartPoint = (int)FileInfoBR.ReadInt64();  //要擷取的 FileInfo 起始點位置  //讀取 8 位元組
                        var ContentSize = FileInfoBR.ReadUInt32(); //讀取 4 位元組 此為 內容位元組大小
                        FileInfoBR.ReadUInt32(); //讀取 4 位元組
                        content.Data = new byte[ContentSize];
                        Array.Copy(FileData, content.DataStartPoint, content.Data, 0, content.Data.Length);

                        if (content.CTypeIndex < HeaderTitleLines.Length)
                        {
                            content.Type = HeaderTitleLines[content.CTypeIndex];
                        }
                        else
                        {
                            content.Type = string.Format("Error ({0})", content.CTypeIndex);
                        }
                        if (!_ContentDic.ContainsKey(content.ID))
                        {
                            _ContentDic.Add(content.ID, content);
                        }
                        else
                        {
                            //_blockD.Add(block.ID, block);
                            _ContentD[content.ID] = content;
                        }
                    }
                }
            }*/
        }
    }
}
