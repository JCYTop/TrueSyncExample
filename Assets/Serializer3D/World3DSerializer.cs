using System;
using System.IO;
using TrueSync.Physics3D;

namespace Serializer3D
{
    /// <summary>
    /// 3D Serialize & Deserialize
    /// </summary>
    public class World3DSerializer
    {
        public static void Serialize<T>(T serializer, World world, string filename) where T : WorldSerializerBase
        {
            using (var fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                serializer.Serialize(world, fs);
            }
        }

        /// <summary>
        /// 反序列化
        /// 默认先执行场景的Init()
        /// </summary>
        /// <param name="filename"></param>
        public static void Deserialize(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                World3DXmlDeserializer.Deserializer(fs);
            }
        }
    }

    [Serializable]
    public enum TSCollierShape
    {
        TSBOX, // 盒子 
        TSCAPSULE, // 胶囊
        TSSPHERE, // 圆
        TSMESH, // Mesh
        TSTERRAIN, // 地形
    }
}