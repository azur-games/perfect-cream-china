using Modules.General.HelperClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SoundConfigsContainer : ScriptableObject
{
    #region Fields

    [SerializeField] [ResourceLinkAttribute] List<AssetLink> soundConfigsAssetsLink;
	
	#endregion



	#region Properties

	public List<AssetLink> SoundConfigsAssetsLink
	{
		get
		{
			return soundConfigsAssetsLink;
		}

		set
		{
			if (soundConfigsAssetsLink != value)
			{
                soundConfigsAssetsLink = value;
			}
		}

	}

    #endregion
}
