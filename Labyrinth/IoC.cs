using Castle.Windsor;
using Castle.Windsor.Installer;

namespace Labyrinth
    {
    public static class IoC 
        {
        public static IWindsorContainer Instance { get; set; }

        public static void Create()
            {
            Dispose();

            Instance = new WindsorContainer();
            Instance.Install(FromAssembly.This());
            }

        public static T Resolve<T>() 
            {
            return Instance.Resolve<T>();
            }

        public static void Release<T>(T instance)
            {
            Instance.Release(instance);
            }

        public static void Dispose() 
            {
            if (Instance != null) 
                Instance.Dispose();
            Instance = null;
            }
        }
    }
