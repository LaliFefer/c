using BlApi;
using BO;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public enum AppUserRole
    {
        Manager,
        Customer
    }

    public class LoginForm : Form
    {
        public AppUserRole SelectedRole { get; private set; } = AppUserRole.Manager;
        public BO.Customer? SelectedCustomer { get; private set; }

        private RadioButton rdoManager = null!;
        private RadioButton rdoCustomer = null!;
        private ComboBox cboCustomer = null!;
        private Button btnLogin = null!;
        private Button btnCancel = null!;
        private Label lblHeader = null!;
        private Label lblValidation = null!;

        public LoginForm()
        {
            InitializeComponent();
            LoadCustomers();
        }

        private void InitializeComponent()
        {
            StyleDialogForm(this, "Cashier Pro - כניסה");
            this.ClientSize = new Size(560, 420);

            var outerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
            };
            this.Controls.Add(outerPanel);

            var card = CreateDialogPanel();
            outerPanel.Controls.Add(card);

            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.Transparent
            };
            card.Controls.Add(headerPanel);

            lblHeader = new Label
            {
                Text = "ברוכים הבאים ל־Cashier Pro",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI Semibold", 18F),
                ForeColor = Color.FromArgb(25, 43, 77)
            };
            headerPanel.Controls.Add(lblHeader);

            var formTitle = new Label
            {
                Text = "בחר משתמש והתחל בעבודה",
                Dock = DockStyle.Bottom,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(104, 119, 151)
            };
            headerPanel.Controls.Add(formTitle);

            var inputPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 220,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 10, 0, 0)
            };
            card.Controls.Add(inputPanel);

            var roleLabel = new Label
            {
                Text = "בחר סוג משתמש:",
                Dock = DockStyle.Top,
                Height = 26,
                ForeColor = Color.FromArgb(38, 58, 92)
            };
            inputPanel.Controls.Add(roleLabel);

            var roleButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 56,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 8, 0, 0)
            };
            inputPanel.Controls.Add(roleButtons);

            rdoManager = new RadioButton
            {
                Text = "כניסה כמנהל",
                Appearance = Appearance.Button,
                AutoSize = false,
                Width = 170,
                Height = 40,
                Checked = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(41, 98, 255),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 8, 0)
            };
            rdoManager.FlatAppearance.BorderSize = 0;
            rdoManager.CheckedChanged += RoleChanged;
            roleButtons.Controls.Add(rdoManager);

            rdoCustomer = new RadioButton
            {
                Text = "כניסה כלקוח",
                Appearance = Appearance.Button,
                AutoSize = false,
                Width = 170,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 42, 71),
                BackColor = Color.FromArgb(233, 240, 250),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(0, 0, 0, 0)
            };
            rdoCustomer.FlatAppearance.BorderSize = 0;
            rdoCustomer.CheckedChanged += RoleChanged;
            roleButtons.Controls.Add(rdoCustomer);

            var customerGroup = new Panel
            {
                Dock = DockStyle.Top,
                Height = 94,
                Padding = new Padding(0, 4, 0, 0)
            };
            inputPanel.Controls.Add(customerGroup);

            var lblCustomer = new Label
            {
                Text = "בחר לקוח קיים:",
                Dock = DockStyle.Top,
                Height = 24,
                ForeColor = Color.FromArgb(64, 84, 121),
                Enabled = false
            };
            customerGroup.Controls.Add(lblCustomer);

            cboCustomer = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Height = 34,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(23, 34, 57),
                Enabled = false
            };
            customerGroup.Controls.Add(cboCustomer);

            lblValidation = new Label
            {
                Text = string.Empty,
                ForeColor = Color.FromArgb(214, 48, 49),
                Dock = DockStyle.Top,
                Height = 26,
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(lblValidation);

            var buttonsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 12, 0, 0)
            };
            card.Controls.Add(buttonsPanel);

            btnLogin = CreateDialogButton("התחבר", Color.FromArgb(41, 98, 255));
            btnLogin.Click += BtnLogin_Click;
            buttonsPanel.Controls.Add(btnLogin);

            btnCancel = CreateDialogButton("ביטול", Color.FromArgb(224, 229, 236));
            btnCancel.ForeColor = Color.FromArgb(34, 44, 63);
            btnCancel.Click += (s, e) => this.Close();
            buttonsPanel.Controls.Add(btnCancel);

            buttonsPanel.BringToFront();
            btnLogin.BringToFront();
            btnCancel.BringToFront();
            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;
            this.KeyPreview = true;

            // Add rounded corners to the form
            var path = new System.Drawing.Drawing2D.GraphicsPath();
            int radius = 16;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            this.Region = new Region(path);

            RoleChanged(this, EventArgs.Empty);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Normal)
            {
                var path = new GraphicsPath();
                int radius = 16;
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(this.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(this.Width - radius, this.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, this.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();
                this.Region = new Region(path);
            }
        }

        private void LoadCustomers()
        {
            try
            {
                var customers = BlApi.Factory.Get().Customer.GetList().ToList();
                cboCustomer.Items.Clear();
                foreach (var customer in customers)
                {
                    cboCustomer.Items.Add(new CustomerComboItem(customer));
                }

                if (cboCustomer.Items.Count > 0)
                {
                    cboCustomer.SelectedIndex = 0;
                }
                else
                {
                    cboCustomer.Items.Add(new CustomerComboItem(new BO.Customer { IDNumber = 0, CustomerName = "לא נמצאו לקוחות", EmailAddress = string.Empty, TelephoneNumber = string.Empty }));
                }
            }
            catch
            {
                cboCustomer.Items.Clear();
                cboCustomer.Items.Add("שגיאת טעינת לקוחות");
            }
        }

        private void RoleChanged(object? sender, EventArgs e)
        {
            bool customerSelected = rdoCustomer.Checked;
            cboCustomer.Enabled = customerSelected;
            var parentControls = cboCustomer.Parent?.Controls;
            if (parentControls != null)
            {
                foreach (Control control in parentControls)
                {
                    if (control is Label label)
                    {
                        label.Enabled = customerSelected;
                    }
                }
            }

            if (rdoManager.Checked)
            {
                rdoManager.BackColor = Color.FromArgb(41, 98, 255);
                rdoManager.ForeColor = Color.White;
                rdoCustomer.BackColor = Color.FromArgb(233, 240, 250);
                rdoCustomer.ForeColor = Color.FromArgb(28, 42, 71);
            }
            else
            {
                rdoCustomer.BackColor = Color.FromArgb(41, 98, 255);
                rdoCustomer.ForeColor = Color.White;
                rdoManager.BackColor = Color.FromArgb(233, 240, 250);
                rdoManager.ForeColor = Color.FromArgb(28, 42, 71);
            }

            lblValidation.Text = string.Empty;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (rdoManager.Checked)
            {
                SelectedRole = AppUserRole.Manager;
                SelectedCustomer = null;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            if (cboCustomer.SelectedItem is CustomerComboItem item && item.Customer.IDNumber > 0)
            {
                SelectedRole = AppUserRole.Customer;
                SelectedCustomer = item.Customer;
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }

            lblValidation.Text = "בחר לקוח תקין כדי להתחבר בתור לקוח.";
        }

        private static Button CreateDialogButton(string text, Color color)
        {
            var b = new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 44,
                Width = 150,
                Margin = new Padding(6),
                Font = new Font("Segoe UI Semibold", 9F),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };

            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color);
            b.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color);

            var path = new GraphicsPath();
            int radius = 10;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(b.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(b.Width - radius, b.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, b.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            b.Region = new Region(path);

            return b;
        }

        private static TextBox CreateDialogTextBox(string placeholder = "")
        {
            return new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(248, 251, 255),
                ForeColor = Color.FromArgb(24, 34, 45),
                BorderStyle = BorderStyle.None,
                Margin = new Padding(6),
                PlaceholderText = placeholder
            };
        }

        private static ComboBox CreateDialogComboBox()
        {
            return new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(248, 251, 255),
                ForeColor = Color.FromArgb(24, 34, 45),
                Margin = new Padding(6)
            };
        }

        private static Label CreateDialogLabel(string text)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI Semibold", 10F),
                ForeColor = Color.FromArgb(55, 65, 81),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                AutoSize = false,
                Padding = new Padding(0, 8, 0, 0),
                Margin = new Padding(0)
            };
        }

        private static void StyleDialogForm(Form form, string title)
        {
            form.FormBorderStyle = FormBorderStyle.None;
            form.BackColor = Color.FromArgb(245, 247, 251);
            form.Padding = new Padding(14);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.Font = new Font("Segoe UI", 10F);
            form.Text = title;
            form.ShowIcon = false;
            form.ShowInTaskbar = false;
        }

        private static Panel CreateDialogPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18),
                BorderStyle = BorderStyle.None
            };
        }

        private static Label CreateDialogHeader(string title)
        {
            return new Label
            {
                Text = title,
                Font = new Font("Segoe UI Semibold", 14F),
                ForeColor = Color.FromArgb(18, 24, 41),
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft,
                AutoSize = false,
                Margin = new Padding(0, 0, 0, 12)
            };
        }

        private sealed class CustomerComboItem
        {
            public BO.Customer Customer { get; }
            public CustomerComboItem(BO.Customer customer) => Customer = customer;
            public override string ToString() => Customer.IDNumber == 0 ? Customer.CustomerName : $"{Customer.CustomerName} ({Customer.IDNumber})";
        }
    }
}
