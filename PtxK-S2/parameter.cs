﻿using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PtxK_S2
{
	[DataContract]
	public class parameter
	{
		[DataMember]
		public int errCode { get; set; }
		[DataMember]
		public string errMsg { get; set; }

		[DataMember]
		public List<string> resoList { get; set; }
		[DataMember]
		public List<string> movieResoList { get; set; }
		[DataMember]
		public List<string> WBModeList { get; set; }
		[DataMember]
		public List<string> stillSizeList { get; set; }
		[DataMember]
		public List<string> movieSizeList { get; set; }
		[DataMember]
		public List<string> shootModeList { get; set; }
		[DataMember]
		public List<string> effectList { get; set; }
		[DataMember]
		public List<string> filterList { get; set; }
		[DataMember]
		public List<string> exposureModeList { get; set; }
		[DataMember]
		public List<string> avList { get; set; }
		[DataMember]
		public List<string> tvList { get; set; }
		[DataMember]
		public List<string> svList { get; set; }
		[DataMember]
		public List<string> xvList { get; set; }

		[DataMember]
		public string exposureModeOption { get; set; }
		[DataMember]
		public string state { get; set; }
		[DataMember]
		public string av { get; set; }
		[DataMember]
		public string tv { get; set; }
		[DataMember]
		public string sv { get; set; }
		[DataMember]
		public string xv { get; set; }
		[DataMember]
		public string WBMode { get; set; }
		[DataMember]
		public string shootMode { get; set; }
		[DataMember]
		public string exposureMode { get; set; }
		[DataMember]
		public string stillSize { get; set; }
		[DataMember]
		public string movieSize { get; set; }
		[DataMember]
		public string effect { get; set; }
		[DataMember]
		public string filter { get; set; }
		[DataMember]
		public bool focused { get; set; }
		[DataMember]
		public List<string> focusCenters { get; set; }
		[DataMember]
		public string focusMode { get; set; }
		[DataMember]
		public string model { get; set; }
		[DataMember]
		public string firmwareVersion { get; set; }
		[DataMember]
		public string macAddress { get; set; }
		[DataMember]
		public string serialNo { get; set; }


		[DataMember]
		public List<int> channelList { get; set; }
		[DataMember]
		public bool hot { get; set; }
		[DataMember]
		public int battery { get; set; }

		[DataMember]
		public List<storage> storages { get; set; }

		[DataMember]
		public string ssid { get; set; }
		[DataMember]
		public string key { get; set; }
		[DataMember]
		public string channel { get; set; }
		[DataMember]
		public string liveState { get; set; }

	}

	public class storage
	{
		[DataMember]
		public string name { get; set; }
		[DataMember]
		public bool equipped { get; set; }
		[DataMember]
		public bool writable { get; set; }
		[DataMember]
		public bool available { get; set; }
		[DataMember]
		public bool active { get; set; }
		[DataMember]
		public string format { get; set; }
		[DataMember]
		public int remain { get; set; }
		[DataMember]
		public string dir { get; set; }
		[DataMember]
		public string file { get; set; }

	}
}

//Beispiel:
//{"errCode": 200,
// "errMsg": "OK",
// "resoList": [ "1080x720", "720x480"],
// "movieResoList": [ "1280x720", "720x404"],
// "WBModeList": [ "auto", "multiAuto", "daylight", "shade", "cloud", "daylightFluorescent", "dayWhiteFluorescent", "coolWhiteFluorescent", "warmWhiteFluorescent", "tungsten", "flash", "cte", "manual1", "colorTemp1"],
// "stillSizeList": [ "L3", "L2", "L1", "M3", "M2", "M1", "S3", "S2", "S1", "XS3", "XS2", "XS1"],
// "movieSizeList": [ "FHD30p", "FHD25p", "FHD24p", "HD60p", "HD50p"],
// "shootModeList": [ "single", "continuousH", "continuousL", "self12s", "self2s", "selfCotinuousH", "selfCotinuousL", "remocon", "remocon3s", "remoconContinousH", "remoconContinousL", "bracket", "bracketSelf", "bracketRemocon", "multiExp", "multiExpContinuousH", "multiExpContinuousL", "multiExpSelf12s", "multiExpSelf2s", "multiExpRemocon", "multiExpRemocon3s", "interval", "intervalSelf12s", "intervalSelf2s", "intervalRemocon", "intervalRemocon3s", "intervalComp", "intervalCompSelf12s", "intervalCompSelf2s", "intervalCompRemocon", "intervalCompRemocon3s"],
// "effectList": [ "cim_natural", "cim_bright", "cim_portrait", "cim_landscape", "cim_vibrant", "cim_radiant", "cim_muted", "cim_bleachBypass", "cim_reversal", "cim_monochrome", "cim_crossProcess"],
// "filterList": ["off", "dfl_extractColor", "dfl_replaceColor", "dfl_toyCamera", "dfl_retro", "dfl_highContrast", "dfl_shading", "dfl_negaPosi", "dfl_solidMonoColor", "dfl_hardMonochrome", "hdr_auto", "hdr_mode1", "hdr_mode2", "hdr_mode3"],
// "exposureModeList": [ "P", "SV", "TV", "AV", "TAV", "M", "B", "U1", "U2", "AHDR", "scene", "autopict", "gps", "movie"],
// "avList": [ "3.5", "4.0", "4.5", "5.0", "5.6", "6.3", "7.1", "8.0", "9.0", "10", "11", "13", "14", "16", "18", "20", "22"],
// "tvList": [ "30.1", "25.1", "20.1", "15.1", "13.1", "10.1", "8.1", "6.1", "5.1", "4.1", "3.1", "25.10", "2.1", "16.10", "13.10", "1.1", "8.10", "6.10", "5.10", "4.10", "3.10", "1.4", "1.5", "1.6", "1.8", "1.10", "1.13", "1.15", "1.20", "1.25", "1.30", "1.40", "1.50", "1.60", "1.80", "1.100", "1.125", "1.160", "1.200", "1.250", "1.320", "1.400", "1.500", "1.640", "1.800", "1.1000", "1.1250", "1.1600", "1.2000", "1.2500", "1.3200", "1.4000", "1.5000", "1.6000"],
// "svList": ["auto", "100", "200", "400", "800", "1600", "3200", "6400", "12800", "25600", "51200"],
// "xvList": [ "+5.0", "+4.7", "+4.3", "+4.0", "+3.7", "+3.3", "+3.0", "+2.7", "+2.3", "+2.0", "+1.7", "+1.3", "+1.0", "+0.7", "+0.3", "0.0", "-0.3", "-0.7", "-1.0", "-1.3", "-1.7", "-2.0", "-2.3", "-2.7", "-3.0", "-3.3", "-3.7", "-4.0", "-4.3", "-4.7", "-5.0"],
// "exposureModeOption": "",
// "state": "idle",
// "av": "3.5",
// "tv": "1.50",
// "sv": "6400",
// "xv": "0.0",
// "WBMode": "auto",
// "shootMode": "single",
// "exposureMode": "P",
// "stillSize": "L3",
// "movieSize": "FHD25p",
// "effect": "cim_bright",
// "filter": "off",
// "focused": false,
// "focusCenters": [],
// "focusMode": "af",
// "model": "PENTAX K-S2",
// "firmwareVersion": "01.01",
// "macAddress": "AC:3F:A4:3B:41:21",
// "serialNo": "6121009",
// "channelList": [0,1,2,3,4,5,6,7,8,9,10,11],
// "hot": false,
// "battery": 66,
// "storages" : [
// {"name":"sd1",
// "equipped": true,
// "writable": true,
// "available": true,
// "active": true,
// "format": "jpeg",
// "remain": 1921
//, "dir": "104_1706",
// "file": "IMGP1858.JPG"
// }
// ],
// "ssid": "PENTAX_3B4121",
// "key": "A43B4121",
// "channel": "1",
// "liveState": "idle"}
