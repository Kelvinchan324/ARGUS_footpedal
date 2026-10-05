using System;
using System.IO;
using System.Collections.Generic;
using SolidWorks.Interop.sldworks;

public class RelocateCad {
 public static void Run(string cadRoot) {
  cadRoot=Path.GetFullPath(cadRoot);
  ISldWorks sw=(ISldWorks)new SldWorksClass();
  var report=new List<string>();
  foreach(string assemblyPath in Directory.GetFiles(cadRoot,"*",SearchOption.AllDirectories)) {
   if(Path.GetFileName(assemblyPath).StartsWith("~$"))continue;
   if(!assemblyPath.EndsWith(".sldasm",StringComparison.OrdinalIgnoreCase))continue;
   // Only copied assemblies inside the explicitly supplied CAD directory may change.
   string folder=Path.GetDirectoryName(assemblyPath);
   string[] deps=(string[])sw.GetDocumentDependencies2(assemblyPath,false,false,false);
   if(deps==null){report.Add(Path.GetFileName(assemblyPath)+": no external dependencies returned");continue;}
   var candidates=new Dictionary<string,List<string>>(StringComparer.OrdinalIgnoreCase);
   foreach(string file in Directory.GetFiles(folder,"*",SearchOption.AllDirectories)) {
    string name=Path.GetFileName(file);
    if(!candidates.ContainsKey(name))candidates[name]=new List<string>();
    candidates[name].Add(file);
   }
   for(int i=1;i<deps.Length;i+=2) {
    string previous=deps[i],name=Path.GetFileName(previous),target=null;
    string sibling=Path.Combine(folder,name);
    if(File.Exists(sibling))target=sibling;
    else if(candidates.ContainsKey(name)&&candidates[name].Count==1)target=candidates[name][0];
    if(target==null){report.Add(Path.GetFileName(assemblyPath)+": unresolved or ambiguous reference "+name);continue;}
    if(string.Equals(previous,target,StringComparison.OrdinalIgnoreCase)) {
     report.Add(Path.GetFileName(assemblyPath)+": already local "+name);continue;
    }
    bool ok=sw.ReplaceReferencedDocument(assemblyPath,previous,target);
    report.Add(Path.GetFileName(assemblyPath)+": relocate "+name+" = "+ok);
    if(!ok)throw new Exception("Failed to relocate "+name+"; ensure the copied assembly is closed.");
   }
  }
  foreach(string entry in report)Console.WriteLine(entry);
  File.WriteAllLines(Path.Combine(cadRoot,"reference_audit.txt"),report.ToArray());
 }
}
