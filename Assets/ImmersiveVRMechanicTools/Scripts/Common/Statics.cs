namespace ImmersiveVRTools.Runtime.Common
{
    public class Layer
    {
        public static int IgnoreRaycast = 2;
    }

    public class CustomLayer: Layer
    {
    }

    public class ObsoleteReasons
    {
        public const string MirrorSerializationEmptyCtorAndRawFieldsRequired =
            "Mirrors requires empty ctor and fields (not properties) for proper serialization";
    }
}
