using System;
using System.Net.Http;
using System.Windows.Forms;
using System.Text;

namespace cliente
{
    public partial class Form1 : Form
    {
        private FlowLayoutPanel mainPanel;
        private static readonly HttpClient _http = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8081/v1/")
        };
        public Form1()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Edificio DTIC - Cliente";
            this.WindowState = FormWindowState.Maximized;

            // Sidebar
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 180,
                BackColor = System.Drawing.Color.FromArgb(45, 45, 48)
            };

            string[] entities = {
                "Validaciones", "ControlPresencia", "ControlAccesos", "Empleados",
                "Notificaciones", "Dispositivos", "Niveles", "Salas"
            };

            foreach (string entity in entities)
            {
                string entityName = entity;
                Button btn = new Button
                {
                    Text = entityName,
                    Dock = DockStyle.Top,
                    Height = 40,
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = System.Drawing.Color.White,
                    BackColor = System.Drawing.Color.FromArgb(45, 45, 48),
                    Font = new System.Drawing.Font("Arial UI", 10, System.Drawing.FontStyle.Bold)
                };
                btn.Click += (s, e) => LoadEntity(entityName);
                sidebar.Controls.Add(btn);
            }

            mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(20),
                BackColor = System.Drawing.Color.White
            };
            this.Controls.Add(mainPanel);
            this.Controls.Add(sidebar);
        }

        private void LoadEntity(string entity)
        {
            mainPanel.Controls.Clear();
            Label title = new Label
            {
                Text = entity,
                Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 20)
            };
            mainPanel.Controls.Add(title);

            switch (entity)
            {
                case "Salas": AddSalasForm(); break;
                case "Niveles": AddNivelesForm(); break;
                case "Dispositivos": AddDispositivosForm(); break;
                case "Notificaciones": AddNotificacionesForm(); break;
                //case "Empleados": AddSoapPlaceholder("Empleados"); break;
                //case "ControlAccesos": AddSoapPlaceholder("ControlAccesos"); break;
                //case "ControlPresencia": AddSoapPlaceholder("ControlPresencia"); break;
                //case "Validaciones": AddSoapPlaceholder("Validaciones"); break;
            }
        }

        private TextBox MakeInput(string placeholder, int width = 300)
        {
            return new TextBox
            {
                PlaceholderText = placeholder,
                Width = width,
                Margin = new Padding(0, 5, 0, 5)
            };
        }

        private Button MakeSubmitButton(string text = "Enviar")
        {
            return new Button
            {
                Text = text,
                Width = 150,
                Height = 35,
                Margin = new Padding(0, 10, 0, 5),
                BackColor = System.Drawing.Color.FromArgb(0, 122, 204),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat
            };
        }

        private TextBox MakeResultBox()
        {
            return new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Width = 600,
                Height = 120,
                ScrollBars = ScrollBars.Vertical,
                Margin = new Padding(0, 10, 0, 0),
                BackColor = System.Drawing.Color.FromArgb(245, 245, 245)
            };
        }

        private ComboBox MakeCombo(string[] items)
        {
            ComboBox cb = new ComboBox
            {
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(0, 5, 0, 5)
            };
            cb.Items.AddRange(items);
            cb.SelectedIndex = 0;
            return cb;
        }

        private void ShowResult(TextBox resultBox, bool success, string content)
        {
            resultBox.ForeColor = success ? System.Drawing.Color.DarkGreen : System.Drawing.Color.Red;
            resultBox.Text = success ? content : $"Error: {content}";
        }

        // ─── SALAS ──────────────────────────────────────────────────

        private void AddSalasForm()
        {
            var opBox = MakeCombo(new[] { "GET", "POST", "PUT", "DELETE" });
            var wsKey = MakeInput("wsKey");
            var codigo = MakeInput("Codigo Sala (integer)");
            var nombre = MakeInput("Nombre");
            var nivel = MakeInput("Nivel (integer)");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            btn.Click += async (s, e) =>
            {
                string op = opBox.SelectedItem.ToString();
                string body = $"{{\"codigoSala\":{codigo.Text},\"nombre\":\"{nombre.Text}\",\"nivel\":{nivel.Text},\"wsKey\":\"{wsKey.Text}\"}}";
                HttpResponseMessage res = op switch
                {
                    "GET" => await _http.GetAsync($"salas/{codigo.Text}?wsKey={wsKey.Text}"),
                    "DELETE" => await _http.DeleteAsync($"salas/{codigo.Text}?wsKey={wsKey.Text}"),
                    "POST" => await _http.PostAsync("salas", new StringContent(body, Encoding.UTF8, "application/json")),
                    "PUT" => await _http.PutAsync($"salas/{codigo.Text}", new StringContent(body, Encoding.UTF8, "application/json")),
                    _ => throw new Exception("Operacion desconocida")
                };
                string content = await res.Content.ReadAsStringAsync();
                ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, codigo, nombre, nivel, btn, resultBox });
        }

        // ─── NIVELES ─────────────────────────────────────────────────

        private void AddNivelesForm()
        {
            var opBox = MakeCombo(new[] { "GET", "POST", "PUT", "DELETE" });
            var wsKey = MakeInput("wsKey");
            var nivelInput = MakeInput("Nivel (integer)");
            var descripcion = MakeInput("Descripcion");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            btn.Click += async (s, e) =>
            {
                string op = opBox.SelectedItem.ToString();
                string body = $"{{\"nivel\":{nivelInput.Text},\"descripcion\":\"{descripcion.Text}\",\"wsKey\":\"{wsKey.Text}\"}}";
                HttpResponseMessage res = op switch
                {
                    "GET" => await _http.GetAsync($"nivel/{nivelInput.Text}?wsKey={wsKey.Text}"),
                    "DELETE" => await _http.DeleteAsync($"nivel/{nivelInput.Text}?wsKey={wsKey.Text}"),
                    "POST" => await _http.PostAsync("nivel", new StringContent(body, Encoding.UTF8, "application/json")),
                    "PUT" => await _http.PutAsync($"nivel/{nivelInput.Text}", new StringContent(body, Encoding.UTF8, "application/json")),
                    _ => throw new Exception("Operacion desconocida")
                };
                string content = await res.Content.ReadAsStringAsync();
                ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, nivelInput, descripcion, btn, resultBox });
        }

        // ─── DISPOSITIVOS ────────────────────────────────────────────

        private void AddDispositivosForm()
        {
            var opBox = MakeCombo(new[] { "GET", "POST", "PUT", "DELETE" });
            var wsKey = MakeInput("wsKey");
            var codigo = MakeInput("Codigo (integer)");
            var descripcion = MakeInput("Descripcion");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            btn.Click += async (s, e) =>
            {
                string op = opBox.SelectedItem.ToString();
                string body = $"{{\"codigo\":{codigo.Text},\"descripcion\":\"{descripcion.Text}\",\"wsKey\":\"{wsKey.Text}\"}}";
                HttpResponseMessage res = op switch
                {
                    "GET" => await _http.GetAsync($"dispositivo/{codigo.Text}?wsKey={wsKey.Text}"),
                    "DELETE" => await _http.DeleteAsync($"dispositivo/{codigo.Text}?wsKey={wsKey.Text}"),
                    "POST" => await _http.PostAsync("dispositivo", new StringContent(body, Encoding.UTF8, "application/json")),
                    "PUT" => await _http.PutAsync($"dispositivo/{codigo.Text}", new StringContent(body, Encoding.UTF8, "application/json")),
                    _ => throw new Exception("Operacion desconocida")
                };
                string content = await res.Content.ReadAsStringAsync();
                ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, codigo, descripcion, btn, resultBox });
        }

        // ─── NOTIFICACIONES ──────────────────────────────────────────

        private void AddNotificacionesForm()
        {
            var opBox = MakeCombo(new[] { "presencia", "usuarioValido", "error" });
            var wsKey = MakeInput("wsKey");
            var codigoSala = MakeInput("Codigo Sala");
            var nifnie = MakeInput("NIF/NIE");
            var errorMsg = MakeInput("Mensaje de error");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                codigoSala.Visible = op == "presencia";
                nifnie.Visible = op == "usuarioValido" || op == "error";
                errorMsg.Visible = op == "error";
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
                {
                    string op = opBox.SelectedItem.ToString();
                    string url = $"notificaciones/{op}?wsKey={wsKey.Text}";
                    string body = op switch
                    {
                        "presencia" => $"{{\"codigoSala\":{codigoSala.Text},\"wsKey\":\"{wsKey.Text}\"}}",
                        "usuarioValido" => $"{{\"nifnie\":\"{nifnie.Text}\",\"wsKey\":\"{wsKey.Text}\"}}",
                        "error" => $"{{\"nifnie\":\"{nifnie.Text}\",\"error\":\"{errorMsg.Text}\",\"wsKey\":\"{wsKey.Text}\"}}",
                        _ => throw new Exception("Tipo desconocido")
                    };
                    HttpResponseMessage res = await _http.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
                    string content = await res.Content.ReadAsStringAsync();
                    ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, codigoSala, nifnie, errorMsg, btn, resultBox });
            UpdateFieldsVisibility();
        }

    }
}
