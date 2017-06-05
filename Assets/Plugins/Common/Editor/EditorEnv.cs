using UnityEngine;
using System.Collections;
using System.IO;

namespace common
{ 
    public class EditorEnv
    {
        /// <summary>
        /// /[YourUnityProject]
        /// </summary>
        public static string dstUnityProjectRoot
        {
            get
            {
                return Path.GetFullPath(Application.dataPath + "/..").Replace('\\', '/');
            }
        }

        /// <summary>
        /// /[YourUnityProject]/Assets
        /// </summary>
        public static string dstUnityProjectAssets
        {
            get
            {
                return dstUnityProjectRoot + "/Assets";
            }
        }

		/// <summary>
		/// /[YourUnityProject]/Assets/Plugins
		/// </summary>
		public static string dstUnityProjectPlugins
        {
            get
            {
                return dstUnityProjectAssets + "/Plugins";
            }
        }

        /// <summary>
        /// /[YourUnityProject]/Assets/Plugins/Common
        /// </summary>
        public static string dstUnityProjectPluginsCommon
        {
            get
            {
                return dstUnityProjectPlugins + "/Common";
            }
        }

        /// <summary>
        /// /[YourUnityProject]/Assets/Plugins/Android, enabled sdks(eclipse project) link to this folder.
        /// </summary>
        public static string dstUnityProjectPluginsAndroid
        {
            get
            {
                return dstUnityProjectPlugins + "/Android";
            }
        }

		/// <summary>
        /// /[YourUnityProject]/Assets/Plugins/iOS
        /// </summary>
        public static string dstUnityProjectPluginsiOS
        {
            get
            {
                return dstUnityProjectPlugins + "/iOS";
            }
        }

		public static string externalRoot
		{
			get
			{
				return dstUnityProjectRoot + "/Externals";
			}
		}
	}
}