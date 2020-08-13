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
            InitWorld();
            Debug.Assert(world3D != null);

            Debug.Log(world3D.Bodies().Count);
            Debug.Assert(world3D.Bodies().Count == 0);

            World3DSerializer.Deserialize(@"..\TrueSyncExample\3D.xml");
            Debug.Assert(world3D.Bodies().Count != 0);
            Debug.Log($" 加载之后的世界数量 : {world3D.Bodies().Count}");

            foreach (TrueSync.Physics3D.RigidBody body in world3D.Bodies())
            {
                Debug.Log("--------------");
                Debug.Log(body.Shape);
                Debug.Log(body.Position);
                Debug.Log("++++++++++++++");
            }
        }

        [Test]
        public void RayCastTest1()
        {
        }
    }
}