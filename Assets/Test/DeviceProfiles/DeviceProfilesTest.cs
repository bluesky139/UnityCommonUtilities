using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class DeviceProfilesTest
{
	[FloatingDebug.Item("DeviceProfiles", "MatchAndSet")]
	static void MatchAndSet()
	{
		DeviceProfiles.MatchAndSetWithProfile();
		Debug.Log("Dlc.maxDownloadThread " + Dlc.maxDownloadThread);
		Debug.Log("Preloader.enabled " + Preloader.enabled);
	}

	[FloatingDebug.Item("DeviceProfiles", "Fast")]
    static void SetToFastest()
    {
        DeviceProfiles.quality = DeviceProfiles.Quality.Fastest;
    }

    [FloatingDebug.Item("DeviceProfiles", "Good")]
    static void SetToGood()
    {
        DeviceProfiles.quality = DeviceProfiles.Quality.Good;
    }

    [FloatingDebug.Item("DeviceProfiles", "Fantastic")]
    static void SetToFantastic()
    {
        DeviceProfiles.quality = DeviceProfiles.Quality.Fantastic;
    }

    [FloatingDebug.Item("DeviceProfiles", "Default")]
    static void SetToDefault()
    {
        DeviceProfiles.quality = DeviceProfiles.defaultQuality;
    }
}
