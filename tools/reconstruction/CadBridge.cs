using System;
using System.IO;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
public class CadBridge {
 public static void Run(string root,string mode) {
  ISldWorks sw=(ISldWorks)new SldWorksClass();
  Console.WriteLine("SolidWorks "+sw.RevisionNumber());
  sw.Visible=true;
  int err=0,warn=0;
  if(mode=="verify") {
   string path=Path.Combine(root,"TriPilot_pedal_reconstructed.SLDASM");
   var expectedNames=new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
   expectedNames.Add(Path.GetFileName(path));
   foreach(string line in File.ReadAllLines(Path.Combine(root,"parts_manifest.txt")))expectedNames.Add(line.Split('|')[0]+".SLDPRT");
   var toClose=new System.Collections.Generic.List<string>();
   IModelDoc2 openDoc=(IModelDoc2)sw.GetFirstDocument();
   while(openDoc!=null) {
    if(expectedNames.Contains(Path.GetFileName(openDoc.GetPathName()))) {
     if(openDoc.GetSaveFlag())throw new Exception("Save or close unsaved reference document before verification: "+openDoc.GetTitle());
     toClose.Add(openDoc.GetTitle());
    }
    openDoc=(IModelDoc2)openDoc.GetNext();
   }
   foreach(string title in toClose)sw.CloseDoc(title);
   IModelDoc2 verifiedDoc=(IModelDoc2)sw.OpenDoc6(path,2,1,"",ref err,ref warn);
   if(verifiedDoc==null||err!=0)throw new Exception("Reopen failed "+err);
   if(!string.Equals(verifiedDoc.GetPathName(),path,StringComparison.OrdinalIgnoreCase))throw new Exception("A different copy of the assembly is open. Close it before verifying this copy.");
   var assembly=(IAssemblyDoc)verifiedDoc;
   var log=new System.Collections.Generic.List<string>();
   log.Add("Reopened saved native assembly; errors="+err+" warnings="+warn);
   object[] components=(object[])assembly.GetComponents(false);
   if(components==null||components.Length!=16)throw new Exception("Incorrect component count");
   int solidCount=0;
   foreach(IComponent2 c in components) {
    string componentPath=c.GetPathName();
    if(!Path.GetFullPath(componentPath).StartsWith(Path.Combine(root,"SolidWorks_parts")+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase))throw new Exception("Reference resolved outside this CAD folder: "+c.Name2);
    IModelDoc2 part=(IModelDoc2)c.GetModelDoc2();
    if(!File.Exists(componentPath)||part==null||c.IsSuppressed()||!c.IsFixed())throw new Exception("Invalid component "+c.Name2);
    object[] bodies=(object[])((IPartDoc)part).GetBodies2(0,false);
    int n=bodies==null?0:bodies.Length;solidCount+=n;
    log.Add(c.Name2+"; resolved=True; fixed=True; solidBodies="+n);
   }
   if(solidCount!=12)throw new Exception("Incorrect solid body count "+solidCount);
   bool rebuilt=verifiedDoc.ForceRebuild3(false);log.Add("Rebuild returned="+rebuilt);
   verifiedDoc.ViewDisplayShaded();verifiedDoc.ShowNamedView2("*Isometric",7);verifiedDoc.ViewZoomtofit2();verifiedDoc.GraphicsRedraw2();
   if(!verifiedDoc.Extension.SaveAs(path,0,1,null,ref err,ref warn)||err!=0)throw new Exception("Preview save failed "+err);
   if(!verifiedDoc.SaveBMP(Path.Combine(root,"assembly_preview.bmp"),1600,1200))throw new Exception("Screenshot failed");
   log.Add("Bounds (metres): "+string.Join(",",(double[])assembly.GetBox(0)));
   File.WriteAllLines(Path.Combine(root,"reopen_validation.txt"),log.ToArray());
   foreach(string line in log)Console.WriteLine(line);
   sw.UserControl=true;
   return;
  }
  if(mode=="build") {
   string partsDir=Path.Combine(root,"SolidWorks_parts");Directory.CreateDirectory(partsDir);
   string asmPath=Path.Combine(root,"TriPilot_pedal_reconstructed.SLDASM");
   int oldUnits=sw.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlUnits);
   int oldType=sw.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlModelType);
   bool oldMesh=sw.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swVrmlStlImportAsPSMesh);
   var lines=File.ReadAllLines(Path.Combine(root,"parts_manifest.txt"));
   var log=new System.Collections.Generic.List<string>();
   try {
    sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlUnits,0);
    sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swVrmlStlImportAsPSMesh,false);
    foreach(string line in lines) {
     string[] fields=line.Split('|');string name=fields[0];bool solid=fields[1]=="solid";
     sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlModelType,solid?2:0);
     string target=Path.Combine(partsDir,name+".SLDPRT");
     IModelDoc2 m;
     if(File.Exists(target))m=(IModelDoc2)sw.OpenDoc6(target,1,1,"",ref err,ref warn);
     else m=(IModelDoc2)sw.LoadFile4(Path.Combine(root,"exchange_parts",name+".stl"),"",null,ref err);
     if(m==null)throw new Exception("Import failed "+name+" error "+err);
     object[] bodies=(object[])((IPartDoc)m).GetBodies2(0,false);
     int count=bodies==null?0:bodies.Length;
     if(solid && count!=1)throw new Exception("Expected one solid for "+name+", got "+count);
     m.Extension.CustomPropertyManager[""].Add3("Source",30,"TriPilot-FF website pedal.glb; retrieved 2026-10-05",2);
     m.Extension.CustomPropertyManager[""].Add3("Representation",30,solid?"Faceted solid reconstructed from source mesh; no original feature history":"Graphics mesh reference for purchased hardware",2);
     m.Extension.CustomPropertyManager[""].Add3("Fabrication_status",30,"REFERENCE - verify dimensions, tolerances and hardware fit before fabrication",2);
     m.Extension.CustomPropertyManager[""].Add3("Source_URL",30,"https://jeong-zju.github.io/RSS-TriPilot-FF-Page/static/models/pedal.glb",2);
     if(solid) {
      double[] col=name.StartsWith("03")||name.StartsWith("04")?new double[]{0.18,0.50,0.70,1,1,0.4,0.3,0,0}:new double[]{0.65,0.68,0.72,1,1,0.3,0.2,0,0};
      m.MaterialPropertyValues=col;
     }
     m.ShowNamedView2("*Isometric",7);m.ViewZoomtofit2();
     bool ok=m.Extension.SaveAs(target,0,1,null,ref err,ref warn);
     if(!ok||err!=0)throw new Exception("Save failed "+name+" "+err);
     string entry=name+" solidBodies="+count+" import="+fields[1]+" saveError="+err+" warnings="+warn;
     log.Add(entry);Console.WriteLine(entry);File.WriteAllLines(Path.Combine(root,"build_validation.txt"),log.ToArray());
    }
    IModelDoc2 assy=(IModelDoc2)sw.NewDocument("C:\\ProgramData\\SOLIDWORKS\\SOLIDWORKS 2024\\templates\\Assembly.ASMDOT",0,0,0);
    if(assy==null)throw new Exception("Assembly template failed");
    IAssemblyDoc a=(IAssemblyDoc)assy;
    IMathUtility mu=(IMathUtility)sw.GetMathUtility();
    foreach(string line in lines) {
     string name=line.Split('|')[0];
     var c=a.AddComponent5(Path.Combine(partsDir,name+".SLDPRT"),0,"",false,"",0,0,0);
     if(c==null)throw new Exception("Insert failed "+name);
     c.Transform2=(MathTransform)mu.CreateTransform(new double[]{1,0,0,0,1,0,0,0,1,0,0,0,1,0,0,0});
     c.Name2=name;
     c.Select4(false,null,false);a.FixComponent();assy.ClearSelection2(true);
    }
    assy.Extension.CustomPropertyManager[""].Add3("Status",30,"STATIC RECONSTRUCTION - fixed source pose; no motion mates; not production validated",2);
    assy.Extension.CustomPropertyManager[""].Add3("Units",30,"mm; scale cross-checked against 57 mm motor housings",2);
    assy.ForceRebuild3(false);assy.ShowNamedView2("*Isometric",7);assy.ViewZoomtofit2();
    if(!assy.Extension.SaveAs(asmPath,0,1,null,ref err,ref warn))throw new Exception("Assembly save error "+err);
    log.Add("Assembly saved: "+Path.GetFileName(asmPath)+" errors="+err+" warnings="+warn);
    log.Add("Components="+a.GetComponentCount(false));
    log.Add("Assembly box (m): "+string.Join(",",(double[])a.GetBox(0)));
    assy.SaveBMP(Path.Combine(root,"assembly_preview.bmp"),1600,1200);
    File.WriteAllLines(Path.Combine(root,"build_validation.txt"),log.ToArray());
    sw.UserControl=true;
    Console.WriteLine("COMPLETE: "+asmPath);
   } finally {
    sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlUnits,oldUnits);
    sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlModelType,oldType);
    sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swVrmlStlImportAsPSMesh,oldMesh);
   }
   return;
  }
  if(mode=="test") {
   sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlUnits,0);
   sw.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swImportStlVrmlModelType,2);
   sw.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swVrmlStlImportAsPSMesh,false);
   IModelDoc2 m=(IModelDoc2)sw.LoadFile4(Path.Combine(root,"exchange_parts","02_Base_plate.stl"),"",null,ref err);
   Console.WriteLine("Import error "+err+" model "+(m!=null));
   if(m!=null){
    object[] bodies=(object[])((IPartDoc)m).GetBodies2(0,false);
    Console.WriteLine("Solids "+(bodies==null?0:bodies.Length));
    Console.WriteLine("Box "+string.Join(",",(double[])((IPartDoc)m).GetPartBox(true)));
    m.ShowNamedView2("*Isometric",7);m.ViewZoomtofit2();
    Console.WriteLine("Save "+m.Extension.SaveAs(Path.Combine(root,"02_Base_plate_test.SLDPRT"),0,1,null,ref err,ref warn)+" err "+err);
    m.SaveBMP(Path.Combine(root,"base_plate_preview.bmp"),1400,1000);
   }
   return;
  }
  string original=Path.Combine(Path.GetDirectoryName(root),"foot_pedal.SLDPRT");
  IModelDoc2 doc=(IModelDoc2)sw.OpenDoc6(original,1,3,"",ref err,ref warn);
  Console.WriteLine("Open original: "+err+" warnings "+warn);
  if(doc!=null) {
   var part=(IPartDoc)doc;
   double[] box=(double[])part.GetPartBox(true);
   Console.WriteLine("Original footplate bounding box (m): "+string.Join(",",box));
   doc.ShowNamedView2("*Isometric",7);doc.ViewZoomtofit2();
   Console.WriteLine("Original features:");
   IFeature f=(IFeature)doc.FirstFeature();
   while(f!=null){Console.WriteLine(f.Name+" : "+f.GetTypeName2()); f=(IFeature)f.GetNextFeature();}
  }
 }
}
