using System.IO;
using System.Xml;
using TrueSync;
using TrueSync.Physics3D;

namespace Serializer3D
{
    public interface WorldSerializerBase
    {
        void Serialize(World world, FileStream stream);
    }

    public static class SerializerBaseExtend
    {
        public static void WriteVector(this XmlWriter writer, string name, TSVector vec)
        {
            writer.WriteElementString(name, $"{vec.x} {vec.y} {vec.z}");
        }
    }
}