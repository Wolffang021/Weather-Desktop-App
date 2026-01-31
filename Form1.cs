using System.Text.Json;

namespace Weather_Desktop_App;

public class City
{
    public string name { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
    public string country { get; set; }
    public string state { get; set; }
}

public partial class Form1 : Form
{
    private string APIKEY;
    TextBox cityInput;
    Button searchButton;

    public Form1()
    {
        InitializeComponent();
    }

    private void SearchUsingEnter(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Search(sender, (EventArgs)e);
        }
    }

    private async Task<City?> GetCity(string cityName)
    {
        string url1 = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid={APIKEY}";
        HttpClient client = new();

        var response = await client.GetAsync(url1);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        var city = JsonSerializer.Deserialize<List<City>>(json);

        if (city == null || city.Count == 0)
        {
            return null;
        }

        // try
        // {
        //     string url2 = $"https://restcountries.com/v3.1/alpha/{city[0].country}";
        //     var response2 = await client.GetAsync(url2);

        //     if ()
        // }

        return city[0];
    }

    private async void Search(object sender, EventArgs e)
    {
        var city = await GetCity(cityInput.Text);

        Console.WriteLine(city.name);
        Console.WriteLine(city.lat);
        Console.WriteLine(city.lon);
        Console.WriteLine(city.state);
        Console.WriteLine(city.country);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        APIKEY = File.ReadAllLines(@"D:\Programming\Projects\Weather-Desktop-App\APIKEY.txt")[0];

        cityInput = new TextBox
        {
            Location = new Point(300, 40),
            Size = new Size(182, 20),
            PlaceholderText = "Enter city..."
        };
        cityInput.KeyUp += SearchUsingEnter;
        this.Controls.Add(cityInput);

        searchButton = new Button
        {
            Location = new Point(482, 38),
            Size = new Size(30, 30),
            Text = "🔎",
            BackColor = System.Drawing.SystemColors.Control
        };
        searchButton.Click += Search;
        this.Controls.Add(searchButton);

        Console.Write(cityInput.Size.Height);
    }
}
