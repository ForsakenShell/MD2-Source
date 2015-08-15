using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Verse;

namespace MD2
{
    public static class BlueprintFiles
    {
        private static List<Blueprint> _allBlueprints = null;

        public static string SavedBlueprintsFolderPath
        {
            get
            {
                string result;
                try
                {
                    result = (string)typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[]
					{
						"MD2Droids"
					});
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to get blueprint save directory");
                    throw ex;
                }
                return result;
            }
        }
        public static IEnumerable<FileInfo> AllFiles
        {
            get
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(BlueprintFiles.SavedBlueprintsFolderPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                return
                    from f in directoryInfo.GetFiles()
                    where f.Extension == ".dbp"
                    orderby f.LastWriteTime descending
                    select f;
            }
        }
        public static string FilePathForSavedBlueprint(string blueprint)
        {
            return Path.Combine(BlueprintFiles.SavedBlueprintsFolderPath, blueprint + ".dbp");
        }
        public static bool HaveBlueprintNamed(string blueprint)
        {
            foreach (string current in
                from f in BlueprintFiles.AllFiles
                select Path.GetFileNameWithoutExtension(f.Name))
            {
                if (current == blueprint)
                {
                    return true;
                }
            }
            return false;
        }
        public static string UnusedDefaultName()
        {
            string text = String.Empty;
            int num = 1;
            do
            {
                text = "Blueprint" + num.ToString();
                num++;
            }
            while (BlueprintFiles.HaveBlueprintNamed(text));
            return text;
        }
        public static void SaveToFile(ref Blueprint bp, string fileName)
        {
            try
            {
                Scribe.InitWriting(BlueprintFiles.FilePathForSavedBlueprint(fileName), "blueprint");
                bp.BpName = fileName;
                Scribe_Deep.LookDeep(ref bp, "Blueprint");
            }
            catch (Exception e)
            {
                Log.Error("Failed to save blueprint");
                throw e;
            }
            finally
            {
                Scribe.FinalizeWriting();
                Scribe.mode = LoadSaveMode.Inactive;
            }
        }
        public static Blueprint LoadFromFile(string fileName)
        {
            Blueprint blueprint = new Blueprint();
            try
            {
                Scribe.InitLoading(BlueprintFiles.FilePathForSavedBlueprint(fileName));
                try
                {
                    Scribe_Deep.LookDeep(ref blueprint, "Blueprint", null);
                }
                catch (Exception e)
                {
                    Messages.Message("Error when loading blueprint", MessageSound.RejectInput);
                    Log.Error(e.ToString());
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
            finally
            {
                Scribe.mode = LoadSaveMode.Inactive;
            }
            return blueprint;
        }
    }
}
