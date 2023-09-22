﻿using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UTUDLocalization
{
    public string Code;
    public string LocalizedText;
}

// UTUD stands for User Tracking Usage Description
[CreateAssetMenu(fileName = "UTUDLocalizationSettings", menuName = "GDPR/User Tracking Usage Description Localization")]
public class UTUDLocalizationSettings : ScriptableObject
{
    [SerializeField]
    private List<UTUDLocalization> _localizations;
    public List<UTUDLocalization> Localization => _localizations;
}
