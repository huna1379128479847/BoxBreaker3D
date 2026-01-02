using BlockBreaker3D.Models.InGame;
using BlockBreaker3D.Models.InGame.Component.GameObjectComp;
using BlockBreaker3D.Utils;
using System.Collections.Generic;
using Zenject;

namespace BlockBreaker3D.Zenject
{
    public class ObjectInitializer : MonoInstaller
    {
            // TODO ; Model層に専用のクラスを作る
            // 一旦ここで代用
        public class ObjectInitializerInternal : IInitializable
        {
            private readonly List<IObject> _objects;
            private readonly List<ICompdata> _comps;

            public ObjectInitializerInternal(List<IObject> objects, List<ICompdata> comps)
            {
                _objects = objects;
                _comps = comps;
            }

            public void Initialize()
            {
                LogPainter.Debug($"ObjectInitializerInternal Initialize called. Objects count: {_objects.Count}, Comps count: {_comps.Count}", BColor.green);
                foreach (var obj in _objects)
                {
                    obj.Initialize();
                }
                foreach (var comp in _comps)
                {
                    comp.Construct();
                }
            }
        }
        public override void InstallBindings()
        {
            Container.Bind<IObject>().FromComponentsInHierarchy().AsTransient();
            Container.Bind<ICompdata>().FromComponentsInHierarchy().AsTransient();
            Container.BindInterfacesAndSelfTo<ObjectInitializerInternal>().AsSingle().NonLazy();
        }
    }
}