#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("pPTrWHS0Evk87SY7nG9PAG/SKdW+QCxU1kT0ITOzB64GMnLwIWdAzvdZimFcnQVxpUsec/7R3d/DtzMwH9h2/kXmvHIjwOCUs3PPVJ6ytfDRUlxTY9FSWVHRUlJT8i1mzXBYv5UE90DUBSMhKcJHvEwVplQ4BepZY9FScWNeVVp51RvVpF5SUlJWU1BjeUYMciw2iksjeAYP+yy5iLlR+eFa3U/XjcxliwTMXu/UHAbLCfK/C2dEdeXhok3FlX7ePWr482IaPmBQmbsRyJYLFMOv/WSj2vxa3SRkSgpbiKpPat1T8rko0Bh7foYJ6u5GunlYabusHB3H7Dz20N+YOxkpmpv9ul3CRLsUxDQgDfBJlhQJWFgWAVpU8h08Gq3SKlFQUlNS");
        private static int[] order = new int[] { 6,9,11,12,9,12,6,11,10,12,12,12,12,13,14 };
        private static int key = 83;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
