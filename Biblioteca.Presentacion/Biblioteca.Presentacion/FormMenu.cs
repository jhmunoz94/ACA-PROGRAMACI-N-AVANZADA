using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Biblioteca.Presentacion
{
    public partial class FormMenu : Form
    {
        private Form formularioActivo = null;

        public FormMenu()
        {
            InitializeComponent();
            this.Load += FormMenu_Load;
        }

        private void FormMenu_Load(object sender, EventArgs e)
        {
            ConfigurarLogoLateral();
            VincularMenuRecursivo(this);
            MostrarPantallaInicio();
        }

        private void ConfigurarLogoLateral()
        {
            try
            {
                string rutaPng = Path.Combine(Application.StartupPath, "inicio_banner.png");
                string rutaJpg = Path.Combine(Application.StartupPath, "inicio_banner.jpeg");
                Image imgLogo = null;

                if (File.Exists(rutaPng))
                {
                    imgLogo = Image.FromFile(rutaPng);
                }
                else if (File.Exists(rutaJpg))
                {
                    imgLogo = Image.FromFile(rutaJpg);
                }

                if (imgLogo != null && this.pictureBox1 != null)
                {
                    this.pictureBox1.Image = imgLogo;
                    this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    this.pictureBox1.BackColor = Color.Transparent;
                    this.pictureBox1.Size = new Size(110, 55);

                    if (this.pictureBox1.Parent != null)
                    {
                        this.pictureBox1.Left = Math.Max(0, (this.pictureBox1.Parent.Width - this.pictureBox1.Width) / 2);
                        this.pictureBox1.Top = 15;
                    }
                    this.pictureBox1.BringToFront();
                }
            }
            catch
            {
            }
        }

        private void VincularMenuRecursivo(Control contenedor)
        {
            foreach (Control c in contenedor.Controls)
            {
                if (c == this.pictureBox1) continue;

                if (c is Button || c is Label || c is PictureBox)
                {
                    c.Click -= ElementoMenu_Click;
                    c.Click += ElementoMenu_Click;
                }

                if (c.HasChildren)
                {
                    VincularMenuRecursivo(c);
                }
            }
        }

        private void ElementoMenu_Click(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl == null) return;

            string texto = (ctrl.Text ?? "").Trim().ToLower();
            string nombre = (ctrl.Name ?? "").Trim().ToLower();

            if (texto.Contains("inicio") || nombre.Contains("inicio"))
            {
                MostrarPantallaInicio();
            }
            else if (texto.Contains("libro") || nombre.Contains("libro"))
            {
                AbrirFormularioHijo(new FormLibros());
            }
            else if (texto.Contains("usuario") || nombre.Contains("usuario"))
            {
                AbrirFormularioHijo(new FormUsuarios());
            }
            else if (texto.Contains("autor") || nombre.Contains("autor"))
            {
                AbrirFormularioHijo(new FormAutores());
            }
            else if (texto.Contains("editorial") || nombre.Contains("editorial"))
            {
                AbrirFormularioHijo(new FormEditoriales());
            }
            else if (texto.Contains("préstamo") || texto.Contains("prestamo") || nombre.Contains("prestamo"))
            {
                AbrirFormularioHijo(new FormPrestamos());
            }
            // === AQUÍ SE AGREGA REPORTES ===
            else if (texto.Contains("reporte") || nombre.Contains("reporte"))
            {
                AbrirFormularioHijo(new FormReportes());
            }
            // ===============================
            else if (texto.Contains("salir") || nombre.Contains("salir"))
            {
                Application.Exit();
            }
        }

        private Control ObtenerPanelContenedor()
        {
            Control panelDestino = this.Controls["panelContenedor"]
                                   ?? this.Controls["panelEscritorio"]
                                   ?? this.Controls["panelCentral"];

            if (panelDestino == null)
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Panel pnl && pnl.Width > 400)
                    {
                        panelDestino = pnl;
                        break;
                    }
                }
            }
            return panelDestino;
        }

        private void AbrirFormularioHijo(Form formHijo)
        {
            Control panelDestino = ObtenerPanelContenedor();

            if (panelDestino != null)
            {
                if (formularioActivo != null)
                {
                    formularioActivo.Close();
                    formularioActivo.Dispose();
                    formularioActivo = null;
                }

                panelDestino.Controls.Clear();

                formularioActivo = formHijo;
                formHijo.TopLevel = false;
                formHijo.FormBorderStyle = FormBorderStyle.None;
                formHijo.Dock = DockStyle.Fill;

                panelDestino.Controls.Add(formHijo);
                panelDestino.Tag = formHijo;
                formHijo.Show();
                formHijo.BringToFront();
            }
            else
            {
                formHijo.StartPosition = FormStartPosition.CenterScreen;
                formHijo.Show();
            }
        }

        private void MostrarPantallaInicio()
        {
            Control panelDestino = ObtenerPanelContenedor();

            if (formularioActivo != null)
            {
                formularioActivo.Close();
                formularioActivo.Dispose();
                formularioActivo = null;
            }

            if (panelDestino != null)
            {
                panelDestino.Controls.Clear();

                Panel pnlBienvenida = new Panel();
                pnlBienvenida.Dock = DockStyle.Fill;
                pnlBienvenida.BackColor = Color.White;

                Panel pnlCentro = new Panel();
                pnlCentro.Width = 950;
                pnlCentro.Height = 600;
                pnlCentro.BackColor = Color.Transparent;

                PictureBox picIlustracion = new PictureBox();
                picIlustracion.Width = 620;
                picIlustracion.Height = 330;
                picIlustracion.SizeMode = PictureBoxSizeMode.Zoom;
                picIlustracion.BackColor = Color.Transparent;

                string rutaImagenPng = Path.Combine(Application.StartupPath, "inicio_banner.png");
                string rutaImagenJpg = Path.Combine(Application.StartupPath, "inicio_banner.jpeg");

                if (File.Exists(rutaImagenPng))
                {
                    picIlustracion.Image = Image.FromFile(rutaImagenPng);
                }
                else if (File.Exists(rutaImagenJpg))
                {
                    picIlustracion.Image = Image.FromFile(rutaImagenJpg);
                }

                Label lblTitulo = new Label();
                lblTitulo.Text = "SISTEMA DE GESTIÓN DE BIBLIOTECA";
                lblTitulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
                lblTitulo.ForeColor = Color.FromArgb(28, 30, 45);
                lblTitulo.AutoSize = false;
                lblTitulo.Width = 950;
                lblTitulo.Height = 40;
                lblTitulo.TextAlign = ContentAlignment.MiddleCenter;

                Label lblSubtitulo = new Label();
                lblSubtitulo.Text = "Selecciona una opción del menú lateral para administrar libros, usuarios y préstamos.";
                lblSubtitulo.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
                lblSubtitulo.ForeColor = Color.FromArgb(115, 120, 140);
                lblSubtitulo.AutoSize = false;
                lblSubtitulo.Width = 950;
                lblSubtitulo.Height = 28;
                lblSubtitulo.TextAlign = ContentAlignment.MiddleCenter;

                Label lblFecha = new Label();
                lblFecha.Text = DateTime.Now.ToLongDateString().ToUpper();
                lblFecha.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                lblFecha.ForeColor = Color.FromArgb(145, 150, 170);
                lblFecha.AutoSize = false;
                lblFecha.Width = 950;
                lblFecha.Height = 24;
                lblFecha.TextAlign = ContentAlignment.MiddleCenter;

                Label lblBienvenida = new Label();
                lblBienvenida.Text = "BIENVENIDOS!!!!";
                lblBienvenida.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
                lblBienvenida.ForeColor = Color.FromArgb(70, 80, 120);
                lblBienvenida.AutoSize = false;
                lblBienvenida.Width = 950;
                lblBienvenida.Height = 30;
                lblBienvenida.TextAlign = ContentAlignment.MiddleCenter;

                picIlustracion.Left = (pnlCentro.Width - picIlustracion.Width) / 2;
                picIlustracion.Top = 10;

                lblTitulo.Left = 0;
                lblTitulo.Top = picIlustracion.Bottom + 16;

                lblSubtitulo.Left = 0;
                lblSubtitulo.Top = lblTitulo.Bottom + 6;

                lblFecha.Left = 0;
                lblFecha.Top = lblSubtitulo.Bottom + 10;

                lblBienvenida.Left = 0;
                lblBienvenida.Top = lblFecha.Bottom + 8;

                pnlCentro.Controls.Add(picIlustracion);
                pnlCentro.Controls.Add(lblTitulo);
                pnlCentro.Controls.Add(lblSubtitulo);
                pnlCentro.Controls.Add(lblFecha);
                pnlCentro.Controls.Add(lblBienvenida);

                pnlCentro.Left = Math.Max(0, (panelDestino.Width - pnlCentro.Width) / 2);
                pnlCentro.Top = Math.Max(0, (panelDestino.Height - pnlCentro.Height) / 2);

                pnlBienvenida.Controls.Add(pnlCentro);
                panelDestino.Controls.Add(pnlBienvenida);
            }
        }

        // ========================================================
        // ENLACES DIRECTOS A LOS MÉTODOS DEL DISEÑADOR
        // ========================================================

        private void btnInicio_Click(object sender, EventArgs e) { MostrarPantallaInicio(); }
        private void btnLibros_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormLibros()); }
        private void btnLibros_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormLibros()); }
        private void btnUsuarios_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormUsuarios()); }
        private void btnUsuarios_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormUsuarios()); }
        private void btnAutores_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormAutores()); }
        private void btnAutores_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormAutores()); }
        private void btnEditoriales_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormEditoriales()); }
        private void btnEditoriales_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormEditoriales()); }
        private void btnPrestamos_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormPrestamos()); }
        private void btnPrestamos_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormPrestamos()); }
        private void btnSalir_Click(object sender, EventArgs e) { Application.Exit(); }
        private void btnSalir_Click_1(object sender, EventArgs e) { Application.Exit(); }

        private void librosToolStripMenuItem_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormLibros()); }
        private void usuariosToolStripMenuItem_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormUsuarios()); }
        private void autoresToolStripMenuItem_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormAutores()); }
        private void editorialesToolStripMenuItem_Click_1(object sender, EventArgs e) { AbrirFormularioHijo(new FormEditoriales()); }
        private void préstamosYDevolucionesToolStripMenuItem_Click(object sender, EventArgs e) { AbrirFormularioHijo(new FormPrestamos()); }
        private void cerrarSistemaToolStripMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }
        private void pictureBox1_Click(object sender, EventArgs e) { }

        private void btnReportes_Click(object sender, EventArgs e)
        {
            AbrirFormularioHijo(new FormReportes());
        }

    } // Esta llave cierra la clase FormMenu
}     // Esta llave cierra el namespace