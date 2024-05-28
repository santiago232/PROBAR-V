using ProyectoFinal2;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal2
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<MovimientoCaja> movimientosCaja = new ObservableCollection<MovimientoCaja>();
        private ObservableCollection<Producto> inventario = new ObservableCollection<Producto>();

        public MainWindow()
        {
            InitializeComponent();

            dgInventario.ItemsSource = inventario;
            dgMovimientos.ItemsSource = movimientosCaja;

            cmbTipoTransaccion.SelectionChanged += cmbTipoTransaccion_SelectionChanged;
        }

        private void cmbTipoTransaccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = cmbTipoTransaccion.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string tipoTransaccion = selectedItem.Content.ToString();
                bool isCompra = tipoTransaccion == "Compra";
                bool isVenta = tipoTransaccion == "Venta";

                lblCodigo.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtCodigo.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblDescripcion.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtDescripcion.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblCantidad.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtCantidad.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblPrecioIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtPrecioIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblPrecioVenta.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                txtPrecioVenta.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                lblFechaIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;
                dpFechaIngreso.Visibility = isCompra ? Visibility.Visible : Visibility.Collapsed;

                lblCantidadVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                txtCantidadVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                lblDescripcionVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                txtDescripcionVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                lblMontoVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
                txtMontoVenta.Visibility = isVenta ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void ActualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el producto seleccionado en el DataGrid
            // Lógica para actualizar el producto seleccionado en el DataGrid
            if (dgInventario.SelectedItem != null)
            {
                Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
                // Aquí puedes implementar la lógica para actualizar los datos del producto seleccionado
                // Por ejemplo: productoSeleccionado.Descripcion = nuevoValor;
            }
            else
            {
                MessageBox.Show("Seleccione un producto para actualizar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EliminarProducto_Click(object sender, RoutedEventArgs e)
        {
            // Obtener el producto seleccionado en el DataGrid
            Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
            if (productoSeleccionado != null)
            {
                MessageBoxResult result = MessageBox.Show("¿Está seguro de que desea eliminar este producto?", "Confirmar Eliminación", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    inventario.Remove(productoSeleccionado);
                    MessageBox.Show("Producto eliminado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Seleccione un producto para eliminar.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void BuscarProducto_Click(object sender, RoutedEventArgs e)
        {
             string textoBusqueda = txtBuscarProducto.Text.ToLower(); // Convertir a minúsculas para una búsqueda no sensible a mayúsculas
    var productosEncontrados = inventario.Where(p => p.Descripcion.ToLower().Contains(textoBusqueda) || p.Codigo.ToLower() == textoBusqueda).ToList();

    // Actualizar el DataGrid con los productos encontrados
    dgInventario.ItemsSource = productosEncontrados;
        }
        private void GenerarReportes_Click(object sender, RoutedEventArgs e)
        {
            int totalCompras = movimientosCaja.Count(m => m.TipoMovimiento == "Compra");
            int totalVentas = movimientosCaja.Count(m => m.TipoMovimiento == "Venta");
            int totalTransacciones = totalCompras + totalVentas;

            decimal porcentajeCompras = (totalCompras / (decimal)totalTransacciones) * 100;
            decimal porcentajeVentas = (totalVentas / (decimal)totalTransacciones) * 100;

            MessageBox.Show($"Porcentaje de Compras: {porcentajeCompras}%\nPorcentaje de Ventas: {porcentajeVentas}%", "Estadísticas", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void RegistrarTransaccion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tipo = (cmbTipoTransaccion.SelectedItem as ComboBoxItem)?.Content.ToString();
                if (tipo == "Compra")
                {
                    string codigo = txtCodigo.Text;
                    string descripcion = txtDescripcion.Text;

                    if (!int.TryParse(txtCantidad.Text, out int cantidad))
                    {
                        MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!decimal.TryParse(txtPrecioIngreso.Text, out decimal precioIngreso))
                    {
                        MessageBox.Show("Por favor, ingrese un precio de ingreso válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (!decimal.TryParse(txtPrecioVenta.Text, out decimal precioVenta))
                    {
                        MessageBox.Show("Por favor, ingrese un precio de venta válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    DateTime fechaIngreso = dpFechaIngreso.SelectedDate.HasValue ? dpFechaIngreso.SelectedDate.Value : DateTime.Now;

                    var productoExistente = inventario.FirstOrDefault(p => p.Codigo == codigo);
                    if (productoExistente != null)
                    {
                        // Actualizar producto existente
                        productoExistente.Cantidad += cantidad;
                        productoExistente.PrecioIngreso = precioIngreso; // Opcional: actualizar el precio de ingreso
                        productoExistente.PrecioVenta = precioVenta; // Opcional: actualizar el precio de venta
                    }
                    else
                    {
                        // Agregar nuevo producto
                        Producto nuevoProducto = new Producto
                        {
                            Codigo = codigo,
                            Descripcion = descripcion,
                            Cantidad = cantidad,
                            PrecioIngreso = precioIngreso,
                            PrecioVenta = precioVenta,
                            FechaIngreso = fechaIngreso
                        };

                        inventario.Add(nuevoProducto);
                    }

                    MovimientoCaja nuevoMovimiento = new MovimientoCaja
                    {
                        TipoMovimiento = "Compra",
                        Cantidad = cantidad,
                        Descripcion = descripcion,
                        Monto = cantidad * precioIngreso,
                        Fecha = DateTime.Now
                    };

                    movimientosCaja.Add(nuevoMovimiento);

                    MessageBox.Show("Compra registrada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (tipo == "Venta")
                {
                    string descripcionVenta = txtDescripcionVenta.Text;

                    if (!int.TryParse(txtCantidadVenta.Text, out int cantidadVenta))
                    {
                        MessageBox.Show("Por favor, ingrese una cantidad válida.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var producto = inventario.FirstOrDefault(p => p.Descripcion == descripcionVenta);
                    if (producto == null)
                    {
                        MessageBox.Show("Producto no encontrado en el inventario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (producto.Cantidad < cantidadVenta)
                    {
                        MessageBox.Show("No hay suficiente cantidad en el inventario para realizar la venta.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    decimal montoVenta = cantidadVenta * producto.PrecioVenta;

                    txtMontoVenta.Text = montoVenta.ToString("F2");

                    producto.Cantidad -= cantidadVenta;

                    MovimientoCaja nuevoMovimiento = new MovimientoCaja
                    {
                        TipoMovimiento = "Venta",
                        Cantidad = cantidadVenta,
                        Descripcion = descripcionVenta,
                        Monto = montoVenta,
                        Fecha = DateTime.Now
                    };

                    movimientosCaja.Add(nuevoMovimiento);

                    MessageBox.Show("Venta registrada con éxito.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al registrar transacción: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

    }
}
