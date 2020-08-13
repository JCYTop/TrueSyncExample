using System;
using System.Collections.Generic;
using System.IO;
using TrueSync;
using TrueSync.Physics2D;
using World = TrueSync.Physics3D.World;

#if Serializer
namespace Serializer3D
{
    internal class World3DXmlDeserializer
    {
        private static World world3D = (World) PhysicsWorldManager.instance.GetWorld();
        // private  Dictionary<DeserializerData  , null > des  
        
        /// <summary>
        /// 服务器使用
        /// 使用前请确认调用InitZ()接口
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Deserializer(FileStream stream)
        {
            //解析
            var root = XMLFragmentParser.LoadFromStream(stream);
            if (root.Name.ToLower() != "world3d")
                throw new Exception("检查序列化Title。。。");
            //Read
            foreach (var entity in root.Elements)
            {
                //加载重力参数
                if (entity.Name.ToLower() == "gravity")
                {
                    world3D.Gravity = ReadVector(entity);
                }

                if (entity.Name.ToLower() == "entity")
                {
                    var data = new DeserializerData();
                    data.name = entity.Attributes[0].Value;
                    foreach (var type in entity.Elements)
                    {
                        if (type.Name.ToLower() == "collider")
                        {
                            switch ((TSCollierShape) Enum.Parse(typeof(TSCollierShape), type.Attributes[0].Value))
                            {
                                case TSCollierShape.TSBOX:
                                    DesColliderBox(ref data, type);
                                    break;
                                case TSCollierShape.TSCAPSULE:
                                    DesColliderCapsule(ref data, type);
                                    break;
                                case TSCollierShape.TSSPHERE:
                                    DesColliderSphere(ref data, type);
                                    break;
                                case TSCollierShape.TSMESH:
                                    DesColliderMesh(ref data, type);
                                    break;
                                case TSCollierShape.TSTERRAIN:
                                    DesColliderTerrain(ref data, type);
                                    break;
                            }

                            ComDesCollider(ref data, type);
                        }

                        if (type.Name.ToLower() == "rigibody")
                        {
                            ComDesRigibody(ref data, type);
                        }
                    }

                    //拿好数据准备进行加载世界
                    
                }
            }
        }

        #region Collider

        private static void DesColliderBox(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "collidersize")
                {
                    data.size = ReadVector(element);
                }
            }
        }

        private static void DesColliderCapsule(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "radius")
                {
                    data.radius = ReadVector(element);
                }

                if (element.Name.ToLower() == "length")
                {
                    data.length = ReadVector(element);
                }
            }
        }

        private static void DesColliderSphere(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "radius")
                {
                    data.radius = ReadVector(element);
                }
            }
        }

        private static void DesColliderMesh(ref DeserializerData data, XMLFragmentElement type)
        {
            throw new NotImplementedException();
        }

        private static void DesColliderTerrain(ref DeserializerData data, XMLFragmentElement type)
        {
            throw new NotImplementedException();
        }

        private static void ComDesCollider(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "shapelossyscale")
                {
                    data.lossyScale = ReadVector(element);
                }

                if (element.Name.ToLower() == "boundsmax")
                {
                    data.boundingmax = ReadVector(element);
                }

                if (element.Name.ToLower() == "boundsmin")
                {
                    data.boundingmin = ReadVector(element);
                }

                if (element.Name.ToLower() == "istrigger")
                {
                    data.istrigger = bool.Parse(element.Value);
                }

                if (element.Name.ToLower() == "enabled")
                {
                    data.enabled = bool.Parse(element.Value);
                }

                if (element.Name.ToLower() == "tag")
                {
                    data.tag = element.Value;
                }

                if (element.Name.ToLower() == "collidercenter")
                {
                    data.collidercenter = ReadVector(element);
                }
            }
        }

        #endregion

        #region Rigibody

        private static void ComDesRigibody(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "tsposition")
                {
                    data.tsposition = ReadVector(element);
                }
            }
        }

        #endregion

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static TSVector ReadVector(XMLFragmentElement node)
        {
            var values = node.Value.Split(' ');
            return new TSVector(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
    }

    internal class DeserializerData
    {
        public string name;

        #region Collider

        #region Box

        public TSVector size;

        #endregion

        #region Capsule || Sphere

        public TSVector radius;
        public TSVector length;

        #endregion

        public TSVector lossyScale;
        public TSVector boundingmax;
        public TSVector boundingmin;
        public bool istrigger;
        public bool enabled;
        public string tag;
        public TSVector collidercenter;

        #endregion

        #region Rigibody

        public TSVector tsposition;

        #endregion
    }
}
#endif