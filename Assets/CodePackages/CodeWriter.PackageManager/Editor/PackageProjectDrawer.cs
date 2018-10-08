using UnityEditor;
using UnityEngine;

namespace CodeWriter.PackageManager
{
    [InitializeOnLoad]
    class PackageProjectDrawer
    {
        static PackageProjectDrawer()
        {
            EditorApplication.projectWindowItemOnGUI += (string guid, Rect rect) =>
            {
                if (rect.height > 50)
                    return;

                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (!path.StartsWith(Helpers.CODEPACKAGES_FOLDER))
                    return;

                if (path.Length == Helpers.CODEPACKAGES_FOLDER.Length)
                {
                    if (!string.IsNullOrEmpty(PackageManager.packagesDirectory))
                    {
                        if (!PackageManager.ready)
                            PackageManager.RefreshPackages();

                        var buttonRect = new Rect(rect) { xMin = rect.xMax - 20, width = 20, height = 20 };
                        if (PackageManager.changedInstalledPackages.Length > 0)
                        {
                            if (GUI.Button(buttonRect, "", Content.warnIconSmall))
                            {
                                PackageManagerWindow.ShowPackageManager();
                            }
                        }
                        else
                        {
                            var color = GUI.color;
                            GUI.color = new Color(color.r, color.g, color.b, 0.35f);
                            if (GUI.Button(buttonRect, "", Content.infoIconSmall))
                            {
                                PackageManagerWindow.ShowPackageManager();
                            }
                            GUI.color = color;
                        }
                    }

                    return;
                }

                var pathStart = Helpers.CODEPACKAGES_FOLDER.Length + 1;
                var projectName = Helpers.currentPackageName;
                if (string.Compare(path, pathStart, projectName, 0, projectName.Length) == 0)
                    return;

                GUI.Box(rect, "", Content.projectWindowGreyOverlay);
            };
        }
    }
}
