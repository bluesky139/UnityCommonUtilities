using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

namespace common
{
    public static class Utils
    {
        public static byte[] ReadAllInUseBytes(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }

		public static bool ReplaceFileText(string path, string from, string to, bool throwExceptionIfNone = false)
		{
			string text = File.ReadAllText(path);
			bool replaced = text.IndexOf(from) >= 0;
			Debug.AssertThrowException(!throwExceptionIfNone || replaced, "ReplaceFileText can't find \"" + from + "\" in " + path);
			text = text.Replace(from, to);
			File.WriteAllText(path, text);
			return replaced;
		}

		public static bool ReplaceFileText(string path, string[] fromList, string to, bool throwExceptionIfNone = false)
		{
			string text = File.ReadAllText(path);
			bool replaced = false;
			foreach (string from in fromList)
			{
				replaced |= text.IndexOf(from) >= 0;
				text = text.Replace(from, to);
			}
			Debug.AssertThrowException(!throwExceptionIfNone || replaced, "ReplaceFileText can't find \"" + fromList[0] + "\" and more in " + path); 
			File.WriteAllText(path, text);
			return replaced;
		}

		public static bool ReplaceFileLineByRegex(string path, string fromRegex, string to, bool throwExceptionIfNone = false)
		{
			string[] lines = File.ReadAllLines(path);
			bool replaced = false;
			lines = lines.Select(line =>
			{
				string newLine = Regex.Replace(line, fromRegex, to);
				if (newLine != line)
				{
					replaced = true;
					return newLine;
				}
				return line;
			}).ToArray();
			Debug.AssertThrowException(!throwExceptionIfNone || replaced, "ReplaceFileLineByRegex can't find \"" + fromRegex + "\" in " + path); 
			File.WriteAllLines(path, lines);
			return replaced;
		}

		public static bool ReplaceFilesLineByRegex(IEnumerable<string> paths, string fromRegex, string to, int expectReplaced, bool throwExceptionIfNone = false)
		{
			int replacedCount = 0;
			foreach (string path in paths)
			{
				bool replaced = ReplaceFileLineByRegex(path, fromRegex, to, false);
				replacedCount += replaced ? 1 : 0;
			}
			Debug.AssertThrowException(!throwExceptionIfNone || replacedCount >= expectReplaced, "ReplaceFilesLineByRegex can't find \"" + fromRegex + "\" in " + ",".Join(paths));
			return replacedCount >= expectReplaced;
		}

		public static void DeleteFileLineByRegex(string path, string fromRegex)
		{
			string[] lines = File.ReadAllLines(path);
			Regex reg = new Regex(fromRegex);
			lines = lines.Where(line => !reg.Match(line).Success).ToArray();
			File.WriteAllLines(path, lines);
		}

		public static void AppendFileLines(string path, IEnumerable<string> linesToAppend)
		{
			var lineList = File.ReadAllLines(path).ToList();
			lineList.AddRange(linesToAppend);
			File.WriteAllLines(path, lineList.ToArray());
		}

		public static bool ContainsRegex(this IEnumerable<string> lines, string reg_)
		{
			Regex reg = new Regex(reg_);
			foreach (string line in lines)
			{
				if (reg.Match(line).Success)
				{
					return true;
				}
			}
			return false;
		}

        public static bool IsBasicType(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type.Equals(typeof(string))
                || type.Equals(typeof(decimal));
        }

        public static string Format(this string s, params object[] args)
        {
            return string.Format(s, args);
        }

		public static string Join(this string s, IEnumerable<string> value)
		{
			return string.Join(s, value.ToArray());
		}
	}
}
