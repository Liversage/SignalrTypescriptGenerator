using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalrTypescriptGenerator.Models;
using MethodInfo = SignalrTypescriptGenerator.Models.MethodInfo;
using ParameterInfo = SignalrTypescriptGenerator.Models.ParameterInfo;
using PropertyInfo = SignalrTypescriptGenerator.Models.PropertyInfo;

namespace SignalrTypescriptGenerator {

  class SignalrHubinator {

    static string assemblyRootFolder;

    readonly DefaultHubManager _hubmanager;

    public SignalrHubinator(string assemblyPath) {
      assemblyRootFolder = Path.GetDirectoryName(assemblyPath);
      LoadAssemblyIntoAppDomain(assemblyPath);

      var defaultDependencyResolver = new DefaultDependencyResolver();
      _hubmanager = new DefaultHubManager(defaultDependencyResolver);
    }

    static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
      var assemblyPath = Path.Combine(assemblyRootFolder, new AssemblyName(args.Name).Name + ".dll");
      if (File.Exists(assemblyPath) == false)
        return null;

      var assembly = Assembly.LoadFrom(assemblyPath);
      return assembly;
    }

    static void LoadAssemblyIntoAppDomain(string assemblyPath) {
      var currentDomain = AppDomain.CurrentDomain;
      currentDomain.AssemblyResolve += LoadFromSameFolder;
      Assembly.LoadFile(assemblyPath);
    }

    public IEnumerable<ITypeInfo> GetInterfaces() {
      var typeInfoCollector = new TypeInfoCollector();
      typeInfoCollector.AddTypeInfo(GetSignalRInterfaceInfo(typeInfoCollector));
      return typeInfoCollector.GetTypeInfos();
    }

    ITypeInfo GetSignalRInterfaceInfo(TypeInfoCollector typeInfoCollector) {
      return new ComplexTypeInfo(
        "SignalR",
        string.Empty,
        GetSignalRMembers(typeInfoCollector),
        null
      );
    }

    IEnumerable<PropertyInfo> GetSignalRMembers(TypeInfoCollector typeInfoCollector) {
      var propertyInfos = _hubmanager
        .GetHubs()
        .Select(
          hubDescriptor => {
            var typeInfo = new ComplexTypeInfo(
              hubDescriptor.HubType.Name,
              hubDescriptor.HubType.Namespace,
              new[] {
                new PropertyInfo("server", GetServerTypeInfo(hubDescriptor, typeInfoCollector)),
                new PropertyInfo("client", GetClientTypeInfo(hubDescriptor, typeInfoCollector))
              },
              null
            );
            typeInfoCollector.AddTypeInfo(typeInfo);
            return new PropertyInfo(
              hubDescriptor.NameSpecified ? hubDescriptor.Name : hubDescriptor.HubType.Name.ToCamelCase(),
              typeInfo
            );
          }
        )
        .ToList();
      return propertyInfos;
    }

    ITypeInfo GetServerTypeInfo(HubDescriptor hubDescriptor, TypeInfoCollector typeInfoCollector) {
      var typeInfo = new ComplexTypeInfo(
        hubDescriptor.HubType.Name + "Server",
        hubDescriptor.HubType.Namespace,
        _hubmanager
          .GetHubMethods(hubDescriptor.Name)
          .Select(
            methodDescriptor => new MethodInfo(
              methodDescriptor.Name.ToCamelCase(),
              typeInfoCollector.GetTypeInfo(typeof(JQueryPromise<>).MakeGenericType(methodDescriptor.ReturnType)),
              methodDescriptor.Parameters.Select(parameter => new ParameterInfo(parameter.Name, typeInfoCollector.GetTypeInfo(parameter.ParameterType))).ToList()
            )
          )
          .ToList(),
          null
        );
      typeInfoCollector.AddTypeInfo(typeInfo);
      return typeInfo;
    }

    static ITypeInfo GetClientTypeInfo(HubDescriptor hubDescriptor, TypeInfoCollector typeInfoCollector) {
      var type = GetClientType(hubDescriptor.HubType);
      if (type == null)
        return BuiltinTypeInfo.Any;
      var typeInfo = new ComplexTypeInfo(type.Name, type.Namespace, GetClientMembers(type, typeInfoCollector).ToList(), null);
      typeInfoCollector.AddTypeInfo(typeInfo);
      return typeInfo;
    }

    static IEnumerable<IMemberInfo> GetClientMembers(IReflect type, TypeInfoCollector typeInfoCollector) {
      return type
        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(methodInfo => !methodInfo.IsSpecialName)
        .Select(
          methodInfo => new PropertyInfo(
            methodInfo.Name.ToCamelCase(),
            new FunctionTypeInfo(
              methodInfo.GetParameters().Select(parameter => new ParameterInfo(parameter.Name, typeInfoCollector.GetTypeInfo(parameter.ParameterType))).ToList(),
              typeInfoCollector.GetTypeInfo(methodInfo.ReturnType)
            )
          )
        );
    }

    static Type GetClientType(Type hubType) {
      while (hubType != null && hubType != typeof(Hub)) {
        if (hubType.IsGenericType && hubType.GetGenericTypeDefinition() == typeof(Hub<>))
          return hubType.GetGenericArguments().Single();
        hubType = hubType.BaseType;
      }
      return null;
    }

  }

}