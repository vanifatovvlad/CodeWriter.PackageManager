using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CodeWriter.PackageManager
{
    class PackageManagerWindow : EditorWindow
    {
        [MenuItem("Window/CodePackage/Package Manager", priority = 0)]
        public static void ShowPackageManager()
        {
            var window = GetWindow<PackageManagerWindow>();
            window.titleContent = new GUIContent("CodePackages");
            window.Show();
        }

        private void OnGUI()
        {
            PackageManager.RefreshPackages();

            if (PackageManager.ready)
            {
                OnManagerGUI();
            }
            else
            {
                OnNotConfiguredGUI();
            }
        }

        private void OnFocus()
        {
            PackageManager.RefreshPackages();
        }

        private void OnNotConfiguredGUI()
        {
            GUILayout.FlexibleSpace();
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                GUILayout.Label("CodePackage Manager not configured", EditorStyles.largeLabel);
                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(3);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                using (new GUILayout.VerticalScope())
                {
                    if (GUILayout.Button("Select packages directory", GUILayout.Width(240), GUILayout.Height(22)))
                    {
                        PackageManager.packagesDirectory = EditorUtility.OpenFolderPanel("Select packages directory", null, null);
                    }
                }
                GUILayout.FlexibleSpace();
            }

            GUILayout.FlexibleSpace();
        }

        private void OnManagerGUI()
        {
            using (new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Space(1);
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton))
                {
                    PackageManager.RefreshPackages();
                }

                GUILayout.FlexibleSpace();

                GUILayout.Label(PackageManager.packagesDirectory, EditorStyles.toolbarButton);
                GUILayout.Space(5);
                if (GUILayout.Button("Reset", EditorStyles.toolbarButton))
                {
                    PackageManager.packagesDirectory = null;
                }
            }

            if (m_packageNamesToImport != null && m_packageNamesToImport.Length > 0)
            {
                using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    GUILayout.FlexibleSpace();
                    using (new GUILayout.VerticalScope())
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            var oldColor = GUI.color;
                            GUI.color = Color.green;
                            if (GUILayout.Button("Import " + m_packageNamesToImport[0], GUILayout.Width(220), GUILayout.Height(22)))
                            {
                                var packageName = m_packageNamesToImport[0];
                                m_packageNamesToImport = m_packageNamesToImport.Skip(1).ToArray();

                                EditorApplication.delayCall += () => PackageManager.ImportPackage(packageName);
                            }
                            GUI.color = oldColor;

                            if (GUILayout.Button("Cancel", GUILayout.Width(60), GUILayout.Height(22)))
                            {
                                m_packageNamesToImport = new string[0];
                            }
                        }

                        for (int i = 0; i < m_packageNamesToImport.Length; i++)
                        {
                            GUILayout.Label(m_packageNamesToImport[i], Content.centeredMiniLabel);
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
            }

            DrawList("Update available", PackageManager.changedInstalledPackages, "Update", Color.yellow, PrepareImportPackage);
            DrawList("Installed", PackageManager.actualInstalledPackages, "Uninstall", Color.white, package =>
            {
                if (EditorUtility.DisplayDialog("Uninstall package", "Are you really want to uninstall " + package.name + " package", "Uninstall", "Cancel"))
                {
                    EditorApplication.delayCall += () => PackageManager.UninstallPackage(package.name);
                }
            });
            DrawList("Not Instaled", PackageManager.notInstalledPackages, "Install", Color.white, PrepareImportPackage);
        }

        private void CollectPackageDeps(List<string> list, CodePackage package)
        {
            if (PackageManager.actualInstalledPackages.Any(p => p.name == package.name))
            {
                return;
            }

            foreach (var reference in package.references)
            {
                var refPackage = PackageManager.allPackages.SingleOrDefault(p => p.name == reference);
                if (refPackage == null)
                {
                    Debug.LogErrorFormat("Failed to resolve {0}", reference);
                }
                else
                {
                    CollectPackageDeps(list, refPackage);
                }
            }

            if (!list.Contains(package.name))
                list.Add(package.name);
        }

        private void PrepareImportPackage(CodePackage package)
        {
            var list = new List<string>();
            CollectPackageDeps(list, package);

            if (list.Count == 1)
            {
                EditorApplication.delayCall += () => PackageManager.ImportPackage(list[0]);
            }
            else
            {
                m_packageNamesToImport = list.ToArray();
            }
        }

        [SerializeField]
        private string[] m_packageNamesToImport;

        private void DrawList(string title, CodePackage[] packages, string actionTitle, Color actionColor, Action<CodePackage> action)
        {
            if (packages.Length == 0)
                return;

            GUILayout.Space(8);

            using (new GUILayout.VerticalScope(Content.CN_Box))
            {
                GUILayout.Label(title, EditorStyles.largeLabel);

                var oldColor = GUI.color;
                foreach (var package in packages)
                {
                    using (new GUILayout.HorizontalScope(Content.IN_BigTitie))
                    {
                        GUILayout.Label(package.name);
                        GUILayout.FlexibleSpace();

                        GUI.color = actionColor;
                        if (GUILayout.Button(actionTitle, EditorStyles.miniButton, GUILayout.Width(60)))
                        {
                            action(package);
                        }
                        GUI.color = oldColor;
                    }
                }
            }
        }
    }
}