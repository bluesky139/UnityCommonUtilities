using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace common
{
	public class DeviceProfiles
	{
		public enum Quality
		{
			Fastest = 0,
			Good,
			Fantastic,
			_Overflow
		}

		struct Rule
		{
			public string reg;
			public string target;
			public string compare;
			public string value;
			public Quality result;
			public Dictionary<string, string> commonSpecs;

			public Rule(string reg, string target, string compare, string value, int result, Dictionary<string, string> commonSpecs = null)
			{
				this.reg = reg;
				this.target = target;
				this.compare = compare;
				this.value = value.ToLower();
				this.result = (Quality)result;
				this.commonSpecs = commonSpecs;
			}

			public override string ToString()
			{
				return "Rule [reg " + reg + ", target " + target + ", compare " + compare + ", value " + value + ", result " + result + "]";
			}
		}

		struct Common
		{
			public string target;
			public string[] values;

			public Common(string target, string[] values)
			{
				this.target = target;
				this.values = values;
			}
		}

		static List<Common> commons;
		static List<Rule> rules;
		static List<Rule> specs;
		static Dictionary<string, string> commonSpecs;
		static Quality quality_ = (Quality)QualitySettings.GetQualityLevel();
        public static Quality defaultQuality { get; private set; }
		public static Quality quality
		{
			get
			{
				return quality_;
			}
			set
			{
				quality_ = value;
				Debug.LogAlways("Quality set to " + value);
				if (QualitySettings.GetQualityLevel() != (int)value)
				{
					QualitySettings.SetQualityLevel((int)value);
					if (onQualityChanged != null)
						onQualityChanged();
				}

				if (commons != null)
				{
					foreach (Common common in commons)
					{
						string commonValue = common.values[(int)value];
						if (value == defaultQuality && commonSpecs != null && commonSpecs.ContainsKey(common.target))
							commonValue = commonSpecs[common.target];
						SetCommonValue(common.target, commonValue);
						Debug.Log("Quality " + common.target + " set to " + commonValue);
					}
				}
				SaveSetting();
			}
		}
		public static System.Action onQualityChanged;


		const string kQuality = "common.quality";
		static Quality LoadSetting()
		{
			int q = PlayerPrefs.GetInt(kQuality, -1);
			if (q >= 0 && q < (int)Quality._Overflow)
			{
				Debug.Log("DeviceProfiles LoadSetting " + q);
				return (Quality)q;
			}
			return Quality._Overflow;
		}

		static void SaveSetting()
		{
			PlayerPrefs.SetInt(kQuality, (int)quality_);
			PlayerPrefs.Save();
		}


		static void Load(JSONClass profile)
		{
			commons = new List<Common>();
			rules = new List<Rule>();
			specs = new List<Rule>();
			JSONClass json = profile;

			// common settings
			JSONArray jArray = json["commons"].AsArray;
			foreach (JSONClass jCommon in jArray)
			{
				string[] values = new string[(int)Quality._Overflow];
				for (int i = 0; i < values.Length; ++i)
				{
					values[i] = jCommon["" + i];
				}
				Common common = new Common(jCommon["target"], values);
				commons.Add(common);
			}

			// rules
			jArray = json["rules"].AsArray;
			foreach (JSONClass jRule in jArray)
			{
				Rule rule = new Rule(jRule["reg"], jRule["target"], jRule["compare"], jRule["value"], jRule["result"].AsInt);
				rules.Add(rule);
			}
			jArray = json["specs"].AsArray;
			foreach (JSONClass jRule in jArray)
			{
				Dictionary<string, string> commonSpecs = null;
				foreach (Common common in commons)
				{
					if (jRule.ContainsKey(common.target))
					{
						if (commonSpecs == null)
							commonSpecs = new Dictionary<string, string>();
						commonSpecs.Add(common.target, jRule[common.target]);
					}
				}
				Rule rule = new Rule(jRule["reg"], jRule["target"], jRule["compare"], jRule["value"], jRule["result"].AsInt, commonSpecs);
				specs.Add(rule);
			}
			Debug.Log("DeviceProfiles have " + rules.Count + " rules, " + specs.Count + " specs.");

			PropertyInfo[] fields = typeof(SystemInfo).GetProperties();
			foreach (PropertyInfo field in fields)
			{
				Debug.LogAlways("SystemInfo." + field.Name + " -> " + field.GetValue(null, null));
			}
			Debug.LogAlways("Resolution: " + Screen.currentResolution.width + "x" + Screen.currentResolution.height);
		}

		public static string CommonsToString()
		{
			if (commons == null)
				return "";
			string str = "Commons:\n";
			foreach (Common common in commons)
			{
				str += common.target + " -> " + GetCommonValue(common.target).ToString() + "\n";
			}
			return str;
		}

		public static string[] GetCommonList()
		{
			if (commons == null)
				return new string[0];
			string[] list = new string[commons.Count];
			for (int i = 0; i < list.Length; ++i)
			{
				list[i] = commons[i].target;
			}
			return list;
		}

		public static object GetCommonValue(string common)
		{
			PropertyInfo prop;
			FieldInfo field;
			TryGetPropOrField(common, out prop, out field);

			if (prop != null)
			{
				object o = prop.GetValue(null, null);
				return o == null ? "[null]" : o;
			}
			else
			{
				object o = field.GetValue(null);
				return o == null ? "[null]" : o;
			}
		}

		public static void SetCommonValue(string common, object value)
		{
			PropertyInfo prop;
			FieldInfo field;
			TryGetPropOrField(common, out prop, out field);

			if (prop != null)
			{
				value = System.Convert.ChangeType(value, prop.PropertyType);
				prop.SetValue(null, value, null);
			}
			else
			{
				value = System.Convert.ChangeType(value, field.FieldType);
				field.SetValue(null, value);
			}
		}

		static void TryGetPropOrField(string common, out PropertyInfo prop, out FieldInfo field)
		{
			int pos = common.LastIndexOf('.');
			string cls = common.Substring(0, pos);
			string val = common.Substring(pos + 1);

			Type type = Type.GetType(cls + ", Assembly-CSharp");
			if (type == null)
				type = Type.GetType(cls);
            if (type == null)
				type = Type.GetType("UnityEngine." + cls + ", UnityEngine");
			Debug.Assert(type != null, "Can't find class " + cls);
			field = type.GetField(val);
			prop = null;
			if (field == null)
				prop = type.GetProperty(val);
			Debug.Assert(prop != null || field != null, "Can't find prop or field " + val + " in class " + cls);
		}

		public static void MatchAndSetWithProfile()
		{
			if (rules == null)
			{
				TextAsset asset = Resources.Load<TextAsset>("DeviceProfiles");
				JSONClass profile = JSON.ParseFromBytes(asset.bytes) as JSONClass;
				Load(profile);
			}

		#if !UNITY_EDITOR && !UNITY_STANDALONE_WIN
			Quality q = Match(specs, true, out commonSpecs);
			if (q == Quality._Overflow)
				q = Match(rules, false, out commonSpecs);
			defaultQuality = q != Quality._Overflow ? q : (Quality) QualitySettings.GetQualityLevel();
		#else
			defaultQuality = Quality.Fantastic;
		#endif

			Quality savedQuality = LoadSetting();
			quality = savedQuality != Quality._Overflow ? savedQuality : defaultQuality;
		}

		static Quality Match(List<Rule> rules, bool quitWhenMatchOne, out Dictionary<string, string> commonSpecs)
		{
			commonSpecs = null;
			Quality quality = Quality._Overflow;
			foreach (Rule rule in rules)
			{
				PropertyInfo prop = typeof(SystemInfo).GetProperty(rule.target);
				if (prop == null)
				{
					Debug.LogWarning("Can't invoke SystemInfo." + rule.target);
					continue;
				}
				string value = prop.GetValue(null, null).ToString();
				Regex reg = new Regex(rule.reg);
				Match match = reg.Match(value);
				if (match == null || !match.Success)
					continue;

				value = match.Groups[1].Value.ToLower();
				bool yes = false;
				try
				{
					switch (rule.compare)
					{
						case "=":
							yes = (value == rule.value);
							break;
						case ">(int)":
							yes = (int.Parse(value) > int.Parse(rule.value));
							break;
						case "<(int)":
							yes = (int.Parse(value) < int.Parse(rule.value));
							break;
						case ">=(int)":
							yes = (int.Parse(value) >= int.Parse(rule.value));
							break;
						case "<=(int)":
							yes = (int.Parse(value) <= int.Parse(rule.value));
							break;
						case ">(string)":
							yes = (value.CompareTo(rule.value) > 0);
							break;
						case "<(string)":
							yes = (value.CompareTo(rule.value) < 0);
							break;
						case ">=(string)":
							yes = (value.CompareTo(rule.value) >= 0);
							break;
						case "<=(string)":
							yes = (value.CompareTo(rule.value) <= 0);
							break;
						default:
							Debug.Assert(false, "Unknown compare " + rule.compare);
							break;
					}
				}
				catch (Exception e)
				{
					Debug.Assert(false, "Device profiles compare error, " + e.Message + ", " + rule.ToString());
				}

				if (yes)
				{
					Debug.LogAlways("Device profiles match " + rule.ToString());
					quality = (Quality)Math.Min((int)quality, (int)rule.result);
					commonSpecs = rule.commonSpecs;
					if (quitWhenMatchOne)
						break;
				}
			}
			return quality;
		}
	}
}