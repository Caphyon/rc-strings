using Caphyon.RcStrings.StringEnhancer;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
    private int mResourceId;
    private string mResourceIdTemp;
    private string mInitialStringValue;
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
        if( ResourceName.Length > 0 )
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

    public StringResourceContext ResourceContext => mRcFilesContexts[SelectedRcFile];

    public IEnumerable<RcFile> RcFiles { get; private set; }

    public RcFile SelectedRcFile
    {
      get => mSelectedRcFile;
      set
      {
        mSelectedRcFile = value;

        if (mSelectedRcFile == null || !AddMode)
          return;

        StringResourceContext context;
        if (!mRcFilesContexts.TryGetValue(mSelectedRcFile, out context))
        {
          context = new StringResourceContext(mSelectedRcFile);
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

    public int ResourceId
    {
      get => mResourceId;
      set => mResourceId = value;
    }
    
    #endregion

    #region Ctor
    public EditStringResourceDialog(List<RcFile> aRcFiles, RcFile aSelectedRcFile,
      string aSelectedText, bool aReplaceCode, string aReplaceWithCodeFormated, StringLine aStringResource = null)
    {
      if (aRcFiles.Count == 0)
        throw new Exception("solution has not rc files");

      InitializeComponent();
      DataContext = this;

      mRcFilesContexts = new Dictionary<RcFile, StringResourceContext>();

      this.AddMode = aStringResource == null;
      this.ReplaceStringCodeFormated = aReplaceWithCodeFormated;
      this.RcFiles = aRcFiles;

      this.SelectedRcFile = aSelectedRcFile == null ?
        RcFiles.ElementAt(0) :
        RcFiles.FirstOrDefault(rcf => string.Equals(
          rcf.FilePath, aSelectedRcFile.FilePath, StringComparison.OrdinalIgnoreCase));

      if (!AddMode)
      {
        this.mInitialStringValue = aStringResource.Value;
        this.ResourceName = aStringResource.Name;
        this.ResourceIdTemp = aStringResource.Id.ToString();
        this.ReplaceCode = false;
        this.ResourceValue = aStringResource.Value;
      }
      else
      {
        this.ReplaceCode = aReplaceCode;
        // Set resource value the text between quotation marks
        int quoteStartIndex = aSelectedText.IndexOf('"');
        int quoteEndIndex = aSelectedText.LastIndexOf('"');
        this.ResourceValue = quoteStartIndex >= 0 && quoteEndIndex >= 0 && quoteStartIndex != quoteEndIndex ?
                                  @aSelectedText.Substring(quoteStartIndex + 1, quoteEndIndex - quoteStartIndex - 1) :
                                  @aSelectedText;
        this.ResourceName = new NameGenerator(this.ResourceValue).Generate();
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

      if (AddMode && ResourceId != 0 && ResourceContext.IdExists(ResourceId))
      {
        MessageBox.Show(
          string.Format("String Id {0} already exists.", ResourceId),
          "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region IDataErrorInfo Implementation

    public string Error => null;

    public string this[string PropertyName]
    {
      get
      {
        string result = String.Empty;
        switch (PropertyName)
        {
          case "ResourceName":
            if (string.IsNullOrEmpty(ResourceName) || 
              ResourceName.Length > ParseConstants.kMaximumResourceNameLength ||
              TagConstants.kStringPreffix.Equals(ResourceName.Substring(0, ResourceName.Length))) 
              result = "Name with preffix IDS_ and maximum length of 247 is required!";
            break;

          case "ResourceValue":
            if (string.IsNullOrEmpty(ResourceValue) 
              || ResourceValue.Length > ParseConstants.kMaximumResourceValueLength)
              result = "Value with maximum length of 4096 characters is required!";
            break;

          case "ResourceIdTemp":
            bool validInput = false;
            if (!string.IsNullOrEmpty(ResourceIdTemp))
              if (ParseUtility.TransformToDecimal(ResourceIdTemp, out mResourceId))
                if (mResourceId >= 0 && mResourceId <= IdGenerator.kMaximumId)
                  validInput = true;
            if (!validInput)
              result = "Positive id less then 65535 is required!";
            break;
        }
        btnAdd.IsEnabled = (result == String.Empty && IsValid(this as DependencyObject));
        return result;
      }
    }

    private bool IsValid(DependencyObject obj) =>
      !Validation.GetHasError(obj) &&
      LogicalTreeHelper.GetChildren(obj)
        .OfType<DependencyObject>()
        .All(IsValid);

    #endregion

  }
}
