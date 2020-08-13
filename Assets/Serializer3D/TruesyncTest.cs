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
            //直接使用不废话
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
        }
    }
}