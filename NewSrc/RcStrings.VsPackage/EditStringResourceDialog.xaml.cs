using StringEnhancer;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace Caphyon.RcStrings.VsPackage
{
  public partial class EditStringResourceDialog : DialogWindow, INotifyPropertyChanged, IDataErrorInfo
  {
    #region Constants
    const string kEditResourceWindowTitle = "Edit String Resource"; // EDIT_RESOURCE_WINDOW_TITLE
    const string kAddResourceWindowTitle = "Add String Resource"; // ADD_RESOURCE_WINDOW_TITLE
    #endregion

    #region Members

    private Dictionary<RcFile, StringResourceContext> mRcFilesContexts;
    private string mReplaceWithCode;
    private string mResourceName;
    private bool mReplaceCode;
    private RcFile mSelectedRcFile;
    private string mResourceIdTemp;
    private string mInitialStringValue;
    private string mInitialStringName;
    private IServiceProvider mServiceProvider;

    #endregion

    #region Properties

    public string ResourceValue { get; set; }
    public string ReplaceStringCodeFormated { get; private set; }

    /// <summary>
    /// This property will allow only editing the string value.
    /// </summary>
    public bool AddMode { get; private set; }
    public string ReplaceWithCode
    {
      get => mReplaceWithCode;
      set
      {
        mReplaceWithCode = value;
        OnPropertyChanged("ReplaceWithCode");
        if (ResourceName.Length > 0)
          ReplaceStringCodeFormated = mReplaceWithCode.Replace(ResourceName, "{0}");
      }
    }
    public string ResourceName
    {
      get => mResourceName;
      set
      {
        mResourceName = value;
        ReplaceWithCode = string.Format(ReplaceStringCodeFormated, mResourceName);
        OnPropertyChanged("ResourceName");
      }
    }

    public bool ReplaceCode
    {
      get => mReplaceCode;
      set
      {
        mReplaceCode = value;
        OnPropertyChanged("ReplaceCode");
      }
    }
    public StringResourceContext ResourceContext
    {
      get
      {
        try
        {
          mRcFilesContexts.TryGetValue(SelectedRcFile, out StringResourceContext context);
          return context;
        }
        catch
        {
          VsShellUtilities.ShowMessageBox(mServiceProvider, "Could not retrieve resource context!", "Error",
            OLEMSGICON.OLEMSGICON_INFO, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
          return null;
        }
      }
    }
    public IEnumerable<RcFile> RcFiles { get; private set; }
    public RcFile SelectedRcFile
    {
      get => mSelectedRcFile;
      set
      {
        mSelectedRcFile = value;
        if (mSelectedRcFile == null)
          return;

        if (!mRcFilesContexts.TryGetValue(mSelectedRcFile, out StringResourceContext context))
        {
          context = new StringResourceContext(mSelectedRcFile, ShowGhostFile);
          mRcFilesContexts[mSelectedRcFile] = context;
        }

        GenerateNewResourceIdTemp();
      }
    }
    public string ResourceIdTemp
    {
      get => mResourceIdTemp;
      set
      {
        mResourceIdTemp = value;
        OnPropertyChanged("ResourceIdTemp");
      }
    }
    public HeaderId ResourceId
    {
      get
      {
        if (IDValidator.IsHexaRepresentation(mResourceIdTemp))
          return new HeaderId(mResourceIdTemp);
        return IDNormalizer.NormalizeHexaID(new HeaderId(mResourceIdTemp));
      }
    }

    public bool ShowGhostFile { get; set; }
    public bool UniqueIdPerProject { get; set; }

    #endregion

    #region Ctor

    public EditStringResourceDialog(IServiceProvider aServiceProvider, List<RcFile> aRcFiles, RcFile aSelectedRcFile,
      string aSelectedText, bool aReplaceCode, string aReplaceWithCodeFormated, bool aShowGhostFile, bool aUniqueIdPerProject, RCFileItem aStringResource = null)
    {
      InitializeComponent();
      DataContext = this;
      mRcFilesContexts = new Dictionary<RcFile, StringResourceContext>();
      mServiceProvider = aServiceProvider;
      this.ShowGhostFile = aShowGhostFile;
      this.UniqueIdPerProject = aUniqueIdPerProject;
      this.AddMode = (aStringResource == null);
      this.ReplaceStringCodeFormated = aReplaceWithCodeFormated;
      this.RcFiles = aRcFiles;
      this.SelectedRcFile = (aSelectedRcFile == null) ?
        RcFiles.ElementAt(0) : RcFiles.FirstOrDefault(rcf =>
        string.Equals(rcf.FilePath, aSelectedRcFile.FilePath, StringComparison.OrdinalIgnoreCase));
      if (!AddMode)
      {
        this.mInitialStringValue = aStringResource.Value;
        this.mInitialStringName = aStringResource.Name;
        this.ResourceName = aStringResource.Name;
        this.ResourceIdTemp = aStringResource.ID.Value;
        this.ReplaceCode = false;
        this.ResourceValue = aStringResource.Value.TrimSuffix("\"").TrimPrefix("\"");
        ResourceValue = new EscapeSequences().Escape(ResourceValue);
      }
      else
      {
        this.ReplaceCode = aReplaceCode;
        this.ResourceValue = @aSelectedText;
        this.ResourceName = new NameGenerator(ResourceValue).Generate();
        GenerateNewResourceIdTemp();
        this.mInitialStringName = ResourceName;
      }
    }
    #endregion

    #region Private methods

    private void GenerateNewResourceIdTemp()
    {
      if (ResourceName == null) return;
      while (ResourceIdTemp != null && ResourceName.EndsWith(ResourceIdTemp))
        ResourceName = ResourceName.Remove(ResourceName.Length - ResourceIdTemp.Length);
      ResourceIdTemp = mRcFilesContexts[mSelectedRcFile].GenerateId(mSelectedRcFile, RcFiles, UniqueIdPerProject);
      while (ResourceContext.ResourceNameExists(ResourceName))
        ResourceName = $"{ResourceName}{ResourceIdTemp}";
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      CloseWindow(false);
    }
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      if (!AddMode && ResourceValue == mInitialStringValue && ResourceName == mInitialStringName)
      {
        CloseWindow(false);
        return;
      }
      if (HasError)
      {
        VsShellUtilities.ShowMessageBox(mServiceProvider, mErrorCollector.First().Value.First(), "Invalid input",
          OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        return;
      }
      CloseWindow(true);
    }
    private void CloseWindow(bool aDialogResult)
    {
      DialogResult = aDialogResult;
      this.Close();
    }
    private void DialogWindow_Loaded(object sender, RoutedEventArgs e)
    {
      if (AddMode)
      {
        tbxResourceName.Focus();
        tbxResourceName.Select(TagConstants.kStringPreffix.Length, tbxResourceName.Text.Length);
      }
      else
      {
        tbxResourceValue.Focus();
        tbxResourceValue.CaretIndex = tbxResourceValue.Text.Length;
      }
    }
    #endregion

    #region INotifyPropertyChanged Implementation

    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region IDataErrorInfo Implementation

    public string Error => null;
    public bool HasError => mErrorCollector.HasAnyErrors();
    private readonly ErrorCollector mErrorCollector = new ErrorCollector();

    public string this[string aPropertyName]
    {
      get
      {
        CollectErrors(aPropertyName);
        OnPropertyChanged("HasError");
        return mErrorCollector.HasError(aPropertyName);
      }
    }

    private void CollectErrors(string aPropertyName)
    {
      mErrorCollector.Clear(aPropertyName);

      if (aPropertyName == "ResourceIdTemp")
      {
        if (AddMode)
        {
          mErrorCollector.CheckId(ResourceContext, ResourceIdTemp, ResourceId);
        }
      }
      else if (aPropertyName == "ResourceName")
      {
        if (ResourceName != mInitialStringName)
          mErrorCollector.CheckName(ResourceContext, ResourceName);
      }
      else if (aPropertyName == "ResourceValue")
      {
        mErrorCollector.CheckValue(ResourceValue);
      }
    }

    #endregion
  }
}
