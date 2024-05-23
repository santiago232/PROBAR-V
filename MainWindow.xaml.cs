using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            btnMostrarCatalogoProductos.Click += MostrarCatalogoProductos_Click;
            cmbTipoTransaccion.SelectionChanged += cmbTipoTransaccion_SelectionChanged;
        }

        private void cmbTipoTransaccion_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selectedItem = cmbTipoTransaccion.SelectedItem as ComboBoxItem;
            if (selectedItem != null)
            {
                string tipoTransaccion = selectedItem.Content.ToString();
                lblCantidad.Visibility = tipoTransaccion == "Compra" || tipoTransaccion == "Venta" ? Visibility.Visible : Visibility.Collapsed;
                txtCantidad.Visibility = tipoTransaccion == "Compra" || tipoTransaccion == "Venta" ? Visibility.Visible : Visibility.Collapsed;
                txtTipoMoneda.Visibility = tipoTransaccion == "Compra" || tipoTransaccion == "Venta" ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void MostrarCatalogoProductos_Click(object sender, RoutedEventArgs e)
        {
            // Crear una nueva ventana
            Window catalogoWindow = new Window();
            catalogoWindow.Title = "Catálogo de Productos";
            catalogoWindow.Width = 800;
            catalogoWindow.Height = 450;

            // Crear un DataGrid y asignar la lista de productos
            DataGrid dgCatalogoProductos = new DataGrid();
            dgCatalogoProductos.ItemsSource = inventario;

            // Agregar el DataGrid a la ventana
            catalogoWindow.Content = dgCatalogoProductos;

            // Mostrar la nueva ventana como diálogo modal
            catalogoWindow.ShowDialog();
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
            decimal cantidad;

            if (!decimal.TryParse(txtMonto.Text, out monto))
            {
                MessageBox.Show("Por favor, ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtCantidad.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida para la transacción.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string tipoMoneda = txtTipoMoneda.Text;

            // Añadir la transacción
            var transaccion = new Transaccion { Tipo = tipo, Descripcion = descripcion, Monto = monto, Cantidad = cantidad, TipoMoneda = tipoMoneda };
            transacciones.Add(transaccion);

            // Actualizar el inventario si es una compra o venta
            if (tipo == "Compra" || tipo == "Venta")
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
                        productoExistente.Cantidad -= (int)cantidad;
                    }
                }
                else
                {
                    if (tipo == "Compra")
                    {
                        inventario.Add(new Producto { Nombre = descripcion, Cantidad = (int)cantidad, PrecioCompra = monto / cantidad, PrecioVenta = 0, FechaCaducidad = DateTime.MinValue, NumeroLote = string.Empty });
                    }
                }
            }

            // Añadir al control de caja
            var movimiento = new MovimientoCaja { TipoMovimiento = tipo, Descripcion = descripcion, Monto = monto, Fecha = DateTime.Now };
            movimientosCaja.Add(movimiento);

            // Limpiar campos
            txtDescripcion.Clear();
            txtMonto.Clear();
            txtCantidad.Clear();
            txtTipoMoneda.Text = "Q";
        }



        private void AgregarProducto_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(dgInventario.ItemsSource);
            view.Refresh();

            string nombre = txtNombreProducto.Text;
            int cantidad;
            decimal precioCompra;
            decimal precioVenta;
            DateTime fechaCaducidad;
            string numeroLote;

            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Por favor, ingrese el nombre del producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!int.TryParse(txtCantidadProducto.Text, out cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Por favor, ingrese una cantidad válida para el producto.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtPrecioCompra.Text, out precioCompra) || precioCompra <= 0)
            {
                MessageBox.Show("Por favor, ingrese un precio de compra válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(txtPrecioVenta.Text, out precioVenta) || precioVenta <= 0)
            {
                MessageBox.Show("Por favor, ingrese un precio de venta válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            fechaCaducidad = dpFechaCaducidad.SelectedDate ?? DateTime.MinValue;
            numeroLote = txtNumeroLote.Text;

            inventario.Add(new Producto { Nombre = nombre, Cantidad = cantidad, PrecioCompra = precioCompra, PrecioVenta = precioVenta, FechaCaducidad = fechaCaducidad, NumeroLote = numeroLote });

            txtNombreProducto.Clear();
            txtCantidadProducto.Clear();
            txtPrecioCompra.Clear();
            txtPrecioVenta.Clear();
            dpFechaCaducidad.SelectedDate = null;
            txtNumeroLote.Clear();
        }

        private void RegistrarMovimiento_Click(object sender, RoutedEventArgs e)
        {
            string tipoMovimiento = (cmbTipoMovimiento.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (string.IsNullOrEmpty(tipoMovimiento))
            {
                MessageBox.Show("Por favor, seleccione un tipo de movimiento.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string descripcion = txtDescripcionMovimiento.Text;
            decimal monto;

            if (!decimal.TryParse(txtMontoMovimiento.Text, out monto))
            {
                MessageBox.Show("Por favor, ingrese un monto válido.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            movimientosCaja.Add(new MovimientoCaja { TipoMovimiento = tipoMovimiento, Descripcion = descripcion, Monto = monto, Fecha = DateTime.Now });

            txtDescripcionMovimiento.Clear();
            txtMontoMovimiento.Clear();
        }

        private void ActualizarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
            if (productoSeleccionado != null)
            {
                // Lógica para mostrar los detalles del producto seleccionado en controles de edición
                txtNombreProducto.Text = productoSeleccionado.Nombre;
                txtCantidadProducto.Text = productoSeleccionado.Cantidad.ToString();
                txtPrecioCompra.Text = productoSeleccionado.PrecioCompra.ToString();
                txtPrecioVenta.Text = productoSeleccionado.PrecioVenta.ToString();
                dpFechaCaducidad.SelectedDate = productoSeleccionado.FechaCaducidad;
                txtNumeroLote.Text = productoSeleccionado.NumeroLote;
            }
        }

        private void BorrarProducto_Click(object sender, RoutedEventArgs e)
        {
            Producto productoSeleccionado = dgInventario.SelectedItem as Producto;
            if (productoSeleccionado != null)
            {
                // Eliminar el producto de la lista
                inventario.Remove(productoSeleccionado);
            }


        }

    }
}
