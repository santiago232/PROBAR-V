using ProyectoFinal4;
using System;
using System.Windows;

namespace ProyectoFinal4
{
    public partial class AgregarProductoWindow : Window
    {
        public AgregarProductoWindow()
        {
            InitializeComponent();
        }

        private void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string nombre = txtNombreProducto.Text;
                int cantidad = int.Parse(txtCantidadProducto.Text);
                decimal precioCompra = decimal.Parse(txtPrecioCompra.Text);
                decimal precioVenta = decimal.Parse(txtPrecioVenta.Text);
                DateTime fechaCaducidad = dpFechaCaducidad.SelectedDate ?? DateTime.MinValue;
                string numeroLote = txtNumeroLote.Text;

                // Crear un nuevo producto
                Producto nuevoProducto = new Producto
                {
                    Nombre = nombre,
                    Cantidad = cantidad,
                    PrecioCompra = precioCompra,
                    PrecioVenta = precioVenta,
                    FechaCaducidad = fechaCaducidad,
                    NumeroLote = numeroLote
                };

                // Agregar el nuevo producto a la gestión de inventario
                MainWindow main = Application.Current.MainWindow as MainWindow;
                main?.AgregarProducto(nuevoProducto);

                MessageBox.Show("Producto agregado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Close(); // Cerrar la ventana de agregar producto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al agregar producto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
