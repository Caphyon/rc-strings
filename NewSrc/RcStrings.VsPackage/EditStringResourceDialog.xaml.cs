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
        if (mSelectedRcFile == null || !AddMode)
          return;

        if (!mRcFilesContexts.TryGetValue(mSelectedRcFile, out StringResourceContext context))
        {
          context = new StringResourceContext(mSelectedRcFile, ShowGhostFile);
          mRcFilesContexts[mSelectedRcFile] = context;
        }
        ResourceIdTemp = context.GetId.ToString();
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

    private Dictionary<string, List<string>> Errors { get; } = new Dictionary<string, List<string>>();
    
    private bool HasError => Errors.Any();

    #endregion

    #region Ctor

    public EditStringResourceDialog(IServiceProvider aServiceProvider, List<RcFile> aRcFiles, RcFile aSelectedRcFile,
      string aSelectedText, bool aReplaceCode, string aReplaceWithCodeFormated, bool aShowGhostFile, RCFileItem aStringResource = null)
    {
      InitializeComponent();
      DataContext = this;
      mRcFilesContexts = new Dictionary<RcFile, StringResourceContext>();
      mServiceProvider = aServiceProvider;
      this.ShowGhostFile = aShowGhostFile;
      this.AddMode = (aStringResource == null);
      this.ReplaceStringCodeFormated = aReplaceWithCodeFormated;
      this.RcFiles = aRcFiles;
      this.SelectedRcFile = (aSelectedRcFile == null) ?
        RcFiles.ElementAt(0) : RcFiles.FirstOrDefault(rcf =>
        string.Equals(rcf.FilePath, aSelectedRcFile.FilePath, StringComparison.OrdinalIgnoreCase));
      if (!AddMode)
      {
        this.mInitialStringValue = aStringResource.Value;
        this.ResourceName = aStringResource.Name;
        this.ResourceIdTemp = aStringResource.ID.Value;
        this.ReplaceCode = false;
        this.ResourceValue = aStringResource.Value.TrimSuffix("\"").TrimPrefix("\"");
        ResourceValue = new EscapeSequences().Escape(ResourceValue);
      }
      else
      {
        // Set resource value the text between quotation marks inclusive
        this.ReplaceCode = aReplaceCode;
        this.ResourceValue = @aSelectedText;
        this.ResourceName = new NameGenerator(this.ResourceValue).Generate();
        if (ResourceContext.ResourceNameExists(this.ResourceName))
          this.ResourceName = string.Format("{0}{1}", this.ResourceName, this.ResourceIdTemp);
      }

    }
    #endregion

    #region Private methods

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {
      CloseWindow(false);
    }
    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
      if (!AddMode && ResourceValue == mInitialStringValue)
      {
        CloseWindow(false);
        return;
      }
      if (HasError)
      {
        VsShellUtilities.ShowMessageBox(mServiceProvider, Errors.First().Value.First(), "Invalid input",
          OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        return;
      }
      if (AddMode && ResourceId != StringEnhancer.Constants.kInvalidID && ResourceContext.IdExists(ResourceId))
      {
        VsShellUtilities.ShowMessageBox(mServiceProvider, string.Format("String Id {0} already exists.", ResourceId),
          "Invalid input", OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
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

    public string this[string PropertyName]
    {
      get
      {
        CollectErrors();
        return Errors.ContainsKey(PropertyName) ?
          String.Join("\n", Errors[PropertyName]) : string.Empty;
      }
    }

    private void CollectErrors()
    {
      Errors.Clear();

      if (AddMode)
      {
        if (string.IsNullOrEmpty(ResourceName) ||
          ResourceName.Contains(" "))
        {
          if (!Errors.ContainsKey(nameof(ResourceName)))
            Errors[nameof(ResourceName)] = new List<string>();

          Errors[nameof(ResourceName)].Add(string.Format(
            $"Oh Snap! Your name field can't contain whitespaces!"));
        }

        if (string.IsNullOrEmpty(ResourceName) ||
          ResourceName.Length > ParseConstants.kMaximumResourceNameLength)
        {
          if (!Errors.ContainsKey(nameof(ResourceName)))
            Errors[nameof(ResourceName)] = new List<string>();

          Errors[nameof(ResourceName)].Add(string.Format(
            $"Name with maximum length of {ParseConstants.kMaximumResourceNameLength} is required!"));
        }

        if (AddMode && (string.IsNullOrEmpty(ResourceName) ||
          ResourceContext.ResourceNameExists(ResourceName)))
        {
          if (!Errors.ContainsKey(nameof(ResourceName)))
            Errors[nameof(ResourceName)] = new List<string>();

          Errors[nameof(ResourceName)].Add(string.Format($"Looks like the name \"{ResourceName}\" already exists!"));
        }

        if (ResourceId == StringEnhancer.Constants.kInvalidID)
        {
          if (!Errors.ContainsKey(nameof(ResourceIdTemp)))
            Errors[nameof(ResourceIdTemp)] = new List<string>();

          Errors[nameof(ResourceIdTemp)].Add(string.Format($"Invalid ID! (Must be integer)"));
        }
        else if (string.IsNullOrEmpty(ResourceId.Value) ||
          (ResourceContext.IdExists(ResourceId) && AddMode))
        {
          if (!Errors.ContainsKey(nameof(ResourceIdTemp)))
            Errors[nameof(ResourceIdTemp)] = new List<string>();

          Errors[nameof(ResourceIdTemp)].Add(string.Format($"Unique ID is required!"));
        }
      }

      if (string.IsNullOrEmpty(ResourceValue) ||
        ResourceValue.Length > ParseConstants.kMaximumResourceValueLength)
      {
        if (!Errors.ContainsKey(nameof(ResourceValue)))
          Errors[nameof(ResourceValue)] = new List<string>();

        Errors[nameof(ResourceValue)].Add(
          string.Format($"Value with maximum length of {ParseConstants.kMaximumResourceValueLength} characters is required!"));
      }
    }
    #endregion
  }
}
