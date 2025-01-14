using System;

namespace ImmersiveVRTools.Runtime.Common.Utilities {
    public static class CommandLineArgs
    {
        //very simple implementation, will get next arg after the one specified, eg '--test-arg 123' will get 123 
        public static string GetNamed(string name)
        {
            var args = System.Environment.GetCommandLineArgs();
            for (var i = 0; i < args.Length; i++)
            {
                if (args[i] == name && args.Length > i + 1)
                {
                    return args[i + 1];
                }
            }
            return null;
        }
    }

    public interface ICommandLineOptions
    {
        object GenerateStaticInstance();
    };
    
    public abstract class CommandLineOptionsBase<T>: ICommandLineOptions
        where T: ICommandLineOptions, new()
    {
        protected abstract T GenerateStaticInstanceTyped();
        
        public object GenerateStaticInstance()
        {
            return GenerateStaticInstanceTyped();
        }
        
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = (T)new T().GenerateStaticInstance();
                }

                return _instance;
            }

            private set => _instance = value;
        }
    }
}