using UnityEngine;

namespace Code.Configs
{
    [CreateAssetMenu(fileName = "GadsmeConfiguration", menuName = "Gadsme Configuration", order = 0)]
    public class GadsmeConfiguration : ScriptableObject
    {
        [field: SerializeField] public GameObject GameplayVideoBillBoard { get; set; }
        [field: SerializeField] public GameObject GameplayBannerBillBoard { get; set; }
        [field: SerializeField] public int NumberGaps { get; set; }
    }
}