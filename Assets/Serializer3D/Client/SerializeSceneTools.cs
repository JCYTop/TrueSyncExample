#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TrueSync;
using TrueSync.Physics3D;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Serializer3D
{
    /// <summary>
    /// 仅用于unity场景序列化
    /// 服务器禁止使用
    /// </summary>
    public static class SerializeSceneTools
    {
        private const string allowExtension = ".asset";

        private static HashSet<Type> colliderAllow = new HashSet<Type>()
        {
            typeof(BoxCollider),
            typeof(SphereCollider),
            typeof(CapsuleCollider),
            typeof(MeshCollider),
        };

        public static bool IsSerializeComplete
        {
            get { return (World) PhysicsManager.instance.GetWorld() != null; }
        }

        public static World World3D
        {
            get { return (World) PhysicsManager.instance.GetWorld(); }
        }

        [MenuItem("Tools/Serialize/SerializeCurrScene")]
        public static void SerializeCurrScene()
        {
            if (Application.isPlaying)
                throw new Exception("--->非Runtime<--- 使用");
            SerializeInit();
            var allGos = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            var previousSelection = Selection.objects;
            Selection.objects = allGos;
            var selectedTransforms = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            Selection.objects = previousSelection;
            foreach (var trans in selectedTransforms)
            {
                var collider = trans.GetComponent<Collider>();
                if (colliderAllow.Contains(collider.GetType()))
                {
                    switch (collider)
                    {
                        case BoxCollider box:
                            SerializeBox(trans, box);
                            break;
                        case SphereCollider sph:
                            SerializeSphere(trans, sph);
                            break;
                        case CapsuleCollider cap:
                            SerializeCapsule(trans, cap);
                            break;
                        case MeshCollider mesh:
                            SerializeMesh(trans, mesh);
                            break;
                    }
                }
            }

            //文件进行序列化
            World3DSerializer.Serialize(new Serializer3DGo(), World3D, $@"..\TrueSyncExample\Serializer\3D{EditorSceneManager.GetActiveScene().name}.xml");
            SerializeUnload();
            Debug.Log($"当前 --->{EditorSceneManager.GetActiveScene().name}<--- 场景序列化完成");
        }

        [MenuItem("Tools/Serialize/SerializeAllScene")]
        public static void SerializeAllScene()
        {
            if (Application.isPlaying)
                throw new Exception("--->非Runtime<--- 使用");
            //暂时写死场景 ^-^
            string[] sceneArr = {"Test",};
            var sceneRootPath = "Assets/Scenes/";
            for (int i = 0; i < sceneArr.Length; i++)
            {
                string sceneName = sceneArr[i];
                string fullPath = Path.Combine(sceneRootPath, sceneName) + ".unity";
                var scene = EditorSceneManager.OpenScene(fullPath, OpenSceneMode.Single);
                SerializeCurrScene();
                EditorSceneManager.CloseScene(scene, true);
            }

            Debug.Log("所有 --- 场景序列化完成");
        }

        private static void SerializeBox(Transform trans, BoxCollider box)
        {
            //trigger获取
            var istrigger = box.isTrigger;
            //scale基础缩放比例
            var lossyScale = TSVector.Abs(trans.localScale.ToTSVector());
            //直接获取unity数据
            var size = new TSVector(box.size.x, box.size.y, box.size.z);
            //获取trans的pos
            var center = GetCenter(trans, box);
            //生成shape类型
            var shape = new BoxShape(TSVector.Scale(size, lossyScale));
            RigibodySetting(shape, center, istrigger);
        }

        private static void SerializeSphere(Transform trans, SphereCollider sphere)
        {
            //trigger获取
            var istrigger = sphere.isTrigger;
            //scale基础缩放比例
            var lossyScale = TSVector.Abs(trans.localScale.ToTSVector());
            //获取unity数据
            var radius = FP.FromFloat(sphere.radius);
            //获取trans的pos
            var center = GetCenter(trans, sphere);
            //生成shape类型
            var shape = new SphereShape(radius);
            RigibodySetting(shape, center, istrigger);
        }

        private static void SerializeCapsule(Transform trans, CapsuleCollider capsule)
        {
            //trigger获取
            var istrigger = capsule.isTrigger;
            //scale基础缩放比例
            var lossyScale = TSVector.Abs(trans.localScale.ToTSVector());
            //获取unity数据
            var radius = FP.FromFloat(capsule.radius);
            var length = FP.FromFloat(capsule.height);
            //获取trans的pos
            var center = GetCenter(trans, capsule);
            //生成shape类型
            //TODO Mark 这里观察与实际表现有出入 这里暂定跟随框架设定使用 可能和人物判断有影响
            var shape = new CapsuleShape(length, radius);
            RigibodySetting(shape, center, istrigger);
        }

        private static void SerializeMesh(Transform trans, MeshCollider mesh)
        {
            //scale基础缩放比例
            var lossyScale = TSVector.Abs(trans.localScale.ToTSVector());
            //convex获取
            var convex = mesh.convex;
            //trigger获取
            var istrigger = mesh.isTrigger;
            //vertices获取
            var vertices =
                mesh.sharedMesh.vertices.Select(p => new TSVector(p.x * lossyScale.x, p.y * lossyScale.y, p.z * lossyScale.z)).ToList();
            var triangles = mesh.sharedMesh.triangles;
            var indices = new List<TriangleVertexIndices>();
            for (int i = 0; i < mesh.sharedMesh.triangles.Length; i += 3)
                indices.Add(new TriangleVertexIndices(triangles[i + 2], triangles[i + 1], triangles[i + 0]));
            //获取trans的pos
            var center = GetCenter(trans, mesh);
            var octree = new Octree(vertices, indices);
            var shape = new TriangleMeshShape(octree);
            RigibodySetting(shape, center, convex && istrigger);
        }

        private static TSVector GetCenter(Transform trans, Collider collider)
        {
            var transpos = trans.position.ToTSVector();
            var collidercenter = default(TSVector);
            switch (collider)
            {
                case BoxCollider box:
                    collidercenter = box.center.ToTSVector();
                    break;
                case SphereCollider sph:
                    collidercenter = sph.center.ToTSVector();
                    break;
                case CapsuleCollider cap:
                    collidercenter = cap.center.ToTSVector();
                    break;
            }

            return transpos + collidercenter;
        }

        private static void RigibodySetting<T>(T shape, TSVector center, bool istrigger) where T : Shape
        {
            var rigidBody = new RigidBody(shape);
            rigidBody.IsColliderOnly = istrigger;
            rigidBody.IsKinematic = false;
            rigidBody.AffectedByGravity = false;
            rigidBody.IsStatic = true;
            rigidBody.SetMassProperties();
            rigidBody.position = new TSVector(center.x, center.y, center.z);
            PhysicsManager.instance.AddBody(rigidBody);
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
            PhysicsManager.instance = null;
        }

        /// <summary>
        /// 遍历查找Project中符合要求的文件夹
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
    }

#endif
}