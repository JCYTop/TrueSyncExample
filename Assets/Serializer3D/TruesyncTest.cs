#define Serializer

using TrueSync;
using UnityEngine;

namespace Serializer3D
{
    /// <summary>
    /// 初始化整个项目使用
    /// </summary>
    public class TruesyncTest : MonoBehaviour
    {
        public TrueSyncConfig TrueSyncGlobalConfig;
        private FP lockedTimeStep;
        private AbstractLockstep lockstep;
        private CoroutineScheduler scheduler;

        private void Awake()
        {
            //直接初始化
            var currentConfig = TrueSyncGlobalConfig;
            lockedTimeStep = currentConfig.lockedTimeStep;
            StateTracker.Init(currentConfig.rollbackWindow);
            TSRandom.Init();
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
#if Serializer
            Serializer();
#else
            Invoke("Deserializer", 0.1f);
#endif
        }

#if Serializer
        private void Serializer()
        {
            throw new System.NotImplementedException();
        }
#else
        private void Deserializer()
        {
            throw new System.NotImplementedException();
        }
#endif
    }
}