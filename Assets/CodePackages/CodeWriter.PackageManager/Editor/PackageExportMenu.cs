using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeWriter.PackageManager
{
    static class PackageExportMenu
    {
        private static readonly string[] m_excludedPackages = new[]
        {
            "CodeWriter.PackageManager",
        };

        [MenuItem("Window/CodePackage/Export", validate = true)]
        public static bool CanExport()
        {
            return AssetDatabase.IsValidFolder(Helpers.currentPackageDirectoryInProject);
        }

        [MenuItem("Window/CodePackage/Export", priority = 50)]
        public static void Export()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Export " + Helpers.currentPackageName, "Exporting...", 0f);

                var referencedPackages = PackageManager.GetInstalledPackages(m_excludedPackages);
                var codePackage = new CodePackage
                {
                    name = Helpers.currentPackageName,
                    guid = System.Guid.NewGuid().ToString(),
                    references = System.Array.ConvertAll(referencedPackages, o => o.name),
                };

                var manifestPath = Helpers.CombinePath(Helpers.currentPackageDirectoryInProject, Helpers.currentPackageName + ".json");
                File.WriteAllText(manifestPath, JsonUtility.ToJson(codePackage, prettyPrint: true));
                AssetDatabase.ImportAsset(manifestPath);

                var packagePath = Helpers.CombinePath(Helpers.currentPackageDirectoryInProject, Helpers.currentPackageName + ".unitypackage");
                AssetDatabase.DeleteAsset(packagePath);

                AssetDatabase.ExportPackage(Helpers.currentPackageDirectoryInProject, packagePath, ExportPackageOptions.Recurse);
                AssetDatabase.ImportAsset(packagePath);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        [MenuItem("Window/CodePackage/Export Selected As...", validate = true)]
        public static bool CanExportSelected()
        {
            var selected = Selection.activeObject;
            if (selected == null)
                return false;

            var packagePath = AssetDatabase.GetAssetPath(selected);
            var packageDirectory = Helpers.FixPath(Path.GetDirectoryName(packagePath));
            if (!Helpers.CODEPACKAGES_FOLDER.Equals(packageDirectory))
                return false;

            return true;
        }

        [MenuItem("Window/CodePackage/Export Selected As...", priority = 51)]
        public static void ExportSelected()
        {
            var selected = Selection.activeObject;
            var packagePath = AssetDatabase.GetAssetPath(selected);
            var packageName = Path.GetFileName(packagePath);

            var exportPath = EditorUtility.SaveFilePanel("Export CodePackage", packageName, packageName, "unitypackage");
            if (string.IsNullOrEmpty(exportPath))
                return;

            AssetDatabase.ExportPackage(packagePath, exportPath, ExportPackageOptions.Recurse);
        }
    }
}