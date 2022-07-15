#region Using
using Autodesk.Revit.DB;
using BeyCons.Core.RevitUtils.DataUtils.Enums;
using BeyCons.Core.RevitUtils.DataUtils.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#endregion

namespace BeyCons.Core.RevitUtils.DataUtils
{
    public static class ParameterUtils
    {
        public static bool AreParametersInFileShareParameter(DefinitionFile definitionFile, IList<ParameterType> parameterTypes, CustomGroupParameters customGroupParameters, List<List<string>> parameterNames)
        {
            if (definitionFile != null)
            {
                DefinitionGroups definitionGroups = definitionFile.Groups;
                if (definitionGroups != null)
                {
                    foreach (DefinitionGroup definitionGroup in definitionGroups)
                    {
                        if (definitionGroup.Name == customGroupParameters.ToString())
                        {
                            for (int i = 0; i < parameterTypes.Count; i++)
                            {
                                foreach (string parameterName in parameterNames[i])
                                {
                                    Definition definition = definitionGroup.Definitions.get_Item(parameterName);
                                    if (definition != null)
                                    {
                                        if (definition.ParameterType != parameterTypes[i])
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public static bool AreParametersInProject(Document document, IList<string> nameParameters, bool isShare)
        {
            IEnumerable<ParameterElement> allParameters = new FilteredElementCollector(document).OfClass(typeof(ParameterElement)).Cast<ParameterElement>();
            List<string> nameParametersProject = new List<string>();
            foreach (ParameterElement parameterElement in allParameters)
            {
                if (isShare)
                {
                    if (parameterElement is SharedParameterElement)
                    {
                        nameParametersProject.Add(parameterElement.GetDefinition().Name);
                    }
                }
                else
                {
                    if (!(parameterElement is SharedParameterElement))
                    {
                        nameParametersProject.Add(parameterElement.GetDefinition().Name);
                    }
                }
            }
            bool result = true;
            foreach (string nameParameter in nameParameters)
            {
                result = result && nameParametersProject.Contains(nameParameter);
            }
            return result;
        }
        public static bool AreParametersShareInProject(IList<string> parameters)
        {
            List<string> parameterShares = new List<string>();
            DefinitionBindingMapIterator definitionBindingMapIterator = RevitData.Instance.Document.ParameterBindings.ForwardIterator();
            definitionBindingMapIterator.Reset();
            while (definitionBindingMapIterator.MoveNext())
            {
                Definition definition = definitionBindingMapIterator.Key;
                if (definition != null)
                {
                    parameterShares.Add(definition.Name);
                }
            }

            foreach (string parameter in parameters)
            {
                if (!parameterShares.Contains(parameter))
                {
                    return false;
                }
            }
            return true;
        }
        public static DefinitionFile GetSharedParamDefinitionFile(Document document, string fileShareName)
        {
            DefinitionFile definitionFile = document.Application.OpenSharedParameterFile();
            if (definitionFile != null)
            {
                string fileShareNameInProject = Path.GetFileNameWithoutExtension(definitionFile.Filename);
                if (fileShareNameInProject == fileShareName)
                {
                    return definitionFile;
                }
            }
            return null;
        }
        public static List<UniqueParameter> CreateParameterUseShare(Document document, string nameFileShareParameter, IList<ParameterInformation> parameterInformations, ParameterType parameterType, IList<BuiltInCategory> builtInCategories, BuiltInParameterGroup builtInParameterGroup, CustomGroupParameters customGroupParameters, bool instance)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), $"{nameFileShareParameter}.txt");
            if (File.Exists(tempFile))
            {
                File.Delete(tempFile);
            }
            using (FileStream fileStream = File.Create(tempFile)) { fileStream.Close(); };
            document.Application.SharedParametersFilename = tempFile;

            CategorySet categorySet = new CategorySet();
            foreach (BuiltInCategory builtInCategory in builtInCategories)
            {
                Category category = Category.GetCategory(document, builtInCategory);
                categorySet.Insert(category);
            }

            DefinitionGroup definitionGroup = document.Application.OpenSharedParameterFile().Groups.Create(customGroupParameters.ToString());

            Binding binding = document.Application.Create.NewTypeBinding(categorySet);
            if (instance) binding = document.Application.Create.NewInstanceBinding(categorySet);

            BindingMap bindingMap = document.ParameterBindings;

            List<UniqueParameter> uniqueParameters = new List<UniqueParameter>();
            foreach (ParameterInformation parameterInformation in parameterInformations)
            {                
                ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterInformation.Name, parameterType) { UserModifiable = !parameterInformation.ReadOnly, Description = parameterInformation.Description, Visible = parameterInformation.Visible };                
                ExternalDefinition externalDefinition = definitionGroup.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
                uniqueParameters.Add(new UniqueParameter() { ParameterInformation = parameterInformation, Guid = externalDefinition.GUID });
                bindingMap.Insert(externalDefinition, binding, builtInParameterGroup);
            }
            return uniqueParameters;
        }
        public static List<UniqueParameter> AddParameterToFileShare(Document document, CustomGroupParameters customGroupParameters, IList<ParameterInformation> parameterInformations, ParameterType parameterType, IList<BuiltInCategory> builtInCategories, BuiltInParameterGroup builtInParameterGroup, bool instance)
        {
            DefinitionFile definitionFile = document.Application.OpenSharedParameterFile();
            List<UniqueParameter> uniqueParameters = new List<UniqueParameter>();
            if (definitionFile != null)
            {
                DefinitionGroup definitionGroup = null;
                foreach (DefinitionGroup group in definitionFile.Groups)
                {
                    if (group.Name == customGroupParameters.ToString())
                    {
                        definitionGroup = group;
                        break;
                    }
                }

                CategorySet categorySet = new CategorySet();
                foreach (BuiltInCategory builtInCategory in builtInCategories)
                {
                    Category category = Category.GetCategory(document, builtInCategory);
                    categorySet.Insert(category);
                }

                if (definitionGroup != null)
                {
                    Binding binding = document.Application.Create.NewTypeBinding(categorySet);
                    if (instance) binding = document.Application.Create.NewInstanceBinding(categorySet);

                    BindingMap bindingMap = document.ParameterBindings;

                    foreach (ParameterInformation parameterInformation in parameterInformations)
                    {
                        ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterInformation.Name, parameterType) { UserModifiable = !parameterInformation.ReadOnly, Description = parameterInformation.Description, Visible = parameterInformation.Visible };
                        ExternalDefinition externalDefinition = definitionGroup.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
                        uniqueParameters.Add(new UniqueParameter() { ParameterInformation = parameterInformation, Guid = externalDefinition.GUID });
                        bindingMap.Insert(externalDefinition, binding, builtInParameterGroup);
                    }
                }
                else
                {
                    definitionGroup = definitionFile.Groups.Create(customGroupParameters.ToString());

                    Binding binding = document.Application.Create.NewTypeBinding(categorySet);
                    if (instance) binding = document.Application.Create.NewInstanceBinding(categorySet);

                    BindingMap bindingMap = document.ParameterBindings;

                    foreach (ParameterInformation parameterInformation in parameterInformations)
                    {
                        ExternalDefinitionCreationOptions externalDefinitionCreationOptions = new ExternalDefinitionCreationOptions(parameterInformation.Name, parameterType) { UserModifiable = parameterInformation.ReadOnly, Description = parameterInformation.Description, Visible = parameterInformation.Visible };
                        ExternalDefinition externalDefinition = definitionGroup.Definitions.Create(externalDefinitionCreationOptions) as ExternalDefinition;
                        uniqueParameters.Add(new UniqueParameter() { ParameterInformation = parameterInformation, Guid = externalDefinition.GUID });
                        bindingMap.Insert(externalDefinition, binding, builtInParameterGroup);
                    }
                }
            }
            return uniqueParameters;
        }
    }
}
