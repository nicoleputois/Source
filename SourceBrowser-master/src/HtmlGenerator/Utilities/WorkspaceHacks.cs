﻿using System;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;

namespace Microsoft.SourceBrowser.HtmlGenerator
{
    public static class WorkspaceHacks
    {
        public static dynamic GetSemanticFactsService(Document document)
        {
            return GetService(document, "Microsoft.CodeAnalysis.LanguageServices.ISemanticFactsService", "Microsoft.CodeAnalysis.Workspaces");
        }

        public static dynamic GetSyntaxFactsService(Document document)
        {
            return GetService(document, "Microsoft.CodeAnalysis.LanguageServices.ISyntaxFactsService", "Microsoft.CodeAnalysis.Workspaces");
        }

        public static object GetMetadataAsSourceService(Document document)
        {
            var language = document.Project.Language;
            var workspace = document.Project.Solution.Workspace;
            var serviceAssembly = Assembly.Load("Microsoft.CodeAnalysis.Features");
            var serviceInterfaceType = serviceAssembly.GetType("Microsoft.CodeAnalysis.MetadataAsSource.IMetadataAsSourceService");
            var result = GetService(workspace, language, serviceInterfaceType);
            return result;
        }

        private static object GetService(Workspace workspace, string language, Type serviceType)
        {
            var languageServices = workspace.Services.GetLanguageServices(language);
            var languageServicesType = typeof(HostLanguageServices);
            var genericMethod = languageServicesType.GetMethod("GetService", BindingFlags.Public | BindingFlags.Instance);
            var closedGenericMethod = genericMethod.MakeGenericMethod(serviceType);
            var result = closedGenericMethod.Invoke(languageServices, new object[0]);
            if (result == null)
            {
                throw new NullReferenceException("Unable to get language service: " + serviceType.FullName + " for " + language);
            }

            return result;
        }

        private static object GetService(Document document, string serviceType, string assemblyName)
        {
            var assembly = typeof(Document).Assembly;
            var documentExtensions = assembly.GetType("Microsoft.CodeAnalysis.Shared.Extensions.DocumentExtensions");
            var serviceAssembly = Assembly.Load(assemblyName);
            var serviceInterfaceType = serviceAssembly.GetType(serviceType);
            var getLanguageServiceMethod = documentExtensions.GetMethod("GetLanguageService", new Type[] { typeof(Document) });
            getLanguageServiceMethod = getLanguageServiceMethod.MakeGenericMethod(serviceInterfaceType);
            var service = getLanguageServiceMethod.Invoke(null, new object[] { document });
            return service;
        }
    }
}
