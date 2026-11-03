using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

public static class IconBuilder {
 public static void Main(string[] args){int[] sizes={16,24,32,48,64,128,256};var frames=new List<byte[]>();using(var source=Image.FromFile(args[0]))foreach(int size in sizes)using(var bitmap=new Bitmap(size,size,PixelFormat.Format32bppArgb)){using(var g=Graphics.FromImage(bitmap)){g.InterpolationMode=InterpolationMode.HighQualityBicubic;g.PixelOffsetMode=PixelOffsetMode.HighQuality;g.DrawImage(source,0,0,size,size);}using(var stream=new MemoryStream()){bitmap.Save(stream,ImageFormat.Png);frames.Add(stream.ToArray());}}using(var stream=File.Create(args[1]))using(var writer=new BinaryWriter(stream)){writer.Write((ushort)0);writer.Write((ushort)1);writer.Write((ushort)sizes.Length);int offset=6+sizes.Length*16;for(int i=0;i<sizes.Length;i++){writer.Write((byte)(sizes[i]==256?0:sizes[i]));writer.Write((byte)(sizes[i]==256?0:sizes[i]));writer.Write((byte)0);writer.Write((byte)0);writer.Write((ushort)1);writer.Write((ushort)32);writer.Write(frames[i].Length);writer.Write(offset);offset+=frames[i].Length;}foreach(var frame in frames)writer.Write(frame);}}
}