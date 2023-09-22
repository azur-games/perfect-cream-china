using System.Collections;
using Gadsme;
using UnityEngine;

namespace Code
{
    public class GadsmeBillBoard : MonoBehaviour
    {
        public GameObject defaultBanner;
        public GameObject videoBackground;
        public GadsmePlacementVideo gadsmePlacementVideo;
        public GadsmePlacementBanner gadsmePlacementBanner;

        private BillBoardType _billBoardType;

        public void Constructor(BillBoardType billBoardType)
        {
            _billBoardType = billBoardType;
        }
    }

    public enum BillBoardType
    {
        Video,
        Banner
    }
}