using UnityEngine;

namespace Code.RateUs
{
    [CreateAssetMenu(fileName = "RateUsConfiguration", menuName = "Rate Us Configuration", order = 0)]
    public class RateUsConfiguration : ScriptableObject
    {
        [field: SerializeField] public int MinLevelToShow { get; set; }
        [field: SerializeField] public int NumberReShowing { get; set; }
        [field: SerializeField] public int NumberSessionsForReShowing { get; set; }
        [field: SerializeField] public int MinTimeForReShowing { get; set; }
    }
}