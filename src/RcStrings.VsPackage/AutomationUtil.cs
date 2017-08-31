using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;

namespace Caphyon.RcStrings.VsPackage
{
  public class AutomationUtil
  {

    #region Methods

    public static List<EnvDTE.Project> GetAllProjects(EnvDTE.DTE aDTE)
    {
      List<EnvDTE.Project> list = new List<EnvDTE.Project>();
      foreach (EnvDTE.Project project in aDTE.Solution.Projects)
      {
        if (null == project)
          continue;

        if (project.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
          list.AddRange(GetSolutionFolderProjects(project));
        else
          list.Add(project);
      }
      return list;
    }

    public static Project ReloadProject(IServiceProvider aServiceProvider, EnvDTE.Project aProject)
    {
      IVsSolution4 solution = aServiceProvider.GetService(typeof(SVsSolution)) as IVsSolution4;
      int err = ((IVsSolution)solution).GetProjectOfUniqueName(aProject.UniqueName, out IVsHierarchy hierarchy);
      if (VSConstants.S_OK != err)
        return null;

      uint itemId = (uint)VSConstants.VSITEMID.Root;
      err = hierarchy.GetGuidProperty(itemId, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid projectGuid);
      if (VSConstants.S_OK != err)
        return null;

      err = solution.EnsureProjectIsLoaded(projectGuid, (uint)__VSBSLFLAGS.VSBSLFLAGS_None);
      if (VSConstants.S_OK != err)
        return null;

      err = ((IVsSolution)solution).GetProjectOfGuid(projectGuid, out IVsHierarchy loadedProject);
      if (VSConstants.S_OK != err)
        return null;

      loadedProject.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out object objProject);
      return objProject as EnvDTE.Project;
    }

    #endregion

    #region Helpers

    private static IEnumerable<EnvDTE.Project> GetSolutionFolderProjects(EnvDTE.Project aSolutionFolderItem)
    {
      List<EnvDTE.Project> list = new List<EnvDTE.Project>();
      foreach (EnvDTE.ProjectItem projectItem in aSolutionFolderItem.ProjectItems)
      {
        EnvDTE.Project subProject = projectItem.SubProject;
        if (null == subProject)
          continue;

        if (subProject.Kind == EnvDTE.Constants.vsProjectKindSolutionItems)
          list.AddRange(GetSolutionFolderProjects(subProject));
        else
          list.Add(subProject);
      }
      return list;
    }

    #endregion
  }
}
