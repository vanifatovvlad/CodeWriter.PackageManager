using System;
using System.IO;
using UnityEngine;

namespace CodeWriter.PackageManager
{
    static class Helpers
    {
        public const string CODEPACKAGES_FOLDER = "Assets/CodePackages";

        private static string m_currentPackageName;
        private static string m_currentPackageDirectoryInProject;
        private static string m_customPackagesDirectory;

        public static string currentPackageName
        {
            get
            {
                if (m_currentPackageName == null)
                {
                    var dataPathParts = Application.dataPath.Split('/');
                    m_currentPackageName = dataPathParts[dataPathParts.Length - 2];
                }
                return m_currentPackageName;
            }
        }

        public static string currentPackageDirectoryInProject
        {
            get
            {
                if (m_currentPackageDirectoryInProject == null)
                {
                    m_currentPackageDirectoryInProject = CODEPACKAGES_FOLDER + "/" + currentPackageName;
                }
                return m_currentPackageDirectoryInProject;
            }
        }

        public static string customPackagesDirectory
        {
            get
            {
                if (m_customPackagesDirectory == null)
                {
                    var path = Application.dataPath;
                    path = path.Remove(path.Length - "Assets".Length);
                    m_customPackagesDirectory = path + CODEPACKAGES_FOLDER;
                }
                return m_customPackagesDirectory;
            }
        }

        public static string FixPath(string path)
        {
            if (path == null)
                return string.Empty;

            path = path.Replace('\\', '/');

            if (path.EndsWith("/"))
                path = path.Remove(path.Length - 1);

            return path;
        }

        public static string[] GetDirectories(string path)
        {
            return Array.ConvertAll(Directory.GetDirectories(path), FixPath);
        }

        public static string CombinePath(params string[] pathes)
        {
            switch (pathes.Length)
            {
                case 0: return "";
                case 1: return pathes[0];
                default: return string.Join("/", pathes);
            }
        }
    }
}