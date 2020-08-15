using System.Collections.Generic;
using System.IO;
using TrueSync;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Serializer3D
{
    /// <summary>
    /// 仅用于unity场景序列化
    /// 服务器禁止使用
    /// </summary>
    public static class SerializeSceneTools
    {
        private const string allowExtension = ".asset";

        [MenuItem("Tools/SerializeScene")]
        public static void SerializeScene()
        {
            SerializeInit();
            //暂时写死
            string[] sceneArr =
            {
                "Test",
            };

            string sceneRootPath = @"Assets/Scenes/";
            for (int i = 0; i < sceneArr.Length; i++)
            {
                var sceneName = sceneArr[i];
                var fullPath = Path.Combine(sceneRootPath, sceneName) + ".unity";
                var scene = EditorSceneManager.OpenScene(fullPath, OpenSceneMode.Single);
                var objects = scene.GetRootGameObjects();
                for (int j = 0; j < objects.Length; j++)
                {
                    Debug.LogFormat("<color=red>Scene : {0} ---> GameObject : {1}</color>", sceneName, objects[j].name);
                }

                EditorSceneManager.CloseScene(scene, true);
            }

            SerializeUnload();
        }

        /// <summary>
        /// 序列化场景之前准备的数据
        /// </summary>
        private static void SerializeInit()
        {
            var dirs = new List<string>();
            TrueSyncConfig config = null; //没有直接报错
            CheckGetConfigDirs(Application.dataPath, ref dirs);
            if (CheckConfig<TrueSyncConfig>(dirs))
            {
                var globalConfigPath = dirs[0];
                config = AssetDatabase.LoadAssetAtPath<TrueSyncConfig>(globalConfigPath);
            }

            PhysicsManager.New(config);
            PhysicsManager.instance.Init();
        }

        /// <summary>
        /// 序列化完成卸载数据
        /// </summary>
        private static void SerializeUnload()
        {
        }

        /// <summary>
        /// 遍历查找场Project中符合要求的文件夹
        /// </summary>
        /// <param name="dirPath">为要查找的总路径</param>
        /// <param name="dirs">保存路径</param>
        private static void CheckGetConfigDirs(string dirPath, ref List<string> dirs)
        {
            foreach (string path in Directory.GetFiles(dirPath))
            {
                //获取所有文件夹中包含后缀为 .prefab 的路径
                if (Path.GetExtension(path) == allowExtension)
                {
                    if (dirs != null)
                        dirs.Add(path.Substring(path.IndexOf("Assets")));
                    // Debug.Log(path.Substring(path.IndexOf("Assets")));
                }
            }

            if (Directory.GetDirectories(dirPath).Length > 0) //遍历所有文件夹
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    CheckGetConfigDirs(path, ref dirs);
                }
            }
        }

        /// <summary>
        /// 默认游戏场景就一个Config文件
        /// </summary>
        /// <param name="dirs"></param>
        /// <param name="limitcount"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static bool CheckConfig<T>(List<string> dirs, int limitcount = 1) where T : Object
        {
            var tmplist = new List<string>();
            foreach (var dir in dirs)
            {
                var res = AssetDatabase.LoadAssetAtPath<T>(dir);
                if (res != null)
                {
                    tmplist.Add(dir);
                }
            }

            if (tmplist.Count == limitcount)
            {
                return true;
            }

            return false;
        }

#if UNITY_EDITOR

        #region 测试使用接口

        public static void TCheckGetConfigDirs(string dirPath, ref List<string> dirs)
        {
            CheckGetConfigDirs(dirPath, ref dirs);
        }

        public static bool TCheckConfig<T>(List<string> dirs, int limitcount = 1) where T : Object
        {
            return CheckConfig<T>(dirs, limitcount);
        }

        public static void TSerializeInit()
        {
            SerializeInit();
        }

        #endregion

#endif
    }
}