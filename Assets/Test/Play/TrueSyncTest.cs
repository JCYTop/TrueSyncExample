using System.Collections;
using NUnit.Framework;
using Serializer3D;
using TrueSync;
using TrueSync.Physics3D;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class TrueSyncTest
    {
        private bool isInit = false;
        private bool isDes = false;
        private World world3D;

        //射线检测
        private IBody body;
        private TSVector normal;
        private FP fraction;

        private void InitWorld()
        {
            if (!isInit)
            {
                var currentConfig = new STrueSyncConfig();
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

        private void DesComplete()
        {
            if (!isDes)
            {
                World3DSerializer.Deserialize($@"..\TrueSyncExample\Serializer\3D_Test.xml");
                Debug.Assert(world3D.Bodies().Count != 0);
                Debug.Log($" 加载之后的世界数量 : {world3D.Bodies().Count}");
                Debug.Log(PhysicsWorldManager.instance.Gravity);
                isDes = true;
            }
        }

        [Test]
        public void TrueSyncInitWorld()
        {
            InitWorld();
            Debug.Assert(world3D != null);
        }

        [UnityTest]
        public IEnumerator TrueSyncLoadXmlWorld()
        {
            InitWorld();
            DesComplete();
            while (!isInit || !isDes)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Cube
        /// position:(0,0,0)
        /// rotation:(0,0,0)
        /// scale:(2,2,2)
        /// colliderPos:(0,0,0)
        /// colliderSize:(2,2,2)
        /// </summary>
        [UnityTest]
        public IEnumerator RayCastTest1()
        {
            InitWorld();
            DesComplete();
            while (!isInit || !isDes)
            {
                yield return null;
            }

            var result = PhysicsWorldManager.instance.Raycast(new TSVector(0, 0, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(0, 100, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(0, 0, 100), TSVector.forward, null, out body, out normal, out fraction);
            Debug.Assert(!result);
        }

        /// <summary>
        /// Cube
        /// position:(10,0,0)
        /// rotation:(0,45,0)
        /// scale:(2,2,2)
        /// colliderPos:(0,0,0)
        /// colliderSize:(2,2,2)
        /// </summary>
        [UnityTest]
        public IEnumerator RayCastTest2()
        {
            InitWorld();
            DesComplete();
            //等待基础
            while (!isInit || !isDes)
            {
                yield return null;
            }

            var result = PhysicsWorldManager.instance.Raycast(new TSVector(10, 0, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(12.5f, 0, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(14, 0, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
        }

        /// <summary>
        /// Cube
        /// position:(20,0,0)
        /// rotation:(0,0,0)
        /// scale:(1,1,1)
        /// colliderPos:(0,5,0)
        /// colliderSize:(1,1,1)
        /// </summary>
        [UnityTest]
        public IEnumerator RayCastTest3()
        {
            InitWorld();
            DesComplete();
            //等待基础
            while (!isInit || !isDes)
            {
                yield return null;
            }

            var result = PhysicsWorldManager.instance.Raycast(new TSVector(20, 0, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(20, 3, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(20, 5, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(result);
            result = PhysicsWorldManager.instance.Raycast(new TSVector(20, 7, 100), TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
        }
    }
}