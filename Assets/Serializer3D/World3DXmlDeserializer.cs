using System;
using System.IO;
using TrueSync;
using TrueSync.Physics2D;
using UnityEngine;
using World = TrueSync.Physics3D.World;

#if Serializer
namespace Serializer3D
{
    internal class World3DXmlDeserializer
    {
        private static World world3D = (World) PhysicsWorldManager.instance.GetWorld();

        /// <summary>
        /// 服务器使用
        /// 使用前请确认调用InitZ()接口
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Deserializer(FileStream stream)
        {
            //解析
            var root = XMLFragmentParser.LoadFromStream(stream);
            if (root.Name.ToLower() != "world3d")
                throw new Exception("检查序列化Title。。。");
            //Read
            foreach (var entity in root.Elements)
            {
                //加载重力参数
                if (entity.Name.ToLower() == "gravity")
                {
                    world3D.Gravity = ReadVector(entity);
                }

                if (entity.Name.ToLower() == "entity")
                {
                    foreach (var type in entity.Elements)
                    {
                        if (type.Name.ToLower() == "collider")
                        {
                            Debug.Log(type.Attributes[0].Value);
                            switch (type.Attributes[0].Value)
                            {
                            }
                        }

                        if (type.Name.ToLower() == "rigibody")
                        {
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static TSVector ReadVector(XMLFragmentElement node)
        {
            var values = node.Value.Split(' ');
            return new TSVector(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
    }
}
#endif