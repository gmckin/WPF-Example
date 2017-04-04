using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PlayThingy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        HttpClient client = new HttpClient();
        public MainWindow()
        {
            InitializeComponent();
            client.BaseAddress = new Uri("http://localhost:8948/");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            this.Loaded += MainWindow_Loaded;
        }
        async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            HttpResponseMessage response = await client.GetAsync("/api/things");
            response.EnsureSuccessStatusCode(); // Throw on error code. 
            var things = await response.Content.ReadAsAsync<IEnumerable<Things>>();
            thingyListView.ItemsSource = things;
        }


        public async Task<IEnumerable<Things>> GetAllThingys()
        {
            HttpResponseMessage response = await client.GetAsync("/api/things");
            response.EnsureSuccessStatusCode(); // Throw on error code. 
            var things = await response.Content.ReadAsAsync<IEnumerable<Things>>();
            return things;
        }

        private async void btnGetThingy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("/api/things/?id=" + txtID.Text);
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                var thingy = await response.Content.ReadAsAsync<Things>();
                thingyDetailsPanel.Visibility = Visibility.Visible;
                thingyDetailsPanel.DataContext = thingy;
                txtID.Text = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Thingy not Found");
            }
        }

        private async void btnNewThingy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var thing = new Things()
                {
                   Name = txtName.Text,
                   ThingInfo = txtInfo.Text,
                   Quantity = int.Parse(txtQuantity.Text)


                    //gender = cbxGender.SelectedItem.ToString(),

                };
                var response = await client.PostAsJsonAsync("/api/things/", thing);
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                MessageBox.Show("Thingy Added Successfully", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                thingyListView.ItemsSource = await GetAllThingys();
                thingyListView.ScrollIntoView(thingyListView.ItemContainerGenerator.Items[thingyListView.Items.Count - 1]);
                txtName.Text = "";
                txtInfo.Text = "";
                txtQuantity.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Thingy not Added, May be due to Duplicate ID");
            }
        }

        private async void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var thing = new Things()
                {
                    ThingID = int.Parse(txID.Text),
                    Name = txtName.Text,
                    ThingInfo = txtInfo.Text,
                    Quantity = int.Parse(txtQuantity.Text)


                    //gender = cbxGender.SelectedItem.ToString(),

                };
                var response = await client.PutAsJsonAsync("/api/things/?id="+txID.Text, thing);
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                MessageBox.Show("Thingy Updated Successfully", "Result", MessageBoxButton.OK, MessageBoxImage.Information);
                thingyListView.ItemsSource = await GetAllThingys();
                thingyListView.ScrollIntoView(thingyListView.ItemContainerGenerator.Items[thingyListView.Items.Count - 1]);
                txID.Text = "";
                txtName.Text = "";
                txtInfo.Text = "";
                txtQuantity.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Failed to Update Thingy");
            }
        }

        private async void btnDeleteThingy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HttpResponseMessage response = await client.DeleteAsync("/api/things/?id=" + txtID.Text);
                response.EnsureSuccessStatusCode(); // Throw on error code. 
                MessageBox.Show("Thingy Successfully Deleted");
                thingyListView.ItemsSource = await GetAllThingys();
                thingyListView.ScrollIntoView(thingyListView.ItemContainerGenerator.Items[thingyListView.Items.Count - 1]);
                txtID.Text = "";
            }
            catch (Exception)
            {
                MessageBox.Show("Thingy Deletion Unsuccessful");
            }
        }

    }
    public class Things
    {
        public int ThingID { get; set; }
        public string Name { get; set; }
        public string ThingInfo { get; set; }
        public int Quantity { get; set; }
    }
}
