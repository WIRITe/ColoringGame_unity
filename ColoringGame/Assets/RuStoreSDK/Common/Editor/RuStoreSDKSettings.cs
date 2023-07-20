using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RuStore.Editor {

    public class RuStoreSDKSettings : IPreprocessBuildWithReport {

        private static string LibPath = Path.Combine("Assets", "Plugins", "Android", "RuStoreSDKSettings.androidlib");
        private static string ValuesPath = Path.Combine(LibPath, "res", "values");
        private static string ValuesFileName = "values.xml";
        private static string ValuesFilePath = Path.Combine(ValuesPath, ValuesFileName);

        private static string ValuesTemplate = "<?xml version=\"1.0\" encoding=\"utf-8\"?><resources>{0}</resources>";
        private static string ValueStringTemplate = "<string name=\"{0}\" translatable=\"false\">{1}</string>";

        private static string ManifestFilePath = Path.Combine(LibPath, "AndroidManifest.xml");
        private static string ManifestFileContents = "<?xml version=\"1.0\" encoding=\"utf-8\"?><manifest xmlns:android=\"http://schemas.android.com/apk/res/android\" package=\"ru.rustore.unitysdk.settings\"></manifest>";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report) {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android) {
                return;
            }

            GenerateSettingsLibrary();
        }

        private void GenerateSettingsLibrary() {
            var baseType = typeof(RuStoreModuleSettings);
            var subclassTypes = Assembly.GetAssembly(baseType).GetTypes().Where(t => t.IsSubclassOf(baseType)).ToArray();

            var settings = new Dictionary<string, string>();

            var loadMethod = baseType.GetMethod(nameof(RuStoreModuleSettings.LoadAsset));

            foreach(var t in subclassTypes) {
                var asset = loadMethod.MakeGenericMethod(t).Invoke(null, null);

                if (asset != null) {
                    var properties = t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                    foreach (var p in properties) {
                        settings.Add("rustore_" + t.Name + "_" + p.Name, p.GetValue(asset).ToString());
                    }
                }
            }

            var settingStrings = new StringBuilder();
            foreach (var s in settings) {
                settingStrings.Append(string.Format(ValueStringTemplate, s.Key, s.Value));
            }

            var ValuesContents = string.Format(ValuesTemplate, settingStrings.ToString());

            if (!Directory.Exists(ValuesPath)) {
                Directory.CreateDirectory(ValuesPath);
            }

            if (!File.Exists(ManifestFilePath)) {
                File.WriteAllText(ManifestFilePath, ManifestFileContents);
                AssetDatabase.ImportAsset(ManifestFilePath);
            }

            if (File.Exists(ValuesFilePath)) {
                var oldValuesContents = File.ReadAllText(ValuesFilePath);

                if (ValuesContents == oldValuesContents) {
                    return;
                }
            } 

            File.WriteAllText(ValuesFilePath, ValuesContents);
            AssetDatabase.Refresh();
        }
    }
}