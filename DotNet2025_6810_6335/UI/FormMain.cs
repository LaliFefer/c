using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace UI
{
    public partial class FormMain : Form
    {
        private readonly IBl _bl = BlApi.Factory.Get();
        private readonly bool _isManager;
        private readonly BO.Customer? _currentCustomer;

        // שליטה בלשונית מוצרים
        private DataGridView dgvProducts = null!;
        private TextBox txtFilterProducts = null!;
        private Button btnAddProduct = null!;
        private Button btnEditProduct = null!;
        private Button btnDeleteProduct = null!;
        private Button btnRestockProduct = null!;
        private Label lblStockSummary = null!;

        // שליטה בלשונית לקוחות
        private DataGridView dgvCustomers = null!;
        private TextBox txtFilterCustomers = null!;
        private Button btnAddCustomer = null!;
        private Button btnEditCustomer = null!;
        private Button btnDeleteCustomer = null!;

        // שליטה בלשונית מבצעים
        private DataGridView dgvSales = null!;
        private TextBox txtFilterSales = null!;
        private Button btnAddSale = null!;
        private Button btnEditSale = null!;
        private Button btnDeleteSale = null!;

        // שליטה בלוח הבקרה
        private const string DashboardAllCategoriesText = "כל הקטגוריות";
        private string _selectedDashboardCategory = DashboardAllCategoriesText;
        private FlowLayoutPanel flpDashboardCards = null!;
        private FlowLayoutPanel flpDashboardStats = null!;
        private FlowLayoutPanel flpDashboardCategoryChips = null!;
        private TextBox txtDashboardSearch = null!;
        private Button btnDashboardRefresh = null!;

        // שליטה בלשונית הזמנות
        private ListView lvProducts = null!; // רשימת מוצרים ברירה
        private DataGridView dgvCart = null!;
        private Label lblTotal = null!;
        private Button btnCheckout = null!;
        private BO.Order currentOrder;

        public FormMain(AppUserRole userRole, BO.Customer? currentCustomer = null)
        {
            _isManager = userRole == AppUserRole.Manager;
            _currentCustomer = currentCustomer;
            currentOrder = new BO.Order { IsClubCustomer = currentCustomer?.IsClubMember ?? false, Products = new List<BO.ProductInOrder>() };

            InitializeComponent();
            try
            {
                SetupTabs();
                ApplyRolePermissions();
                UpdateHeaderForUser();
                tabMain.SelectedTab = tabDashboard;
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאת אתחול", $"שגיאה בהפעלת הממשק: {ex}", true);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            using var brush = new LinearGradientBrush(
                ClientRectangle,
                ColorTranslator.FromHtml("#F5F7FA"),
                ColorTranslator.FromHtml("#E9EDF4"),
                90f);
            e.Graphics.FillRectangle(brush, ClientRectangle);
        }

        private void SetupTabs()
        {
            SetupDashboard();

            // לשונית מוצרים
            dgvProducts = CreateStyledDataGrid();

            txtFilterProducts = new TextBox { PlaceholderText = "חפש מוצר...", BackColor = Color.FromArgb(245,245,248), ForeColor = Color.FromArgb(20,30,45), BorderStyle = BorderStyle.None, Width = 360, Height = 40, Font = new Font("Segoe UI", 10F) };
            txtFilterProducts.TextChanged += (s, e) => LoadProducts();

            var lblProductsHeading = new Label
            {
                Text = "ניהול מוצרים",
                Font = new Font("Segoe UI Semibold", 14F),
                ForeColor = Color.FromArgb(18, 24, 41),
                AutoSize = false,
                Width = 220,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var panelTopP = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.FromArgb(244, 247, 251), Padding = new Padding(18) };
            btnAddProduct = CreateActionButton("＋ הוסף", ColorTranslator.FromHtml("#27AE60"));
            btnEditProduct = CreateActionButton("✎ ערוך", ColorTranslator.FromHtml("#3498DB"));
            btnDeleteProduct = CreateActionButton("🗑 הסר", ColorTranslator.FromHtml("#E74C3C"));
            btnRestockProduct = CreateActionButton("📦 מלאי", ColorTranslator.FromHtml("#F39C12"));
            btnAddProduct.Click += BtnAddProduct_Click;
            btnEditProduct.Click += BtnEditProduct_Click;
            btnDeleteProduct.Click += BtnDeleteProduct_Click;
            btnRestockProduct.Click += BtnRestockProduct_Click;
            var actionsFlowP = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 420, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6), AutoSize = true, WrapContents = false };
            actionsFlowP.Controls.AddRange(new Control[] { btnDeleteProduct, btnEditProduct, btnRestockProduct, btnAddProduct });
            panelTopP.Controls.Add(actionsFlowP);
            panelTopP.Controls.Add(lblProductsHeading);
            panelTopP.Controls.Add(txtFilterProducts);
            panelTopP.Margin = new Padding(0, 0, 0, 12);

            txtFilterProducts.Location = new Point(18, 46);
            lblProductsHeading.Location = new Point(18, 10);

            lblStockSummary = new Label
            {
                Text = "",
                Dock = DockStyle.Top,
                Height = 28,
                ForeColor = Color.FromArgb(181, 74, 76),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(18, 0, 0, 0)
            };

            var pProducts = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.White, BorderStyle = BorderStyle.None };
            pProducts.Controls.Add(panelTopP);
            pProducts.Controls.Add(lblStockSummary);
            pProducts.Controls.Add(dgvProducts);
            txtFilterProducts.Height =34;
            txtFilterProducts.Margin = new Padding(6);
            lblStockSummary.Height = 28;
            // מפעיל / מנטרל את כפתורי הפעולה לפי בחירת שורה
            dgvProducts.SelectionChanged += (s,e) => {
                bool has = dgvProducts.CurrentRow?.DataBoundItem != null;
                btnEditProduct.Enabled = has; btnDeleteProduct.Enabled = has; btnRestockProduct.Enabled = has;
            };
            tabProducts.BackColor = Color.FromArgb(245, 247, 251);
            tabProducts.Padding = new Padding(18);
            tabProducts.Controls.Add(pProducts);
            dgvProducts.DoubleClick += DgvProducts_DoubleClick;
            dgvProducts.DataBindingComplete += DgvProducts_DataBindingComplete;

            // לשונית לקוחות
            dgvCustomers = CreateStyledDataGrid();

            txtFilterCustomers = new TextBox { PlaceholderText = "חפש לקוח...", BackColor = Color.FromArgb(245,245,248), ForeColor = Color.FromArgb(20,30,45), BorderStyle = BorderStyle.None, Width = 360, Height = 40, Font = new Font("Segoe UI", 10F) };
            txtFilterCustomers.TextChanged += (s, e) => LoadCustomers();

            var lblCustomersHeading = new Label
            {
                Text = "ניהול לקוחות",
                Font = new Font("Segoe UI Semibold", 14F),
                ForeColor = Color.FromArgb(18, 24, 41),
                AutoSize = false,
                Width = 220,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var panelTopC = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.FromArgb(244, 247, 251), Padding = new Padding(18) };
            btnAddCustomer = CreateActionButton("＋ הוסף", ColorTranslator.FromHtml("#27AE60"));
            btnEditCustomer = CreateActionButton("✎ ערוך", ColorTranslator.FromHtml("#3498DB"));
            btnDeleteCustomer = CreateActionButton("🗑 הסר", ColorTranslator.FromHtml("#E74C3C"));
            btnAddCustomer.Click += BtnAddCustomer_Click;
            btnEditCustomer.Click += BtnEditCustomer_Click;
            btnDeleteCustomer.Click += BtnDeleteCustomer_Click;
            var actionsFlowC = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 300, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6), AutoSize = true, WrapContents = false };
            actionsFlowC.Controls.AddRange(new Control[] { btnDeleteCustomer, btnEditCustomer, btnAddCustomer });
            panelTopC.Controls.Add(actionsFlowC);
            panelTopC.Controls.Add(lblCustomersHeading);
            panelTopC.Controls.Add(txtFilterCustomers);
            panelTopC.Margin = new Padding(0, 0, 0, 12);

            txtFilterCustomers.Location = new Point(18, 46);
            lblCustomersHeading.Location = new Point(18, 10);

            var pCustomers = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16), BackColor = Color.White, BorderStyle = BorderStyle.None };
            pCustomers.Controls.Add(panelTopC);
            pCustomers.Controls.Add(dgvCustomers);
            txtFilterCustomers.Height =34;
            txtFilterCustomers.Margin = new Padding(6);
            dgvCustomers.SelectionChanged += (s,e) => {
                bool has = dgvCustomers.CurrentRow?.DataBoundItem != null;
                btnEditCustomer.Enabled = has; btnDeleteCustomer.Enabled = has;
            };
            tabCustomers.BackColor = Color.FromArgb(245, 247, 251);
            tabCustomers.Padding = new Padding(18);
            tabCustomers.Controls.Add(pCustomers);
            dgvCustomers.DoubleClick += DgvCustomers_DoubleClick;

            // לשונית מבצעים
            dgvSales = CreateStyledDataGrid();

            txtFilterSales = new TextBox { PlaceholderText = "חפש לפי מזהה מוצר...", BackColor = Color.FromArgb(245,245,248), ForeColor = Color.FromArgb(20,30,45), BorderStyle = BorderStyle.None, Width = 360, Height = 40, Font = new Font("Segoe UI", 10F) };
            txtFilterSales.TextChanged += (s,e) => LoadSales();

            var lblSalesHeading = new Label
            {
                Text = "ניהול מבצעים",
                Font = new Font("Segoe UI Semibold", 14F),
                ForeColor = Color.FromArgb(18, 24, 41),
                AutoSize = false,
                Width = 220,
                Height = 40,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var panelTopS = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.FromArgb(244, 247, 251), Padding = new Padding(18) };
            btnAddSale = CreateActionButton("＋ הוסף", ColorTranslator.FromHtml("#27AE60"));
            btnEditSale = CreateActionButton("✎ ערוך", ColorTranslator.FromHtml("#3498DB"));
            btnDeleteSale = CreateActionButton("🗑 הסר", ColorTranslator.FromHtml("#E74C3C"));
            btnAddSale.Click += BtnAddSale_Click;
            btnEditSale.Click += BtnEditSale_Click;
            btnDeleteSale.Click += BtnDeleteSale_Click;
            var actionsFlowS = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 300, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(6), AutoSize = true, WrapContents = false };
            actionsFlowS.Controls.AddRange(new Control[] { btnDeleteSale, btnEditSale, btnAddSale });
            panelTopS.Controls.Add(actionsFlowS);
            panelTopS.Controls.Add(lblSalesHeading);
            panelTopS.Controls.Add(txtFilterSales);
            panelTopS.Margin = new Padding(0, 0, 0, 12);

            txtFilterSales.Location = new Point(14, 42);
            lblSalesHeading.Location = new Point(14, 8);

            var pSales = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            pSales.Controls.Add(panelTopS);
            pSales.Controls.Add(dgvSales);
            txtFilterSales.Height =34;
            txtFilterSales.Margin = new Padding(6);
            dgvSales.SelectionChanged += (s,e) => {
                bool has = dgvSales.CurrentRow?.DataBoundItem != null;
                btnEditSale.Enabled = has; btnDeleteSale.Enabled = has;
            };
            tabSales.BackColor = Color.FromArgb(245, 247, 251);
            tabSales.Padding = new Padding(18);
            tabSales.Controls.Add(pSales);
            dgvSales.DoubleClick += DgvSales_DoubleClick;

            // לשונית הזמנות (חלוקה לשני אזורים)
            var split = new SplitContainer { Dock = DockStyle.Fill, SplitterDistance = 380 };
            split.Panel1.Padding = new Padding(8);
            split.Panel2.Padding = new Padding(8);

            dgvCart = CreateStyledDataGrid();
            dgvCart.ReadOnly = true;
            dgvCart.AutoGenerateColumns = false;
            dgvCart.RowHeadersVisible = false;
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מוצר", DataPropertyName = "ProductName", Width = 200 });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "כמות", DataPropertyName = "Quantity", Width = 70 });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מחיר", DataPropertyName = "TotalPrice", Width = 110 });

            var cartPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16), BorderStyle = BorderStyle.None };
            var cartHeading = new Label
            {
                Text = "סל קניות",
                Font = new Font("Segoe UI Semibold", 16F),
                ForeColor = Color.FromArgb(18, 24, 41),
                Dock = DockStyle.Top,
                Height = 42,
                TextAlign = ContentAlignment.MiddleLeft
            };
            cartPanel.Controls.Add(cartHeading);
            cartPanel.Controls.Add(dgvCart);
            split.Panel1.Controls.Add(cartPanel);

            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16), BorderStyle = BorderStyle.None };
            var orderHeading = new Label
            {
                Text = "בחר מוצרים להזמנה",
                Font = new Font("Segoe UI Semibold", 16F),
                ForeColor = Color.FromArgb(18, 24, 41),
                Dock = DockStyle.Top,
                Height = 42,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtSearchOrder = new TextBox
            {
                PlaceholderText = "חפש מוצר להזמנה...",
                Width = 280,
                Height = 38,
                BackColor = Color.FromArgb(245, 245, 248),
                ForeColor = Color.FromArgb(20, 30, 45),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };
            txtSearchOrder.TextChanged += (s, e) => LoadProductsIntoListView(txtSearchOrder.Text);

            var txtAddId = new TextBox
            {
                Width = 120,
                Height = 38,
                PlaceholderText = "מזהה מוצר",
                BackColor = Color.FromArgb(245, 245, 248),
                ForeColor = Color.FromArgb(20, 30, 45),
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F)
            };

            var btnAddById = CreateActionButton("הוסף לפי מזהה", ColorTranslator.FromHtml("#3498DB"));
            btnAddById.Height = 38;
            btnAddById.Width = 140;
            btnAddById.Click += (s, e) =>
            {
                if (int.TryParse(txtAddId.Text, out int id))
                {
                    AddProductToCart(id, 1);
                }
                else
                {
                    ShowAppNotification(this, "שגיאה", "מזהה לא חוקי", true);
                }
            };

            var productTools = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 52,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0, 10, 0, 10)
            };
            productTools.Controls.Add(txtSearchOrder);
            productTools.Controls.Add(txtAddId);
            productTools.Controls.Add(btnAddById);

            lvProducts = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                HeaderStyle = ColumnHeaderStyle.Nonclickable,
                BackColor = Color.FromArgb(248, 251, 255),
                BorderStyle = BorderStyle.None
            };
            lvProducts.Columns.Add("מ\"צ", 70);
            lvProducts.Columns.Add("שם", 180);
            lvProducts.Columns.Add("מחיר", 100);
            lvProducts.DoubleClick += LvProducts_DoubleClick;

            rightPanel.Controls.Add(orderHeading);
            rightPanel.Controls.Add(productTools);
            rightPanel.Controls.Add(lvProducts);
            split.Panel2.Controls.Add(rightPanel);

            var bottomPanel = new Panel { Dock = DockStyle.Bottom, Height = 92, BackColor = Color.Transparent, Padding = new Padding(16) };
            lblTotal = new Label { Text = "סה\"כ: 0.00", Font = new Font("Segoe UI", 18F, FontStyle.Bold), AutoSize = false, Width = 320, Height = 48, TextAlign = ContentAlignment.MiddleLeft, ForeColor = Color.FromArgb(18, 24, 41) };
            btnCheckout = CreateActionButton("בצע הזמנה", ColorTranslator.FromHtml("#27AE60"));
            btnCheckout.Width = 180;
            btnCheckout.Height = 48;
            btnCheckout.Dock = DockStyle.Right;
            btnCheckout.Click += BtnCheckout_Click;
            bottomPanel.Controls.Add(btnCheckout);
            bottomPanel.Controls.Add(lblTotal);

            tabOrder.BackColor = Color.FromArgb(245, 247, 251);
            tabOrder.Padding = new Padding(18);
            var orderContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 251, 255) };
            orderContainer.Controls.Add(split);
            orderContainer.Controls.Add(bottomPanel);
            tabOrder.Controls.Add(orderContainer);

            // טוען נתונים ראשוניים
            ConfigureProductGridColumns();
            ConfigureCustomerGridColumns();
            ConfigureSalesGridColumns();
            // טען ויזרוק אתחול אוטומטי אם ריק
            LoadProducts();
            LoadCustomers();
            // אם שני האוספים ריקים, ספק נתוני התחלה בסיסיים
            try
            {
                var pcount = _bl.Product.GetList().Count();
                var ccount = _bl.Customer.GetList().Count();
                if (pcount ==0 && ccount ==0)
                {
                    // ספק נתוני התחלה מינימליים
                    _bl.Product.Add(new BO.Product { IDNumber =0, ProductName = "T-Shirt", Category = BO.Categories.MEN, Price =49.9, QuantityInStock =10 });
                    _bl.Product.Add(new BO.Product { IDNumber =0, ProductName = "Dress", Category = BO.Categories.WOMEN, Price =129.9, QuantityInStock =5 });
                    _bl.Customer.Add(new BO.Customer { IDNumber =10000001, CustomerName = "Alice", EmailAddress = "alice@example.com", TelephoneNumber = "050-1111111" });
                    LoadProducts(); LoadCustomers();
                }
            }
            catch { }
            LoadSales();
            LoadProductsIntoListView();
            RefreshCartUI();
        }

        private void SetupDashboard()
        {
            txtDashboardSearch = new TextBox
            {
                PlaceholderText = "חפש מוצר...",
                Width = 280,
                Height = 40,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(18, 24, 41),
                BorderStyle = BorderStyle.None,
                Margin = new Padding(0, 0, 12, 0),
                Font = new Font("Segoe UI", 10F)
            };
            txtDashboardSearch.TextChanged += (s, e) => LoadDashboard();

            _selectedDashboardCategory = DashboardAllCategoriesText;

            btnDashboardRefresh = CreateActionButton("רענן", ColorTranslator.FromHtml("#3498DB"));
            btnDashboardRefresh.Height = 40;
            btnDashboardRefresh.Click += (s, e) => LoadDashboard();

            var dashboardTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 72,
                Padding = new Padding(24, 20, 24, 0),
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                WrapContents = false
            };
            dashboardTop.Controls.Add(txtDashboardSearch);
            dashboardTop.Controls.Add(btnDashboardRefresh);

            flpDashboardCategoryChips = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(24, 10, 24, 6),
                BackColor = Color.Transparent
            };

            flpDashboardStats = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 140,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(24, 16, 24, 16),
                BackColor = Color.Transparent
            };

            flpDashboardCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(24, 12, 24, 24),
                BackColor = Color.Transparent
            };

            tabDashboard.BackColor = Color.FromArgb(245, 247, 251);
            tabDashboard.Padding = new Padding(18);
            var dashboardContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            dashboardContainer.Controls.Add(dashboardTop);
            dashboardContainer.Controls.Add(flpDashboardCategoryChips);
            dashboardContainer.Controls.Add(flpDashboardStats);
            dashboardContainer.Controls.Add(flpDashboardCards);
            tabDashboard.Controls.Add(dashboardContainer);

            LoadDashboard();
        }

        private void LoadDashboard()
        {
            try
            {
                var allProducts = _bl.Product.GetList().ToList();
                var list = allProducts.ToList();
                var filter = txtDashboardSearch.Text.Trim();
                if (!string.IsNullOrEmpty(filter))
                {
                    list = list.Where(p => p.ProductName.Contains(filter, StringComparison.OrdinalIgnoreCase)
                        || p.Category.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.Equals(_selectedDashboardCategory, DashboardAllCategoriesText, StringComparison.OrdinalIgnoreCase))
                {
                    list = list.Where(p => p.Category.ToString() == _selectedDashboardCategory).ToList();
                }

                flpDashboardStats.Controls.Clear();
                flpDashboardStats.Controls.Add(CreateDashboardStatCard("מוצרים", allProducts.Count.ToString(), "#3B82F6"));
                flpDashboardStats.Controls.Add(CreateDashboardStatCard("במלאי נמוך", allProducts.Count(p => p.QuantityInStock <= 3).ToString(), "#F59E0B"));
                flpDashboardStats.Controls.Add(CreateDashboardStatCard("מבצעים פעילים", _bl.Sale.GetList().Count().ToString(), "#10B981"));
                flpDashboardStats.Controls.Add(CreateDashboardStatCard("קטגוריות", allProducts.Select(p => p.Category).Distinct().Count().ToString(), "#6366F1"));

                UpdateDashboardCategoryChips(allProducts.Select(p => p.Category.ToString()).Distinct().OrderBy(c => c).ToList());

                flpDashboardCards.Controls.Clear();
                if (list.Count == 0)
                {
                    var empty = new Label
                    {
                        Text = "לא נמצאו מוצרים",
                        AutoSize = true,
                        ForeColor = Color.FromArgb(102, 114, 133),
                        Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                        Margin = new Padding(24)
                    };
                    flpDashboardCards.Controls.Add(empty);
                    return;
                }

                foreach (var product in list)
                {
                    flpDashboardCards.Controls.Add(CreateDashboardCard(product));
                }
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private Control CreateDashboardCard(BO.Product product)
        {
            var card = new Panel
            {
                Width = 280,
                Height = 170,
                Margin = new Padding(8),
                BackColor = Color.White,
                Padding = new Padding(18),
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                using var path = GetRoundedRectPath(new Rectangle(0, 0, card.Width - 1, card.Height - 1), 18);
                using var brush = new SolidBrush(Color.White);
                using var pen = new Pen(Color.FromArgb(227, 232, 239), 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            };

            var title = new Label
            {
                Text = product.ProductName,
                Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(18, 24, 41),
                AutoSize = true,
                Dock = DockStyle.Top
            };

            var category = new Label
            {
                Text = product.Category.ToString(),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(105, 119, 139),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 6, 0, 0)
            };

            var price = new Label
            {
                Text = $"{product.Price:C}",
                Font = new Font("Segoe UI Semibold", 13F),
                ForeColor = Color.FromArgb(37, 117, 255),
                AutoSize = true,
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var details = new Label
            {
                Text = $"מלאי: {product.QuantityInStock}",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(88, 102, 126),
                AutoSize = true,
                Dock = DockStyle.Bottom,
                Margin = new Padding(0, 8, 0, 0)
            };

            card.Controls.Add(price);
            card.Controls.Add(details);
            card.Controls.Add(category);
            card.Controls.Add(title);
            card.Click += (s, e) => ShowAppNotification(this, "פרטי מוצר", product.ToString());

            return card;
        }

        private void UpdateDashboardCategoryChips(List<string> categories)
        {
            flpDashboardCategoryChips.Controls.Clear();
            flpDashboardCategoryChips.Controls.Add(new Label
            {
                Text = "קטגוריות:",
                Font = new Font("Segoe UI Semibold", 10F),
                ForeColor = Color.FromArgb(80, 95, 121),
                AutoSize = true,
                Margin = new Padding(0, 8, 16, 8)
            });
            flpDashboardCategoryChips.Controls.Add(CreateCategoryChip(DashboardAllCategoriesText));
            foreach (var category in categories)
            {
                flpDashboardCategoryChips.Controls.Add(CreateCategoryChip(category));
            }
        }

        private Button CreateCategoryChip(string categoryName)
        {
            var isSelected = string.Equals(_selectedDashboardCategory, categoryName, StringComparison.OrdinalIgnoreCase);
            var button = new Button
            {
                Text = categoryName,
                AutoSize = true,
                Height = 36,
                Padding = new Padding(14, 0, 14, 0),
                Margin = new Padding(0, 0, 8, 8),
                FlatStyle = FlatStyle.Flat,
                ForeColor = isSelected ? Color.White : Color.FromArgb(37, 43, 66),
                BackColor = isSelected ? Color.FromArgb(41, 98, 255) : Color.FromArgb(238, 242, 247),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (s, e) =>
            {
                _selectedDashboardCategory = categoryName;
                LoadDashboard();
            };

            return button;
        }

        private Control CreateDashboardStatCard(string title, string value, string accentHex)
        {
            var panel = new Panel
            {
                Width = 240,
                Height = 112,
                Margin = new Padding(8),
                Padding = new Padding(16),
                BackColor = Color.White
            };

            panel.Paint += (s, e) =>
            {
                using var path = GetRoundedRectPath(new Rectangle(0, 0, panel.Width - 1, panel.Height - 1), 18);
                using var brush = new SolidBrush(Color.White);
                using var pen = new Pen(Color.FromArgb(227, 232, 239), 1);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                AutoSize = false,
                Height = 22,
                Dock = DockStyle.Top
            };

            var valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI Semibold", 22F),
                ForeColor = ColorTranslator.FromHtml(accentHex),
                AutoSize = false,
                Height = 48,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleLeft
            };

            panel.Controls.Add(valueLabel);
            panel.Controls.Add(titleLabel);
            return panel;
        }

        private void ApplyRolePermissions()
        {
            if (!_isManager)
            {
                // קופאי יכול לראות את הלקוחות ולהתאים לקוחות למועדון, אך אינו יכול לנהל מלאי ומבצעים
                btnAddProduct.Visible = false;
                btnEditProduct.Visible = false;
                btnDeleteProduct.Visible = false;
                btnRestockProduct.Visible = false;
                btnAddSale.Visible = false;
                btnEditSale.Visible = false;
                btnDeleteSale.Visible = false;
            }
        }

        private void UpdateHeaderForUser()
        {
            if (_currentCustomer != null)
            {
                btnProfile.Text = $"לקוח: {_currentCustomer.CustomerName}";
                lblTitle.Text = "קופה - הזמנה";
            }
            else if (_isManager)
            {
                btnProfile.Text = "מנהל מערכת";
            }
        }

        #region Products handlers
        private void ConfigureProductGridColumns()
        {
            dgvProducts.Columns.Clear();
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מ\"צ", DataPropertyName = "IDNumber", Width =60 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "שם", DataPropertyName = "ProductName", Width =200 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "קטגוריה", DataPropertyName = "Category", Width =120 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מחיר", DataPropertyName = "Price", Width =100 });
            dgvProducts.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מלאי", DataPropertyName = "QuantityInStock", Width =80 });
        }

        private void LoadProducts()
        {
            try
            {
                var list = _bl.Product.GetList().ToList();
                var filter = txtFilterProducts.Text.Trim();
                if (!string.IsNullOrEmpty(filter)) list = list.Where(p => p.ProductName.Contains(filter, StringComparison.OrdinalIgnoreCase) || p.Category.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
                dgvProducts.DataSource = list;
                UpdateStockSummary(list);
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private void UpdateStockSummary(IEnumerable<BO.Product> products)
        {
            var lowStock = products.Count(p => p.QuantityInStock <= 3);
            lblStockSummary.Text = lowStock > 0 ? $"מוצרים במלאי נמוך: {lowStock} (כמות קטנה מ-4)" : "אין מוצרים במלאי נמוך כרגע";
        }

        private void DgvProducts_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.DataBoundItem is BO.Product p && p.QuantityInStock <= 3)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 239, 239);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(130, 0, 0);
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(20, 30, 45);
                }
            }
        }

        private void BtnAddProduct_Click(object? sender, EventArgs e)
        {
            using var dlg = new ProductDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _bl.Product.Add(dlg.Product);
                    LoadProducts();
                    LoadProductsIntoListView();
                    ShowAppNotification(this, "מידע", "המוצר נוסף בהצלחה.");
                }
                catch (Exception ex)
                {
                    ShowAppNotification(this, "שגיאה", ex.ToString(), true);
                }
            }
        }

        private void BtnEditProduct_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is BO.Product p)
            {
                using var dlg = new ProductDialog(p);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _bl.Product.Update(dlg.Product);
                        LoadProducts();
                        LoadProductsIntoListView();
                        ShowAppNotification(this, "מידע", "המוצר עודכן.");
                    }
                    catch (Exception ex)
                    {
                        ShowAppNotification(this, "שגיאה", ex.ToString(), true);
                    }
                }
            }
        }

        private void BtnDeleteProduct_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is BO.Product p)
            {
                if (ShowAppConfirmation(this, $"אישור", $"האם למחוק את המוצר {p.ProductName}?"))
                {
                    try
                    {
                        _bl.Product.Delete(p.IDNumber);
                        LoadProducts();
                        LoadProductsIntoListView();
                        ShowAppNotification(this, "מידע", "המוצר נמחק.");
                    }
                    catch (Exception ex)
                    {
                        ShowAppNotification(this, "שגיאה", ex.ToString(), true);
                    }
                }
            }
        }

        private void BtnRestockProduct_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is not BO.Product product)
                return;

            using var dlg = new Form();
            StyleDialogForm(dlg, "הזמנת מלאי");
            dlg.Width = 400;
            dlg.Height = 240;

            var card = CreateDialogPanel();
            dlg.Controls.Add(card);
            card.Controls.Add(CreateDialogHeader("הזמנת מלאי"));

            var panel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(0), Margin = new Padding(0) };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
            card.Controls.Add(panel);

            panel.Controls.Add(CreateDialogLabel("כמות להזמנה:"), 0, 0);
            var nudAmount = CreateDialogNumericUpDown();
            nudAmount.Minimum = 1;
            nudAmount.Maximum = 1000;
            nudAmount.Value = 10;
            panel.Controls.Add(nudAmount, 1, 0);

            panel.Controls.Add(CreateDialogLabel($"מוצר:"), 0, 1);
            panel.Controls.Add(new Label { Text = product.ProductName, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(26, 35, 53), AutoSize = false }, 1, 1);

            panel.Controls.Add(CreateDialogLabel("מלאי נוכחי:"), 0, 2);
            panel.Controls.Add(new Label { Text = product.QuantityInStock.ToString(), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(26, 35, 53), AutoSize = false }, 1, 2);

            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            var btnOk = CreateDialogButton("הוסף מלאי", Color.FromArgb(46, 134, 193));
            btnOk.DialogResult = DialogResult.OK;
            var btnCancel = CreateDialogButton("ביטול", Color.FromArgb(233, 237, 245));
            btnCancel.ForeColor = Color.FromArgb(37, 43, 66);
            btnCancel.DialogResult = DialogResult.Cancel;
            actionPanel.Controls.Add(btnOk);
            actionPanel.Controls.Add(btnCancel);
            card.Controls.Add(actionPanel);

            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var updated = new BO.Product
                    {
                        IDNumber = product.IDNumber,
                        ProductName = product.ProductName,
                        Category = product.Category,
                        Price = product.Price,
                        QuantityInStock = product.QuantityInStock + (int)nudAmount.Value
                    };
                    _bl.Product.Update(updated);
                    LoadProducts();
                    LoadProductsIntoListView();
                    ShowAppNotification(this, "מלאי עודכן", $"הוספו {(int)nudAmount.Value} יחידות למוצר {product.ProductName}.");
                }
                catch (Exception ex)
                {
                    ShowAppNotification(this, "שגיאה", ex.Message, true);
                }
            }
        }

        private void DgvProducts_DoubleClick(object? sender, EventArgs e)
        {
            if (dgvProducts.CurrentRow?.DataBoundItem is BO.Product p)
                ShowAppNotification(this, "פרטי מוצר", p.ToString());
        }
        #endregion

        #region Customers handlers
        private void ConfigureCustomerGridColumns()
        {
            dgvCustomers.Columns.Clear();
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "ת\"ז", DataPropertyName = "IDNumber", Width =100 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "שם", DataPropertyName = "CustomerName", Width =200 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "אימייל", DataPropertyName = "EmailAddress", Width =200 });
            dgvCustomers.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "טלפון", DataPropertyName = "TelephoneNumber", Width =120 });
            dgvCustomers.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "לקוח מועדון", DataPropertyName = "IsClubMember", Width =120 });
        }

        private void LoadCustomers()
        {
            try
            {
                // שימוש ב-BL כדי לשמור על שכבות מסודרות ולמנוע תלות ישירה ב-DAL מה-UI
                var list = _bl.Customer.GetList().ToList();
                var filter = txtFilterCustomers.Text?.Trim() ?? string.Empty;
                if (!string.IsNullOrEmpty(filter))
                {
                    list = list.Where(c => c.CustomerName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                dgvCustomers.DataSource = list;
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private void BtnAddCustomer_Click(object? sender, EventArgs e)
        {
            using var dlg = new CustomerDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _bl.Customer.Add(dlg.Customer);
                    LoadCustomers();
                }
                catch (Exception ex)
                {
                    ShowAppNotification(this, "שגיאה", ex.Message, true);
                }
            }
        }

        private void BtnEditCustomer_Click(object? sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is BO.Customer c)
            {
                using var dlg = new CustomerDialog(c);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _bl.Customer.Update(dlg.Customer);
                        LoadCustomers();
                    }
                    catch (Exception ex)
                    {
                        ShowAppNotification(this, "שגיאה", ex.Message, true);
                    }
                }
            }
        }

        private void BtnDeleteCustomer_Click(object? sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is BO.Customer c)
            {
                if (ShowAppConfirmation(this, "אישור", $"האם למחוק את הלקוח {c.CustomerName}?"))
                {
                    try
                    {
                        // ה-BL לא מספק מחיקה ללקוחות; משתמשים ב-DAL ישירות
                        DalApi.Factory.Get.Customer.Delete(c.IDNumber);
                        LoadCustomers();
                        ShowAppNotification(this, "מידע", "הלקוח נמחק.");
                    }
                    catch (Exception ex)
                    {
                        ShowAppNotification(this, "שגיאה", ex.Message, true);
                    }
                }
            }
        }

        private void DgvCustomers_DoubleClick(object? sender, EventArgs e)
        {
            if (dgvCustomers.CurrentRow?.DataBoundItem is BO.Customer c)
                ShowAppNotification(this, "פרטי לקוח", c.ToString());
        }
        #endregion

        #region Sales handlers
        private void ConfigureSalesGridColumns()
        {
            dgvSales.Columns.Clear();
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מ\"צ", DataPropertyName = "IDNumber", Width =80 });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "מ\"צ מוצר", DataPropertyName = "ProductIDNumber", Width =100 });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "כמות", DataPropertyName = "QuantityItemsRequiredtoReceivetheSale", Width =80 });
            dgvSales.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "סה\"כ", DataPropertyName = "FullPrice", Width =100 });
        }

        private void LoadSales()
        {
            try
            {
                var list = _bl.Sale.GetList().ToList();
                var filter = txtFilterSales.Text.Trim();
                if (!string.IsNullOrEmpty(filter) && int.TryParse(filter, out int pid)) list = list.Where(s => s.ProductIDNumber == pid).ToList();
                dgvSales.DataSource = list;
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private void BtnAddSale_Click(object? sender, EventArgs e)
        {
            using var dlg = new SaleDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _bl.Sale.Add(dlg.Sale);
                    LoadSales();
                }
                catch (Exception ex)
                {
                    ShowAppNotification(this, "שגיאה", ex.Message, true);
                }
            }
        }

        private void BtnEditSale_Click(object? sender, EventArgs e)
        {
            if (dgvSales.CurrentRow?.DataBoundItem is BO.Sale s)
            {
                using var dlg = new SaleDialog(s);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _bl.Sale.Update(dlg.Sale);
                        LoadSales();
                    }
                    catch (Exception ex)
                    {
                        ShowAppNotification(this, "שגיאה", ex.Message, true);
                    }
                }
            }
        }

        private void BtnDeleteSale_Click(object? sender, EventArgs e)
        {
            if (dgvSales.CurrentRow?.DataBoundItem is BO.Sale s)
            {
                if (ShowAppConfirmation(this, "אישור", $"האם למחוק את המבצע {s.IDNumber}?"))
                {
                    try
                    {
                        _bl.Sale.Delete(s.IDNumber);
                        LoadSales();
                    }
                    catch (Exception ex)
                    {
                        ShowAppNotification(this, "שגיאה", ex.Message, true);
                    }
                }
            }
        }

        private void DgvSales_DoubleClick(object? sender, EventArgs e)
        {
            if (dgvSales.CurrentRow?.DataBoundItem is BO.Sale s)
                ShowAppNotification(this, "פרטי מבצע", s.ToString());
        }
        #endregion

        #region Order handlers
        private void LoadProductsIntoListView(string filter = "")
        {
            try
            {
                lvProducts.Items.Clear();
                var prods = _bl.Product.GetList().ToList();
                if (!string.IsNullOrWhiteSpace(filter))
                {
                    prods = prods.Where(p => p.ProductName.Contains(filter, StringComparison.OrdinalIgnoreCase)
                        || p.IDNumber.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                foreach (var p in prods)
                {
                    var item = new ListViewItem(p.IDNumber.ToString()) { Tag = p };
                    item.SubItems.Add(p.ProductName);
                    item.SubItems.Add(p.Price.ToString("C"));
                    lvProducts.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private void LvProducts_DoubleClick(object? sender, EventArgs e)
        {
            if (lvProducts.SelectedItems.Count == 0) return;
            var p = lvProducts.SelectedItems[0].Tag as BO.Product;
            if (p == null) return;
            AddProductToCart(p.IDNumber, 1);
        }

        private void AddProductToCart(int productId, int qty)
        {
            try
            {
                // אם המזהה לא נמצא, מציג הודעה
                var product = _bl.Product.GetById(productId);
                if (product == null)
                {
                    ShowAppNotification(this, "שגיאה", "המוצר לא נמצא.", true);
                    return;
                }
                var sales = _bl.Order.AddProductToOrder(currentOrder, productId, qty);
                // רענן את הסל מההזמנה הנוכחית
                RefreshCartUI();
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private void RefreshCartUI()
        {
            try
            {
                dgvCart.DataSource = null;
                var list = currentOrder.Products?.ToList() ?? new List<BO.ProductInOrder>();
                dgvCart.DataSource = list;
                // וודא שסכומים מעודכנים
                try { _bl.Order.CalcTotalPrice(currentOrder); }
                catch { /* ignore */ }
                lblTotal.Text = $"סה\"כ {currentOrder.TotalPrice:C}";
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }

        private void BtnCheckout_Click(object? sender, EventArgs e)
        {
            try
            {
                _bl.Order.DoOrder(currentOrder);
                ShowAppNotification(this, "הצלחה", "ההזמנה בוצעה בהצלחה.");
                currentOrder = new BO.Order { IsClubCustomer = false, Products = new List<BO.ProductInOrder>() };
                RefreshCartUI();
                LoadProducts();
                LoadProductsIntoListView();
            }
            catch (Exception ex)
            {
                ShowAppNotification(this, "שגיאה", ex.Message, true);
            }
        }
        #endregion

        #region Dialogs
        // חלון שיח פשוט למוצר
        private class ProductDialog : Form
        {
            public BO.Product Product { get; private set; } = null!;
            private TextBox txtName = null!;
            private ComboBox cmbCategory = null!;
            private NumericUpDown nudPrice = null!;
            private NumericUpDown nudQty = null!;
            private NumericUpDown nudId = null!;

            public ProductDialog(BO.Product? existing = null)
            {
                StyleDialogForm(this, existing == null ? "הוסף מוצר" : "ערוך מוצר");
                Width = 420;
                Height = 360;

                var card = CreateDialogPanel();
                Controls.Add(card);
                card.Controls.Add(CreateDialogHeader(Text));

                var pnl = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 5,
                    ColumnCount = 2,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));

                nudId = CreateDialogNumericUpDown();
                nudId.Minimum = 0;
                nudId.Maximum = 999999;

                txtName = CreateDialogTextBox("שם מוצר");
                cmbCategory = CreateDialogComboBox();
                cmbCategory.Items.AddRange(Enum.GetNames(typeof(BO.Categories)));

                nudPrice = CreateDialogNumericUpDown();
                nudPrice.DecimalPlaces = 2;
                nudPrice.Maximum = 100000;

                nudQty = CreateDialogNumericUpDown();
                nudQty.Maximum = 100000;

                pnl.Controls.Add(CreateDialogLabel("מספר מוצר:"), 0, 0);
                pnl.Controls.Add(nudId, 1, 0);
                pnl.Controls.Add(CreateDialogLabel("שם מוצר:"), 0, 1);
                pnl.Controls.Add(txtName, 1, 1);
                pnl.Controls.Add(CreateDialogLabel("קטגוריה:"), 0, 2);
                pnl.Controls.Add(cmbCategory, 1, 2);
                pnl.Controls.Add(CreateDialogLabel("מחיר:"), 0, 3);
                pnl.Controls.Add(nudPrice, 1, 3);
                pnl.Controls.Add(CreateDialogLabel("מלאי:"), 0, 4);
                pnl.Controls.Add(nudQty, 1, 4);

                var footer = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 56,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                var btnOk = CreateDialogButton("שמור", Color.FromArgb(39, 174, 96));
                btnOk.DialogResult = DialogResult.OK;

                var btnCancel = CreateDialogButton("ביטול", Color.FromArgb(233, 237, 245));
                btnCancel.ForeColor = Color.FromArgb(37, 43, 66);
                btnCancel.DialogResult = DialogResult.Cancel;

                footer.Controls.Add(btnOk);
                footer.Controls.Add(btnCancel);
                card.Controls.Add(pnl);
                card.Controls.Add(footer);

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                if (existing != null)
                {
                    nudId.Value = existing.IDNumber;
                    txtName.Text = existing.ProductName;
                    cmbCategory.SelectedItem = existing.Category.ToString();
                    nudPrice.Value = (decimal)existing.Price;
                    nudQty.Value = existing.QuantityInStock;
                }

                btnOk.Click += (s, e) =>
                {
                    Product = new BO.Product
                    {
                        IDNumber = (int)nudId.Value,
                        ProductName = txtName.Text,
                        Category = Enum.Parse<BO.Categories>(cmbCategory.SelectedItem?.ToString() ?? "MEN"),
                        Price = (double)nudPrice.Value,
                        QuantityInStock = (int)nudQty.Value
                    };
                };
            }
        }

        // חלון שיח פשוט ללקוח
        private class CustomerDialog : Form
        {
            public BO.Customer Customer { get; private set; } = null!;
            private NumericUpDown nudId = null!;
            private TextBox txtName = null!;
            private TextBox txtEmail = null!;
            private TextBox txtPhone = null!;
            private CheckBox chkClub = null!;

            public CustomerDialog(BO.Customer? existing = null)
            {
                StyleDialogForm(this, existing == null ? "הוסף לקוח" : "ערוך לקוח");
                Width = 440;
                Height = 380;

                var card = CreateDialogPanel();
                Controls.Add(card);
                card.Controls.Add(CreateDialogHeader(Text));

                var pnl = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 5,
                    ColumnCount = 2,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
                for (int i = 0; i < 5; i++)
                {
                    pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                }

                nudId = CreateDialogNumericUpDown();
                nudId.Minimum = 0;
                nudId.Maximum = 99999999;

                txtName = CreateDialogTextBox("שם לקוח");
                txtEmail = CreateDialogTextBox("מייל");
                txtPhone = CreateDialogTextBox("טלפון");
                chkClub = new CheckBox
                {
                    Text = "לקוח מועדון",
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F)
                };

                pnl.Controls.Add(CreateDialogLabel("מספר לקוח:"), 0, 0);
                pnl.Controls.Add(nudId, 1, 0);
                pnl.Controls.Add(CreateDialogLabel("שם:"), 0, 1);
                pnl.Controls.Add(txtName, 1, 1);
                pnl.Controls.Add(CreateDialogLabel("מייל:"), 0, 2);
                pnl.Controls.Add(txtEmail, 1, 2);
                pnl.Controls.Add(CreateDialogLabel("טלפון:"), 0, 3);
                pnl.Controls.Add(txtPhone, 1, 3);
                pnl.Controls.Add(CreateDialogLabel("מועדון:"), 0, 4);
                pnl.Controls.Add(chkClub, 1, 4);

                var footer = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 56,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                var btnOk = CreateDialogButton("שמור", Color.FromArgb(39, 174, 96));
                btnOk.DialogResult = DialogResult.OK;

                var btnCancel = CreateDialogButton("ביטול", Color.FromArgb(233, 237, 245));
                btnCancel.ForeColor = Color.FromArgb(37, 43, 66);
                btnCancel.DialogResult = DialogResult.Cancel;

                footer.Controls.Add(btnOk);
                footer.Controls.Add(btnCancel);
                card.Controls.Add(pnl);
                card.Controls.Add(footer);

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                if (existing != null)
                {
                    nudId.Value = existing.IDNumber;
                    txtName.Text = existing.CustomerName;
                    txtEmail.Text = existing.EmailAddress;
                    txtPhone.Text = existing.TelephoneNumber;
                    chkClub.Checked = existing.IsClubMember;
                }

                btnOk.Click += (s, e) =>
                {
                    Customer = new BO.Customer
                    {
                        IDNumber = (int)nudId.Value,
                        CustomerName = txtName.Text,
                        EmailAddress = txtEmail.Text,
                        TelephoneNumber = txtPhone.Text,
                        IsClubMember = chkClub.Checked
                    };
                };
            }
        }

        // חלון שיח פשוט למבצע
        private class SaleDialog : Form
        {
            public BO.Sale Sale { get; private set; } = null!;
            private NumericUpDown nudId = null!;
            private NumericUpDown nudProdId = null!;
            private NumericUpDown nudQty = null!;
            private NumericUpDown nudPrice = null!;
            private CheckBox chkClub = null!;
            private TextBox txtStart = null!;
            private TextBox txtEnd = null!;

            public SaleDialog(BO.Sale? existing = null)
            {
                StyleDialogForm(this, existing == null ? "הוסף מבצע" : "ערוך מבצע");
                Width = 440;
                Height = 420;

                var card = CreateDialogPanel();
                Controls.Add(card);
                card.Controls.Add(CreateDialogHeader(Text));

                var pnl = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 7,
                    ColumnCount = 2,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
                pnl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
                for (int i = 0; i < 6; i++)
                {
                    pnl.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                }

                nudId = CreateDialogNumericUpDown();
                nudId.Minimum = 0;
                nudId.Maximum = 999999;

                nudProdId = CreateDialogNumericUpDown();
                nudProdId.Minimum = 0;
                nudProdId.Maximum = 999999;

                nudQty = CreateDialogNumericUpDown();
                nudQty.Minimum = 1;
                nudQty.Maximum = 10000;

                nudPrice = CreateDialogNumericUpDown();
                nudPrice.DecimalPlaces = 2;
                nudPrice.Maximum = 100000;

                chkClub = new CheckBox
                {
                    Text = "לקוח מועדון",
                    Dock = DockStyle.Fill,
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10F)
                };

                txtStart = CreateDialogTextBox("תאריך התחלה");
                txtEnd = CreateDialogTextBox("תאריך סיום");

                pnl.Controls.Add(CreateDialogLabel("מספר מבצע:"), 0, 0);
                pnl.Controls.Add(nudId, 1, 0);
                pnl.Controls.Add(CreateDialogLabel("מוצר ID:"), 0, 1);
                pnl.Controls.Add(nudProdId, 1, 1);
                pnl.Controls.Add(CreateDialogLabel("מינימום כמות:"), 0, 2);
                pnl.Controls.Add(nudQty, 1, 2);
                pnl.Controls.Add(CreateDialogLabel("מחיר כולל:"), 0, 3);
                pnl.Controls.Add(nudPrice, 1, 3);
                pnl.Controls.Add(CreateDialogLabel("התחלה:"), 0, 4);
                pnl.Controls.Add(txtStart, 1, 4);
                pnl.Controls.Add(CreateDialogLabel("סיום:"), 0, 5);
                pnl.Controls.Add(txtEnd, 1, 5);
                pnl.Controls.Add(CreateDialogLabel("ללקוחות מועדון:"), 0, 6);
                pnl.Controls.Add(chkClub, 1, 6);

                var footer = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 56,
                    FlowDirection = FlowDirection.RightToLeft,
                    Padding = new Padding(0),
                    Margin = new Padding(0)
                };

                var btnOk = CreateDialogButton("שמור", Color.FromArgb(39, 174, 96));
                btnOk.DialogResult = DialogResult.OK;

                var btnCancel = CreateDialogButton("ביטול", Color.FromArgb(233, 237, 245));
                btnCancel.ForeColor = Color.FromArgb(37, 43, 66);
                btnCancel.DialogResult = DialogResult.Cancel;

                footer.Controls.Add(btnOk);
                footer.Controls.Add(btnCancel);
                card.Controls.Add(pnl);
                card.Controls.Add(footer);

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                if (existing != null)
                {
                    nudId.Value = existing.IDNumber;
                    nudProdId.Value = existing.ProductIDNumber;
                    nudQty.Value = existing.QuantityItemsRequiredtoReceivetheSale;
                    nudPrice.Value = (decimal)existing.FullPrice;
                    chkClub.Checked = existing.SaleOnlyforClubCustomers;
                    txtStart.Text = existing.SaleStartDate;
                    txtEnd.Text = existing.SaleEndDate;
                }

                btnOk.Click += (s, e) =>
                {
                    Sale = new BO.Sale
                    {
                        IDNumber = (int)nudId.Value,
                        ProductIDNumber = (int)nudProdId.Value,
                        QuantityItemsRequiredtoReceivetheSale = (int)nudQty.Value,
                        FullPrice = (double)nudPrice.Value,
                        SaleOnlyforClubCustomers = chkClub.Checked,
                        SaleStartDate = txtStart.Text,
                        SaleEndDate = txtEnd.Text
                    };
                };
            }
        }
        #endregion

        // סגנון Claymorphism ל-DataGridView: פינות מעוגלות, מרווחים רחבים והסרת קווים אנכיים
        private DataGridView CreateStyledDataGrid()
        {
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                GridColor = Color.FromArgb(225, 230, 235),
                BackgroundColor = Color.FromArgb(239, 244, 249),
                ForeColor = Color.FromArgb(20, 30, 45),
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                AllowUserToResizeRows = false,
                Margin = new Padding(0)
            };

            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 44, 74);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 10F);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 0, 12, 0);
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = grid.ColumnHeadersDefaultCellStyle.BackColor;

            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(24, 34, 45);
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(229, 243, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(24, 34, 45);
            grid.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grid.DefaultCellStyle.Padding = new Padding(18, 14, 18, 14);
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            grid.RowTemplate.Height = 56;
            grid.RowTemplate.DividerHeight = 8;
            grid.CellPainting += StyledDataGridCellPainting;

            return grid;
        }

        private Button CreateActionButton(string text, Color color)
        {
            var b = new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 36,
                Width = 110,
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

        private static Button CreateDialogButton(string text, Color color)
        {
            var b = new Button
            {
                Text = text,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Height = 36,
                Width = 110,
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

        private static NumericUpDown CreateDialogNumericUpDown()
        {
            return new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BackColor = Color.FromArgb(248, 251, 255),
                ForeColor = Color.FromArgb(24, 34, 45),
                BorderStyle = BorderStyle.None,
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
            form.StartPosition = FormStartPosition.CenterParent;
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

        private static Form CreateMessageDialog(string title, string message, bool isConfirm = false, Color? accentColor = null)
        {
            var dlg = new Form();
            StyleDialogForm(dlg, title);
            dlg.Width = 420;
            dlg.Height = isConfirm ? 220 : 180;

            var card = CreateDialogPanel();
            dlg.Controls.Add(card);
            card.Controls.Add(CreateDialogHeader(title));

            var messageLabel = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(50, 62, 79),
                Dock = DockStyle.Top,
                Height = 80,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = false,
                Padding = new Padding(8)
            };
            card.Controls.Add(messageLabel);

            var actionPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            if (isConfirm)
            {
                var btnYes = CreateDialogButton("כן", accentColor ?? Color.FromArgb(39, 174, 96));
                btnYes.DialogResult = DialogResult.Yes;
                var btnNo = CreateDialogButton("לא", Color.FromArgb(233, 237, 245));
                btnNo.ForeColor = Color.FromArgb(37, 43, 66);
                btnNo.DialogResult = DialogResult.No;
                actionPanel.Controls.Add(btnYes);
                actionPanel.Controls.Add(btnNo);
            }
            else
            {
                var btnOk = CreateDialogButton("אישור", accentColor ?? Color.FromArgb(39, 174, 96));
                btnOk.DialogResult = DialogResult.OK;
                actionPanel.Controls.Add(btnOk);
                dlg.AcceptButton = btnOk;
            }

            card.Controls.Add(actionPanel);
            return dlg;
        }

        private static void ShowAppNotification(IWin32Window owner, string title, string message, bool isError = false)
        {
            using var dlg = CreateMessageDialog(title, message, false, isError ? Color.FromArgb(242, 81, 74) : Color.FromArgb(39, 174, 96));
            dlg.ShowDialog(owner);
        }

        private static bool ShowAppConfirmation(IWin32Window owner, string title, string message)
        {
            using var dlg = CreateMessageDialog(title, message, true, Color.FromArgb(46, 134, 193));
            return dlg.ShowDialog(owner) == DialogResult.Yes;
        }

        // ציור מותאם לשורות DataGridView כך שהן ייראו כ'כרטיסים' עם פינות מעוגלות
        private void StyledDataGridCellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (sender is not DataGridView grid)
            {
                return;
            }

            if (e.Graphics == null)
            {
                return;
            }

            e.PaintBackground(e.CellBounds, true);

            if (e.ColumnIndex == 0)
            {
                var rowWidth = grid.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
                var rowRect = new Rectangle(10, e.CellBounds.Top + 2, Math.Min(rowWidth + 8, grid.ClientSize.Width - 20), e.CellBounds.Height - 4);
                using var path = GetRoundedRectPath(rowRect, 18);
                using var brush = new SolidBrush(e.State.HasFlag(DataGridViewElementStates.Selected)
                    ? Color.FromArgb(229, 243, 255)
                    : Color.White);
                using var borderPen = new Pen(Color.FromArgb(220, 225, 233), 1f);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(borderPen, path);
            }

            e.Paint(e.CellBounds, DataGridViewPaintParts.ContentForeground | DataGridViewPaintParts.Focus);
            e.Handled = true;
        }

        private GraphicsPath GetRoundedRectPath(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, radius, radius, 180, 90);
            path.AddArc(bounds.Right - radius, bounds.Y, radius, radius, 270, 90);
            path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            return path;
        }

        // יישום פינות מעוגלות לכפתורים גדולים בסגנון Claymorphism
        private void ApplyRoundedButton(Button button, int radius = 16)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(button.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(button.Width - radius, button.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, button.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();
            button.Region = new Region(path);
        }
    }
}
