using System;
using System.IO;
using System.Xml;
using JetBrains.Annotations;
using TrueSync;
using TrueSync.Physics3D;
using UnityEngine;

#if Serializer
namespace Serializer3D
{
    internal static class World3DXmlSerializer
    {
        private static XmlWriter writer;

        /// <summary>
        /// 客户端使用
        /// 暂时运行时生效
        /// </summary>
        /// <param name="world">3D World</param>
        /// <param name="stream"></param>
        public static void Serialize(World world, FileStream stream)
        {
            //setting
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = true;

            writer = XmlWriter.Create(stream, settings);
            writer.WriteStartElement("World3D");
            writer.WriteAttributeString("World3DVer", "1.0");
            WriteVector("Gravity", world.Gravity);
            // 以下具体每个
            foreach (RigidBody body in world.RigidBodies)
            {
                var go = PhysicsManager.instance.GetGameObject(body);
                var collider = go.GetComponent<TSCollider>();
                if (collider == null) throw new NullReferenceException();
                writer.WriteStartElement("Entity");
                writer.WriteAttributeString("Name", collider.name);
                SerializeCollider(collider);
                SerializeRigibody(body);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        /// <summary>
        /// 获取Collider信息
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SerializeCollider(TSCollider collider)
        {
            switch (collider)
            {
                case TSBoxCollider boxCollider:
                    BoxCollider((TSBoxCollider) collider);
                    break;
                case TSCapsuleCollider boxCollider:
                    break;
                case TSSphereCollider boxCollider:
                    break;
                case TSMeshCollider boxCollider:
                    break;
                case TSTerrainCollider boxCollider:
                    break;
            }

            ComSerializeCollider(collider);
        }

        /// <summary>
        /// BoxCollider特殊序列化
        /// </summary>
        /// <param name="collider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void BoxCollider([NotNull] TSBoxCollider collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));
            writer.WriteAttributeString("CollierType", $"{collider.Shape}");
            //collider 的 size
            //和shape有倍数关系
            WriteVector("ColliderSize", collider.size);
        }

        /// <summary>
        /// 通用Common处理
        /// </summary>
        /// <param name="collider"></param>
        private static void ComSerializeCollider(TSCollider collider)
        {
            WriteVector("ShapelossyScale", collider.SerializelossyScale);
        }

        /// <summary>
        /// 获取Rigibody里面的信息
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SerializeRigibody(RigidBody body)
        {
            throw new System.NotImplementedException();
        }

        private static void WriteVector(string name, TSVector vec)
        {
            writer.WriteElementString(name, $"{vec.x} {vec.y} {vec.z}");
        }
    }
}
#endif