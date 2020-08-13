using System;
using System.Collections.Generic;
using System.IO;
using TrueSync;
using TrueSync.Physics2D;
using TrueSync.Physics3D;
using Shape = TrueSync.Physics3D.Shape;
using World = TrueSync.Physics3D.World;

#if Serializer
namespace Serializer3D
{
    internal class World3DXmlDeserializer
    {
        private static World world3D = (World) PhysicsWorldManager.instance.GetWorld();
        private static Dictionary<DeserializerData, RigidBody> desSet = new Dictionary<DeserializerData, RigidBody>();

        /// <summary>
        /// 服务器使用
        /// 使用前请确认调用InitZ()接口
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static void Deserializer(FileStream stream)
        {
            desSet.Clear();
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
                    DesAddWorld(data);
                }
            }
        }

        #region Collider

        private static void DesColliderBox(ref DeserializerData data, XMLFragmentElement type)
        {
            data.colliershape = TSCollierShape.TSBOX;
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
            data.colliershape = TSCollierShape.TSCAPSULE;
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "radius")
                {
                    data.radius = FP.FromFloat(float.Parse(element.Value));
                }

                if (element.Name.ToLower() == "length")
                {
                    data.length = FP.FromFloat(float.Parse(element.Value));
                }
            }
        }

        private static void DesColliderSphere(ref DeserializerData data, XMLFragmentElement type)
        {
            data.colliershape = TSCollierShape.TSSPHERE;
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "radius")
                {
                    data.radius = FP.FromFloat(float.Parse(element.Value));
                }
            }
        }

        private static void DesColliderMesh(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "indices")
                {
                    foreach (var indicese in element.Elements)
                    {
                        var value = indicese.Value.Split(' ');
                        data.indices.Add(new TriangleVertexIndices(int.Parse(value[0]), int.Parse(value[1]), int.Parse(value[2])));
                    }
                }

                if (element.Name.ToLower() == "vertices")
                {
                    foreach (var vertex in element.Elements)
                    {
                        data.vertices.Add(ReadVector(vertex));
                    }
                }
            }
        }

        private static void DesColliderTerrain(ref DeserializerData data, XMLFragmentElement type)
        {
            throw new NotImplementedException();
        }

        private static void ComDesCollider(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "Friction")
                {
                    data.friction = FP.FromFloat(float.Parse(element.Value));
                }

                if (element.Name.ToLower() == "Restitution")
                {
                    data.restitution = FP.FromFloat(float.Parse(element.Value));
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

                if (element.Name.ToLower() == "isKinematic")
                {
                    data.isKinematic = bool.Parse(element.Value);
                }
            }
        }

        #endregion

        #region ADD ---> World

        private static void DesAddWorld(DeserializerData data)
        {
            var rigibody = CreateShape(data);
            rigibody.Position = data.tsposition;
            desSet.Add(data, rigibody);
            PhysicsManager.instance.AddBody(rigibody);
        }

        private static RigidBody CreateShape(DeserializerData data)
        {
            var shape = default(Shape);
            switch (data.colliershape)
            {
                case TSCollierShape.TSBOX:
                    shape = new BoxShape(TSVector.Scale(data.size, data.lossyScale));
                    break;
                case TSCollierShape.TSCAPSULE:
                    shape = new CapsuleShape(data.length, data.radius);
                    break;
                case TSCollierShape.TSSPHERE:
                    shape = new CapsuleShape(data.length, data.radius);
                    break;
                case TSCollierShape.TSMESH:
                    var octree = new Octree(data.vertices, data.indices);
                    shape = new TriangleMeshShape(octree);
                    break;
                case TSCollierShape.TSTERRAIN:
                    //TODO 这里太长了之后看看
                    break;
            }

            var newBody = new RigidBody(shape);
            //这里具体情况具体赋值
            {
                if (data.friction > FP.MinValue)
                {
                    newBody.TSFriction = data.friction;
                }

                if (data.restitution > FP.MinValue)
                {
                    newBody.TSRestitution = data.restitution;
                }
            }
            newBody.IsColliderOnly = data.istrigger; //这里就是collider的trigger
            newBody.IsKinematic = data.isKinematic;
            // TODO Mark ??? 我直接复制了
            {
                // bool isStatic =
                newBody.AffectedByGravity = false;
                newBody.IsStatic = true;
                newBody.SetMassProperties();
            }
            return newBody;
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

    /// <summary>
    /// 构造中间类 ---> 垃圾桶
    /// </summary>
    internal class DeserializerData
    {
        public string name;
        public TSCollierShape colliershape;

        #region Collider

        #region Box

        public TSVector size;

        #endregion

        #region Capsule || Sphere

        public FP radius;
        public FP length;

        #endregion

        public TSVector lossyScale;
        public TSVector boundingmax;
        public TSVector boundingmin;
        public bool istrigger;
        public bool enabled;
        public string tag;
        public TSVector collidercenter;
        public FP friction = FP.MinValue;
        public FP restitution = FP.MinValue;

        #endregion

        #region Rigibody

        public TSVector tsposition;
        public bool isKinematic;

        #endregion

        #region Mesh

        public List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();
        public List<TSVector> vertices = new List<TSVector>();

        #endregion
    }
}
#endif