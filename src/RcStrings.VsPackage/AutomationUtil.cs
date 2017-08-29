using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caphyon.RcStrings.VsPackage
{
  public class AutomationUtil
  {

    #region Methods
    public static int GetProjectFromIVsHierarchy(IVsHierarchy pHierarchy, out EnvDTE.Project aProject)
    {
      aProject = null;
      if (null == pHierarchy)
        return VSConstants.E_INVALIDARG;

      int err = pHierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ExtObject, out object projObj);
      if (VSConstants.S_OK != err)
        return err;

      aProject = projObj as EnvDTE.Project;
      return VSConstants.S_OK;
    }

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

    public static EnvDTE.Project FindProjectByGuid(IServiceProvider aServiceProvider, Guid aGuid)
    {
      var solution = aServiceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
      if (null == solution)
        return null;

      int err = solution.GetProjectOfGuid(aGuid, out IVsHierarchy hierarchy);
      if (VSConstants.S_OK != err)
        return null;

      err = GetProjectFromIVsHierarchy(hierarchy, out EnvDTE.Project project);
      if (VSConstants.S_OK != err)
        return null;

      return project;
    }

    public static Guid GetProjectGuid(IServiceProvider aServiceProvider, EnvDTE.Project aProject)
    {
      var solution = aServiceProvider.GetService(typeof(SVsSolution)) as IVsSolution;
      if (null == solution)
        return Guid.Empty;

      int err = solution.GetProjectOfUniqueName(aProject.UniqueName, out IVsHierarchy hierarchy);
      if (VSConstants.S_OK != err)
        return Guid.Empty;

      err = hierarchy.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid projectGuid);
      if (VSConstants.S_OK != err)
        return Guid.Empty;

      return projectGuid;
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
