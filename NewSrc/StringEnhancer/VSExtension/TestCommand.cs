using System;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Task = System.Threading.Tasks.Task;

namespace VSExtension
{
  /// <summary>
  /// Command handler
  /// </summary>
  internal sealed class TestCommand
  {
    /// <summary>
    /// Command ID.
    /// </summary>
    public const int CommandId = 0x0100;

    /// <summary>
    /// Command menu group (command set GUID).
    /// </summary>
    public static readonly Guid CommandSet = new Guid("657ed51e-6e2e-42fe-9a1b-2a0990c8c389");

    /// <summary>
    /// VS Package that provides this command, not null.
    /// </summary>
    private readonly AsyncPackage package;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestCommand"/> class.
    /// Adds our command handlers for menu (commands must exist in the command table file)
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    /// <param name="commandService">Command service to add command to, not null.</param>
    private TestCommand(AsyncPackage package, OleMenuCommandService commandService)
    {
      this.package = package ?? throw new ArgumentNullException(nameof(package));
      commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

      var menuCommandID = new CommandID(CommandSet, CommandId);
      var menuItem = new MenuCommand(this.Execute, menuCommandID);
      commandService.AddCommand(menuItem);
    }

    /// <summary>
    /// Gets the instance of the command.
    /// </summary>
    public static TestCommand Instance
    {
      get;
      private set;
    }

    /// <summary>
    /// Gets the service provider from the owner package.
    /// </summary>
    private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
    {
      get
      {
        return this.package;
      }
    }

    /// <summary>
    /// Initializes the singleton instance of the command.
    /// </summary>
    /// <param name="package">Owner package, not null.</param>
    public static async Task InitializeAsync(AsyncPackage package)
    {
      // Verify the current thread is the UI thread - the call to AddCommand in TestCommand's constructor requires
      // the UI thread.
      ThreadHelper.ThrowIfNotOnUIThread();

      OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
      Instance = new TestCommand(package, commandService);
    }

    /// <summary>
    /// This function is the callback used to execute the command when the menu item is clicked.
    /// See the constructor to see how the menu item is associated with this function using
    /// OleMenuCommandService service and MenuCommand class.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event args.</param>
    private async void Execute(object sender, EventArgs e)
    {
      var dte = await ServiceProvider.GetServiceAsync(typeof(DTE)) as DTE2;
      var solutionExplorer = dte.ToolWindows.SolutionExplorer;

      PrintItemsInSolution(dte.Solution);
    }

    private void PrintItemsInSolution(Solution solution)
    {
      StringBuilder names = new StringBuilder();

      foreach (Project project in solution.Projects)
      {
        names.AppendLine("-------------------------");
        RecurseItems(project.ProjectItems, names);
      }

      MessageBox.Show(names.ToString());
    }

    private void RecurseItems(ProjectItems items, StringBuilder names)
    {
      foreach (ProjectItem item in items)
      {
        if (item.Name.EndsWith(".cs"))
          names.AppendLine(item.Name);

        RecurseItems(item.ProjectItems, names);
      }
    }
  }
}
