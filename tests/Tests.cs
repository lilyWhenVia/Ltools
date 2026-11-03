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
 [STAThread] public static int Main(){string temp=Path.Combine(Path.GetTempPath(),"Ltool-Test-"+Guid.NewGuid());Directory.CreateDirectory(temp);string file=Path.Combine(temp,"sample file.txt");File.WriteAllText(file,"test");try{
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
  Console.WriteLine("Passed "+checks+" checks.");return 0;
 }catch(Exception ex){Console.Error.WriteLine(ex.Message);return 1;}finally{File.Delete(file);Directory.Delete(temp);}}
}
