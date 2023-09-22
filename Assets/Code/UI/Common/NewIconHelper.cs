using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewIconHelper : MonoBehaviour
{
    [SerializeField] private List<ContentAsset.AssetType> listeningTypes;

    public void Refresh()
    {
        bool hasNewItems = Env.Instance.Inventory.IsHasNewItems(listeningTypes.ToArray());
        this.gameObject.SetActive(hasNewItems);
    }
}
