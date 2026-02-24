using System;
using System.Net.Http;
using System.Windows.Forms;
using System.Text;
using System.Text.Json;

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
                case "Empleados": AddEmpleadosForm(); break;
                case "ControlAccesos": AddControlAccesosForm(); break;
                case "ControlPresencia": AddControlPresenciaForm(); break;
                case "Validaciones": AddValidacionesForm(); break;
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

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                bool needsAllFields = op == "POST" || op == "PUT";
                nombre.Visible = needsAllFields;
                nivel.Visible = needsAllFields;
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
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

                    if (res.IsSuccessStatusCode && op == "GET" && content.Length > 0)
                    {
                        try
                        {
                            var json = System.Text.Json.JsonDocument.Parse(content);
                            var root = json.RootElement;
                            string formatted = $"Codigo Sala: {root.GetProperty("codigoSala").GetInt32()}{Environment.NewLine}";
                            formatted += $"Nombre: {root.GetProperty("nombre").GetString()}{Environment.NewLine}";
                            formatted += $"Nivel: {root.GetProperty("nivel").GetInt32()}";
                            ShowResult(resultBox, true, formatted);
                        }
                        catch
                        {
                            ShowResult(resultBox, true, content);
                        }
                    }
                    else
                    {
                        ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, codigo, nombre, nivel, btn, resultBox });
            UpdateFieldsVisibility();
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

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                bool needsDescription = op == "POST" || op == "PUT";
                descripcion.Visible = needsDescription;
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
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

                    if (res.IsSuccessStatusCode && op == "GET" && content.Length > 0)
                    {
                        try
                        {
                            var json = System.Text.Json.JsonDocument.Parse(content);
                            var root = json.RootElement;
                            string formatted = $"Nivel: {root.GetProperty("nivel").GetInt32()}{Environment.NewLine}";
                            formatted += $"Descripcion: {root.GetProperty("descripcion").GetString()}";
                            ShowResult(resultBox, true, formatted);
                        }
                        catch
                        {
                            ShowResult(resultBox, true, content);
                        }
                    }
                    else
                    {
                        ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, nivelInput, descripcion, btn, resultBox });
            UpdateFieldsVisibility();
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

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                bool needsDescription = op == "POST" || op == "PUT";
                descripcion.Visible = needsDescription;
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
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

                    if (res.IsSuccessStatusCode && op == "GET" && content.Length > 0)
                    {
                        try
                        {
                            var json = System.Text.Json.JsonDocument.Parse(content);
                            var root = json.RootElement;
                            string formatted = $"Codigo: {root.GetProperty("codigo").GetInt32()}{Environment.NewLine}";
                            formatted += $"Descripcion: {root.GetProperty("descripcion").GetString()}";
                            ShowResult(resultBox, true, formatted);
                        }
                        catch
                        {
                            ShowResult(resultBox, true, content);
                        }
                    }
                    else
                    {
                        ShowResult(resultBox, res.IsSuccessStatusCode, content.Length > 0 ? content : res.StatusCode.ToString());
                    }
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, wsKey, codigo, descripcion, btn, resultBox });
            UpdateFieldsVisibility();
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

        // ─── EMPLEADOS (SOAP) ────────────────────────────────────────

        private void AddEmpleadosForm()
        {
            var opBox = MakeCombo(new[] { "consultar", "nuevo", "modificar", "borrar" });
            var nifnie = MakeInput("NIF/NIE");
            var nombreApellidos = MakeInput("Nombre y Apellidos");
            var email = MakeInput("Email");
            var naf = MakeInput("NAF");
            var iban = MakeInput("IBAN");
            var idNivel = MakeInput("ID Nivel (integer)");
            var usuario = MakeInput("Usuario");
            var password = MakeInput("Password");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                bool isConsultar = op == "consultar";
                bool isBorrar = op == "borrar";
                nombreApellidos.Visible = !isConsultar && !isBorrar;
                email.Visible = !isConsultar && !isBorrar;
                naf.Visible = !isConsultar && !isBorrar;
                iban.Visible = !isConsultar && !isBorrar;
                idNivel.Visible = !isConsultar && !isBorrar;
                usuario.Visible = !isConsultar && !isBorrar;
                password.Visible = !isConsultar && !isBorrar;
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
                {
                    string op = opBox.SelectedItem.ToString();
                    var client = new EmpleadosServicio.empleadosClient();
                    string result = "";

                    switch (op)
                    {
                        case "consultar":
                            var consultarRes = await client.consultarAsync(nifnie.Text);
                            var emp = consultarRes.@out;
                            if (emp != null)
                            {
                                result = $"ID: {emp.id}{Environment.NewLine}NIF/NIE: {emp.nifnie}{Environment.NewLine}Nombre: {emp.nombreApellidos}{Environment.NewLine}Email: {emp.email}{Environment.NewLine}NAF: {emp.naf}{Environment.NewLine}IBAN: {emp.iban}{Environment.NewLine}Nivel: {emp.idNivel}{Environment.NewLine}Usuario: {emp.usuario}{Environment.NewLine}Valido: {emp.valido}";
                            }
                            else
                            {
                                result = "Empleado no encontrado";
                            }
                            ShowResult(resultBox, consultarRes.@out != null, result);
                            break;

                        case "nuevo":
                            var validacionClient = new ValidacionesServicio.ValidacionesClient();

                            // Validar NIF/NIE
                            bool nifnieValido = false;
                            if (nifnie.Text.StartsWith("X") || nifnie.Text.StartsWith("Y") || nifnie.Text.StartsWith("Z"))
                            {
                                var nieRes = await validacionClient.validarNIEAsync(nifnie.Text);
                                nifnieValido = nieRes.Body.@out;
                                if (!nifnieValido)
                                {
                                    ShowResult(resultBox, false, "NIE inválido");
                                    return;
                                }
                            }
                            else
                            {
                                var nifRes = await validacionClient.validarNIFAsync(nifnie.Text);
                                nifnieValido = nifRes.Body.@out;
                                if (!nifnieValido)
                                {
                                    ShowResult(resultBox, false, "NIF inválido");
                                    return;
                                }
                            }

                            // Validar NAF
                            var nafValidRes = await validacionClient.validarNAFAsync(naf.Text);
                            if (!nafValidRes.Body.@out)
                            {
                                ShowResult(resultBox, false, "NAF inválido");
                                return;
                            }

                            // Validar IBAN
                            var ibanValidRes = await validacionClient.validarIBANAsync(iban.Text);
                            if (!ibanValidRes.Body.@out)
                            {
                                ShowResult(resultBox, false, "IBAN inválido");
                                return;
                            }

                            // Si todas las validaciones pasan, crear empleado
                            var empleado = new EmpleadosServicio.Empleado
                            {
                                nifnie = nifnie.Text,
                                nombreApellidos = nombreApellidos.Text,
                                email = email.Text,
                                naf = naf.Text,
                                iban = iban.Text,
                                idNivel = int.Parse(idNivel.Text),
                                usuario = usuario.Text,
                                password = password.Text
                            };
                            var nuevoRes = await client.nuevoAsync(empleado);
                            ShowResult(resultBox, nuevoRes.@out, nuevoRes.@out ? "Empleado creado exitosamente" : "Error al crear empleado (posible duplicado)");
                            break;

                        case "modificar":
                            var validacionClientMod = new ValidacionesServicio.ValidacionesClient();

                            // Validar NIF/NIE
                            bool nifnieValidoMod = false;
                            if (nifnie.Text.StartsWith("X") || nifnie.Text.StartsWith("Y") || nifnie.Text.StartsWith("Z"))
                            {
                                var nieResMod = await validacionClientMod.validarNIEAsync(nifnie.Text);
                                nifnieValidoMod = nieResMod.Body.@out;
                                if (!nifnieValidoMod)
                                {
                                    ShowResult(resultBox, false, "NIE inválido");
                                    return;
                                }
                            }
                            else
                            {
                                var nifResMod = await validacionClientMod.validarNIFAsync(nifnie.Text);
                                nifnieValidoMod = nifResMod.Body.@out;
                                if (!nifnieValidoMod)
                                {
                                    ShowResult(resultBox, false, "NIF inválido");
                                    return;
                                }
                            }

                            // Validar NAF
                            var nafValidResMod = await validacionClientMod.validarNAFAsync(naf.Text);
                            if (!nafValidResMod.Body.@out)
                            {
                                ShowResult(resultBox, false, "NAF inválido");
                                return;
                            }

                            // Validar IBAN
                            var ibanValidResMod = await validacionClientMod.validarIBANAsync(iban.Text);
                            if (!ibanValidResMod.Body.@out)
                            {
                                ShowResult(resultBox, false, "IBAN inválido");
                                return;
                            }

                            // Si todas las validaciones pasan, modificar empleado
                            var empModificar = new EmpleadosServicio.Empleado
                            {
                                nifnie = nifnie.Text,
                                nombreApellidos = nombreApellidos.Text,
                                email = email.Text,
                                naf = naf.Text,
                                iban = iban.Text,
                                idNivel = int.Parse(idNivel.Text),
                                usuario = usuario.Text,
                                password = password.Text
                            };
                            var modificarRes = await client.modificarAsync(empModificar);
                            ShowResult(resultBox, modificarRes.@out, modificarRes.@out ? "Empleado modificado exitosamente" : "Error al modificar empleado (no encontrado)");
                            break;

                        case "borrar":
                            var borrarRes = await client.borrarAsync(nifnie.Text);
                            ShowResult(resultBox, borrarRes.@out, borrarRes.@out ? "Empleado eliminado exitosamente" : "Error al eliminar empleado");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, nifnie, nombreApellidos, email, naf, iban, idNivel, usuario, password, btn, resultBox });
            UpdateFieldsVisibility();
        }

        // ─── CONTROL ACCESOS (SOAP) ──────────────────────────────────

        private void AddControlAccesosForm()
        {
            var opBox = MakeCombo(new[] { "registrar", "consultar" });
            var nif = MakeInput("NIF");
            var codigoSala = MakeInput("Codigo Sala (integer)");
            var codigoDispositivo = MakeInput("Codigo Dispositivo (integer)");
            var fechaInicio = MakeInput("Fecha Inicio (yyyy-MM-dd)");
            var fechaFin = MakeInput("Fecha Fin (yyyy-MM-dd)");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                bool isRegistrar = op == "registrar";
                fechaInicio.Visible = !isRegistrar;
                fechaFin.Visible = !isRegistrar;
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
                {
                    string op = opBox.SelectedItem.ToString();
                    var client = new ControlAccesosServicio.ControlAccesosClient();

                    if (op == "registrar")
                    {
                        var res = await client.registrarAsync(
                            nif.Text,
                            int.Parse(codigoSala.Text),
                            int.Parse(codigoDispositivo.Text)
                        );
                        ShowResult(resultBox, res.@out, res.@out ? "Acceso registrado exitosamente" : "Error al registrar acceso");
                    }
                    else
                    {
                        var res = await client.consultarAsync(
                            nif.Text,
                            int.Parse(codigoSala.Text),
                            int.Parse(codigoDispositivo.Text),
                            fechaInicio.Text,
                            fechaFin.Text
                        );
                        if (res.@out != null && res.@out.Length > 0)
                        {
                            string result = $"Registros de acceso:{Environment.NewLine}";
                            foreach (var registro in res.@out)
                            {
                                result += $"{Environment.NewLine}ID: {registro.id}, Empleado: {registro.idEmpleado}, Sala: {registro.idSala}, Dispositivo: {registro.idDispositivo}, Fecha: {registro.fechaHora}";
                            }
                            ShowResult(resultBox, true, result);
                        }
                        else
                        {
                            ShowResult(resultBox, false, "No se encontraron registros");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, nif, codigoSala, codigoDispositivo, fechaInicio, fechaFin, btn, resultBox });
            UpdateFieldsVisibility();
        }

        // ─── CONTROL PRESENCIA (SOAP) ────────────────────────────────

        private void AddControlPresenciaForm()
        {
            var opBox = MakeCombo(new[] { "registrar", "eliminar", "controlEmpleadosSala" });
            var nif = MakeInput("NIF");
            var codigoSala = MakeInput("Codigo Sala (integer)");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            void UpdateFieldsVisibility()
            {
                string op = opBox.SelectedItem.ToString();
                nif.Visible = op != "controlEmpleadosSala";
            }

            opBox.SelectedIndexChanged += (s, e) => UpdateFieldsVisibility();

            btn.Click += async (s, e) =>
            {
                try
                {
                    string op = opBox.SelectedItem.ToString();
                    var client = new ControlPresenciaServicio.ControlPresenciaClient();

                    switch (op)
                    {
                        case "registrar":
                            var regRes = await client.registrarAsync(
                                nif.Text,
                                int.Parse(codigoSala.Text)
                            );
                            ShowResult(resultBox, regRes.@out, regRes.@out ? "Presencia registrada exitosamente" : "Error al registrar presencia");
                            break;

                        case "eliminar":
                            var elimRes = await client.eliminarAsync(
                                nif.Text,
                                int.Parse(codigoSala.Text)
                            );
                            ShowResult(resultBox, elimRes.@out, elimRes.@out ? "Presencia eliminada exitosamente" : "Error al eliminar presencia");
                            break;

                        case "controlEmpleadosSala":
                            var controlRes = await client.controlEmpleadosSalaAsync(
                                int.Parse(codigoSala.Text)
                            );
                            if (controlRes.@out != null && controlRes.@out.Length > 0)
                            {
                                string result = $"Empleados en sala {codigoSala.Text}:{Environment.NewLine}";
                                foreach (var empleado in controlRes.@out)
                                {
                                    result += $"{Environment.NewLine}- {empleado.nombreApellidos} ({empleado.nifnie})";
                                }
                                ShowResult(resultBox, true, result);
                            }
                            else
                            {
                                ShowResult(resultBox, true, "No hay empleados en la sala");
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, nif, codigoSala, btn, resultBox });
            UpdateFieldsVisibility();
        }

        // ─── VALIDACIONES (SOAP) ─────────────────────────────────────

        private void AddValidacionesForm()
        {
            var opBox = MakeCombo(new[] { "validarNIF", "validarNIE", "validarNAF", "validarIBAN" });
            var valorInput = MakeInput("Valor a validar");
            var resultBox = MakeResultBox();
            var btn = MakeSubmitButton();

            btn.Click += async (s, e) =>
            {
                try
                {
                    string op = opBox.SelectedItem.ToString();
                    var client = new ValidacionesServicio.ValidacionesClient();
                    bool resultado = false;

                    switch (op)
                    {
                        case "validarNIF":
                            var nifRes = await client.validarNIFAsync(valorInput.Text);
                            resultado = nifRes.Body.@out;
                            break;

                        case "validarNIE":
                            var nieRes = await client.validarNIEAsync(valorInput.Text);
                            resultado = nieRes.Body.@out;
                            break;

                        case "validarNAF":
                            var nafRes = await client.validarNAFAsync(valorInput.Text);
                            resultado = nafRes.Body.@out;
                            break;

                        case "validarIBAN":
                            var ibanRes = await client.validarIBANAsync(valorInput.Text);
                            resultado = ibanRes.Body.@out;
                            break;
                    }

                    ShowResult(resultBox, resultado, resultado ? $"{op}: VÁLIDO ✓" : $"{op}: INVÁLIDO ✗");
                }
                catch (Exception ex)
                {
                    ShowResult(resultBox, false, ex.Message);
                }
            };

            mainPanel.Controls.AddRange(new Control[] { opBox, valorInput, btn, resultBox });
        }

    }
}
