using System.IO;
using System.Xml;
using TrueSync.Physics3D;

namespace Serializer3D
{
    public class Serializer3DGo : WorldSerializerBase
    {
        private XmlWriter writer;

        public void Serialize(World world, FileStream stream)
        {
            //setting
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;
            writer = XmlWriter.Create(stream, settings);

            writer.WriteStartElement("World3D");
            writer.WriteAttributeString("World3DVer", "1.0");
            writer.WriteVector("Gravity", world.Gravity);
            // 以下具体每个
            foreach (RigidBody body in world.RigidBodies)
            {
                // var go = PhysicsManager.instance.GetGameObject(body);
                // var collider = go.GetComponent<TSCollider>();
                // if (collider == null) throw new NullReferenceException();
                // //判断是否是激活状态
                // //只处理激活的
                // if (collider.enabled)
                // {
                //     writer.WriteStartElement("Entity");
                //     writer.WriteAttributeString("Name", collider.name);
                //     SerializeCollider(collider);
                //     SerializeRigibody(body);
                //     writer.WriteEndElement();
                // }
            }

            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }
    }
}