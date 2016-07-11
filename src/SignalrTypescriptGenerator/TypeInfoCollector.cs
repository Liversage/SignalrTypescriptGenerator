using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SignalrTypescriptGenerator.Models;
using FieldInfo = SignalrTypescriptGenerator.Models.FieldInfo;
using MethodInfo = SignalrTypescriptGenerator.Models.MethodInfo;
using ParameterInfo = SignalrTypescriptGenerator.Models.ParameterInfo;
using PropertyInfo = SignalrTypescriptGenerator.Models.PropertyInfo;

namespace SignalrTypescriptGenerator {

  public class TypeInfoCollector {

    readonly Dictionary<string, ITypeInfo> _typeInfoCache = new Dictionary<string, ITypeInfo>();

    readonly List<ITypeInfo> _additionalTypeInfos = new List<ITypeInfo>();

    public ITypeInfo GetTypeInfo(Type type) {
      if (type == null)
        throw new ArgumentNullException(nameof(type));

      var key = type.FullName;
      if (key == null)
        // Key is null for generic type parameters. There is no need to cache
        // these anyway.
        return GetTypeInfoCore(type);

      if (_typeInfoCache.ContainsKey(key))
        return _typeInfoCache[key];

      var typeInfo = GetTypeInfoCore(type);
      _typeInfoCache[key] = typeInfo;

      return typeInfo;

    }

    public void AddTypeInfo(ITypeInfo typeInfo) {
      if (typeInfo == null)
        throw new ArgumentNullException(nameof(typeInfo));

      _additionalTypeInfos.Add(typeInfo);
    }

    public IEnumerable<ITypeInfo> GetTypeInfos() {
      var interfaceTypeInfos = _typeInfoCache
        .Values
        .OfType<ComplexTypeInfo>()
        .Select(typeInfo => typeInfo.GenericTypeDefinition ?? typeInfo);
      var enumTypeInfos = _typeInfoCache
        .Values
        .OfType<EnumTypeInfo>();
      return interfaceTypeInfos
        .Concat(enumTypeInfos)
        .Concat(_additionalTypeInfos)
        .OrderBy(typeInfo => typeInfo.FullName);
    }

    ITypeInfo GetTypeInfoCore(Type type) {
      if (type == typeof(string))
        return new BuiltinTypeInfo("string");
      if (IsNumberType(type))
        return new BuiltinTypeInfo("number");
      if (type == typeof(bool))
        return new BuiltinTypeInfo("boolean");
      if (type == typeof(DateTime) || type == typeof(DateTimeOffset))
        return new BuiltinTypeInfo("Date");
      if (type == typeof(void) || type == typeof(Task))
        return new BuiltinTypeInfo("void");

      if (type.IsEnum)
        return new EnumTypeInfo(type.Name, type.Namespace, GetEnumMembers(type).ToList());

      if (type.IsArray)
        return new ArrayTypeInfo(GetTypeInfo(type.GetElementType()));

      if (type.IsGenericTypeDefinition)
        return new GenericTypeDefinitionInfo(TrimGenericTypeName(type.Name), type.Namespace, GetMembers(type).ToList(), type.GetGenericArguments().Select(GetTypeInfo).ToList(), GetBaseInterface(type));

      if (type.IsGenericParameter)
        return new GenericParameterTypeInfo(type.Name);

      if (!type.IsGenericType)
        return new ComplexTypeInfo(type.Name, type.Namespace, GetMembers(type).ToList(), GetBaseInterface(type));

      if (type.GetGenericTypeDefinition() == typeof(JQueryPromise<>))
        return new ComplexTypeInfo("JQueryPromise", string.Empty, Enumerable.Empty<IMemberInfo>(), type.GetGenericArguments().Select(GetTypeInfo).ToList(), null);

      if (typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
        return new ArrayTypeInfo(GetTypeInfo(type.GenericTypeArguments.First()));

      if (typeof(Task<>).IsAssignableFrom(type.GetGenericTypeDefinition()) || typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
        return GetTypeInfo(type.GetGenericArguments().First());

      return new ComplexTypeInfo(
        TrimGenericTypeName(type.Name),
        type.Namespace,
        GetMembers(type).ToList(),
        type.GetGenericArguments().Select(GetTypeInfo).ToList(),
        GetTypeInfo(type.GetGenericTypeDefinition()),
        GetBaseInterface(type)
      );
    }

    static string TrimGenericTypeName(string genericTypeName) {
      var backtickIndex = genericTypeName.IndexOf('`');
      return backtickIndex >= 0 ? genericTypeName.Substring(0, backtickIndex) : genericTypeName;
    }

    IEnumerable<IMemberInfo> GetMembers(Type type) {
      foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(pi => pi.CanRead))
        yield return new PropertyInfo(propertyInfo.Name.ToCamelCase(), GetTypeInfo(propertyInfo.PropertyType));
      foreach (var methodInfo in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(methodInfo => !methodInfo.IsSpecialName))
        yield return new MethodInfo(
          methodInfo.Name.ToCamelCase(),
          GetTypeInfo(methodInfo.ReturnType),
          methodInfo.GetParameters().Select(parameter => new ParameterInfo(parameter.Name, GetTypeInfo(parameter.ParameterType))).ToList()
        );
    }

    IInterfaceTypeInfo GetBaseInterface(Type type) {
      return type.BaseType != typeof(object)
        ? (IInterfaceTypeInfo) GetTypeInfo(type.BaseType)
        : null;
    }

    static IEnumerable<IMemberInfo> GetEnumMembers(IReflect type) {
      return type
        .GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(fieldInfo => fieldInfo.GetValue(null))
        .Select(
          value => new FieldInfo(
            value.ToString(),
            Convert.ToInt64(value).ToString()
          )
        );
    }

    static bool IsNumberType(Type type) {
      return type == typeof(byte)
        || type == typeof(sbyte)
        || type == typeof(short)
        || type == typeof(ushort)
        || type == typeof(int)
        || type == typeof(uint)
        || type == typeof(long)
        || type == typeof(ulong)
        || type == typeof(float)
        || type == typeof(double)
        || type == typeof(decimal);
    }

  }

}