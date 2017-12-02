////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

public class UMEditorMenu : EditorWindow {
	
	//--------------------------------------
    //  GENERAL
    //--------------------------------------

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Edit Settings", false, 0)]
    public static void Edit() {
        Selection.activeObject = UltimateMobileSettings.Instance;
    }

    //--------------------------------------
    //  GETTING STARTED
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Getting Started/About", false, 0)]
    public static void UMGSOverview() {
		string url = "https://unionassets.com/ultimate-mobile/overview-218";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Getting Started/Setup and Update", false, 0)]
    public static void UMGSSetupAndUpdate() {
		string url = "https://unionassets.com/ultimate-mobile/setup-and-update-219";
        Application.OpenURL(url);
    }

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Getting Started/Features and Compatibility", false, 0)]
	public static void UMGSFeaturesCompatibility() {
		string url = "https://unionassets.com/ultimate-mobile/features-and-compatibility-535";
		Application.OpenURL(url);
	}

    //--------------------------------------
    //  IN-APP PURCHACES
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/In-App Purchases/Setup", false, 0)]
    public static void UMIASetup() {
        string url = "https://unionassets.com/ultimate-mobile/setup-225";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/In-App Purchases/Coding Guidelines", false, 0)]
    public static void UMIACodingGuidelines() {
        string url = "https://unionassets.com/ultimate-mobile/coding-guidelines-226";
        Application.OpenURL(url);
    }

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/In-App Purchases/Platform behavior differences", false, 0)]
	public static void UMIAPlatformDifferences() {
		string url = "https://unionassets.com/ultimate-mobile/platform-behavior-differences-591";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/In-App Purchases/Restoring Purchases", false, 0)]
	public static void UMIARestoringPurchases() {
		string url = "https://unionassets.com/ultimate-mobile/restoring-purchases-592";
		Application.OpenURL(url);
	}

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/In-App Purchases/Validation", false, 0)]
    public static void UMIAValidation() {
        string url = "https://unionassets.com/ultimate-mobile/validation-434";
        Application.OpenURL(url);
    }

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/In-App Purchases/API Reference", false, 0)]
	public static void UMIAAPIReference() {
		string url = "https://unionassets.com/ultimate-mobile/api-reference-593";
		Application.OpenURL(url);
	}

    //--------------------------------------
    //  GAME SERVICES
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/Setup", false, 0)]
    public static void UMGSSetup() {
        string url = "https://unionassets.com/ultimate-mobile/setup-227";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/Sing in", false, 0)]
    public static void UMGSSingIn() {
        string url = "https://unionassets.com/ultimate-mobile/sing-in-251";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/Achievements", false, 0)]
    public static void UMAGSAchievements() {
        string url = "https://unionassets.com/ultimate-mobile/achievements-253";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/Leaderboards", false, 0)]
    public static void UMGSLeaderboards() {
        string url = "https://unionassets.com/ultimate-mobile/leaderboards-252";
        Application.OpenURL(url);
    }

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/Friends", false, 0)]
	public static void UMGSFriends() {
		string url = "https://unionassets.com/ultimate-mobile/friends-493";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/Turn-Based Multiplayer", false, 0)]
	public static void UMGSTBM() {
		string url = "https://unionassets.com/ultimate-mobile/turn-based-multiplayer-515";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Game Services/API Reference", false, 0)]
	public static void UMGSAPIReference() {
		string url = "https://unionassets.com/ultimate-mobile/game-services-api-reference-494";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  Social
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Social/Facebook", false, 0)]
	public static void UMFacebook() {
		string url = "https://unionassets.com/ultimate-mobile/facebook-555";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Social/Twitter", false, 0)]
	public static void UMTwitter() {
		string url = "https://unionassets.com/ultimate-mobile/twitter-556";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Social/Instagram", false, 0)]
	public static void UMInstagram() {
		string url = "https://unionassets.com/ultimate-mobile/instagram-557";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Social/Whatsapp", false, 0)]
	public static void UMWhatsapp() {
		string url = "https://unionassets.com/ultimate-mobile/whatsapp-558";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Social/E-mail", false, 0)]
	public static void UMEmail() {
		string url = "https://unionassets.com/ultimate-mobile/e-mail-559";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Social/Default Sharing Dialog", false, 0)]
	public static void UMDefaultSocialSharing() {
		string url = "https://unionassets.com/ultimate-mobile/default-sharing-dialog-560";
		Application.OpenURL(url);
	}

    //--------------------------------------
    //  ADVERTISEMENT
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Advertisement/Setup", false, 0)]
    public static void UMADSetup() {
        string url = "https://unionassets.com/ultimate-mobile/setup-228";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Advertisement/Coding Guidelines", false, 0)]
    public static void UMADCodingGuidelines() {
        string url = "https://unionassets.com/ultimate-mobile/coding-guidelines-229";
        Application.OpenURL(url);
    }

    //--------------------------------------
    //  MORE UNIFIED API
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Social Sharing", false, 0)]
    public static void UMMUSocialSharing() {
        string url = "https://unionassets.com/ultimate-mobile/social-sharing-230";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Camera and Gallery", false, 0)]
    public static void UMMUCameraAndGallery() {
        string url = "https://unionassets.com/ultimate-mobile/camera-and-gallery-231";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Local And Push Notifications", false, 0)]
    public static void UMMULocalAndPushNotifications() {
        string url = "https://unionassets.com/ultimate-mobile/local-and-push-notifications-232";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Pop Ups and Pre-loaders", false, 0)]
    public static void UMMUPopUpsAndPreLoaders() {
        string url = "https://unionassets.com/ultimate-mobile/pop-ups-and-pre-loaders-235";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Analytics Tracking", false, 0)]
    public static void UMMUAnalyticsTracking() {
        string url = "https://unionassets.com/ultimate-mobile/analytics-tracking-234";
        Application.OpenURL(url);
    }

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Locale", false, 0)]
	public static void UMMULocale() {
		string url = "https://unionassets.com/ultimate-mobile/location-499";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/More Unified API/Local Storage", false, 0)]
	public static void UMMULocalStorage() {
		string url = "https://unionassets.com/ultimate-mobile/local-storage-554";
		Application.OpenURL(url);
	}

    //--------------------------------------
    //  PLAYMAKER
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Playmaker/Actions List", false, 0)]
    public static void UMPActionsList() {
        string url = "https://unionassets.com/ultimate-mobile/playmaker-277";
        Application.OpenURL(url);
    }

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Playmaker/InApp Purchasing with Playmaker", false, 0)]
    public static void UMPInAppPurchasingWithPlaymaker() {
        string url = "https://unionassets.com/ultimate-mobile/inapp-purchasing-with-playmaker-466";
        Application.OpenURL(url);
    }

    //--------------------------------------
    //  THIRD-PARTY PLUG-INS SUPPORT
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Third-Party Plug-ins Support/Anti-Cheat Toolkit", false, 0)]
    public static void UMTPAntiCheatToolkit() {
        string url = "https://unionassets.com/ultimate-mobile/anti-cheat-toolkit-421";
        Application.OpenURL(url);
    }

	//--------------------------------------
	//  Notes
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/Notes", false, 0)]
	public static void UMNotes() {
		string url = "https://unionassets.com/ultimate-mobile/manual#notes";
		Application.OpenURL(url);
	}

    //--------------------------------------
    //  FAQ
    //--------------------------------------

    [MenuItem("Window/Stan's Assets/Ultimate Mobile/Documentation/FAQ", false, 0)]
    public static void UMFAQ() {
        string url = "https://unionassets.com/ultimate-mobile/manual#faq";
        Application.OpenURL(url);
    }
    
}
#endif
