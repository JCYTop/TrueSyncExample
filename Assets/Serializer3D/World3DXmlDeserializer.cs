using System.IO;

#if !Serializer
namespace Serializer3D
{

    internal class World3DXmlDeserializer
    {
        /// <summary>
        /// 服务器使用
        /// 使用前请确认调用InitZ()接口
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Deserializer(FileStream stream)
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif