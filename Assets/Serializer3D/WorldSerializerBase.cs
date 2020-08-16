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

        public static void WriteMatrix(this XmlWriter writer, string name, TSMatrix matrix)
        {
            writer.WriteElementString(name,
                $"{matrix.M11} {matrix.M12} {matrix.M13} {matrix.M21} {matrix.M22} {matrix.M23} {matrix.M31} {matrix.M32} {matrix.M33}");
        }
    }
}