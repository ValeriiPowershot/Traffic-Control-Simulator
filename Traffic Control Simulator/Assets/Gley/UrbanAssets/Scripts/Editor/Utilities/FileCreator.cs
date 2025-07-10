using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace Gley.UrbanAssets.Editor
{
    public class FileCreator
    {
        public static void CreateAgentTypesFile<T>(List<string> carCategories, string directive, string fileNamespace, string folderPath) where T : struct, IConvertible
        {
            if (carCategories == null)
            {
                carCategories = new List<string>();
                var allCarTypes = Enum.GetValues(typeof(T)).Cast<T>();
                foreach (T car in allCarTypes)
                {
                    carCategories.Add(car.ToString());
                }
            }

            CreateFolder("Assets" + folderPath);

            string text =
            "#if " + directive + "\n" +
            "namespace " + fileNamespace + "\n" +
            "{\n" +
            "\tpublic enum " + typeof(T).Name + "\n" +
            "\t{\n";
            for (int i = 0; i < carCategories.Count; i++)
            {
                text += "\t\t" + carCategories[i] + ",\n";
            }
            text += "\t}\n" +
                "}\n" +
                "#endif";

            File.WriteAllText(Application.dataPath + folderPath +"/"+ typeof(T).Name + ".cs", text);
            AssetDatabase.Refresh();
        }


        public static void CreateFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string[] folders = path.Split('/');
                string tempPath = "";
                for (int i = 0; i < folders.Length - 1; i++)
                {
                    tempPath += folders[i];
                    if (!AssetDatabase.IsValidFolder(tempPath + "/" + folders[i + 1]))
                    {
                        AssetDatabase.CreateFolder(tempPath, folders[i + 1]);
                        AssetDatabase.Refresh();
                    }
                    tempPath += "/";
                }
            }
        }
    }
}
