using System;
using System.Windows;

namespace ProyectoFinal4
{
    public partial class ActualizarProductoWindow : Window
    {
        private Producto productoActual;

        public ActualizarProductoWindow(Producto producto)
        {
            InitializeComponent();
            productoActual = producto;

            txtNombreProducto.Text = producto.Nombre;
            txtCantidadProducto.Text = producto.Cantidad.ToString();
            txtPrecioCompra.Text = producto.PrecioCompra.ToString();
            txtPrecioVenta.Text = producto.PrecioVenta.ToString();
            dpFechaCaducidad.SelectedDate = producto.FechaCaducidad;
            txtNumeroLote.Text = producto.NumeroLote;
        }

        private void ActualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
   
                productoActual.Nombre = txtNombreProducto.Text;
                productoActual.Cantidad = int.Parse(txtCantidadProducto.Text);
                productoActual.PrecioCompra = decimal.Parse(txtPrecioCompra.Text);
                productoActual.PrecioVenta = decimal.Parse(txtPrecioVenta.Text);
                productoActual.FechaCaducidad = dpFechaCaducidad.SelectedDate ?? DateTime.MinValue;
                productoActual.NumeroLote = txtNumeroLote.Text;

                MessageBox.Show("Producto actualizado con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                Close(); // Cerrar la ventana de actualización de producto
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al actualizar producto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
