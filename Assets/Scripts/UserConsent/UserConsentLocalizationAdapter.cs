using System;

/// <summary>
/// Implement methods to provide localized strings
/// </summary>
public class UserConsentLocalizationAdapter 
{
    public static UserConsentLocalizationAdapter Instance => instance ??= new UserConsentLocalizationAdapter();

    private static UserConsentLocalizationAdapter instance;

    public string GetGDPRHeaderLocalized()
    {
       return "We've updated our Terms";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }
    
    public string GetGDPRHeaderLocalizedIOS()
    {
      return "By using this app I agree to Azur Games’ Terms of Use and confirm that I have read Privacy Policy";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }
    
    public string GetGDPRDescriptionLocalized()
    {
       return "By using this app I agree to Azur Games’ Terms of Use and confirm that I have read Privacy Policy";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }
    
    public string GetIDFADescriptionLocalizedInEditor()
    {
        return "Text";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }

    public string GetGDPRTermsButtonTextLocalized()
    {
     return "Terms of use";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }

    public string GetGDPRPrivacyButtonTextLocalized()
    {
       return "Privacy Policy";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }

    public string GetGDPRDescriptionLocalizedIOS()
    {
      return "Press “Continue” to start using this app";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }

    public string GetAcceptText()
    {
       return "Accept";
        throw new NotImplementedException("It's required to connect your localization system to User agreement");
    }
}
