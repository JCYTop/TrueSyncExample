using NUnit.Framework;
using Serializer3D;
using TrueSync;
using TrueSync.Physics3D;
using UnityEngine;

namespace Tests
{
    public class TrueSyncTest
    {
        private bool isInit = false;
        private World world3D;

        private void InitWorld()
        {
            if (!isInit)
            {
                var currentConfig = new TrueSyncConfig();
                currentConfig.lockedTimeStep = 0.01667;
                currentConfig.physics3DEnabled = true;
                currentConfig.gravity3D = new TSVector(0, -10, 0);
                PhysicsManager.New(currentConfig);
                PhysicsManager.instance.Init();
                world3D = (World) PhysicsManager.instance.GetWorld();
                isInit = true;
                Debug.Log("执行完初始化");
            }
        }

        [Test]
        public void TrueSyncInitWorld()
        {
            InitWorld();
            Debug.Assert(world3D != null);
        }

        [Test]
        public void TrueSyncDeserializerTest()
        {
            //防止与其他测试代码冲突

            // InitWorld();
            // Debug.Assert(world3D != null);
            //
            // Debug.Log(world3D.Bodies().Count);
            // Debug.Assert(world3D.Bodies().Count == 0);

            // World3DSerializer.Deserialize(@"..\TrueSyncExample\3D.xml");
            // Debug.Assert(world3D.Bodies().Count != 0);
            // Debug.Log($" 加载之后的世界数量 : {world3D.Bodies().Count}");
            //
            // foreach (TrueSync.Physics3D.RigidBody body in world3D.Bodies())
            // {
            //     Debug.Log("--------------");
            //     Debug.Log(body.Shape);
            //     Debug.Log(body.Position);
            //     Debug.Log("++++++++++++++");
            // }
        }

        [Test]
        public void RayCastTest1()
        {
            #region 测试xml文件

//             <World3D World3DVer="1.0">
//   <Gravity>0 -9.8 0</Gravity>
//   <Entity Name="Sphere">
//     <Collider CollierType="TSSPHERE">
//       <Radius>2</Radius>
//       <ShapelossyScale>1 1 1</ShapelossyScale>
//       <BoundsMax>7 5 7</BoundsMax>
//       <BoundsMin>3 1 3</BoundsMin>
//       <IsTrigger>False</IsTrigger>
//       <Enabled>True</Enabled>
//       <Tag>Untagged</Tag>
//       <ColliderCenter>0 0 0</ColliderCenter>
//     </Collider>
//     <Rigibody>
//       <IsActive>True</IsActive>
//       <IsKinematic>False</IsKinematic>
//       <IsStatic>False</IsStatic>
//       <TSPosition>5 3 5</TSPosition>
//       <Shape>TrueSync.Physics3D.SphereShape</Shape>
//     </Rigibody>
//   </Entity>
//   <Entity Name="Cube">
//     <Collider CollierType="TSBOX">
//       <ColliderSize>2 2 2</ColliderSize>
//       <ShapelossyScale>2 2 2</ShapelossyScale>
//       <BoundsMax>3 3 3</BoundsMax>
//       <BoundsMin>-1 -1 -1</BoundsMin>
//       <IsTrigger>False</IsTrigger>
//       <Enabled>True</Enabled>
//       <Tag>Untagged</Tag>
//       <ColliderCenter>0 0 0</ColliderCenter>
//     </Collider>
//     <Rigibody>
//       <IsActive>True</IsActive>
//       <IsKinematic>False</IsKinematic>
//       <IsStatic>False</IsStatic>
//       <TSPosition>1 1 1</TSPosition>
//       <Shape>TrueSync.Physics3D.BoxShape</Shape>
//     </Rigibody>
//   </Entity>
//   <Entity Name="Cube">
//     <Collider CollierType="TSBOX">
//       <ColliderSize>2 2 2</ColliderSize>
//       <ShapelossyScale>1 1 1</ShapelossyScale>
//       <BoundsMax>-4 1 1</BoundsMax>
//       <BoundsMin>-6 -1 -1</BoundsMin>
//       <IsTrigger>False</IsTrigger>
//       <Enabled>True</Enabled>
//       <Tag>Untagged</Tag>
//       <ColliderCenter>0 0 0</ColliderCenter>
//     </Collider>
//     <Rigibody>
//       <IsActive>True</IsActive>
//       <IsKinematic>False</IsKinematic>
//       <IsStatic>False</IsStatic>
//       <TSPosition>-5 0 0</TSPosition>
//       <Shape>TrueSync.Physics3D.BoxShape</Shape>
//     </Rigibody>
//   </Entity>
// </World3D>

            #endregion

            InitWorld();
            Debug.Log(PhysicsManager.instance.Gravity);
            Debug.Log(world3D.Gravity);
            World3DSerializer.Deserialize(@"..\TrueSyncExample\3D.xml");
            Debug.Assert(world3D.Bodies().Count != 0);
            Debug.Log($" 加载之后的世界数量 : {world3D.Bodies().Count}");
            Debug.Log(PhysicsWorldManager.instance.Gravity);
            Debug.Log(world3D.Gravity);
            //射线检测
            IBody body;
            TSVector normal;
            FP fraction;

            var result = PhysicsWorldManager.instance.Raycast(TSVector.zero, new TSVector(0, 0, -0.1f), null, out body, out normal, out fraction);
            Debug.Assert(!result);

            result = PhysicsWorldManager.instance.Raycast(new TSVector(-5, 0, 0), new TSVector(0, 0, -1f), null, out body, out normal, out fraction);
            Debug.Assert(result);
        }
    }
}