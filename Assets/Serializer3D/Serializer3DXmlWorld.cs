using System;
using System.IO;
using System.Xml;
using TrueSync;
using TrueSync.Physics3D;

namespace Serializer3D
{
    /// <summary>
    /// 直接非运行下获取数据
    /// </summary>
    public class Serializer3DXmlWorld
    {
        protected XmlWriter writer;

        public void Serialize(World world, FileStream stream)
        {
            //setting
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;
            writer = XmlWriter.Create(stream, settings);

            writer.WriteStartElement("World3D");
            writer.WriteAttributeString("World3DVer", TimeTick().ToString());
            WriteVector("Gravity", world.Gravity);
            // 以下具体每个
            foreach (RigidBody body in world.RigidBodies)
            {
                writer.WriteStartElement("Entity");
                writer.WriteAttributeString("Name", body.Name);
                SerializeRigibody(body);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        private void SerializeRigibody(RigidBody body)
        {
            writer.WriteStartElement("Rigibody");
            ComSerializeRigibody(body);
            writer.WriteEndElement();
        }

        private void ComSerializeRigibody(RigidBody body)
        {
            writer.WriteElementString("IsActive", body.IsActive.ToString());
            writer.WriteElementString("IsKinematic", body.IsKinematic.ToString());
            writer.WriteElementString("IsStatic", body.IsStatic.ToString());
            WriteVector("TSPosition", body.TSPosition); //中心点
            SerializeShape(body);
        }

        private void SerializeShape(RigidBody body)
        {
            writer.WriteStartElement("Shape", body.ToString());
            switch (body.Shape)
            {
                case BoxShape box:
                    SerBoxShape(body, box);
                    break;
                case CapsuleShape capsule:
                    SerCapsuleShape(body, capsule);
                    break;
                case SphereShape sphere:
                    SerSphereShape(body, sphere);
                    break;
                case TriangleMeshShape mesh:
                    SerMeshShape(body, mesh);
                    break;
            }

            SerComShape(body, body.Shape);
            writer.WriteEndElement();
        }

        private void SerBoxShape(RigidBody body, BoxShape box)
        {
            if (box == null)
                throw new ArgumentNullException(nameof(box));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSBOX}");
            WriteVector("ColliderSize", box.Size);
        }

        private void SerCapsuleShape(RigidBody body, CapsuleShape capsule)
        {
            if (capsule == null)
                throw new ArgumentNullException(nameof(capsule));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSCAPSULE}");
            writer.WriteElementString("Radius", capsule.Radius.ToString()); // FP
            writer.WriteElementString("Length", capsule.Length.ToString()); // FP
        }

        private void SerSphereShape(RigidBody body, SphereShape sphere)
        {
            if (sphere == null)
                throw new ArgumentNullException(nameof(sphere));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSSPHERE}");
            writer.WriteElementString("Radius", sphere.Radius.ToString()); // FP
        }

        private void SerMeshShape(RigidBody body, TriangleMeshShape mesh)
        {
            if (mesh == null)
                throw new ArgumentNullException(nameof(mesh));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSMESH}");
            //Indices 写入
            writer.WriteStartElement("Indices");
            writer.WriteAttributeString("Count", mesh.Octree.Tris.Count.ToString());
            foreach (var indicese in mesh.Octree.Tris)
            {
                writer.WriteElementString("Item", $"{indicese.I0} {indicese.I1} {indicese.I2}");
            }

            writer.WriteEndElement();

            //Vertices 写入
            writer.WriteStartElement("Vertices");
            writer.WriteAttributeString("Count", mesh.Octree.Positions.Count.ToString());
            foreach (var vertex in mesh.Octree.Positions)
            {
                WriteVector("Item", vertex);
            }

            writer.WriteEndElement();
        }

        private void SerComShape(RigidBody body, Shape bodyShape)
        {
            WriteMatrix("Orientation", body.Orientation);
            WriteVector("BoundsMax", bodyShape.BoundingBox.max);
            WriteVector("BoundsMin", bodyShape.BoundingBox.min);
            writer.WriteElementString("IsTrigger", body.IsColliderOnly.ToString());
            writer.WriteElementString("Tag", body.Tag); //Unity --> 服务器可能使用到
            writer.WriteElementString("Layer", body.Layer.ToString()); //Unity --> 服务器可能使用到
        }

        /// <summary>
        /// 时间戳
        /// </summary>
        /// <returns></returns>
        public long TimeTick()
        {
            var ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public void WriteVector(string name, TSVector vec)
        {
            writer.WriteElementString(name, $"{vec.x} {vec.y} {vec.z}");
        }

        public void WriteMatrix(string name, TSMatrix matrix)
        {
            writer.WriteElementString(name,
                $"{matrix.M11} {matrix.M12} {matrix.M13} {matrix.M21} {matrix.M22} {matrix.M23} {matrix.M31} {matrix.M32} {matrix.M33}");
        }
    }
}