using System;
using System.Collections;
using System.Collections.Generic;
using Code.Configs;
using Gadsme;
using UnityEngine;

namespace Code
{
    public class GadsmeService : MonoBehaviour
    {
        public static GadsmeService Instance;
        private Camera _targetCamera;

        public void InitializeInstance()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance == this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);
        }

        public void InitializeService()
        {
            GadsmeSDK.SetAllowConsentDialog(false);
            GadsmeSDK.SetGdprConsent(UserConsentManager.Instance.HasUserConsent);
            GadsmeSDK.Init();
            DontDestroyOnLoad(gameObject);
        }

        public void OnGamePhaseChange(Camera newCamera)
        {
            PlacementVideos.Clear();
            PlacementBanners.Clear();
            GadsmeSDK.SetMainCamera(newCamera);
        }

        public void ChangeVideoBannersInteractivity(bool state)
        {
            StartCoroutine(ChangeVideoBannersInteractivityEnumerator(state));
        }

        private IEnumerator ChangeVideoBannersInteractivityEnumerator(bool state)
        {
            yield return null;

            foreach (var placementVideo in PlacementVideos)
            {
                placementVideo.clickInteraction = state;
            }

            foreach (var placementBanner in PlacementBanners)
            {
                placementBanner.clickInteraction = state;
            }
        }

        [field: SerializeField] public GadsmeConfiguration Configuration { get; set; }
        public bool GadsmeVideoClicked { get; set; }
        public Action GadsmeClickedCallback { get; set; }
        public List<GadsmePlacementVideo> PlacementVideos { get; } = new List<GadsmePlacementVideo>();
        public List<GadsmePlacementBanner> PlacementBanners { get; } = new List<GadsmePlacementBanner>();
    }
}