using System;
using System.IO;
using System.Text;

using PlayOnline.Core;

namespace PlayOnline.FFXI {

  public class FFXI {

    private FFXI() { /* static use only */ }

    public static bool GetFilePath(int FileNumber, out byte App, out byte Dir, out byte File) {
    string DataRoot = POL.GetApplicationPath(AppID.FFXI);
      for (byte i = 1; i < 10; ++i) {
      string Suffix = "";
      string DataDir = DataRoot;
	if (i > 1) {
	  Suffix = i.ToString();
	  DataDir = Path.Combine(DataRoot, "Rom" + Suffix);
	}
      string VTableFile = Path.Combine(DataDir, String.Format("VTABLE{0}.DAT", Suffix));
      string FTableFile = Path.Combine(DataDir, String.Format("FTABLE{0}.DAT", Suffix));
	if (i == 1) // add the Rom now (not needed for the *TABLE.DAT, but needed for the other DAT paths)
	  DataDir = Path.Combine(DataRoot, "Rom");
	if (System.IO.File.Exists(VTableFile) && System.IO.File.Exists(FTableFile)) {
	  try {
	  BinaryReader VBR = new BinaryReader(new FileStream(VTableFile, FileMode.Open, FileAccess.Read, FileShare.Read));
	    if (FileNumber < VBR.BaseStream.Length) {
	      VBR.BaseStream.Seek(FileNumber, SeekOrigin.Begin);
	      if (VBR.ReadByte() == i) {
	      BinaryReader FBR = new BinaryReader(new FileStream(FTableFile, FileMode.Open, FileAccess.Read, FileShare.Read));
		FBR.BaseStream.Seek(2 * FileNumber, SeekOrigin.Begin);
	      ushort FileDir = FBR.ReadUInt16();
		App  = (byte) (i - 1);
		Dir  = (byte) (FileDir / 0x80);
		File = (byte) (FileDir % 0x80);
		FBR.Close();
		return true;
	      }
	    }
	    VBR.Close();
	  }
	  catch { }
	}
      }
      App = Dir = File = 0;
      return false;
    }

    public static string GetFilePath(byte App, byte Dir, byte File) {
    string ROMDir = "Rom";
      if (App > 0) {
	++App;
	ROMDir += App.ToString();
      }
      return Path.Combine(POL.GetApplicationPath(AppID.FFXI), Path.Combine(ROMDir, Path.Combine(Dir.ToString(), Path.ChangeExtension(File.ToString(), ".dat"))));
    }

    public static string GetFilePath(int FileNumber) {
    byte App  = 0;
    byte Dir  = 0;
    byte File = 0;
      if (!FFXI.GetFilePath(FileNumber, out App, out Dir, out File))
	return null;
      return FFXI.GetFilePath(App, Dir, File);
    }

  }

}