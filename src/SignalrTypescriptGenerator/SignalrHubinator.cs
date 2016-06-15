using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalrTypescriptGenerator.Models;
using TypeInfo = SignalrTypescriptGenerator.Models.TypeInfo;

namespace SignalrTypescriptGenerator {

  class SignalrHubinator {

    static string assemblyRootFolder;

    readonly DefaultHubManager _hubmanager;

    public SignalrHubinator(string assemblyPath) {
      assemblyRootFolder = Path.GetDirectoryName(assemblyPath);
      LoadAssemblyIntoAppDomain(assemblyPath);

      TypeHelper = new TypeHelper();

      var defaultDependencyResolver = new DefaultDependencyResolver();
      _hubmanager = new DefaultHubManager(defaultDependencyResolver);
    }

    public TypeHelper TypeHelper { get; }

    static Assembly LoadFromSameFolder(object sender, ResolveEventArgs args) {
      var assemblyPath = Path.Combine(assemblyRootFolder, new AssemblyName(args.Name).Name + ".dll");
      if (File.Exists(assemblyPath) == false)
        return null;

      var assembly = Assembly.LoadFrom(assemblyPath);
      return assembly;
    }

    void LoadAssemblyIntoAppDomain(string assemblyPath) {
      var currentDomain = AppDomain.CurrentDomain;
      currentDomain.AssemblyResolve += LoadFromSameFolder;
      Assembly.LoadFile(assemblyPath);
    }

    public List<TypeInfo> GetHubs() {

      var typeInfos = from hub in _hubmanager.GetHubs()
                      let name = hub.NameSpecified ? hub.Name : hub.Name.ToCamelCase()
                      let typename = hub.HubType.FullName
                      select new TypeInfo { Name = name, TypescriptType = typename };
      return typeInfos.ToList();
    }

    public List<ServiceInfo> GetServiceContracts() {
      var list = new List<ServiceInfo>();
      var serviceInfo = new ServiceInfo();

      foreach (var hub in _hubmanager.GetHubs()) {
        var hubType = hub.HubType;

        var moduleName = hubType.Namespace;
        var interfaceName = hubType.Name;
        serviceInfo.ModuleName = moduleName;
        serviceInfo.InterfaceName = interfaceName;

        var clientType = TypeHelper.ClientType(hubType);
        var clientTypeName = clientType != null ? clientType.FullName : "any";
        serviceInfo.ClientType = clientTypeName;

        // Server type and functions
        var serverType = hubType.Name + "Server";
        var serverFullNamespace = hubType.FullName + "Server";
        serviceInfo.ServerType = serverType;
        serviceInfo.ServerTypeFullNamespace = serverFullNamespace;
        foreach (var method in _hubmanager.GetHubMethods(hub.Name)) {
          var ps = method.Parameters.Select(x => x.Name + ": " + TypeHelper.GetTypeContractName(x.ParameterType));
          var functionDetails = new FunctionDetails {
            Name = method.Name.ToCamelCase(),
            Arguments = "(" + string.Join(", ", ps) + ")",
            ReturnType = "JQueryPromise<" + TypeHelper.GetTypeContractName(method.ReturnType) + ">"
          };

          serviceInfo.ServerFunctions.Add(functionDetails);
        }

        list.Add(serviceInfo);
      }

      return list;
    }

    public List<ClientInfo> GetClients() {
      var list = new List<ClientInfo>();

      foreach (var hub in _hubmanager.GetHubs()) {
        var hubType = hub.HubType;
        var clientType = TypeHelper.ClientType(hubType);

        if (clientType != null) {
          var moduleName = clientType.Namespace;
          var interfaceName = clientType.Name;
          var clientInfo = new ClientInfo();

          clientInfo.ModuleName = moduleName;
          clientInfo.InterfaceName = interfaceName;
          clientInfo.FunctionDetails = TypeHelper.GetClientFunctions(hubType);
          list.Add(clientInfo);
        }
      }

      return list;
    }

    public List<DataContractInfo> GetDataContracts() {
      var list = new List<DataContractInfo>();

      while (TypeHelper.InterfaceTypes.Count != 0) {
        var type = TypeHelper.InterfaceTypes.Pop();
        var dataContractInfo = new DataContractInfo();

        var moduleName = type.Namespace;
        var interfaceName = TypeHelper.GenericSpecificName(type, false);

        dataContractInfo.ModuleName = moduleName;
        dataContractInfo.InterfaceName = interfaceName;

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) {
          var propertyName = property.Name;
          var typeName = TypeHelper.GetTypeContractName(property.PropertyType);

          dataContractInfo.Properties.Add(new TypeInfo { Name = propertyName, TypescriptType = typeName });
        }

        list.Add(dataContractInfo);
      }

      return list;
    }

    public List<EnumInfo> GetEnums() {
      var list = new List<EnumInfo>();

      while (TypeHelper.EnumTypes.Count != 0) {
        var type = TypeHelper.EnumTypes.Pop();
        var enuminfo = new EnumInfo();

        var moduleName = type.Namespace;
        var interfaceName = TypeHelper.GenericSpecificName(type, false);

        enuminfo.ModuleName = moduleName;
        enuminfo.InterfaceName = interfaceName;

        foreach (var name in Enum.GetNames(type)) {
          var propertyName = name;
          string typeName = $"{Enum.Parse(type, name):D}";

          enuminfo.Properties.Add(new TypeInfo { Name = propertyName, TypescriptType = typeName });
        }

        list.Add(enuminfo);
      }

      return list;
    }

  }

}