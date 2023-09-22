#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class AppleTangle
    {
        private static byte[] data = System.Convert.FromBase64String("LcZXE6mheQr5Eu6blbBlIEHVAdYaOywpfy4gOSBrWlpGTwpjREkEGy4sOSh/eRs5GjssKX8uIDkga1paBRqr6SwiASwrLy8tKCgaq5wwq5lTCktZWV9HT1kKS0lJT1peS0RJTyJ0GqgrOywpfzcKLqgrIhqoKy4aXV0ES1paRk8ESUVHBUtaWkZPSUsEaozdbWdVInQaNSwpfzcJLjIaPPMcVeutf/ONs5MYaNHy/1u0VIt4Na+przGzF20d2IOxaqQG/pu6OPJNpSKeCt3hhgYKRVqcFSsapp1p5QCsYqzdJysrLy8qGkgbIRojLCl/gvZUCB/gD//zJfxB/ogOCTvdi4ZVa4Ky0/vgTLYOQTv6iZHOMQDpNVAaqCtcGiQsKX83JSsr1S4uKSgrc40vI1Y9anw7NF75naEJEW2J/0U8Gj4sKX8uKTkna1paRk8KeEVFXl5DTENJS15PCkhTCktEUwpaS1heXkJFWENeUxs8Gj4sKX8uKTkna1rjM1jfdyT/VXWx2A8pkH+lZ3cn2xkccBpIGyEaIywpfy4sOSh/eRs5qCsqLCMArGKs3UlOLysaq9gaACwKRUwKXkJPCl5CT0QKS1paRkNJSy8qKagrJSoaqCsgKKgrKyrOu4MjBgpJT1heQ0xDSUteTwpaRUZDSVMsGiUsKX83OSsr1S4vGikrK9UaN7+0UCaObaFx/jwdGeHuJWfkPkP7Q0xDSUteQ0VECmtfXkJFWENeUxtaRk8KeEVFXgppaxo0PScaHBoeGEZPCmNESQQbDBoOLCl/LiE5N2taWkZPCmlPWF5DTENJS15DRUQKa18ltxfZAWMCMOLU5J+TJPN0NvzhFwpLRE4KSU9YXkNMQ0lLXkNFRApaIgEsKy8vLSgrPDRCXl5aWRAFBV2hM6P002FG3y2BCBoowjIU0noj+SwpfzckLjwuPgH6Q22+XCPU3kGngYlbuG15f+uFBWuZ0tHJWufMiWZOHwk/YT9zN5m+3dy2tOV6kOtyeg7IwfudWvUlb8sN4NtHUsfNnz09HxgbHhoZHHA9JxkfGhgaExgbHhpIRk8KWV5LRE5LWE4KXk9YR1kKSzW78TRtesEvx3RTrgfBHIh9Zn/GY/JctRk+T4tdvuMHKCkrKiuJqCsXDE0KoBlA3Seo5fTBiQXTeUBxTnhPRkNLRElPCkVECl5CQ1kKSU9YmxpyxnAuGKZCmaU39E9Z1U10T5afEIfeJSQquCGbCzwEXv8WJ/FIPAwaDiwpfy4hOTdrWlpGTwppT1heCmlrGqgrCBonLCMArGKs3ScrKyudMZe5aA44AO0lN5xntnRJ4mGqPaVZq0rsMXEjBbiY0m5i2koStD/flN5ZscT4TiXhU2Ue8ogU01LVQeInLCMArGKs3ScrKy8vKimoKysqdhyzZgdSncemsfbZXbHYXPhdGmXrGqgukRqoKYmKKSgrKCgrKBonLCPqSRld3RAtBnzB8CULJPCQWTNln29UNWZBerxro+5eSCE6qWutGaCrqj4B+kNtvlwj1N5BpwRqjN1tZ1VETgpJRUROQ15DRURZCkVMCl9ZT1hLSV5DSU8KWV5LXk9HT0ReWQQaeoCg//DO1vojLR2aX18L");
        private static int[] order = new int[] { 51,9,2,37,40,33,36,31,58,49,21,49,54,17,28,52,54,39,33,19,58,34,41,28,44,29,33,49,36,37,40,41,45,35,54,40,44,58,53,41,42,52,49,56,53,49,53,54,52,59,55,55,54,54,55,57,58,59,58,59,60 };
        private static int key = 42;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
