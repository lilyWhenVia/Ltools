using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

public static class UiIcons {
 static object Get(object target,string name){return target.GetType().GetField(name,BindingFlags.NonPublic|BindingFlags.Instance).GetValue(target);}
 static void Call(object target,string name,params object[] args){target.GetType().GetMethod(name,BindingFlags.NonPublic|BindingFlags.Instance).Invoke(target,args);}
 [STAThread] public static void Main(){Application.EnableVisualStyles();Application.SetCompatibleTextRenderingDefault(false);var form=new Launcher();var timer=new Timer{Interval=1400};timer.Tick+=(s,e)=>{timer.Stop();try{Call(form,"Open","应用");var query=(TextBox)Get(form,"query");query.Text="图标预览";var list=(ListBox)Get(form,"results");list.Items.Clear();string windows=Environment.GetFolderPath(Environment.SpecialFolder.Windows);string app=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","dist","Ltools.exe");list.Items.Add(new Result{Title="Ltools",Tag="应用",IconPath=app});list.Items.Add(new Result{Title="文件资源管理器",Tag="应用",IconPath=Path.Combine(windows,"explorer.exe")});list.Items.Add(new Result{Title="README.md",Tag="文件",IconPath=Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"..","README.md")});list.Items.Add(new Result{Title="Windows",Tag="文件夹",IconPath=windows});list.SelectedIndex=0;Call(form,"ApplyCompactLayout");using(var bitmap=new Bitmap(form.Width,form.Height)){form.DrawToBitmap(bitmap,new Rectangle(0,0,bitmap.Width,bitmap.Height));bitmap.Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"ui-icons.png"));}Console.WriteLine("PASS: application, file, and directory icon rows render with branded search header");}finally{form.GetType().GetField("quitting",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(form,true);timer.Dispose();Application.Exit();}};form.Shown+=(s,e)=>timer.Start();Application.Run(form);}
}
