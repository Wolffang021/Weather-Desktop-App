using System.Net.NetworkInformation;
using System.Text.Json;

namespace Weather_Desktop_App;

public class City
{
    public string? name { get; set; }
    public float lat { get; set; }
    public float lon { get; set; }
    public string? country { get; set; }
    public string? state { get; set; }
}

public class WeatherInfo
{
    public string? main { get; set; }
    public string? description { get; set; }
    public string? icon { get; set; }
}

public class MainInfo
{
    public float temp { get; set; }
    public float feels_like { get; set; }
    public float temp_min { get; set; }
    public float temp_max { get; set; }
    public int pressure { get; set; }
    public int humidity { get; set; }
}

public class WindInfo
{
    public float speed { get; set; }
}

public class SysInfo
{
    public long sunrise { get; set; }
    public long sunset { get; set; }
}

public class CloudsInfo
{
    public int all { get; set; }
}

public class Weather
{
    public List<WeatherInfo>? weather { get; set; }
    public MainInfo? main { get; set; }
    public int visibility { get; set; }
    public WindInfo? wind { get; set; }
    public SysInfo? sys { get; set; }
    public int timezone { get; set; }
    public long dt { get; set; }
    public CloudsInfo? clouds { get; set; }
}


public partial class Form1 : Form
{
    private string? APIKEY;
    TextBox? cityInput;
    Button? searchButton;
    Panel? mainPanel;
    Label? cityGeoDetails;
    Label? temperatureDetails;

    public Form1()
    {
        InitializeComponent();
    }

    private async Task<Weather?> GetWeather(float lat, float lon)
    {
        HttpClient client = new();
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={APIKEY}";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        var weathers = JsonSerializer.Deserialize<Weather>(json);

        return weathers;
    }

    private async Task<City?> GetCity(string cityName)
    {
        HttpClient client = new();
        string url = $"https://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid={APIKEY}";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        var cities = JsonSerializer.Deserialize<List<City>>(json);

        if (cities == null || cities.Count == 0)
        {
            return null;
        }

        try
        {
            string url2 = $"https://restcountries.com/v3.1/alpha/{cities[0].country}";
            var response2 = await client.GetAsync(url2);

            if (!response2.IsSuccessStatusCode)
            {
                throw new Exception();
            }

            var json2 = await response2.Content.ReadAsStringAsync();
            string? country = JsonDocument.Parse(json2).RootElement[0].GetProperty("name").GetProperty("common").ToString();

            if (country == null || country == "" || country.Length == 0)
            {
                throw new Exception();
            }

            cities[0].country = country;
        }
        catch (Exception) {}

        return cities[0];
    }

    private void SearchUsingEnter(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            Search(sender, (EventArgs)e);
        }
    }

    private async void Search(object? sender, EventArgs e)
    {
        cityGeoDetails.Text = "Searching...";
        temperatureDetails.Text = "";

        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            cityGeoDetails.Text = "No connection...";
            return;
        }

        var city = await GetCity(cityInput.Text);

        if (city == null)
        {
            cityGeoDetails.Text = "Something went wrong...";
            return;
        }

        var weather = await GetWeather(city.lat, city.lon);

        cityGeoDetails.Text = city.name;
        cityGeoDetails.Text = city.state != null && city.state.Length > 0 ? cityGeoDetails.Text + ", " + city.state : cityGeoDetails.Text;
        cityGeoDetails.Text += $", {city.country}";

        cityGeoDetails.Text += $"\n{Math.Abs(city.lat)}°";
        cityGeoDetails.Text += city.lat > 0 ? " N" : city.lat < 0 ? " S" : "";

        cityGeoDetails.Text += $" {Math.Abs(city.lon)}°";
        cityGeoDetails.Text += city.lon > 0 ? " E" : city.lon < 0 ? " W" : "";

        temperatureDetails.Location = new Point(0, cityGeoDetails.Location.Y + cityGeoDetails.Size.Height + 5);

        if (weather == null)
        {
            temperatureDetails.Text = "Couldn't fetch weather details...";
            return;
        }
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - this.Width, Screen.PrimaryScreen.WorkingArea.Height - this.Height);

        APIKEY = File.ReadAllLines(@$"{AppContext.BaseDirectory[..^25]}APIKEY.txt")[0];

        cityInput = new TextBox
        {
            Location = new Point(100, 40),
            Size = new Size(182, 20),
            PlaceholderText = "Enter city..."
        };
        cityInput.KeyUp += SearchUsingEnter;
        this.Controls.Add(cityInput);

        searchButton = new Button
        {
            Location = new Point(282, 38),
            Size = new Size(30, 30),
            Font = new Font("Wide Latin", 8, FontStyle.Regular),
            Text = "🔎",
            BackColor = System.Drawing.SystemColors.Control
        };
        searchButton.Click += Search;
        this.Controls.Add(searchButton);

        mainPanel = new Panel
        {
            Location = new Point(50, 100),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 300)
        };
        this.Controls.Add(mainPanel);

        cityGeoDetails = new Label
        {
            Location = new Point(0, 5),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 0),
            Text = "Try searching a city..",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.White
        };
        mainPanel.Controls.Add(cityGeoDetails);

        temperatureDetails = new Label
        {
            Location = new Point(0, cityGeoDetails.Location.Y + cityGeoDetails.Size.Height + 5),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.White
        };
        mainPanel.Controls.Add(temperatureDetails);
    }
}
