﻿using System;
using System.IO;
using System.Xml;
using JetBrains.Annotations;
using TrueSync;
using TrueSync.Physics3D;

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
                //判断是否是激活状态
                //只处理激活的
                if (collider.enabled)
                {
                    writer.WriteStartElement("Entity");
                    writer.WriteAttributeString("Name", collider.name);
                    SerializeCollider(collider);
                    SerializeRigibody(body);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
            writer.Flush();
            writer.Close();
        }

        #region collider序列化

        /// <summary>
        /// 获取Collider信息
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SerializeCollider(TSCollider collider)
        {
            writer.WriteStartElement("Collider");
            switch (collider)
            {
                case TSBoxCollider box:
                    SerBoxCollider(box);
                    break;
                case TSCapsuleCollider capsule:
                    SerCapsuleCollider(capsule);
                    break;
                case TSSphereCollider sphere:
                    SerSphereCollider(sphere);
                    break;
                case TSMeshCollider mesh:
                    SerMeshCollider(mesh);
                    break;
                case TSTerrainCollider terrain:
                    SerTerrainCollider(terrain);
                    break;
            }

            ComSerializeCollider(collider);
            writer.WriteEndElement();
        }

        /// <summary>
        /// BoxCollider序列化
        /// </summary>
        /// <param name="collider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void SerBoxCollider([NotNull] TSBoxCollider collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSBOX}");
            //collider 的 size
            //和shape有倍数关系
            WriteVector("ColliderSize", collider.size);
        }

        /// <summary>
        /// Capsule序列化
        /// </summary>
        /// <param name="collider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void SerCapsuleCollider([NotNull] TSCapsuleCollider collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSCAPSULE}");
            writer.WriteElementString("Radius", $"{collider.radius}"); // FP
            writer.WriteElementString("Length", $"{collider.length}"); // FP
        }

        /// <summary>
        /// Sphere序列化
        /// </summary>
        /// <param name="collider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void SerSphereCollider([NotNull] TSSphereCollider collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSSPHERE}");
            writer.WriteElementString("Radius", $"{collider.radius}"); // FP
        }

        /// <summary>
        /// Mesh序列化
        /// </summary>
        /// <param name="collider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void SerMeshCollider([NotNull] TSMeshCollider collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));
            //TODO 暂处理
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSMESH}");
            writer.WriteElementString("Mesh", $"{collider.Mesh}");
        }

        /// <summary>
        /// Terrain序列化
        /// </summary>
        /// <param name="collider"></param>
        /// <exception cref="ArgumentNullException"></exception>
        private static void SerTerrainCollider([NotNull] TSTerrainCollider collider)
        {
            if (collider == null) throw new ArgumentNullException(nameof(collider));
            writer.WriteAttributeString("CollierType", $"{TSCollierShape.TSTERRAIN}");
        }

        /// <summary>
        /// 通用Common处理
        /// </summary>
        /// <param name="collider"></param>
        private static void ComSerializeCollider(TSCollider collider)
        {
            WriteVector("ShapelossyScale", collider.SerializelossyScale);
            WriteVector("BoundsMax", collider.bounds.max);
            WriteVector("BoundsMin", collider.bounds.min);
            if (collider.tsMaterial != null)
            {
                writer.WriteElementString("Friction", collider.tsMaterial.friction.ToString());
                writer.WriteElementString("Restitution", collider.tsMaterial.restitution.ToString());
            }

            writer.WriteElementString("IsTrigger", collider.isTrigger.ToString());
            //Unity 可以过滤 这里只是场景激活的
            writer.WriteElementString("Enabled", collider.enabled.ToString());
            writer.WriteElementString("Tag", collider.tag); //Unity --> 服务器可能使用到
            WriteVector("ColliderCenter", collider.Center); //TSVector
        }

        #endregion

        #region Rigibody序列化信息

        /// <summary>
        /// 获取Rigibody里面的信息
        /// </summary>
        /// <param name="body"></param>
        /// <exception cref="NotImplementedException"></exception>
        private static void SerializeRigibody(RigidBody body)
        {
            writer.WriteStartElement("Rigibody");
            ComSerializeRigibody(body);
            writer.WriteEndElement();
        }

        private static void ComSerializeRigibody(RigidBody body)
        {
            writer.WriteElementString("IsActive", body.IsActive.ToString());
            writer.WriteElementString("IsKinematic", body.IsKinematic.ToString());
            writer.WriteElementString("IsStatic", body.IsStatic.ToString());
            //WriteVector("Position", body.Position) 这俩个一样
            WriteVector("TSPosition", body.TSPosition);
            writer.WriteElementString("Shape", body.Shape.ToString());
        }

        #endregion

        private static void WriteVector(string name, TSVector vec)
        {
            writer.WriteElementString(name, $"{vec.x} {vec.y} {vec.z}");
        }
    }
}
#endif