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
        public static void Serialize(World world, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                var ser = new Serializer3DXmlWorld();
                ser.Serialize(world, fs);
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
                var des = new World3DXmlDeserializer();
                des.Deserializer(fs);
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
    }
}