using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace GridInventorySystem
{
    public static class Utilities
    {
        private static Dictionary<Type, FieldInfo[]> m_SerializedFieldInfoLookup;
        private readonly static Dictionary<MemberInfo, object[]> m_MemberAttributeLookup;

        static Utilities()
        {
            m_SerializedFieldInfoLookup = new Dictionary<Type, FieldInfo[]>();
            m_MemberAttributeLookup = new Dictionary<MemberInfo, object[]>();
        }

        static Regex arrayElementRegex = new Regex(@"\GArray\.data\[(\d+)\]", RegexOptions.Compiled);

        public static Dir GetNextDir(Dir dir)
        {
            switch (dir)
            {
                case Dir.Up:        return Dir.Right;
                case Dir.Right:     return Dir.Up;

                //case Dir.Up:  return Dir.Right;
                //case Dir.Right:  return Dir.Down;
                //case Dir.Down:    return Dir.Left;
                //case Dir.Left: return Dir.Up;

                default: return Dir.Up;
            }
        }

        public static int GetRotationAngle(Dir dir)
        {
            switch (dir)
            {
                default:
                case Dir.Up: return 0;
                case Dir.Left: return 90;
                case Dir.Down: return 180;
                case Dir.Right: return 270;
            }
        }


        /// <summary>
		/// Creates a custom asset
		/// </summary>
		/// <returns>The asset</returns>		
		/// <typeparam name="T">The type parameter</typeparam>
		public static T CreateAsset<T>(bool displayFilePanel) where T : ScriptableObject
        {
            return (T)CreateAsset(typeof(T));
        }

        /// <summary>
        /// Creates a custom asset
        /// </summary>
        /// <returns>The asset</returns>
        /// <param name="type">Type</param>
        public static ScriptableObject CreateAsset(Type type)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (path == "")            
                path = "Assets";
            
            else if (System.IO.Path.GetExtension(path) != "")            
                path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
           
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + type.Name + ".asset");
            if (string.IsNullOrEmpty(assetPathAndName))
            {
                return null;
            }
            ScriptableObject data = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.SaveAssets();
            return data;
        }


        public static List<ItemDatabase> GetAllDatabases()
        {           
            List<ItemDatabase> databases = new List<ItemDatabase>();

            string[] guids = AssetDatabase.FindAssets("t:ItemDatabase");
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDatabase));
                databases.Add(obj as ItemDatabase);               
            }

            return databases;
        }

        public static string GenerateID()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static IEnumerable<SerializedProperty> EnumerateChildProperties(SerializedProperty property)
        {
            var iterator = property.Copy();
            var end = iterator.GetEndProperty();
            if (iterator.NextVisible(enterChildren: true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(iterator, end))
                        yield break;

                    yield return iterator;
                }
                while (iterator.NextVisible(enterChildren: false));
            }
        }

        public static object GetValue(SerializedProperty property)
        {
            string propertyPath = property.propertyPath;
            object value = property.serializedObject.targetObject;
            int i = 0;
            while (NextPropertyPath(propertyPath, ref i, out var token))
                value = GetPropertyPathValue(value, token);
            return value;
        }

        private static bool NextPropertyPath(string propertyPath, ref int index, out PropertyPath component)
        {
            component = new PropertyPath();

            if (index >= propertyPath.Length)
                return false;

            var arrayElementMatch = arrayElementRegex.Match(propertyPath, index);
            if (arrayElementMatch.Success)
            {
                index += arrayElementMatch.Length + 1;
                component.elementIndex = int.Parse(arrayElementMatch.Groups[1].Value);
                return true;
            }

            int dot = propertyPath.IndexOf('.', index);
            if (dot == -1)
            {
                component.propertyName = propertyPath.Substring(index);
                index = propertyPath.Length;
            }
            else
            {
                component.propertyName = propertyPath.Substring(index, dot - index);
                index = dot + 1;
            }

            return true;
        }

        private static object GetPropertyPathValue(object container, PropertyPath component)
        {
            if (component.propertyName == null)
            {
                IList list = (IList)container;
                if (list.Count - 1 < component.elementIndex)
                {
                    for (int i = list.Count - 1; i < component.elementIndex; i++)
                    {
                        list.Add(default);
                    }
                }
                return list[component.elementIndex];
            }
            else
            {
                return GetFieldValue(container, component.propertyName);
            }
        }

        private static object GetFieldValue(object container, string name)
        {
            if (container == null)
                return null;
            var type = container.GetType();
            FieldInfo field = type.GetSerializedField(name);
            return field.GetValue(container);
        }

        public static FieldInfo GetSerializedField(this Type type, string name)
        {
            return type.GetAllSerializedFields().Where(x => x.Name == name).FirstOrDefault();
        }

        public static FieldInfo[] GetAllSerializedFields(this Type type)
        {
            if (type == null)
            {
                return new FieldInfo[0];
            }
            FieldInfo[] fields = GetSerializedFields(type).Concat(GetAllSerializedFields(type.BaseType)).ToArray();
            fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
            return fields;
        }

        public static FieldInfo[] GetSerializedFields(this Type type)
        {
            FieldInfo[] fields;
            if (!m_SerializedFieldInfoLookup.TryGetValue(type, out fields))
            {
                fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.IsPublic && !x.HasAttribute(typeof(NonSerializedAttribute)) || x.HasAttribute(typeof(SerializeField)) || x.HasAttribute(typeof(SerializeReference))).ToArray();
                fields = fields.OrderBy(x => x.DeclaringType.BaseTypesAndSelf().Count()).ToArray();
                m_SerializedFieldInfoLookup.Add(type, fields);
            }
            return fields;
        }

        public static IEnumerable<Type> BaseTypesAndSelf(this Type type)
        {
            while (type != null)
            {
                yield return type;
                type = type.BaseType;
            }
        }

        public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType)
        {
            object[] objArray = GetCustomAttributes(memberInfo, true);
            for (int i = 0; i < (int)objArray.Length; i++)
            {
                if (objArray[i].GetType() == attributeType || objArray[i].GetType().IsSubclassOf(attributeType))
                {
                    return true;
                }
            }
            return false;
        }

        public static object[] GetCustomAttributes(MemberInfo memberInfo, bool inherit)
        {
            object[] customAttributes;
            if (!m_MemberAttributeLookup.TryGetValue(memberInfo, out customAttributes))
            {
                customAttributes = memberInfo.GetCustomAttributes(inherit);
                m_MemberAttributeLookup.Add(memberInfo, customAttributes);
            }
            return customAttributes;
        }

        public static Type GetElementType(Type type)
        {
            Type[] interfaces = type.GetInterfaces();

            return (from i in interfaces
                    where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)
                    select i.GetGenericArguments()[0]).FirstOrDefault();
        }

    }

    struct PropertyPath
    {
        public string propertyName;
        public int elementIndex;
    }

    public enum Dir
    {
        Up = 0,
        Left,
        Right,
        Down,
    }    
}