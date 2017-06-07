using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace AirInspector
{
    public struct DynamicMember
    {
        public string name;
        public string type;
        public object value;
    }

    public class AirUtils
    {
        public static byte[] ObjectSerialize(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, obj);
            return stream.ToArray();
        }

        public static object ObjectDeserialize(Type type, byte[] bytes)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            MemoryStream stream = new MemoryStream(bytes);
            return serializer.Deserialize(stream);
        }

        public static bool TestSerializable(object o)
        {
            try
            {
                XmlSerializer writer = new XmlSerializer(o.GetType());
                MemoryStream stream = new MemoryStream();
                writer.Serialize(stream, o);
                Debug.Log("can serialize. " + (o == null ? "(object is null)" : o.GetType().ToString()));
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("can't serialize " + (o == null ? "(object is null)" : o.GetType().ToString()) + ", " + e.Message);
                return false;
            }
        }

        public static byte[] SerializeExtraTypes(List<DynamicMember> members, out Type[] extraTypes_)
        {
            extraTypes_ = members == null ? new Type[0] : null;
            List<string> extraTypes = new List<string>();

            if (members != null)
            {
                foreach (DynamicMember member in members)
                {
                    if (!extraTypes.Contains(member.type))
                        extraTypes.Add(member.type);
                }
                extraTypes_ = new Type[extraTypes.Count];
                for (int i = 0; i < extraTypes_.Length; ++i)
                {
                    extraTypes_[i] = Type.GetType(extraTypes[i]);
                }
            }

            MemoryStream stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(extraTypes.GetType());
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            xml.Serialize(writer, extraTypes);
            return stream.ToArray();
        }

        public static Type[] DeserializeExtraTypes(byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            XmlSerializer xml = new XmlSerializer(typeof(List<string>));
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            List<string> extraTypes = (List<string>)xml.Deserialize(reader);

            Type[] types = null;
            if (extraTypes != null)
            {
                types = new Type[extraTypes.Count];
                for (int i = 0; i < types.Length; ++i)
                    types[i] = Type.GetType(extraTypes[i]);
            }
            return types;
        }

        public static byte[] SerializeObj(object obj, Type[] extraTypes)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(obj.GetType(), extraTypes);
            StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
            xml.Serialize(writer, obj);
            return stream.ToArray();
        }

        public static object DeserializeObj(Type type, byte[] bytes, Type[] extraTypes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            XmlSerializer xml = new XmlSerializer(type, extraTypes);
            StreamReader reader = new StreamReader(stream, Encoding.UTF8);
            return xml.Deserialize(reader);
        }

    #region Dynamic Class Creator
        public static object CreateNewObject(List<DynamicMember> members, string cls, Type parent = null)
        {
            var myType = CompileResultType(members, cls, parent);
            var myObject = Activator.CreateInstance(myType);
            foreach (DynamicMember member in members)
            {
                FieldInfo field = myType.GetField(member.name);
                field.SetValue(myObject, member.value);
            }
            return myObject;
        }

        public static Type CompileResultType(List<DynamicMember> members, string cls, Type parent = null)
        {
            TypeBuilder tb = GetTypeBuilder(cls, parent);
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (DynamicMember member in members)
            {
                CreateProperty(tb, member.name, Type.GetType(member.type));
            }
            Type objectType = tb.CreateType();
            return objectType;
        }

        static TypeBuilder GetTypeBuilder(string cls, Type parent)
        {
            var typeSignature = "Air_" + cls;
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder tb = moduleBuilder.DefineType(typeSignature
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout |
                                TypeAttributes.Serializable
                                , parent);
            return tb;
        }

        static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = tb.DefineField(propertyName, propertyType, FieldAttributes.Public);

            //PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            //MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            //ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            //getIl.Emit(OpCodes.Ldarg_0);
            //getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            //getIl.Emit(OpCodes.Ret);

            //MethodBuilder setPropMthdBldr =
            //    tb.DefineMethod("set_" + propertyName,
            //        MethodAttributes.Public |
            //        MethodAttributes.SpecialName |
            //        MethodAttributes.HideBySig,
            //        null, new[] { propertyType });

            //ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            //Label modifyProperty = setIl.DefineLabel();
            //Label exitSet = setIl.DefineLabel();

            //setIl.MarkLabel(modifyProperty);
            //setIl.Emit(OpCodes.Ldarg_0);
            //setIl.Emit(OpCodes.Ldarg_1);
            //setIl.Emit(OpCodes.Stfld, fieldBuilder);

            //setIl.Emit(OpCodes.Nop);
            //setIl.MarkLabel(exitSet);
            //setIl.Emit(OpCodes.Ret);

            //propertyBuilder.SetGetMethod(getPropMthdBldr);
            //propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    #endregion
    }
}
