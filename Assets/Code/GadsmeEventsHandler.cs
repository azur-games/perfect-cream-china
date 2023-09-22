using System;
using Gadsme;
using Modules.Advertising;
using UnityEngine;

namespace Code
{
    public class GadsmeEventsHandler : MonoBehaviour
    {
        public GameStateEnum gameStateEnum;
        public GameObject defaultBanner;
        public bool adsLoaded;

        private void OnEnable()
        {
            GadsmeEvents.PlacementClickedEvent += OnPlacementClicked;
            GadsmeEvents.PlacementLoadedEvent += GetLoadedPlacement;
            GadsmeEvents.PlacementViewableEvent += GadsmeEventsOnPlacementViewableEvent;
            GadsmeEvents.PlacementVisibleEvent += GadsmeEventsOnPlacementVisibleEvent;
            GadsmeEvents.PlacementFailedEvent += GadsmeEventsOnPlacementFailedEvent;
            GadsmeEvents.PlacementNoAdEvent += GadsmeEventsOnPlacementNoAdEvent;
        }

        private void OnDisable()
        {
            GadsmeEvents.PlacementClickedEvent -= OnPlacementClicked;
            GadsmeEvents.PlacementLoadedEvent -= GetLoadedPlacement;
            GadsmeEvents.PlacementViewableEvent -= GadsmeEventsOnPlacementViewableEvent;
            GadsmeEvents.PlacementVisibleEvent -= GadsmeEventsOnPlacementVisibleEvent;
            GadsmeEvents.PlacementFailedEvent -= GadsmeEventsOnPlacementFailedEvent;
            GadsmeEvents.PlacementNoAdEvent -= GadsmeEventsOnPlacementNoAdEvent;
        }

        private void GadsmeEventsOnPlacementNoAdEvent(GadsmePlacement obj)
        {
            Debug.Log($"[GadsmeLog] GadsmeEventsOnPlacementNoAdEvent: {obj}");
        }

        private void GadsmeEventsOnPlacementFailedEvent(GadsmePlacement obj)
        {
            Debug.Log($"[GadsmeLog] GadsmeEventsOnPlacementFailedEvent: {obj}");
        }

        private void GadsmeEventsOnPlacementVisibleEvent(GadsmePlacement obj)
        {
            Debug.Log($"[GadsmeLog] GadsmeEventsOnPlacementVisibleEvent: {obj}");
        }

        private void GadsmeEventsOnPlacementViewableEvent(GadsmePlacement obj)
        {
            Debug.Log($"[GadsmeLog] GadsmeEventsOnPlacementViewableEvent: {obj}");
        }

        private void OnPlacementClicked(GadsmePlacement obj)
        {
            if(!adsLoaded) return;
            
            GadsmeService.Instance.GadsmeVideoClicked = true;
            GadsmeService.Instance.GadsmeClickedCallback?.Invoke();
        }

        private void GetLoadedPlacement(GadsmePlacement gadsmePlacement)
        {
            adsLoaded = true;
        }
    }

    public enum GameStateEnum
    {
        Gameplay,
        Metagame,
    }
}