using UnityEngine;
using System.Collections;
using System;

#if !(UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) || UNITY_EDITOR
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
#endif

///Developed By Indie Studio
///https://assetstore.unity.com/publishers/9268
///www.indiestd.com
///info@indiestd.com

public class FileManager {

		/// <summary>
		/// Save the data to XML file.
		/// </summary>
		/// <param name="data">Data.</param>
		public static void SaveDataToXMLFile<Type>(Type data,string filePath)
		{
				#if !(UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) 
		
				if (string.IsNullOrEmpty(filePath))
				{
				Debug.Log("Null or Empty path");
				return;
				}

				if (data == null)
				{
				Debug.Log("Data is Null");
				return;
				}

				TextWriter textWriter = new StreamWriter(filePath);

				try{
				Debug.Log("Saving Data to XML File "+filePath);
				XmlSerializer serializer = new XmlSerializer(typeof(Type));
				serializer.Serialize(textWriter, data);
				textWriter.Close();
				}catch(Exception ex){
					textWriter.Close();
					Debug.LogError("Exception : " + ex.Message);
				}
				#endif
		}

		/// <summary>
		/// Load the data from XML file.
		/// </summary>
		/// <returns>The data from XML file.</returns>
		public static Type LoadDataFromXMLFile<Type>(string filePath)
		{
				Type data = default(Type);

				#if !(UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) 
	
				if (string.IsNullOrEmpty(filePath))
				{
				Debug.Log("Null or Empty file path");
				return data;
				}


				if (!File.Exists(filePath))
				{
				Debug.Log(filePath + " is not exists");
				return data;
				}

				TextReader textReader = new StreamReader(filePath);

				try{
						Debug.Log("Loading Data from XML File "+filePath);
				XmlSerializer deserializer = new XmlSerializer(typeof(Type));
				data = (Type)deserializer.Deserialize(textReader);
				textReader.Close();
				}catch (Exception ex){
					textReader.Close();
					Debug.LogError("Exception : " + ex.Message);
				}
				#endif
				return data;
		}

		/// <summary>
		/// Save data to the binary file.
		/// </summary>
		/// <param name="data"> The data.</param>
		public static void SaveDataToBinaryFile<Type>(Type data,string filePath)
		{
				#if !(UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) 
		
				if (string.IsNullOrEmpty(filePath))
				{
				Debug.Log("Null or Empty file path");
				return;
				}

				if (data == null)
				{
				Debug.Log("Data is Null");
				return;
				}

				Debug.Log("Saving Data to Binary File "+filePath);

				FileStream file = null;
				try
				{
				BinaryFormatter bf = new BinaryFormatter();
				file = File.Open(filePath, FileMode.Create);
				bf.Serialize(file, data);
				file.Close();
				}
				catch (Exception ex)
				{
				file.Close();
				Debug.LogError("Exception : " + ex.Message);
				}
				#endif
		}

		/// <summary>
		/// Load data from the binary file.
		/// </summary>
		public static Type LoadDataFromBinaryFile<Type>(string filePath)
		{
				Type data = default(Type);

				#if !(UNITY_WP8 || UNITY_WP8_1 || UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL) 
			
				if (string.IsNullOrEmpty(filePath))
				{
				Debug.Log("Null or Empty file path");
				return data;
				}

				if (!File.Exists(filePath))
				{
				Debug.Log(filePath + " is not exists");
				return data;
				}

				Debug.Log("Loading Data from Binary File "+filePath);

				FileStream file = null;
				try
				{
				BinaryFormatter bf = new BinaryFormatter();
				file = File.Open(filePath, FileMode.Open);
				data = (Type)bf.Deserialize(file);
				file.Close();
				}
				catch (Exception ex)
				{
				file.Close();
				Debug.LogError("Exception : " + ex.Message);
				}
				#endif
				return data;
		}


		/// <summary>
		/// Clean the files in a folder.
		/// </summary>
		/// <param name="path">Path.</param>
		public static void CleanFolderFiles (string path){

				if (string.IsNullOrEmpty (path)) {
						return;
				}

				#if UNITY_EDITOR
				string [] filesPath = Directory.GetFiles (path);
				foreach (string filePath in filesPath) {
						if(string.IsNullOrEmpty(filePath)){
								continue;
						}
						File.Delete (filePath);
				}
				#endif
		}

		/// <summary>
		/// Get current platform file folder.
		/// </summary>
		/// <returns>The current platform file folder.</returns>
		public static string GetCurrentPlatformFileFolder ()
		{
				string path = null;
				#if UNITY_ANDROID
				path = FileManager.GetAndroidFileFolder ();
				#elif UNITY_IPHONE
				path = FileManager.GetIPhoneFileFolder();
				#elif !(UNITY_WP8 || UNITY_WP8_1 ||UNITY_WSA || UNITY_WSA_8_0 || UNITY_WSA_8_1 || UNITY_WSA_10_0 || UNITY_WEBPLAYER || UNITY_WEBGL)
				path = FileManager.GetOthersFileFolder();
				#endif
				return path;
		}

		public static string GetAndroidFileFolder()
		{
				return Application.persistentDataPath;
		}

		public static string GetIPhoneFileFolder()
		{
				return Application.persistentDataPath;
		}

		public static string GetOthersFileFolder()
		{
				return Application.dataPath;
		}
}
