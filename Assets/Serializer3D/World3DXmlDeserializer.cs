using System;
using System.Collections.Generic;
using System.IO;
using TrueSync;
using TrueSync.Physics2D;
using TrueSync.Physics3D;
using Shape = TrueSync.Physics3D.Shape;

namespace Serializer3D
{
    internal class World3DXmlDeserializer
    {
        private Dictionary<DeserializerData, RigidBody> desDic = new Dictionary<DeserializerData, RigidBody>();

        /// <summary>
        /// 服务器使用
        /// 使用前请确认调用Init()接口
        /// </summary>
        /// <param name="stream"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Deserializer(FileStream stream)
        {
            desDic.Clear();
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
                    PhysicsWorldManager.instance.Gravity = ReadVector(entity);
                }

                if (entity.Name.ToLower() == "entity")
                {
                    var data = new DeserializerData();
                    data.name = entity.Attributes[0].Value;
                    foreach (var ent in entity.Elements)
                    {
                        if (ent.Name.ToLower() == "rigibody")
                        {
                            DesRigibody(ref data, ent);
                        }
                    }

                    Create(data);
                }
            }
        }

        private void DesRigibody(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "isactive")
                {
                    data.isactive = bool.Parse(element.Value);
                }

                if (element.Name.ToLower() == "iskinematic")
                {
                    data.iskinematic = bool.Parse(element.Value);
                }

                if (element.Name.ToLower() == "isstatic")
                {
                    data.isstatic = bool.Parse(element.Value);
                }

                if (element.Name.ToLower() == "tsposition")
                {
                    data.tsposition = ReadVector(element);
                }

                if (element.Name.ToLower() == "shape")
                {
                    switch ((TSCollierShape) Enum.Parse(typeof(TSCollierShape), element.Attributes[0].Value))
                    {
                        case TSCollierShape.TSBOX:
                            DesBox(ref data, element);
                            break;
                        case TSCollierShape.TSCAPSULE:
                            DesCapsule(ref data, element);
                            break;
                        case TSCollierShape.TSSPHERE:
                            DesSphere(ref data, element);
                            break;
                        case TSCollierShape.TSMESH:
                            DesMesh(ref data, element);
                            break;
                    }
                }
            }
        }

        private void DesBox(ref DeserializerData data, XMLFragmentElement type)
        {
            data.colliershape = TSCollierShape.TSBOX;
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "collidersize")
                {
                    data.size = ReadVector(element);
                }
            }

            ComDesCollider(ref data, type);
        }

        private void DesCapsule(ref DeserializerData data, XMLFragmentElement type)
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

            ComDesCollider(ref data, type);
        }

        private void DesSphere(ref DeserializerData data, XMLFragmentElement type)
        {
            data.colliershape = TSCollierShape.TSSPHERE;
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "radius")
                {
                    data.radius = FP.FromFloat(float.Parse(element.Value));
                }
            }

            ComDesCollider(ref data, type);
        }

        private void DesMesh(ref DeserializerData data, XMLFragmentElement type)
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

            ComDesCollider(ref data, type);
        }

        private void ComDesCollider(ref DeserializerData data, XMLFragmentElement type)
        {
            foreach (var element in type.Elements)
            {
                if (element.Name.ToLower() == "orientation")
                {
                    data.orientation = ReadMatrix(element);
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

                if (element.Name.ToLower() == "tag")
                {
                    data.tag = element.Value;
                }

                if (element.Name.ToLower() == "layer")
                {
                    data.layer = int.Parse(element.Value);
                }
            }
        }

        #region ADD ---> World

        private void Create(DeserializerData data)
        {
            var shape = default(Shape);
            switch (data.colliershape)
            {
                case TSCollierShape.TSBOX:
                    shape = new BoxShape(data.size);
                    break;
                case TSCollierShape.TSCAPSULE:
                    shape = new CapsuleShape(data.length, data.radius);
                    break;
                case TSCollierShape.TSSPHERE:
                    shape = new CapsuleShape(data.length, data.radius);
                    break;
                case TSCollierShape.TSMESH:
                    shape = new TriangleMeshShape(new Octree(data.vertices, data.indices));
                    break;
            }

            var rigidBody = new RigidBody(shape);
            rigidBody.IsColliderOnly = data.istrigger; //这里就是collider的trigger
            rigidBody.IsKinematic = data.iskinematic;
            rigidBody.AffectedByGravity = false; // TODO Mark ??? 我直接复制了
            rigidBody.IsStatic = true; // TODO Mark ??? 我直接复制了
            rigidBody.SetMassProperties(); // TODO Mark ??? 我直接复制了
            rigidBody.TSPosition = new TSVector(data.tsposition.x, data.tsposition.y, data.tsposition.z);
            rigidBody.Name = data.name;
            rigidBody.Tag = data.tag;
            rigidBody.Layer = data.layer;
            rigidBody.Orientation = data.orientation;
            desDic.Add(data, rigidBody);
            PhysicsManager.instance.AddBody(rigidBody);
        }

        #endregion

        private TSVector ReadVector(XMLFragmentElement node)
        {
            var values = node.Value.Split(' ');
            return new TSVector(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }

        private TSMatrix ReadMatrix(XMLFragmentElement node)
        {
            var values = node.Value.Split(' ');
            var matrix = new TSMatrix();
            matrix.M11 = float.Parse(values[0]);
            matrix.M12 = float.Parse(values[1]);
            matrix.M13 = float.Parse(values[2]);
            matrix.M21 = float.Parse(values[3]);
            matrix.M22 = float.Parse(values[4]);
            matrix.M23 = float.Parse(values[5]);
            matrix.M31 = float.Parse(values[6]);
            matrix.M32 = float.Parse(values[7]);
            matrix.M33 = float.Parse(values[8]);
            return matrix;
        }
    }

    /// <summary>
    /// 构造中间类 ---> 垃圾桶 不用care
    /// </summary>
    internal class DeserializerData
    {
        public string name;
        public bool isactive;
        public bool iskinematic;
        public bool isstatic;
        public TSVector tsposition;
        public TSCollierShape colliershape;
        public TSVector size;
        public TSMatrix orientation;
        public TSVector boundingmax;
        public TSVector boundingmin;
        public int layer;
        public FP radius;
        public FP length;
        public List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();
        public List<TSVector> vertices = new List<TSVector>();
        public bool istrigger;
        public string tag;
    }
}