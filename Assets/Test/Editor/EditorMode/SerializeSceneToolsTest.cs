﻿using System.Collections.Generic;
using NUnit.Framework;
using Serializer3D;
using TrueSync;
using TrueSync.Physics3D;
using UnityEngine;

namespace Tests
{
    public class SerializeSceneToolsTest
    {
        [Test]
        public void TrueSyncSerWorld()
        {
            SerializeSceneTools.SerializeCurrScene();
        }

        [Test]
        public void TCheckGetConfigDirs()
        {
            //场景默认已经添加了配置文件
            var dirs = new List<string>();
            SerializeSceneTools.TCheckGetConfigDirs(Application.dataPath, ref dirs);
            Debug.Assert(dirs.Count > 0);
        }

        [Test]
        public void TCheckConfig()
        {
            var dirs = new List<string>();
            SerializeSceneTools.TCheckGetConfigDirs(Application.dataPath, ref dirs);
            var result = SerializeSceneTools.TCheckConfig<TrueSyncConfig>(dirs);
            Debug.Assert(result);
            result = SerializeSceneTools.TCheckConfig<GameObject>(dirs);
            Debug.Assert(!result);
        }

        [Test]
        public void TSerializeInit()
        {
            SerializeSceneTools.TSerializeInit();
            var world3d = (World) PhysicsWorldManager.instance.GetWorld();
            Debug.Assert(world3d != null);
        }

        [Test]
        public void IsSerializeComplete()
        {
            SerializeSceneTools.TSerializeInit();
            Debug.Assert(SerializeSceneTools.IsSerializeComplete);
        }
    }
}