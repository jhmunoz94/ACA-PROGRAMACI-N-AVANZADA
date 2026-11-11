using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    public partial class FormEditoriales : Form
    {
        private EditorialNegocio negocioEditorial = new EditorialNegocio();

        public FormEditoriales()
        {
            InitializeComponent();
            this.Load += FormEditoriales_Load;
            this.Shown += (s, ev) => EstilizarVista("GESTIÓN DE EDITORIALES", dgvEditoriales);

            btnGuardar.Click += btnGuardar_Click;
            btnModificar.Click += btnModificar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            if (dgvEditoriales != null)
            {
                dgvEditoriales.CellClick += dgvEditoriales_CellClick;
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

        private void FormEditoriales_Load(object sender, EventArgs e)
        {
            if (dgvEditoriales != null)
            {
                dgvEditoriales.RowHeadersVisible = false;
                dgvEditoriales.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvEditoriales.MultiSelect = false;
                dgvEditoriales.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            CargarGrilla();
            LimpiarCampos();
        }

        private void CargarGrilla()
        {
            try
            {
                if (dgvEditoriales == null) return;
                dgvEditoriales.DataSource = negocioEditorial.Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar editoriales: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtNombre.Clear();

            Control txtP = this.Controls.Find("txtPais", true).FirstOrDefault();
            if (txtP is TextBox tbPais) tbPais.Clear();

            if (dgvEditoriales != null) dgvEditoriales.ClearSelection();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtNombre.Text))
                {
                    MessageBox.Show("Ingrese el nombre de la editorial.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Control txtP = this.Controls.Find("txtPais", true).FirstOrDefault();

                Editorial nueva = new Editorial
                {
                    Nombre = txtNombre.Text.Trim(),
                    Pais = txtP?.Text.Trim()
                };

                string resultado = negocioEditorial.Registrar(nueva);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Editorial registrada con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Seleccione una editorial de la tabla para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Control txtP = this.Controls.Find("txtPais", true).FirstOrDefault();

                Editorial modificada = new Editorial
                {
                    IdEditorial = Convert.ToInt32(txtId.Text),
                    Nombre = txtNombre.Text.Trim(),
                    Pais = txtP?.Text.Trim()
                };

                string resultado = negocioEditorial.Modificar(modificada);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Editorial modificada con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Seleccione una editorial de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("¿Está seguro de eliminar esta editorial?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(txtId.Text);
                    string resultado = negocioEditorial.Eliminar(id);

                    if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Editorial eliminada con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dgvEditoriales_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvEditoriales.Rows[e.RowIndex];
                txtId.Text = fila.Cells["IdEditorial"].Value?.ToString();
                txtNombre.Text = fila.Cells["Nombre"].Value?.ToString();

                Control txtP = this.Controls.Find("txtPais", true).FirstOrDefault();
                if (txtP is TextBox tbPais && fila.Cells["Pais"] != null)
                    tbPais.Text = fila.Cells["Pais"].Value?.ToString();
            }
        }
    }
}