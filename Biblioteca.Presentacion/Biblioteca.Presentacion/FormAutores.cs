using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    public partial class FormAutores : Form
    {
        private AutorNegocio negocioAutor = new AutorNegocio();

        public FormAutores()
        {
            InitializeComponent();
            this.Load += FormAutores_Load;
            this.Shown += (s, ev) => EstilizarVista("GESTIÓN DE AUTORES", dgvAutores);

            btnGuardar.Click += btnGuardar_Click;
            btnModificar.Click += btnModificar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            if (dgvAutores != null)
            {
                dgvAutores.CellClick += dgvAutores_CellClick;
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

        private void FormAutores_Load(object sender, EventArgs e)
        {
            if (dgvAutores != null)
            {
                dgvAutores.RowHeadersVisible = false;
                dgvAutores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvAutores.MultiSelect = false;
                dgvAutores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            CargarGrilla();
            LimpiarCampos();
        }

        private void CargarGrilla()
        {
            try
            {
                if (dgvAutores == null) return;
                dgvAutores.DataSource = negocioAutor.Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar autores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void AsignarPropiedadSiExiste(Autor autor, string nombrePropiedad, string valor)
        {
            var prop = typeof(Autor).GetProperty(nombrePropiedad);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(autor, valor);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();

            TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
            if (tbApe != null) tbApe.Clear();

            TextBox tbNac = ObtenerCajaTexto("txtNacionalidad", "txtPais");
            if (tbNac != null) tbNac.Clear();

            if (dgvAutores != null) dgvAutores.ClearSelection();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
                TextBox tbNac = ObtenerCajaTexto("txtNacionalidad", "txtPais");

                string nombre = txtNombre.Text.Trim();
                string apellido = tbApe != null ? tbApe.Text.Trim() : "";
                string nacionalidad = tbNac != null ? tbNac.Text.Trim() : "";

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("El nombre del autor es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(apellido))
                {
                    MessageBox.Show("El apellido del autor es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (tbApe != null) tbApe.Focus();
                    return;
                }

                Autor nuevo = new Autor
                {
                    Nombre = nombre,
                    Nacionalidad = nacionalidad
                };

                AsignarPropiedadSiExiste(nuevo, "Apellido", apellido);
                AsignarPropiedadSiExiste(nuevo, "Apellidos", apellido);

                string resultado = negocioAutor.Registrar(nuevo);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Autor registrado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Seleccione un autor de la tabla para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
                TextBox tbNac = ObtenerCajaTexto("txtNacionalidad", "txtPais");

                string nombre = txtNombre.Text.Trim();
                string apellido = tbApe != null ? tbApe.Text.Trim() : "";
                string nacionalidad = tbNac != null ? tbNac.Text.Trim() : "";

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("El nombre del autor es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNombre.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(apellido))
                {
                    MessageBox.Show("El apellido del autor es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (tbApe != null) tbApe.Focus();
                    return;
                }

                Autor modificado = new Autor
                {
                    IdAutor = Convert.ToInt32(txtId.Text),
                    Nombre = nombre,
                    Nacionalidad = nacionalidad
                };

                AsignarPropiedadSiExiste(modificado, "Apellido", apellido);
                AsignarPropiedadSiExiste(modificado, "Apellidos", apellido);

                string resultado = negocioAutor.Modificar(modificado);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Autor modificado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Seleccione un autor de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("¿Está seguro de eliminar este autor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(txtId.Text);
                    string resultado = negocioAutor.Eliminar(id);

                    if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Autor eliminado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dgvAutores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvAutores.Rows[e.RowIndex];
                txtId.Text = fila.Cells["IdAutor"].Value?.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();

                TextBox tbApe = ObtenerCajaTexto("txtApellido", "txtApellidos");
                if (tbApe != null && fila.Cells["Apellido"] != null)
                {
                    tbApe.Text = fila.Cells["Apellido"].Value?.ToString();
                }

                TextBox tbNac = ObtenerCajaTexto("txtNacionalidad", "txtPais");
                if (tbNac != null && fila.Cells["Nacionalidad"] != null)
                {
                    tbNac.Text = fila.Cells["Nacionalidad"].Value?.ToString();
                }
            }
        }
    }
}