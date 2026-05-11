using System;
using System.Windows.Forms;

namespace UI
{
 internal static class Program
 {
 [STAThread]
 static void Main()
 {
 Application.EnableVisualStyles();
 Application.SetHighDpiMode(HighDpiMode.SystemAware);

 // Global exception handlers to capture runtime errors and show details
 Application.ThreadException += (sender, args) =>
 {
 ShowException(args.Exception);
 };
 AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
 {
 if (args.ExceptionObject is Exception ex)
 ShowException(ex);
 else
 MessageBox.Show("Unhandled non-exception object thrown: " + args.ExceptionObject, "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
 };

 try
 {
 using var loginForm = new LoginForm();
 if (loginForm.ShowDialog() != DialogResult.OK)
 {
 return;
 }

 // run main form (catch exceptions during startup)
 try
 {
 Application.Run(new FormMain(loginForm.SelectedRole, loginForm.SelectedCustomer));
 }
 catch (Exception ex)
 {
 ShowException(ex);
 }
 }
 catch (Exception ex)
 {
 ShowException(ex);
 }
 }

 private static void ShowException(Exception ex)
 {
 try
 {
 MessageBox.Show(ex.ToString(), "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
 }
 catch
 {
 // swallow any exceptions while showing the error
 }
 }
 }
}
