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
                Debug.Log(PhysicsManager.instance.Gravity);
                Debug.Log(world3D.Gravity);
                World3DSerializer.Deserialize(@"..\TrueSyncExample\3D.xml");
                Debug.Assert(world3D.Bodies().Count != 0);
                Debug.Log($" 加载之后的世界数量 : {world3D.Bodies().Count}");
                Debug.Log(PhysicsWorldManager.instance.Gravity);
                Debug.Log(world3D.Gravity);
                isDes = true;
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

        /// <summary>
        /// 需要自己设定下下，懒得敲代码设定了
        /// 测试场景：
        /// Cube
        /// position:(1,1,-10)
        /// rotation:(0,0,0)
        /// scale:(2,2,2)
        /// colliderPos:(0,0,0)
        /// colliderSize:(2,2,2)
        ///
        /// 射线起点（0，0，0）
        /// 射线方向（0，0，-1）
        ///
        /// 断言 true
        /// </summary>
        [UnityTest]
        public IEnumerator RayCastTest1()
        {
            InitWorld();
            DesComplete();
            //等待基础
            while (!isInit || !isDes)
            {
                yield return null;
            }

            var result = PhysicsWorldManager.instance.Raycast(TSVector.zero, TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(result);
        }

        /// <summary>
        /// 需要自己设定下下，懒得敲代码设定了
        /// 测试场景：
        /// Cube
        /// position:(1,5,-10)
        /// rotation:(0,0,0)
        /// scale:(2,2,2)
        /// colliderPos:(0,0,0)
        /// colliderSize:(2,2,2)
        ///
        /// 射线起点（0，0，0）
        /// 射线方向（0，0，-1）
        ///
        /// 断言 false
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

            var result = PhysicsWorldManager.instance.Raycast(TSVector.zero, TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
        }

        /// <summary>
        /// 需要自己设定下下，懒得敲代码设定了
        /// 测试场景：原点测试
        /// Cube
        /// position:(0,0,0)
        /// rotation:(0,0,0)
        /// scale:(2,2,2)
        /// colliderPos:(0,0,0)
        /// colliderSize:(2,2,2)
        ///
        /// 射线起点（0，0，0）
        /// 射线方向（0，0，-1）
        ///
        /// 断言 true
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

            var result = PhysicsWorldManager.instance.Raycast(TSVector.zero, TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(result);
        }

        /// <summary>
        /// 需要自己设定下下，懒得敲代码设定了
        /// 测试场景：原点测试
        /// Cube
        /// position:(0,0,0)
        /// rotation:(0,0,0)
        /// scale:(2,2,2)
        /// colliderPos:(0,2,0)
        /// colliderSize:(1,1,1)
        ///
        /// 射线起点（0，0，0）
        /// 射线方向（0，0，-1）
        ///
        /// 断言 false
        /// </summary>
        [UnityTest]
        public IEnumerator RayCastTest4()
        {
            InitWorld();
            DesComplete();
            //等待基础
            while (!isInit || !isDes)
            {
                yield return null;
            }

            var result = PhysicsWorldManager.instance.Raycast(TSVector.zero, TSVector.back, null, out body, out normal, out fraction);
            Debug.Assert(!result);
        }
    }
}