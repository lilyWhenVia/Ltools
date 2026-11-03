using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

public static class Tests {
 static int checks;
 static void Check(bool result,string name){if(!result)throw new Exception("FAIL: "+name);Console.WriteLine("PASS: "+name);checks++;}
 [STAThread] public static int Main(){string temp=Path.Combine(Path.GetTempPath(),"Ltools-Test-"+Guid.NewGuid());Directory.CreateDirectory(temp);string file=Path.Combine(temp,"sample file.txt");File.WriteAllText(file,"test");try{
  Check(Recognition.TypeOf("https://example.com/")=="网址","URL recognition");
  Check(Recognition.TypeOf("javascript:alert(1)")=="文本","non-HTTP scheme is not an open-URL action");
  Check(Recognition.TypeOf("test@example.com")=="邮箱","email recognition");
  Check(Recognition.LocalPath("/"+file.Replace('\\','/'))==file,"slash-prefixed path normalization");
  Check(Recognition.LocalPath(new Uri(file).AbsoluteUri)==file,"file URI with spaces normalization");
  Check(Recognition.TypeOf("/"+file.Replace('\\','/'))=="路径","existing file recognition");
  Check(Recognition.Calculate("(2+3)*4")=="20","arithmetic");
  bool blocked=false;try{Recognition.Calculate("alert(1)");}catch{blocked=true;}Check(blocked,"non-arithmetic rejected");
  Check(Recognition.Matches("Visual Studio Code","visual code"),"multi-keyword matching");
  var entries=new List<ClipEntry>{new ClipEntry{Text="test"},new ClipEntry{Files=new[]{file}}};using(var ms=new MemoryStream()){var xml=new XmlSerializer(typeof(List<ClipEntry>));xml.Serialize(ms,entries);ms.Position=0;var restored=(List<ClipEntry>)xml.Deserialize(ms);Check(restored[0].Text=="test"&&restored[1].Files[0]==file,"clipboard persistence");}
  // Public source must not contain local development fixtures or credentials.
  string source=File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","src","Launcher.cs"));
  Check(!source.Contains("TestPathInteraction")&&!source.Contains("UiSmoke"),"development UI fixtures excluded");
  string history=Path.Combine(temp,"history.xml");try{HistoryStorage.Save(history,new List<ClipEntry>{new ClipEntry{Text="older"}});HistoryStorage.Save(history,new List<ClipEntry>{new ClipEntry{Text="newer"}});Check(HistoryStorage.Load(history)[0].Text=="newer","history survives process restart");Check(File.Exists(history+".bak"),"previous history backup exists");File.WriteAllText(history,"corrupt");Check(HistoryStorage.Load(history)[0].Text=="older","corrupt history recovers backup");HistoryStorage.Save(history,new List<ClipEntry>{new ClipEntry{Text="recovered"}});Check(HistoryStorage.Load(history)[0].Text=="recovered","recovered history can be saved again");}finally{foreach(string p in new[]{history,history+".bak",history+".tmp"})if(File.Exists(p))File.Delete(p);}
  string legacy=Path.Combine(temp,"legacy"),migrated=Path.Combine(temp,"migrated");Directory.CreateDirectory(legacy);try{string image=Path.Combine(legacy,"sample.png");using(var b=new Bitmap(20,20))b.Save(image,System.Drawing.Imaging.ImageFormat.Png);HistoryStorage.Save(Path.Combine(legacy,"history.xml"),new List<ClipEntry>{new ClipEntry{Text="kept"},new ClipEntry{ImagePath=image}});File.WriteAllText(Path.Combine(legacy,"search-state.xml"),"<SearchState />");DataMigration.Migrate(legacy,migrated);var restored=HistoryStorage.Load(Path.Combine(migrated,"history.xml"));Check(restored.Count==2&&restored[0].Text=="kept","rebrand migrates text history");Check(restored[1].ImagePath==Path.Combine(migrated,"sample.png")&&File.Exists(restored[1].ImagePath),"rebrand migrates images and rewrites paths");Check(File.Exists(Path.Combine(legacy,"history.xml"))&&File.Exists(image),"migration preserves original data");DataMigration.Migrate(legacy,migrated);Check(HistoryStorage.Load(Path.Combine(migrated,"history.xml")).Count==2,"migration is idempotent");}finally{foreach(string folder in new[]{legacy,migrated}){if(Directory.Exists(folder)){foreach(string p in Directory.GetFiles(folder))File.Delete(p);Directory.Delete(folder);}}}
  using(var icons=new ResultIconCache()){Check(icons.Get(file)!=null,"file icon loaded from Windows Shell");Check(icons.Get(Environment.GetFolderPath(Environment.SpecialFolder.Windows))!=null,"folder icon loaded from Windows Shell");}
  Console.WriteLine("Passed "+checks+" checks.");return 0;
 }catch(Exception ex){Console.Error.WriteLine(ex.Message);return 1;}finally{File.Delete(file);Directory.Delete(temp);}}
}
