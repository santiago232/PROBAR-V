using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal4
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<MovimientoCaja> movimientosCaja = new ObservableCollection<MovimientoCaja>();
        private ObservableCollection<Transaccion> transacciones = new ObservableCollection<Transaccion>();
        private ObservableCollection<Producto> inventario = new ObservableCollection<Producto>();

        public MainWindow()
        {
            InitializeComponent();

            dgTransacciones.ItemsSource = transacciones;
            dgInventario.ItemsSource = inventario;
            dgMovimientos.ItemsSource = movimientosCaja;

            cmbTipoTransaccion.SelectionChanged += cmbTipoTransaccion_SelectionChanged;
        }

        private void ActualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
            if (productoSeleccionado != null)
            {
                var actualizarProductoWindow = new ActualizarProductoWindow(productoSeleccionado);
                actualizarProductoWindow.ShowDialog();
                dgInventario.Items.Refresh(); // Actualizar el DataGrid después de la actualización del producto
            }
            else
            {
                MessageBox.Show("Seleccione un producto para actualizar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProducto_Click(object sender, RoutedEventArgs e)
        {
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
                MessageBox.Show("Seleccione un producto para eliminar.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void cmbTipoTransaccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = cmbTipoTransaccion.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string tipoTransaccion = selectedItem.Content.ToString();
                lblCantidad.Visibility = (tipoTransaccion == "Compra" || tipoTransaccion == "Venta") ? Visibility.Visible : Visibility.Collapsed;
                txtCantidad.Visibility = (tipoTransaccion == "Compra" || tipoTransaccion == "Venta") ? Visibility.Visible : Visibility.Collapsed;
                txtTipoMoneda.Visibility = (tipoTransaccion == "Compra" || tipoTransaccion == "Venta") ? Visibility.Visible : Visibility.Collapsed;
            }
        }



        private void RegistrarTransaccion_Click(object sender, RoutedEventArgs e)
        {
            string tipo = (cmbTipoTransaccion.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(tipo))
            {
                MessageBox.Show("Por favor, seleccione un tipo de transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string descripcion = txtDescripcion.Text;
            decimal monto;
            decimal cantidad = 0;

            if (!decimal.TryParse(txtMonto.Text, out monto))
            {
                MessageBox.Show("Por favor, ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (tipo == "Compra" || tipo == "Venta")
            {
                if (!decimal.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
                {
                    MessageBox.Show("Por favor, ingrese una cantidad válida para la transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ActualizarInventario(tipo, descripcion, cantidad);
            }

            string tipoMoneda = txtTipoMoneda.Text;

            var nuevaTransaccion = new Transaccion
            {
                Tipo = tipo,
                Descripcion = descripcion,
                Monto = monto,
                Cantidad = cantidad,
                TipoMoneda = tipoMoneda
            };

            transacciones.Add(nuevaTransaccion);
            AgregarMovimientoCaja(nuevaTransaccion);

            txtDescripcion.Clear();
            txtMonto.Clear();
            txtCantidad.Clear();
            txtTipoMoneda.Text = "Q";
        }

        private void ActualizarInventario(string tipo, string descripcion, decimal cantidad)
        {
            var productoExistente = inventario.FirstOrDefault(p => p.Nombre == descripcion);
            if (productoExistente != null)
            {
                if (tipo == "Compra")
                {
                    productoExistente.Cantidad += (int)cantidad;
                }
                else if (tipo == "Venta")
                {
                    if (productoExistente.Cantidad >= cantidad)
                    {
                        productoExistente.Cantidad -= (int)cantidad;
                    }
                    else
                    {
                        MessageBox.Show("Cantidad insuficiente en el inventario.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else if (tipo == "Compra")
            {
                inventario.Add(new Producto
                {
                    Nombre = descripcion,
                    Cantidad = (int)cantidad,
                    PrecioCompra = 0, // Puedes agregar campos adicionales si son necesarios
                    PrecioVenta = 0,  // Puedes agregar campos adicionales si son necesarios
                    FechaCaducidad = DateTime.MinValue, // O algún valor por defecto
                    NumeroLote = ""
                });
            }
        }

        private void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            // Abrir la ventana para agregar producto
            var agregarProductoWindow = new AgregarProductoWindow();
            agregarProductoWindow.ShowDialog(); // Mostrar como ventana modal
        }

        private void AgregarMovimientoCaja(Transaccion transaccion)
        {
            var nuevoMovimiento = new MovimientoCaja
            {
                TipoMovimiento = transaccion.Tipo,
                Descripcion = transaccion.Descripcion,
                Monto = transaccion.Monto,
                Fecha = DateTime.Now
            };

            movimientosCaja.Add(nuevoMovimiento);
        }
        public void AgregarProducto(Producto producto)
        {
            var productoExistente = inventario.FirstOrDefault(p => p.Nombre == producto.Nombre);
            if (productoExistente != null)
            {
                productoExistente.Cantidad += producto.Cantidad;
            }
            else
            {
                inventario.Add(producto);
            }
        }
        private void RegistrarMovimiento_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
