using System;
using TrueSync;
using TrueSync.Physics3D;
using UnityEngine;

namespace Serializer3D
{
    /// <summary>
    /// 初步尝试使用
    /// </summary>
    public class TruesyncTest : MonoBehaviour
    {
        public TrueSyncConfig TrueSyncGlobalConfig;
        private FP lockedTimeStep;
        private AbstractLockstep lockstep;
        private CoroutineScheduler scheduler;
        private World world3D;

        private void Awake()
        {
            //直接初始化
            var currentConfig = TrueSyncGlobalConfig;
            lockedTimeStep = currentConfig.lockedTimeStep;
            StateTracker.Init(currentConfig.rollbackWindow);
            // TSRandom.Init();
            if (currentConfig.physics2DEnabled || currentConfig.physics3DEnabled)
            {
                PhysicsManager.New(currentConfig);
                PhysicsManager.instance.LockedTimeStep = lockedTimeStep;
                PhysicsManager.instance.Init();
            }

            StateTracker.AddTracking(this, "time");
        }

        private void Start()
        {
            Application.runInBackground = true;
            Invoke("Serializer", 0.1f);
            // Invoke("Deserializer", 0.5f);
        }

        private void Serializer()
        {
            world3D = (World) PhysicsWorldManager.instance.GetWorld();
            if (world3D == null) throw new NullReferenceException();
            World3DSerializer.Serialize(new Serializer3DTS(), world3D, @"..\TrueSyncExample\Serializer\3D.xml");
        }

        private void Deserializer()
        {
            World3DSerializer.Deserialize(@"..\TrueSyncExample\Serializer\3D.xml");
        }
    }
}