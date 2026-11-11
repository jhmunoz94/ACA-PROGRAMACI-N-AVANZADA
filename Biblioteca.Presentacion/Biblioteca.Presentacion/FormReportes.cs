using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Biblioteca.Entidades;
using Biblioteca.Negocio;

namespace Biblioteca.Presentacion
{
    public partial class FormReportes : Form
    {
        private PrestamoNegocio negocioPrestamo = new PrestamoNegocio();
        private LibroNegocio negocioLibro = new LibroNegocio();
        private UsuarioNegocio negocioUsuario = new UsuarioNegocio();

        private DataGridView dgvReporte;
        private ComboBox cboFiltro;
        private Label lblTotal;

        public FormReportes()
        {
            InitializeComponent();
            ConfigurarInterfaz();
            this.Load += FormReportes_Load;
        }

        private void ConfigurarInterfaz()
        {
            this.BackColor = Color.White;
            this.Text = "Reportes y Consultas";

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.Black
            };
            Label lblTitulo = new Label
            {
                Text = "MÓDULO DE CONSULTAS Y REPORTES",
                Font = new Font("Segoe UI", 15f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlHeader.Controls.Add(lblTitulo);
            this.Controls.Add(pnlHeader);

            Panel pnlFiltros = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(245, 246, 250)
            };

            Label lblSeleccione = new Label
            {
                Text = "Seleccionar Reporte:",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Left = 20,
                Top = 20,
                AutoSize = true
            };

            cboFiltro = new ComboBox
            {
                Left = 180,
                Top = 16,
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10f)
            };
            cboFiltro.Items.Add("1. Todos los Préstamos (General)");
            cboFiltro.Items.Add("2. Préstamos Activos (Libros Pendientes)");
            cboFiltro.Items.Add("3. Préstamos Finalizados (Devueltos)");
            cboFiltro.Items.Add("4. Catálogo de Libros y Existencias");
            cboFiltro.Items.Add("5. Listado General de Usuarios");
            cboFiltro.SelectedIndex = 0;
            cboFiltro.SelectedIndexChanged += (s, e) => CargarReporte();

            lblTotal = new Label
            {
                Text = "Total de registros: 0",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(28, 30, 45),
                Left = 530,
                Top = 20,
                AutoSize = true
            };

            pnlFiltros.Controls.Add(lblSeleccione);
            pnlFiltros.Controls.Add(cboFiltro);
            pnlFiltros.Controls.Add(lblTotal);
            this.Controls.Add(pnlFiltros);

            dgvReporte = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false
            };
            dgvReporte.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 30, 45);
            dgvReporte.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReporte.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgvReporte.ColumnHeadersHeight = 35;
            dgvReporte.RowTemplate.Height = 28;

            this.Controls.Add(dgvReporte);
            dgvReporte.BringToFront();
        }

        private void FormReportes_Load(object sender, EventArgs e)
        {
            CargarReporte();
        }

        private void CargarReporte()
        {
            try
            {
                dgvReporte.DataSource = null;

                if (cboFiltro.SelectedIndex == 0) // Todos los préstamos
                {
                    List<Prestamo> lista = negocioPrestamo.Listar() ?? new List<Prestamo>();
                    dgvReporte.DataSource = lista;
                    lblTotal.Text = $"Total de registros: {lista.Count}";
                }
                else if (cboFiltro.SelectedIndex == 1) // Préstamos Activos
                {
                    List<Prestamo> lista = negocioPrestamo.Listar() ?? new List<Prestamo>();
                    var activos = lista.Where(p => (p.Estado ?? "").Equals("Prestado", StringComparison.OrdinalIgnoreCase)).ToList();
                    dgvReporte.DataSource = activos;
                    lblTotal.Text = $"Total de registros: {activos.Count}";
                }
                else if (cboFiltro.SelectedIndex == 2) // Préstamos Devueltos
                {
                    List<Prestamo> lista = negocioPrestamo.Listar() ?? new List<Prestamo>();
                    var devueltos = lista.Where(p => (p.Estado ?? "").Equals("Devuelto", StringComparison.OrdinalIgnoreCase)).ToList();
                    dgvReporte.DataSource = devueltos;
                    lblTotal.Text = $"Total de registros: {devueltos.Count}";
                }
                else if (cboFiltro.SelectedIndex == 3) // Catálogo de Libros
                {
                    List<Libro> libros = negocioLibro.Listar() ?? new List<Libro>();
                    dgvReporte.DataSource = libros;
                    lblTotal.Text = $"Total de registros: {libros.Count}";
                }
                else if (cboFiltro.SelectedIndex == 4) // Usuarios
                {
                    List<Usuario> usuarios = negocioUsuario.Listar() ?? new List<Usuario>();
                    dgvReporte.DataSource = usuarios;
                    lblTotal.Text = $"Total de registros: {usuarios.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar reporte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}