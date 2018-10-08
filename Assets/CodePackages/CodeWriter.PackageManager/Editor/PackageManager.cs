using UnityEditor;
using System.Linq;
using UnityEngine;
using System.IO;

namespace CodeWriter.PackageManager
{
    class PackageManager
    {
        public static string packagesDirectory
        {
            get { return EditorPrefs.GetString("CodePackage.packagesDirectory", null); }
            set { EditorPrefs.SetString("CodePackage.packagesDirectory", Helpers.FixPath(value)); }
        }

        public static bool configured
        {
            get { return !string.IsNullOrEmpty(packagesDirectory); }
        }

        public static CodePackage[] allPackages { get; private set; }
        public static CodePackage[] actualInstalledPackages { get; private set; }
        public static CodePackage[] changedInstalledPackages { get; private set; }
        public static CodePackage[] notInstalledPackages { get; private set; }

        public static bool ready
        {
            get { return allPackages != null; }
        }

        [InitializeOnLoadMethod]
        public static void RefreshPackages()
        {
            if (string.IsNullOrEmpty(packagesDirectory))
                return;

            allPackages = Helpers.GetDirectories(packagesDirectory)
                .Select(path =>
                {
                    var packageName = GetPackageName(path);
                    return Helpers.CombinePath(path, Helpers.CODEPACKAGES_FOLDER, packageName, packageName + ".json");
                })
                .Where(path => File.Exists(path))
                .Select(path => LoadCodePackage(path))
                .Where(pack => pack.name != Helpers.currentPackageName)
                .ToArray();

            var installedPackages = GetInstalledPackages();

            changedInstalledPackages = allPackages
                .Where(pack =>
                {
                    var installed = installedPackages.SingleOrDefault(p => p.name == pack.name);
                    return installed != null && installed.guid != pack.guid;
                })
                .ToArray();

            actualInstalledPackages = allPackages
                .Where(pack =>
                {
                    var installed = installedPackages.SingleOrDefault(p => p.name == pack.name);
                    return installed != null && installed.guid == pack.guid;
                })
                .ToArray();

            notInstalledPackages = allPackages
                .Where(pack =>
                {
                    var installed = installedPackages.SingleOrDefault(p => p.name == pack.name);
                    return installed == null;
                })
                .ToArray();
        }

        public static CodePackage[] GetInstalledPackages(string[] excludedPackages = null)
        {
            return Helpers.GetDirectories(Helpers.customPackagesDirectory)
                .Select(path => Helpers.CombinePath(path, GetPackageName(path) + ".json"))
                .Where(path => File.Exists(path))
                .Select(path => LoadCodePackage(path))
                .Where(pack => pack.name != Helpers.currentPackageName)
                .Where(pack => excludedPackages == null || !excludedPackages.Contains(pack.name))
                .ToArray();
        }

        private static string GetPackageName(string path)
        {
            var ind = path.LastIndexOf('/');
            return path.Substring(ind + 1);
        }

        private static CodePackage LoadCodePackage(string path)
        {
            var json = File.ReadAllText(path);
            return JsonUtility.FromJson<CodePackage>(json);
        }

        public static void ImportPackage(string packageName)
        {
            var packagePath = Helpers.CombinePath(packagesDirectory, packageName, Helpers.CODEPACKAGES_FOLDER, packageName, packageName + ".unitypackage");
            AssetDatabase.ImportPackage(packagePath, true);
        }

        public static void UninstallPackage(string packageName)
        {
            var path = Helpers.CombinePath(Helpers.CODEPACKAGES_FOLDER, packageName);
            AssetDatabase.DeleteAsset(path);
        }
    }
}