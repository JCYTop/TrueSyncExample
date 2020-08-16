using TrueSync;
using TrueSync.Physics3D;

namespace Serializer3D
{
    public class ServerTrueSync
    {
        private STrueSyncConfig initConfig = null;

        public STrueSyncConfig InitConfig
        {
            get
            {
                if (initConfig == null)
                {
                    initConfig = new STrueSyncConfig();
                }

                initConfig.lockedTimeStep = 0.01667;
                initConfig.physics3DEnabled = true;
                initConfig.gravity3D = new TSVector(0, -9.8f, 0);
                return initConfig;
            }
        }

        public World World3D
        {
            get { return (World) PhysicsManager.instance.GetWorld(); }
        }

        public PhysicsWorldManager Phy3DMgr
        {
            get
            {
                if (PhysicsManager.instance == null)
                {
                    ServerInit();
                }

                return PhysicsWorldManager.instance;
            }
        }

        /// <summary>
        /// init
        /// </summary>
        public void ServerInit()
        {
            PhysicsManager.New(InitConfig);
            PhysicsManager.instance.Init();
        }

        // public void Raycast()
        // {
        //     
        // }
    }
}