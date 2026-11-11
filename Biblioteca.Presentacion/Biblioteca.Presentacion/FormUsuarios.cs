using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    public partial class FormUsuarios : Form
    {
        private UsuarioNegocio negocioUsuario = new UsuarioNegocio();

        public FormUsuarios()
        {
            InitializeComponent();
            this.Load += FormUsuarios_Load;
            this.Shown += (s, ev) => EstilizarVista("GESTIÓN DE USUARIOS", dgvUsuarios);

            btnGuardar.Click += btnGuardar_Click;
            btnModificar.Click += btnModificar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            if (dgvUsuarios != null)
            {
                dgvUsuarios.CellClick += dgvUsuarios_CellClick;
            }
        }

        private void EstilizarVista(string tituloModulo, DataGridView grilla)
        {
            this.BackColor = Color.White;

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Black
            };
            Label lblTitulo = new Label
            {
                Text = tituloModulo,
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitulo);
            this.Controls.Add(pnlHeader);

            if (grilla != null)
            {
                grilla.Dock = DockStyle.None;
                grilla.Left = 0;
                grilla.Width = this.ClientSize.Width;
                grilla.Height = 260;
                grilla.Top = this.ClientSize.Height - 260;
                grilla.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                grilla.BringToFront();
            }

            var controlesForm = this.Controls.Cast<Control>()
                .Where(c => c != grilla && c != pnlHeader && !(c is Panel))
                .ToList();

            if (controlesForm.Count > 0)
            {
                int minX = controlesForm.Min(c => c.Left);
                int minY = controlesForm.Min(c => c.Top);
                int maxX = controlesForm.Max(c => c.Right);
                int maxY = controlesForm.Max(c => c.Bottom);

                Panel pnlCentral = new Panel
                {
                    Width = (maxX - minX) + 30,
                    Height = (maxY - minY) + 30,
                    BackColor = Color.Transparent
                };

                foreach (var ctrl in controlesForm)
                {
                    this.Controls.Remove(ctrl);
                    ctrl.Left -= (minX - 15);
                    ctrl.Top -= (minY - 15);
                    pnlCentral.Controls.Add(ctrl);
                }

                pnlCentral.Left = Math.Max(0, (this.ClientSize.Width - pnlCentral.Width) / 2);
                pnlCentral.Top = 75;
                pnlCentral.Anchor = AnchorStyles.Top;

                this.Controls.Add(pnlCentral);
                pnlCentral.BringToFront();
            }
        }

        private void FormUsuarios_Load(object sender, EventArgs e)
        {
            if (dgvUsuarios != null)
            {
                dgvUsuarios.RowHeadersVisible = false;
                dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvUsuarios.MultiSelect = false;
                dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            CargarGrilla();
            LimpiarCampos();
        }

        private void CargarGrilla()
        {
            try
            {
                if (dgvUsuarios == null) return;
                dgvUsuarios.DataSource = negocioUsuario.Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private TextBox ObtenerCajaTexto(params string[] posiblesNombres)
        {
            foreach (string nombre in posiblesNombres)
            {
                Control[] encontrados = this.Controls.Find(nombre, true);
                if (encontrados.Length > 0 && encontrados[0] is TextBox tb)
                {
                    return tb;
                }
            }
            return null;
        }

        private void AsignarPropiedadSiExiste(Usuario usuario, string nombrePropiedad, string valor)
        {
            var prop = typeof(Usuario).GetProperty(nombrePropiedad);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(usuario, valor);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();

            TextBox tbDoc = ObtenerCajaTexto("txtDocumento", "txtDoc");
            if (tbDoc != null) tbDoc.Clear();

            TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
            if (tbApe != null) tbApe.Clear();

            TextBox tbTel = ObtenerCajaTexto("txtTelefono", "txtTel");
            if (tbTel != null) tbTel.Clear();

            TextBox tbCor = ObtenerCajaTexto("txtCorreo", "txtEmail");
            if (tbCor != null) tbCor.Clear();

            if (dgvUsuarios != null) dgvUsuarios.ClearSelection();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox tbDoc = ObtenerCajaTexto("txtDocumento", "txtDoc");
                TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
                TextBox tbTel = ObtenerCajaTexto("txtTelefono", "txtTel");
                TextBox tbCor = ObtenerCajaTexto("txtCorreo", "txtEmail");

                string documento = tbDoc != null ? tbDoc.Text.Trim() : "";
                string nombre = txtNombre.Text.Trim();
                string apellido = tbApe != null ? tbApe.Text.Trim() : "";
                string telefono = tbTel != null ? tbTel.Text.Trim() : "";
                string correo = tbCor != null ? tbCor.Text.Trim() : "";

                if (string.IsNullOrWhiteSpace(documento))
                {
                    MessageBox.Show("El documento del usuario es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (tbDoc != null) tbDoc.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("El nombre del usuario es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(apellido))
                {
                    MessageBox.Show("El apellido del usuario es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (tbApe != null) tbApe.Focus();
                    return;
                }

                Usuario nuevo = new Usuario
                {
                    Nombre = nombre,
                    Documento = documento,
                    Telefono = telefono
                };

                AsignarPropiedadSiExiste(nuevo, "Apellido", apellido);
                AsignarPropiedadSiExiste(nuevo, "Apellidos", apellido);
                AsignarPropiedadSiExiste(nuevo, "Correo", correo);
                AsignarPropiedadSiExiste(nuevo, "Email", correo);

                string resultado = negocioUsuario.Registrar(nuevo);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Usuario registrado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrilla();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show(resultado, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Seleccione un usuario de la tabla para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TextBox tbDoc = ObtenerCajaTexto("txtDocumento", "txtDoc");
                TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
                TextBox tbTel = ObtenerCajaTexto("txtTelefono", "txtTel");
                TextBox tbCor = ObtenerCajaTexto("txtCorreo", "txtEmail");

                string documento = tbDoc != null ? tbDoc.Text.Trim() : "";
                string nombre = txtNombre.Text.Trim();
                string apellido = tbApe != null ? tbApe.Text.Trim() : "";
                string telefono = tbTel != null ? tbTel.Text.Trim() : "";
                string correo = tbCor != null ? tbCor.Text.Trim() : "";

                if (string.IsNullOrWhiteSpace(apellido))
                {
                    MessageBox.Show("El apellido del usuario es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (tbApe != null) tbApe.Focus();
                    return;
                }

                Usuario modificado = new Usuario
                {
                    IdUsuario = Convert.ToInt32(txtId.Text),
                    Nombre = nombre,
                    Documento = documento,
                    Telefono = telefono
                };

                AsignarPropiedadSiExiste(modificado, "Apellido", apellido);
                AsignarPropiedadSiExiste(modificado, "Apellidos", apellido);
                AsignarPropiedadSiExiste(modificado, "Correo", correo);
                AsignarPropiedadSiExiste(modificado, "Email", correo);

                string resultado = negocioUsuario.Modificar(modificado);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Usuario modificado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrilla();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show(resultado, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al modificar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Seleccione un usuario de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("¿Está seguro de eliminar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(txtId.Text);
                    string resultado = negocioUsuario.Eliminar(id);

                    if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Usuario eliminado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        CargarGrilla();
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show(resultado, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void dgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvUsuarios.Rows[e.RowIndex];
                txtId.Text = fila.Cells["IdUsuario"].Value?.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();

                TextBox tbDoc = ObtenerCajaTexto("txtDocumento", "txtDoc");
                if (tbDoc != null && fila.Cells["Documento"] != null)
                    tbDoc.Text = fila.Cells["Documento"].Value?.ToString();

                TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
                if (tbApe != null)
                {
                    if (fila.Cells["Apellido"] != null) tbApe.Text = fila.Cells["Apellido"].Value?.ToString();
                    else if (fila.Cells["Apellidos"] != null) tbApe.Text = fila.Cells["Apellidos"].Value?.ToString();
                }

                TextBox tbTel = ObtenerCajaTexto("txtTelefono", "txtTel");
                if (tbTel != null)
                {
                    if (fila.Cells["Telefono"] != null) tbTel.Text = fila.Cells["Telefono"].Value?.ToString();
                }

                TextBox tbCor = ObtenerCajaTexto("txtCorreo", "txtEmail");
                if (tbCor != null)
                {
                    if (fila.Cells["Correo"] != null) tbCor.Text = fila.Cells["Correo"].Value?.ToString();
                    else if (fila.Cells["Email"] != null) tbCor.Text = fila.Cells["Email"].Value?.ToString();
                }
            }
        }
    }
}