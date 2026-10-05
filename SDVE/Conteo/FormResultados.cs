using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SDVE
{
    // Panel normal pero sin parpadeo al dibujar las gráficas
    public class PanelSinParpadeo : Panel
    {
        public PanelSinParpadeo()
        {
            this.DoubleBuffered = true;
        }
    }

    // Ventana del módulo 3: conteo y resultados.
    public class FormResultados : Form
    {
        // Datos
        private List<Alumno> alumnos;
        private List<Voto> votos;

        // Últimos resultados calculados
        private List<ResultadoCandidato> candidatos;
        private List<ResultadoCandidato> totalesCandidatos;
        private List<ResultadoParticipacion> participacion;
        private string convocatoriaActual;
        private string agruparActual;

        // Controles
        private ComboBox cmbConvocatoria;
        private ComboBox cmbAgrupar;
        private Button btnCalcular;
        private Button btnExportarCsv;
        private Button btnExportarXml;
        private Label lblResumen;
        private TabControl tabs;
        private DataGridView gridCandidatos;
        private DataGridView gridParticipacion;
        private PanelSinParpadeo panelGraficaVotos;
        private PanelSinParpadeo panelGraficaParticipacion;

        public FormResultados()
        {
            alumnos = DatosVotacion.ObtenerAlumnos();
            votos = DatosVotacion.ObtenerVotos();

            CrearControles();
            Calcular();
        }

        // ---------------------------------------------------------
        // Crear la pantalla
        // ---------------------------------------------------------
        private void CrearControles()
        {
            this.Text = "SDVE - Resultados de la votación";
            this.Size = new Size(1020, 650);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ----- Panel de arriba con los filtros y botones -----
            Panel panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 85;

            Label lblConv = new Label();
            lblConv.Text = "Convocatoria:";
            lblConv.Location = new Point(10, 17);
            lblConv.AutoSize = true;

            cmbConvocatoria = new ComboBox();
            cmbConvocatoria.Location = new Point(130, 13);
            cmbConvocatoria.Width = 200;
            cmbConvocatoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConvocatoria.Items.AddRange(ConteoService.Convocatorias);
            cmbConvocatoria.SelectedIndex = 0;

            Label lblAgrupar = new Label();
            lblAgrupar.Text = "Agrupar por:";
            lblAgrupar.Location = new Point(360, 17);
            lblAgrupar.AutoSize = true;

            cmbAgrupar = new ComboBox();
            cmbAgrupar.Location = new Point(460, 13);
            cmbAgrupar.Width = 120;
            cmbAgrupar.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAgrupar.Items.AddRange(ConteoService.FormasAgrupar);
            cmbAgrupar.SelectedIndex = 0;

            btnCalcular = new Button();
            btnCalcular.Text = "Calcular";
            btnCalcular.Location = new Point(610, 10);
            btnCalcular.Size = new Size(90, 30);
            btnCalcular.Click += btnCalcular_Click;

            btnExportarCsv = new Button();
            btnExportarCsv.Text = "Exportar CSV";
            btnExportarCsv.Location = new Point(710, 10);
            btnExportarCsv.Size = new Size(110, 30);
            btnExportarCsv.Click += btnExportarCsv_Click;

            btnExportarXml = new Button();
            btnExportarXml.Text = "Exportar XML";
            btnExportarXml.Location = new Point(830, 10);
            btnExportarXml.Size = new Size(110, 30);
            btnExportarXml.Click += btnExportarXml_Click;

            lblResumen = new Label();
            lblResumen.Location = new Point(10, 55);
            lblResumen.AutoSize = true;
            lblResumen.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            panelTop.Controls.Add(lblConv);
            panelTop.Controls.Add(cmbConvocatoria);
            panelTop.Controls.Add(lblAgrupar);
            panelTop.Controls.Add(cmbAgrupar);
            panelTop.Controls.Add(btnCalcular);
            panelTop.Controls.Add(btnExportarCsv);
            panelTop.Controls.Add(btnExportarXml);
            panelTop.Controls.Add(lblResumen);

            // ----- Pestañas -----
            tabs = new TabControl();
            tabs.Dock = DockStyle.Fill;

            // Pestaña 1: votos por candidato
            TabPage tabCandidatos = new TabPage("Votos por candidato");
            gridCandidatos = new DataGridView();
            ConfigurarGrid(gridCandidatos);
            gridCandidatos.Dock = DockStyle.Fill;
            tabCandidatos.Controls.Add(gridCandidatos);

            // Pestaña 2: participación y abstencionismo (tabla arriba, gráfica abajo)
            TabPage tabParticipacion = new TabPage("Participación y abstencionismo");
            gridParticipacion = new DataGridView();
            ConfigurarGrid(gridParticipacion);
            gridParticipacion.Dock = DockStyle.Top;
            gridParticipacion.Height = 200;

            panelGraficaParticipacion = new PanelSinParpadeo();
            panelGraficaParticipacion.Dock = DockStyle.Fill;
            panelGraficaParticipacion.Paint += PintarGraficaParticipacion;
            panelGraficaParticipacion.Resize += delegate { panelGraficaParticipacion.Invalidate(); };

            tabParticipacion.Controls.Add(gridParticipacion);
            tabParticipacion.Controls.Add(panelGraficaParticipacion);
            panelGraficaParticipacion.BringToFront();   // para que ocupe el espacio que sobra

            // Pestaña 3: gráfica de votos
            TabPage tabGrafica = new TabPage("Gráfica de votos");
            panelGraficaVotos = new PanelSinParpadeo();
            panelGraficaVotos.Dock = DockStyle.Fill;
            panelGraficaVotos.Paint += PintarGraficaVotos;
            panelGraficaVotos.Resize += delegate { panelGraficaVotos.Invalidate(); };
            tabGrafica.Controls.Add(panelGraficaVotos);

            tabs.TabPages.Add(tabCandidatos);
            tabs.TabPages.Add(tabParticipacion);
            tabs.TabPages.Add(tabGrafica);

            this.Controls.Add(tabs);
            this.Controls.Add(panelTop);
            tabs.BringToFront();
        }

        private void ConfigurarGrid(DataGridView grid)
        {
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.BackgroundColor = Color.White;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // ---------------------------------------------------------
        // Calcular y mostrar resultados
        // ---------------------------------------------------------
        private void btnCalcular_Click(object sender, EventArgs e)
        {
            Calcular();
        }

        private void Calcular()
        {
            if (alumnos.Count == 0)
            {
                MessageBox.Show("No hay alumnos en el padrón, no se puede calcular.");
                return;
            }

            convocatoriaActual = cmbConvocatoria.SelectedItem.ToString();
            agruparActual = cmbAgrupar.SelectedItem.ToString();

            candidatos = ConteoService.ContarCandidatos(votos, alumnos, convocatoriaActual, agruparActual);
            totalesCandidatos = ConteoService.ContarCandidatos(votos, alumnos, convocatoriaActual, "General");
            participacion = ConteoService.CalcularParticipacion(votos, alumnos, convocatoriaActual, agruparActual);

            // Tablas
            gridCandidatos.DataSource = null;
            gridCandidatos.DataSource = candidatos;
            gridCandidatos.Columns["Agrupacion"].HeaderText = "Agrupación";
            gridCandidatos.Columns["Porcentaje"].HeaderText = "Porcentaje (%)";

            gridParticipacion.DataSource = null;
            gridParticipacion.DataSource = participacion;
            gridParticipacion.Columns["Agrupacion"].HeaderText = "Agrupación";
            gridParticipacion.Columns["Padron"].HeaderText = "Padrón";
            gridParticipacion.Columns["Abstenciones"].HeaderText = "Abstenciones";
            gridParticipacion.Columns["PorcentajeParticipacion"].HeaderText = "Participación (%)";
            gridParticipacion.Columns["PorcentajeAbstencion"].HeaderText = "Abstencionismo (%)";

            // Resumen de arriba (siempre con el total general)
            ResultadoParticipacion general = ConteoService
                .CalcularParticipacion(votos, alumnos, convocatoriaActual, "General")[0];
            int totalVotos = totalesCandidatos.Sum(c => c.Votos);

            lblResumen.Text = "Total de votos: " + totalVotos +
                              "   |   Participación: " + general.PorcentajeParticipacion + "%" +
                              "   |   Abstencionismo: " + general.PorcentajeAbstencion + "%";

            // Redibujar gráficas
            panelGraficaVotos.Invalidate();
            panelGraficaParticipacion.Invalidate();
        }

        // ---------------------------------------------------------
        // Gráficas (barras dibujadas a mano con GDI+)
        // ---------------------------------------------------------
        private void PintarGraficaVotos(object sender, PaintEventArgs e)
        {
            List<string> etiquetas = new List<string>();
            List<double> valores = new List<double>();

            if (totalesCandidatos != null)
            {
                // Solo los 10 con más votos para que quepan
                foreach (ResultadoCandidato c in totalesCandidatos.Take(10))
                {
                    etiquetas.Add(c.Candidato);
                    valores.Add(c.Votos);
                }
            }

            string titulo = "Votos por candidato - " + convocatoriaActual + " (máx. 10)";
            DibujarBarras(e.Graphics, panelGraficaVotos.ClientRectangle, titulo, etiquetas, valores, "");
        }

        private void PintarGraficaParticipacion(object sender, PaintEventArgs e)
        {
            List<string> etiquetas = new List<string>();
            List<double> valores = new List<double>();

            if (participacion != null)
            {
                foreach (ResultadoParticipacion p in participacion)
                {
                    if (p.Agrupacion == "TOTAL") continue;
                    etiquetas.Add(p.Agrupacion);
                    valores.Add(p.PorcentajeParticipacion);
                }
            }

            string titulo = "% de participación - " + convocatoriaActual;
            DibujarBarras(e.Graphics, panelGraficaParticipacion.ClientRectangle, titulo, etiquetas, valores, "%");
        }

        private void DibujarBarras(Graphics g, Rectangle area, string titulo,
                                   List<string> etiquetas, List<double> valores, string sufijo)
        {
            g.Clear(Color.White);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Font fuenteTitulo = new Font("Segoe UI", 11, FontStyle.Bold);
            Font fuenteTexto = new Font("Segoe UI", 8);
            g.DrawString(titulo, fuenteTitulo, Brushes.Black, 10, 8);

            if (valores.Count == 0)
            {
                g.DrawString("Sin datos para mostrar", fuenteTexto, Brushes.Gray, 10, 40);
                return;
            }

            int margenIzq = 20;
            int margenSup = 45;
            int margenInf = 45;   // espacio para los nombres abajo

            int anchoUtil = area.Width - margenIzq - 20;
            int altoUtil = area.Height - margenSup - margenInf;
            if (anchoUtil < 50 || altoUtil < 30) return;   // ventana muy chica

            double maximo = valores.Max();
            if (maximo <= 0) maximo = 1;

            int anchoCasilla = anchoUtil / valores.Count;
            int anchoBarra = Math.Max(8, anchoCasilla - 16);
            int yBase = margenSup + altoUtil;   // línea donde empiezan las barras

            Color[] colores = { Color.SteelBlue, Color.OrangeRed, Color.SeaGreen, Color.Goldenrod, Color.MediumPurple };

            StringFormat centrado = new StringFormat();
            centrado.Alignment = StringAlignment.Center;

            for (int i = 0; i < valores.Count; i++)
            {
                int alto = (int)(valores[i] / maximo * (altoUtil - 20));
                int x = margenIzq + i * anchoCasilla + (anchoCasilla - anchoBarra) / 2;
                int y = yBase - alto;

                using (Brush pincel = new SolidBrush(colores[i % colores.Length]))
                {
                    g.FillRectangle(pincel, x, y, anchoBarra, alto);
                }

                // Valor encima de la barra
                RectangleF zonaValor = new RectangleF(margenIzq + i * anchoCasilla, y - 16, anchoCasilla, 16);
                g.DrawString(valores[i].ToString("0.##") + sufijo, fuenteTexto, Brushes.Black, zonaValor, centrado);

                // Nombre debajo
                RectangleF zonaNombre = new RectangleF(margenIzq + i * anchoCasilla, yBase + 4, anchoCasilla, margenInf - 4);
                g.DrawString(etiquetas[i], fuenteTexto, Brushes.Black, zonaNombre, centrado);
            }

            // Línea base
            g.DrawLine(Pens.Black, margenIzq, yBase, margenIzq + anchoUtil, yBase);
        }

        // ---------------------------------------------------------
        // Exportar
        // ---------------------------------------------------------
        private void btnExportarCsv_Click(object sender, EventArgs e)
        {
            if (candidatos == null || participacion == null)
            {
                MessageBox.Show("Primero presiona Calcular.");
                return;
            }

            SaveFileDialog dialogo = new SaveFileDialog();
            dialogo.Filter = "Archivo CSV (*.csv)|*.csv";
            dialogo.FileName = "resultados";

            if (dialogo.ShowDialog() != DialogResult.OK) return;

            try
            {
                // Se guardan 2 archivos: uno con candidatos y otro con participación
                string carpeta = Path.GetDirectoryName(dialogo.FileName);
                string nombreBase = Path.GetFileNameWithoutExtension(dialogo.FileName);
                string rutaCandidatos = Path.Combine(carpeta, nombreBase + "_candidatos.csv");
                string rutaParticipacion = Path.Combine(carpeta, nombreBase + "_participacion.csv");

                Exportador.ExportarCandidatosCsv(rutaCandidatos, candidatos);
                Exportador.ExportarParticipacionCsv(rutaParticipacion, participacion);

                MessageBox.Show("Se guardaron los archivos:\n" + rutaCandidatos + "\n" + rutaParticipacion);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo exportar: " + ex.Message);
            }
        }

        private void btnExportarXml_Click(object sender, EventArgs e)
        {
            if (candidatos == null || participacion == null)
            {
                MessageBox.Show("Primero presiona Calcular.");
                return;
            }

            SaveFileDialog dialogo = new SaveFileDialog();
            dialogo.Filter = "Archivo XML (*.xml)|*.xml";
            dialogo.FileName = "resultados";

            if (dialogo.ShowDialog() != DialogResult.OK) return;

            try
            {
                Exportador.ExportarXml(dialogo.FileName, convocatoriaActual, agruparActual, candidatos, participacion);
                MessageBox.Show("Archivo guardado:\n" + dialogo.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo exportar: " + ex.Message);
            }
        }
    }
}
