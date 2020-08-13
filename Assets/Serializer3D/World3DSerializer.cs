using System.IO;
using TrueSync.Physics3D;

namespace Serializer3D
{
    public class World3DSerializer
    {
        /// <summary>
        /// 3D Serialize & Deserialize
        /// </summary>
        public static void Serialize(World world, string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Create))
            {
                World3DXmlSerializer.Serialize(world, fs);
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
}