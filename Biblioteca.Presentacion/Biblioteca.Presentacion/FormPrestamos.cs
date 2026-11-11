using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    public partial class FormPrestamos : Form
    {
        private PrestamoNegocio negocioPrestamo = new PrestamoNegocio();
        private UsuarioNegocio negocioUsuario = new UsuarioNegocio();
        private LibroNegocio negocioLibro = new LibroNegocio();

        private TextBox txtId = new TextBox();
        private TextBox txtEstado = new TextBox();

        public FormPrestamos()
        {
            InitializeComponent();

            this.Load += FormPrestamos_Load;
            this.Shown += (s, ev) =>
            {
                if (dgvPrestamos != null)
                {
                    dgvPrestamos.Dock = DockStyle.None;
                    dgvPrestamos.Left = 0;
                    dgvPrestamos.Width = this.ClientSize.Width;
                    dgvPrestamos.Height = 260;
                    dgvPrestamos.Top = this.ClientSize.Height - 260;
                    dgvPrestamos.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    dgvPrestamos.BringToFront();
                }

                CentrarElementos();
            };

            btnPrestar.Click += btnPrestar_Click;
            btnDevolver.Click += btnDevolver_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            if (dgvPrestamos != null)
            {
                dgvPrestamos.CellClick += dgvPrestamos_CellClick;
            }
        }

        private void CentrarElementos()
        {
            // 1. Centrar el panel de datos
            if (panelCentral != null)
            {
                panelCentral.Left = Math.Max(0, (this.ClientSize.Width - panelCentral.Width) / 2);
                panelCentral.Top = 75;
                panelCentral.Anchor = AnchorStyles.Top;
            }

            // 2. Centrar perfectamente el título en la barra superior negra
            foreach (Control c in this.Controls)
            {
                if (c is Panel pnl && (pnl.Dock == DockStyle.Top || pnl.Top == 0 || pnl.BackColor == Color.Black))
                {
                    pnl.Dock = DockStyle.Top;
                    pnl.Height = 50;
                    pnl.BackColor = Color.Black;

                    foreach (Control sub in pnl.Controls)
                    {
                        if (sub is Label lbl)
                        {
                            lbl.AutoSize = false;
                            lbl.Dock = DockStyle.Fill;
                            lbl.TextAlign = ContentAlignment.MiddleCenter;
                            lbl.ForeColor = Color.White;
                            lbl.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
                        }
                    }
                }
                else if (c is Label lbl && (lbl.Text.ToUpper().Contains("PRÉSTAMO") || lbl.Text.ToUpper().Contains("PRESTAMO")))
                {
                    lbl.AutoSize = false;
                    lbl.Width = this.ClientSize.Width;
                    lbl.Height = 45;
                    lbl.Left = 0;
                    lbl.Top = 5;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.ForeColor = Color.White;
                    lbl.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
                    lbl.BringToFront();
                }
            }
        }

        private void FormPrestamos_Load(object sender, EventArgs e)
        {
            if (dgvPrestamos != null)
            {
                dgvPrestamos.RowHeadersVisible = false;
                dgvPrestamos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvPrestamos.MultiSelect = false;
                dgvPrestamos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            CargarCombos();
            CargarGrilla();
            LimpiarCampos();
        }

        private void CargarCombos()
        {
            try
            {
                if (cboUsuario != null)
                {
                    cboUsuario.DataSource = negocioUsuario.Listar();
                    cboUsuario.DisplayMember = "Nombre";
                    cboUsuario.ValueMember = "IdUsuario";
                }

                if (cboLibro != null)
                {
                    cboLibro.DataSource = negocioLibro.Listar();
                    cboLibro.DisplayMember = "Titulo";
                    cboLibro.ValueMember = "IdLibro";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar listados: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrilla()
        {
            try
            {
                if (dgvPrestamos == null) return;

                dgvPrestamos.DataSource = negocioPrestamo.Listar();

                if (dgvPrestamos.Columns["IdUsuario"] != null) dgvPrestamos.Columns["IdUsuario"].Visible = false;
                if (dgvPrestamos.Columns["IdLibro"] != null) dgvPrestamos.Columns["IdLibro"].Visible = false;
                if (dgvPrestamos.Columns["IdPrestamo"] != null) dgvPrestamos.Columns["IdPrestamo"].HeaderText = "N° Préstamo";
                if (dgvPrestamos.Columns["FechaPrestamo"] != null) dgvPrestamos.Columns["FechaPrestamo"].HeaderText = "Fecha Préstamo";
                if (dgvPrestamos.Columns["FechaDevolucion"] != null) dgvPrestamos.Columns["FechaDevolucion"].HeaderText = "Fecha Devolución";
                if (dgvPrestamos.Columns["Estado"] != null) dgvPrestamos.Columns["Estado"].HeaderText = "Estado";
                if (dgvPrestamos.Columns["NombreUsuario"] != null) dgvPrestamos.Columns["NombreUsuario"].HeaderText = "Usuario";
                if (dgvPrestamos.Columns["TituloLibro"] != null) dgvPrestamos.Columns["TituloLibro"].HeaderText = "Libro";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar grilla: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtEstado.Clear();

            if (cboUsuario != null) cboUsuario.SelectedIndex = -1;
            if (cboLibro != null) cboLibro.SelectedIndex = -1;

            if (dtpFechaPrestamo != null) dtpFechaPrestamo.Value = DateTime.Now;
            if (dtpFechaDevolucion != null)
            {
                dtpFechaDevolucion.Value = DateTime.Now;
                dtpFechaDevolucion.Checked = false;
            }

            if (dgvPrestamos != null) dgvPrestamos.ClearSelection();
        }

        private void btnPrestar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboUsuario?.SelectedValue == null || cboLibro?.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un usuario y un libro válidos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Prestamo nuevoPrestamo = new Prestamo
                {
                    IdUsuario = Convert.ToInt32(cboUsuario.SelectedValue),
                    IdLibro = Convert.ToInt32(cboLibro.SelectedValue),
                    FechaPrestamo = dtpFechaPrestamo != null ? dtpFechaPrestamo.Value : DateTime.Now,
                    FechaDevolucion = (dtpFechaDevolucion != null && dtpFechaDevolucion.Checked) ? (DateTime?)dtpFechaDevolucion.Value : null,
                    Estado = "Prestado"
                };

                bool resultado = negocioPrestamo.Registrar(nuevoPrestamo);

                if (resultado)
                {
                    MessageBox.Show("Préstamo registrado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrilla();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("No se pudo registrar el préstamo. El libro seleccionado no cuenta con ejemplares en stock.", "Sin Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDevolver_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtId.Text))
                {
                    MessageBox.Show("Seleccione un préstamo de la tabla para realizar la devolución.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (txtEstado.Text == "Devuelto")
                {
                    MessageBox.Show("Este libro ya fue devuelto con anterioridad.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int idPrestamo = Convert.ToInt32(txtId.Text);
                bool resultado = negocioPrestamo.Devolver(idPrestamo, DateTime.Now);

                if (resultado)
                {
                    MessageBox.Show("Libro devuelto con éxito. Stock reincorporado.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarGrilla();
                    LimpiarCampos();
                }
                else
                {
                    MessageBox.Show("No se pudo procesar la devolución.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al devolver: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarCampos();
        }

        private void dgvPrestamos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvPrestamos.Rows[e.RowIndex];
                txtId.Text = fila.Cells["IdPrestamo"].Value?.ToString();
                txtEstado.Text = fila.Cells["Estado"].Value?.ToString();

                if (fila.Cells["NombreUsuario"].Value != null && cboUsuario != null)
                {
                    cboUsuario.Text = fila.Cells["NombreUsuario"].Value.ToString();
                }

                if (fila.Cells["TituloLibro"].Value != null && cboLibro != null)
                {
                    cboLibro.Text = fila.Cells["TituloLibro"].Value.ToString();
                }

                if (dtpFechaPrestamo != null && fila.Cells["FechaPrestamo"].Value != null && DateTime.TryParse(fila.Cells["FechaPrestamo"].Value.ToString(), out DateTime fp))
                {
                    dtpFechaPrestamo.Value = fp;
                }

                if (dtpFechaDevolucion != null)
                {
                    if (fila.Cells["FechaDevolucion"].Value != null && DateTime.TryParse(fila.Cells["FechaDevolucion"].Value.ToString(), out DateTime fd))
                    {
                        dtpFechaDevolucion.Checked = true;
                        dtpFechaDevolucion.Value = fd;
                    }
                    else
                    {
                        dtpFechaDevolucion.Checked = false;
                    }
                }
            }
        }

        // ========================================================
        // MÉTODOS REQUERIDOS POR EL DISEÑADOR VISUAL
        // ========================================================

        private void dgvPrestamos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void panelCentral_Paint(object sender, PaintEventArgs e)
        {
        }

        private void FormPrestamos_Load_1(object sender, EventArgs e)
        {
        }
    }
}