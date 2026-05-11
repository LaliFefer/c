using System;
using System.Drawing;

namespace UI
{
 partial class FormMain
 {
 private System.ComponentModel.IContainer components = null;
 private System.Windows.Forms.Panel panelSidebar;
 private System.Windows.Forms.PictureBox pictureBoxLogo;
 private System.Windows.Forms.Label lblLogoText;
 private System.Windows.Forms.Button btnDashboard;
 private System.Windows.Forms.Button btnProducts;
 private System.Windows.Forms.Button btnCustomers;
 private System.Windows.Forms.Button btnSales;
 private System.Windows.Forms.Button btnOrder;
 private System.Windows.Forms.Panel panelHeader;
 private System.Windows.Forms.Label lblTitle;
 private System.Windows.Forms.Label lblDate;
 private System.Windows.Forms.Button btnMinimize;
 private System.Windows.Forms.Button btnProfile;
 private System.Windows.Forms.Button btnSignOut;
 private System.Windows.Forms.Button btnClose;
 private System.Windows.Forms.TabControl tabMain;
 private System.Windows.Forms.TabPage tabDashboard;
 private System.Windows.Forms.TabPage tabProducts;
 private System.Windows.Forms.TabPage tabCustomers;
 private System.Windows.Forms.TabPage tabSales;
 private System.Windows.Forms.TabPage tabOrder;

 protected override void Dispose(bool disposing)
 {
 if (disposing && (components != null))
 {
 components.Dispose();
 }
 base.Dispose(disposing);
 }
 #region Windows Form Designer generated code
 private void InitializeComponent()
 {
 this.components = new System.ComponentModel.Container();
 this.SuspendLayout();
 // FormMain
 this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
 this.Text = "Cashier Pro";
 this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
 this.ClientSize = new System.Drawing.Size(1200,760);
 this.Font = new System.Drawing.Font("Segoe UI Semibold",10F);
 this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
 this.BackColor = System.Drawing.ColorTranslator.FromHtml("#F5F7FA");
 this.MinimumSize = new System.Drawing.Size(1000,650);

 // תפריט צד מודרני בצבע נייבי כהה עם כפתור פעיל עבור מוצרים
 panelSidebar = new System.Windows.Forms.Panel();
 panelSidebar.BackColor = System.Drawing.ColorTranslator.FromHtml("#101D36");
 panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
 panelSidebar.Width = 260;
 panelSidebar.Padding = new System.Windows.Forms.Padding(22,22,22,22);
 this.Controls.Add(panelSidebar);

 pictureBoxLogo = new System.Windows.Forms.PictureBox();
 pictureBoxLogo.BackColor = System.Drawing.ColorTranslator.FromHtml("#1C315A");
 pictureBoxLogo.Width = 56;
 pictureBoxLogo.Height = 56;
 pictureBoxLogo.Top = 12;
 pictureBoxLogo.Left = (panelSidebar.Width - pictureBoxLogo.Width) / 2;
 pictureBoxLogo.BorderStyle = System.Windows.Forms.BorderStyle.None;
 panelSidebar.Controls.Add(pictureBoxLogo);

 lblLogoText = new System.Windows.Forms.Label();
 lblLogoText.Text = "CASHIER PRO";
 lblLogoText.ForeColor = System.Drawing.Color.White;
 lblLogoText.AutoSize = false;
 lblLogoText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
 lblLogoText.Dock = System.Windows.Forms.DockStyle.Top;
 lblLogoText.Height = 40;
 lblLogoText.Font = new System.Drawing.Font("Segoe UI Semibold",12F);
 lblLogoText.Margin = new System.Windows.Forms.Padding(0,16,0,14);
 panelSidebar.Controls.Add(lblLogoText);

 var sidebarMenu = new System.Windows.Forms.FlowLayoutPanel();
 sidebarMenu.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
 sidebarMenu.WrapContents = false;
 sidebarMenu.Dock = System.Windows.Forms.DockStyle.Top;
 sidebarMenu.AutoSize = true;
 sidebarMenu.Padding = new System.Windows.Forms.Padding(0,12,0,0);
 sidebarMenu.Margin = new System.Windows.Forms.Padding(0);
 sidebarMenu.BackColor = System.Drawing.Color.Transparent;
 panelSidebar.Controls.Add(sidebarMenu);

 btnDashboard = CreateSidebarButton("🏠 לוח בקרה");
 btnProducts = CreateSidebarButton("🏷 מוצרים", true);
 btnCustomers = CreateSidebarButton("👥 לקוחות");
 btnSales = CreateSidebarButton("🧾 מבצעים");
 btnOrder = CreateSidebarButton("🛒 הזמנה חדשה");
 sidebarMenu.Controls.AddRange(new Control[] { btnDashboard, btnProducts, btnCustomers, btnSales, btnOrder });

 // כותרת עליונה לבנה עם כפתורים מודרניים ופינות מעוגלות
 panelHeader = new System.Windows.Forms.Panel();
 panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
 panelHeader.Height = 88;
 panelHeader.BackColor = System.Drawing.ColorTranslator.FromHtml("#F8FAFD");
 panelHeader.Padding = new System.Windows.Forms.Padding(20, 12, 20, 12);
 panelHeader.BorderStyle = System.Windows.Forms.BorderStyle.None;
 this.Controls.Add(panelHeader);

 lblTitle = new System.Windows.Forms.Label();
 lblTitle.Text = "לוח בקרה";
 lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold",18F, System.Drawing.FontStyle.Bold);
 lblTitle.AutoSize = false;
 lblTitle.Width = 260;
 lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
 lblTitle.ForeColor = System.Drawing.Color.FromArgb(18,24,41);
 panelHeader.Controls.Add(lblTitle);

 var headerNav = new System.Windows.Forms.FlowLayoutPanel();
 headerNav.Dock = System.Windows.Forms.DockStyle.Left;
 headerNav.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
 headerNav.WrapContents = false;
 headerNav.Padding = new System.Windows.Forms.Padding(0, 18, 0, 0);
 headerNav.AutoSize = true;
 headerNav.Margin = new System.Windows.Forms.Padding(18, 0, 0, 0);
 panelHeader.Controls.Add(headerNav);

 var btnHeaderDashboard = CreateHeaderNavButton("לוח בקרה", true);
 var btnHeaderCustomers = CreateHeaderNavButton("לקוחות");
 var btnHeaderSales = CreateHeaderNavButton("מבצעים");
 var headerButtons = new[] { btnHeaderDashboard, btnHeaderCustomers, btnHeaderSales };
 Action<System.Windows.Forms.Button> setActiveHeaderButton = btn =>
 {
     foreach (var item in headerButtons)
     {
         item.BackColor = item == btn ? System.Drawing.Color.White : System.Drawing.ColorTranslator.FromHtml("#F3F6FB");
         item.ForeColor = item == btn ? System.Drawing.ColorTranslator.FromHtml("#101D36") : System.Drawing.ColorTranslator.FromHtml("#4E627F");
         item.FlatAppearance.BorderColor = item == btn ? System.Drawing.ColorTranslator.FromHtml("#D5E2F5") : System.Drawing.ColorTranslator.FromHtml("#E8EEF6");
     }
 };

 btnHeaderDashboard.Click += (s,e) => { tabMain.SelectedTab = tabDashboard; lblTitle.Text = "לוח בקרה"; setActiveHeaderButton(btnHeaderDashboard); };
 btnHeaderCustomers.Click += (s,e) => { tabMain.SelectedTab = tabCustomers; lblTitle.Text = "לקוחות"; setActiveHeaderButton(btnHeaderCustomers); };
 btnHeaderSales.Click += (s,e) => { tabMain.SelectedTab = tabSales; lblTitle.Text = "מבצעים"; setActiveHeaderButton(btnHeaderSales); };

 headerNav.Controls.Add(btnHeaderDashboard);
 headerNav.Controls.Add(btnHeaderCustomers);
 headerNav.Controls.Add(btnHeaderSales);

 setActiveHeaderButton(btnHeaderDashboard);

 var headerActions = new System.Windows.Forms.FlowLayoutPanel();
 headerActions.Dock = System.Windows.Forms.DockStyle.Right;
 headerActions.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
 headerActions.WrapContents = false;
 headerActions.Padding = new System.Windows.Forms.Padding(0, 18, 0, 0);
 headerActions.AutoSize = true;
 headerActions.Margin = new System.Windows.Forms.Padding(0);
 panelHeader.Controls.Add(headerActions);

 btnClose = new System.Windows.Forms.Button();
 btnClose.Text = "✕";
 btnClose.Width = 40;
 btnClose.Height = 36;
 btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
 btnClose.FlatAppearance.BorderSize = 0;
 btnClose.BackColor = System.Drawing.ColorTranslator.FromHtml("#E63946");
 btnClose.ForeColor = System.Drawing.Color.White;
 btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
 btnClose.FlatAppearance.MouseOverBackColor = System.Drawing.ColorTranslator.FromHtml("#FF4D66");
 btnClose.Click += (s,e) => { this.Close(); };
 ApplyRoundedButton(btnClose, 18);

 btnMinimize = new System.Windows.Forms.Button();
 btnMinimize.Text = "━";
 btnMinimize.Width = 40;
 btnMinimize.Height = 36;
 btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
 btnMinimize.FlatAppearance.BorderSize = 0;
 btnMinimize.BackColor = System.Drawing.ColorTranslator.FromHtml("#F5F7FA");
 btnMinimize.ForeColor = System.Drawing.Color.FromArgb(102,114,133);
 btnMinimize.Cursor = System.Windows.Forms.Cursors.Hand;
 btnMinimize.Click += (s,e) => { this.WindowState = System.Windows.Forms.FormWindowState.Minimized; };
 ApplyRoundedButton(btnMinimize, 18);

 btnProfile = new System.Windows.Forms.Button();
 btnProfile.Text = "👤 משתמש";
 btnProfile.Width = 150;
 btnProfile.Height = 36;
 btnProfile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
 btnProfile.FlatAppearance.BorderSize = 0;
 btnProfile.BackColor = System.Drawing.ColorTranslator.FromHtml("#151D2F");
 btnProfile.ForeColor = System.Drawing.Color.White;
 btnProfile.Cursor = System.Windows.Forms.Cursors.Hand;
 btnProfile.Font = new System.Drawing.Font("Segoe UI Semibold",9F);
 btnProfile.Padding = new System.Windows.Forms.Padding(12,0,12,0);
 btnProfile.FlatAppearance.MouseOverBackColor = System.Drawing.ColorTranslator.FromHtml("#1E2C45");
 btnProfile.FlatAppearance.MouseDownBackColor = System.Drawing.ColorTranslator.FromHtml("#131825");
 btnProfile.Click += (s,e) => { MessageBox.Show("כפתור הפרופיל נלחץ", "מידע", MessageBoxButtons.OK, MessageBoxIcon.Information); };
 ApplyRoundedButton(btnProfile, 18);

 btnSignOut = new System.Windows.Forms.Button();
 btnSignOut.Text = "יציאה";
 btnSignOut.Width = 90;
 btnSignOut.Height = 36;
 btnSignOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
 btnSignOut.FlatAppearance.BorderSize = 0;
 btnSignOut.BackColor = System.Drawing.Color.White;
 btnSignOut.ForeColor = System.Drawing.Color.FromArgb(37, 55, 79);
 btnSignOut.Cursor = System.Windows.Forms.Cursors.Hand;
 btnSignOut.Font = new System.Drawing.Font("Segoe UI Semibold",9F);
 btnSignOut.Padding = new System.Windows.Forms.Padding(12,0,12,0);
 btnSignOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(238, 243, 250);
 btnSignOut.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(225, 233, 245);
 btnSignOut.Click += (s,e) => { this.Close(); };
 ApplyRoundedButton(btnSignOut, 18);

 lblDate = new System.Windows.Forms.Label();
 lblDate.AutoSize = false;
 lblDate.Width = 160;
 lblDate.Height = 36;
 lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
 lblDate.Font = new System.Drawing.Font("Segoe UI",9F);
 lblDate.ForeColor = System.Drawing.Color.FromArgb(102,114,133);
 lblDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
 lblDate.Margin = new System.Windows.Forms.Padding(0, 0, 12, 0);

 headerActions.Controls.Add(btnClose);
 headerActions.Controls.Add(btnMinimize);
 headerActions.Controls.Add(btnProfile);
 headerActions.Controls.Add(btnSignOut);
 headerActions.Controls.Add(lblDate);

 // Main container - TabControl hiding headers
 tabMain = new System.Windows.Forms.TabControl();
 tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
 tabMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
 tabMain.ItemSize = new System.Drawing.Size(0,1);
 tabMain.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
 tabMain.TabStop = false;
 this.Controls.Add(tabMain);

tabDashboard = new System.Windows.Forms.TabPage("לוח בקרה");
tabProducts = new System.Windows.Forms.TabPage("מוצרים");
tabCustomers = new System.Windows.Forms.TabPage("לקוחות");
tabSales = new System.Windows.Forms.TabPage("מבצעים");
tabOrder = new System.Windows.Forms.TabPage("הזמנה חדשה");

tabMain.Controls.Add(tabDashboard);
tabMain.Controls.Add(tabProducts);
tabMain.Controls.Add(tabCustomers);
tabMain.Controls.Add(tabSales);
tabMain.Controls.Add(tabOrder);

var sidebarButtons = new[] { btnDashboard, btnProducts, btnCustomers, btnSales, btnOrder };
Action<System.Windows.Forms.Button> setActiveSidebarButton = btn =>
{
    foreach (var item in sidebarButtons)
    {
        item.BackColor = item == btn ? System.Drawing.ColorTranslator.FromHtml("#24395E") : System.Drawing.ColorTranslator.FromHtml("#101D36");
        item.ForeColor = Color.White;
    }
};

btnDashboard.Click += (s,e) => { tabMain.SelectedTab = tabDashboard; lblTitle.Text = "לוח בקרה"; setActiveSidebarButton(btnDashboard); };
btnProducts.Click += (s,e) => { tabMain.SelectedTab = tabProducts; lblTitle.Text = "מוצרים"; setActiveSidebarButton(btnProducts); };
btnCustomers.Click += (s,e) => { tabMain.SelectedTab = tabCustomers; lblTitle.Text = "לקוחות"; setActiveSidebarButton(btnCustomers); };
btnSales.Click += (s,e) => { tabMain.SelectedTab = tabSales; lblTitle.Text = "מבצעים"; setActiveSidebarButton(btnSales); };
btnOrder.Click += (s,e) => { tabMain.SelectedTab = tabOrder; lblTitle.Text = "הזמנה חדשה"; setActiveSidebarButton(btnOrder); };
btnProfile.Click += (s,e) => { MessageBox.Show("כפתור הפרופיל נלחץ", "מידע", MessageBoxButtons.OK, MessageBoxIcon.Information); };

setActiveSidebarButton(btnDashboard);

tabMain.SendToBack();
panelSidebar.BringToFront();
panelHeader.BringToFront();

 System.Windows.Forms.Button CreateSidebarButton(string text, bool active = false)
 {
 var btn = new System.Windows.Forms.Button();
 btn.Text = text;
 btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
 btn.FlatAppearance.BorderSize = 0;
 btn.ForeColor = System.Drawing.Color.White;
 btn.BackColor = active ? System.Drawing.ColorTranslator.FromHtml("#1D3860") : System.Drawing.ColorTranslator.FromHtml("#101D36");
 btn.Width = 220;
 btn.Height = 52;
 btn.Cursor = System.Windows.Forms.Cursors.Hand;
 btn.Font = new System.Drawing.Font("Segoe UI",10F, System.Drawing.FontStyle.Regular);
 btn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
 btn.Padding = new System.Windows.Forms.Padding(18, 0, 0, 0);
 btn.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
 btn.FlatAppearance.MouseOverBackColor = System.Drawing.ColorTranslator.FromHtml("#1B3261");
 return btn;
 }

 System.Windows.Forms.Button CreateHeaderNavButton(string text, bool active = false)
 {
 var btn = new System.Windows.Forms.Button();
 btn.Text = text;
 btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
 btn.FlatAppearance.BorderSize = 1;
 btn.FlatAppearance.BorderColor = System.Drawing.ColorTranslator.FromHtml("#D9E3F2");
 btn.ForeColor = active ? System.Drawing.ColorTranslator.FromHtml("#101D36") : System.Drawing.ColorTranslator.FromHtml("#4E627F");
 btn.BackColor = active ? System.Drawing.Color.White : System.Drawing.ColorTranslator.FromHtml("#F5F8FC");
 btn.Width = 130;
 btn.Height = 44;
 btn.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
 btn.Cursor = System.Windows.Forms.Cursors.Hand;
 btn.Font = new System.Drawing.Font("Segoe UI Semibold",9F);
 btn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
 btn.Padding = new System.Windows.Forms.Padding(0);
 return btn;
 }

 this.ResumeLayout(false);
 }
 #endregion
 }
}
