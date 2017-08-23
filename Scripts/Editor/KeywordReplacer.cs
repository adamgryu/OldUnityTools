using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace QuickUnityTools.Editor {
    public class KeywordReplace : UnityEditor.AssetModificationProcessor {

        public static void OnWillCreateAsset(string path) {
            path = path.Replace(".meta", "");
            int index = path.LastIndexOf(".");
            if (index < 0)
                return;

            string file = path.Substring(index);
            if (file != ".cs" && file != ".js" && file != ".boo")
                return;

            index = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, index) + path;
            if (!System.IO.File.Exists(path))
                return;

            string fileContent = System.IO.File.ReadAllText(path);
            fileContent = fileContent.Replace("#NAMESPACE#", GetNamespaceForPath(path));

            System.IO.File.WriteAllText(path, fileContent);
            AssetDatabase.Refresh();
        }

        public static string GetNamespaceForPath(string path) {
            return Application.productName;
        }
    }
}
