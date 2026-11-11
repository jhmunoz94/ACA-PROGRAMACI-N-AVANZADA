using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    public partial class FormLibros : Form
    {
        private LibroNegocio negocioLibro = new LibroNegocio();
        private AutorNegocio negocioAutor = new AutorNegocio();
        private EditorialNegocio negocioEditorial = new EditorialNegocio();

        public FormLibros()
        {
            InitializeComponent();
            this.Load += FormLibros_Load;
            this.Shown += (s, ev) => EstilizarVista("GESTIÓN DE LIBROS", dgvLibros);

            btnGuardar.Click += btnGuardar_Click;
            btnModificar.Click += btnModificar_Click;
            btnEliminar.Click += btnEliminar_Click;
            btnLimpiar.Click += btnLimpiar_Click;

            if (dgvLibros != null)
            {
                dgvLibros.CellClick += dgvLibros_CellClick;
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

        private void FormLibros_Load(object sender, EventArgs e)
        {
            if (dgvLibros != null)
            {
                dgvLibros.RowHeadersVisible = false;
                dgvLibros.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvLibros.MultiSelect = false;
                dgvLibros.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }

            CargarCombos();
            CargarGrilla();
            LimpiarCampos();
        }

        private void CargarCombos()
        {
            try
            {
                if (cboAutor != null)
                {
                    cboAutor.DataSource = negocioAutor.Listar();
                    cboAutor.DisplayMember = "Nombre";
                    cboAutor.ValueMember = "IdAutor";
                }

                if (cboEditorial != null)
                {
                    cboEditorial.DataSource = negocioEditorial.Listar();
                    cboEditorial.DisplayMember = "Nombre";
                    cboEditorial.ValueMember = "IdEditorial";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar desplegables: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarGrilla()
        {
            try
            {
                if (dgvLibros == null) return;
                dgvLibros.DataSource = negocioLibro.Listar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar grilla: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private string ObtenerValorIsbn()
        {
            TextBox txtIsbn = ObtenerCajaTexto("txtCodigoISBN", "txtCodigoIsbn", "txtIsbn", "txtISBN");
            return txtIsbn != null ? txtIsbn.Text.Trim() : "";
        }

        private void AsignarIsbn(Libro libro, string valorIsbn)
        {
            var propCodigoIsbn = typeof(Libro).GetProperty("CodigoISBN");
            if (propCodigoIsbn != null)
            {
                propCodigoIsbn.SetValue(libro, valorIsbn);
            }

            var propIsbn = typeof(Libro).GetProperty("ISBN");
            if (propIsbn != null)
            {
                propIsbn.SetValue(libro, valorIsbn);
            }
        }

        private void LimpiarCampos()
        {
            txtId.Clear();
            txtTitulo.Clear();
            txtStock.Clear();

            TextBox tbIsbn = ObtenerCajaTexto("txtCodigoISBN", "txtCodigoIsbn", "txtIsbn", "txtISBN");
            if (tbIsbn != null) tbIsbn.Clear();

            TextBox tbAnio = ObtenerCajaTexto("txtAnio", "txtAnioPublicacion");
            if (tbAnio != null) tbAnio.Clear();

            if (cboAutor != null) cboAutor.SelectedIndex = -1;
            if (cboEditorial != null) cboEditorial.SelectedIndex = -1;

            if (dgvLibros != null) dgvLibros.ClearSelection();
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                string isbn = ObtenerValorIsbn();

                if (string.IsNullOrWhiteSpace(isbn))
                {
                    MessageBox.Show("El código ISBN es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTitulo.Text))
                {
                    MessageBox.Show("Ingrese el título del libro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtTitulo.Focus();
                    return;
                }

                if (cboAutor?.SelectedValue == null || Convert.ToInt32(cboAutor.SelectedValue) <= 0)
                {
                    MessageBox.Show("Seleccione un autor para el libro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (cboEditorial?.SelectedValue == null || Convert.ToInt32(cboEditorial.SelectedValue) <= 0)
                {
                    MessageBox.Show("Seleccione una editorial para el libro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TextBox tbAnio = ObtenerCajaTexto("txtAnio", "txtAnioPublicacion");
                int anioVal = (tbAnio != null && int.TryParse(tbAnio.Text, out int a)) ? a : DateTime.Now.Year;

                Libro nuevoLibro = new Libro
                {
                    Titulo = txtTitulo.Text.Trim(),
                    IdAutor = Convert.ToInt32(cboAutor.SelectedValue),
                    IdEditorial = Convert.ToInt32(cboEditorial.SelectedValue),
                    AnioPublicacion = anioVal,
                    Stock = int.TryParse(txtStock.Text, out int s) ? s : 1
                };

                AsignarIsbn(nuevoLibro, isbn);

                string resultado = negocioLibro.Registrar(nuevoLibro);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Libro registrado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Seleccione un libro de la tabla para modificar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string isbn = ObtenerValorIsbn();
                if (string.IsNullOrWhiteSpace(isbn))
                {
                    MessageBox.Show("El código ISBN es obligatorio.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTitulo.Text))
                {
                    MessageBox.Show("Ingrese el título del libro.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TextBox tbAnio = ObtenerCajaTexto("txtAnio", "txtAnioPublicacion");
                int anioVal = (tbAnio != null && int.TryParse(tbAnio.Text, out int a)) ? a : DateTime.Now.Year;

                Libro libroModificado = new Libro
                {
                    IdLibro = Convert.ToInt32(txtId.Text),
                    Titulo = txtTitulo.Text.Trim(),
                    IdAutor = cboAutor?.SelectedValue != null ? Convert.ToInt32(cboAutor.SelectedValue) : 0,
                    IdEditorial = cboEditorial?.SelectedValue != null ? Convert.ToInt32(cboEditorial.SelectedValue) : 0,
                    AnioPublicacion = anioVal,
                    Stock = int.TryParse(txtStock.Text, out int s) ? s : 1
                };

                AsignarIsbn(libroModificado, isbn);

                string resultado = negocioLibro.Modificar(libroModificado);

                if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Libro modificado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                    MessageBox.Show("Seleccione un libro de la tabla para eliminar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("¿Está seguro de eliminar este libro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int id = Convert.ToInt32(txtId.Text);
                    string resultado = negocioLibro.Eliminar(id);

                    if (resultado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Libro eliminado con éxito.", "Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void dgvLibros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvLibros.Rows[e.RowIndex];
                txtId.Text = fila.Cells["IdLibro"].Value?.ToString();
                txtTitulo.Text = fila.Cells["Titulo"].Value?.ToString();

                if (fila.Cells["Stock"] != null)
                    txtStock.Text = fila.Cells["Stock"].Value?.ToString();

                TextBox tbAnio = ObtenerCajaTexto("txtAnio", "txtAnioPublicacion");
                if (tbAnio != null && fila.Cells["AnioPublicacion"] != null)
                    tbAnio.Text = fila.Cells["AnioPublicacion"].Value?.ToString();

                TextBox tbIsbn = ObtenerCajaTexto("txtCodigoISBN", "txtCodigoIsbn", "txtIsbn", "txtISBN");
                if (tbIsbn != null)
                {
                    if (fila.Cells["CodigoISBN"] != null && fila.Cells["CodigoISBN"].Value != null)
                        tbIsbn.Text = fila.Cells["CodigoISBN"].Value.ToString();
                    else if (fila.Cells["ISBN"] != null && fila.Cells["ISBN"].Value != null)
                        tbIsbn.Text = fila.Cells["ISBN"].Value.ToString();
                }

                if (cboAutor != null && fila.Cells["IdAutor"] != null)
                    cboAutor.SelectedValue = fila.Cells["IdAutor"].Value;

                if (cboEditorial != null && fila.Cells["IdEditorial"] != null)
                    cboEditorial.SelectedValue = fila.Cells["IdEditorial"].Value;
            }
        }
    }
}