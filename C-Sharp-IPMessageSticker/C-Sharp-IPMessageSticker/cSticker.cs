﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace C_Sharp_IPMessageSticker
{
    public class cSticker
    {
        public static readonly string Root = @"Stickers/";
        public static readonly string Recent = @"Recent/";

        public static IEnumerable<StickerSet> GetStickers()
        {
            var stickerSets = new List<StickerSet>();

            if (!Directory.Exists(Root))
                Directory.CreateDirectory(Root);

            // Get Folder list
            var folders = Directory.GetDirectories(Root);
            foreach (var folder in folders)
            {
                var temp = new StickerSet();
                var files = new DirectoryInfo(folder + @"/").GetFiles();

                temp.NameHeader = folder.Replace(Root, "").Trim();
                temp.IconHeader = folder + @"/" + files.FirstOrDefault().Name;

                var tempStickers = new List<Sticker>();
                foreach (var file in files.OrderByDescending(x => x.CreationTime))
                {
                    var filePath = folder + @"/" + file.Name;

                    var sTemp = new Sticker();
                    sTemp.Name = file.Name.Replace(@".png", "");
                    sTemp.Path = filePath;
                    tempStickers.Add(sTemp);
                }

                temp.Stickers = tempStickers;
                stickerSets.Add(temp);
            }

            return stickerSets;
        }

        public static void ImportStickers(string stickerName, string folder)
        {
            if (string.IsNullOrWhiteSpace(stickerName))
                throw new Exception("Sticker Name Invalid.");

            if (!Directory.Exists(folder))
                throw new Exception("Directory Invalid.");

            if (Directory.Exists(Root + stickerName))
                throw new Exception("Directory Exists.");

            Directory.CreateDirectory(Root + stickerName);

            var Files = new DirectoryInfo(folder).GetFiles();
            foreach (FileInfo file in Files)
            {
                CopySticker(file.FullName, Root + stickerName + @"/" + file.Name.Replace(file.Extension, "") + ".png");
            }
        }

        public static void DeleteSticker(string stickerName)
        {
            if (string.IsNullOrWhiteSpace(stickerName))
                throw new Exception("Sticker Name Invalid.");

            if (!Directory.Exists(Root + stickerName))
                throw new Exception("No Sticker Name");

            Directory.Delete(Root + stickerName.Trim(), true);
        }

        public static void RenameSticker(string stickerName, string newName)
        {
            if (string.IsNullOrWhiteSpace(stickerName))
                throw new Exception("Sticker Name Invalid.");

            if (!Directory.Exists(Root + stickerName))
                throw new Exception("No Sticker Name");

            if (Directory.Exists(Root + newName))
                throw new Exception("Directory Exists.");
        }

        public static void AddToRecent(Sticker sticker)
        {
            if (sticker == null)
                return;

            if (!Directory.Exists(Root + Recent))
                Directory.CreateDirectory(Root + Recent);

            string pathTemp = Root + Recent + sticker.Path.Split('/')[2].Trim();

            // ต้องไม่ได้มาจาก Recent ด้วยกัน
            if (!sticker.Path.Contains("Recent") && !pathTemp.Contains("Recent"))
                File.Copy(sticker.Path, pathTemp, true);

            // Limit 20 items (ลบอันเก่าสุดออก)
            var files = new DirectoryInfo(Root + Recent).GetFiles();
            if (files.Count() > 20)
            {
                File.Delete(files.OrderByDescending(x => x.CreationTime).Last().FullName);
            }
        }

        public static void ClearRecent()
        {
            if (!Directory.Exists(Root + Recent))
            {
                Directory.CreateDirectory(Root + Recent);
                return;
            }

            Directory.Delete(Root + Recent, true);
            Directory.CreateDirectory(Root + Recent);
        }

        private static void CopySticker(string source, string destination)
        {
            var bmp1 = cMain.LoadImageFormPath(source);
            var jpgEncoder = GetEncoder(ImageFormat.Png);
            var myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100);

            bmp1 = (Image)(new Bitmap(bmp1, new Size(96, 96)));
            bmp1.Save(destination, jpgEncoder, myEncoderParameters);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
        }
    }

    public class StickerSet
    {
        public string NameHeader { get; set; }
        public string IconHeader { get; set; }

        public IEnumerable<Sticker> Stickers { get; set; }
    }

    public class Sticker
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}