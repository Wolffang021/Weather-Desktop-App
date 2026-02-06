using System.ComponentModel;
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
    public float deg { get; set; }
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

public class MainAqiInfo
{
    public int aqi { get; set; }
}

public class ComponentsInfo
{
    public float co { get; set; }
    public float no { get; set; }
    public float no2 { get; set; }
    public float o3 { get; set; }
    public float so2 { get; set; }
    public float pm2_5 { get; set; }
    public float pm10 { get; set; }
    public float nh3 { get; set; }
}

public class Aqi
{
    public MainAqiInfo? main { get; set; }
    public ComponentsInfo? components { get; set; }
}


public partial class Form1 : Form
{
    private string? APIKEY;
    TextBox? cityInput;
    Button? searchButton;
    Panel? mainPanel;
    Label? cityGeoLbl;
    Label? temperatureLbl;
    Label? weatherLbl;
    PictureBox? weatherIcon;
    Label? leftMiscDetailLbl;
    Label? rightMiscDetailLbl;
    Label? bottomMiscDetailLbl;
    Label? sunSetRiseLbl;
    Label? aqiLbl;
    Label? cityTimeLbl;
    Label? lastUpdateLbl;

    public Form1()
    {
        InitializeComponent();
    }

    private async Task<Aqi?> GetAqi(float lat, float lon)
    {
        HttpClient client = new();
        string url = $"https://api.openweathermap.org/data/2.5/air_pollution?lat={lat}&lon={lon}&appid={APIKEY}";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();
        var aqiVar = JsonSerializer.Deserialize<Aqi>(JsonDocument.Parse(json).RootElement.GetProperty("list")[0]);

        if (aqiVar == null)
        {
            return null;
        }

        return aqiVar;
    }

    private async Task<Weather?> GetWeather(float lat, float lon)
    {
        HttpClient client = new();
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={APIKEY}&units=metric";

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

    private void ClearScreen()
    {
        mainPanel.BorderStyle = BorderStyle.None;

        cityGeoLbl.Text = "Searching...";

        temperatureLbl.Text = "";
        temperatureLbl.MinimumSize = new Size(300, 0);
        temperatureLbl.MaximumSize = new Size(300, 0);
        temperatureLbl.Location = new Point(temperatureLbl.Location.X, cityGeoLbl.Location.Y + cityGeoLbl.Size.Height + 5);

        weatherLbl.Text = "";
        weatherLbl.MinimumSize = new Size(300, 0);
        weatherLbl.MaximumSize = new Size(300, 0);
        weatherLbl.Location = new Point(weatherLbl.Location.X, temperatureLbl.Location.Y + temperatureLbl.Size.Height - 5);

        weatherIcon.ImageLocation = null;
        weatherIcon.BorderStyle = BorderStyle.None;
        weatherIcon.BackColor = Color.FromArgb(0, 0, 0, 0);

        leftMiscDetailLbl.Text = "";
        leftMiscDetailLbl.Location = new Point(leftMiscDetailLbl.Location.X, weatherLbl.Location.Y + weatherLbl.Size.Height + 8);

        rightMiscDetailLbl.Text = "";
        rightMiscDetailLbl.Location = new Point(rightMiscDetailLbl.Location.X, weatherLbl.Location.Y + weatherLbl.Size.Height + 8);

        bottomMiscDetailLbl.Text = "";
        bottomMiscDetailLbl.Location = new Point(bottomMiscDetailLbl.Location.X, leftMiscDetailLbl.Location.Y + leftMiscDetailLbl.Size.Height + 8);

        sunSetRiseLbl.Text = "";
        sunSetRiseLbl.Location = new (sunSetRiseLbl.Location.X, bottomMiscDetailLbl.Location.Y + bottomMiscDetailLbl.Size.Height + 5);

        aqiLbl.Text = "";
        aqiLbl.ForeColor = Color.White;
        aqiLbl.Location = new Point(aqiLbl.Location.X, sunSetRiseLbl.Location.Y + sunSetRiseLbl.Size.Height + 5);

        cityTimeLbl.Text = "";
        cityTimeLbl.Location = new Point(mainPanel.Location.X, mainPanel.Location.Y + mainPanel.Size.Height + 2);

        lastUpdateLbl.Text = "";
        lastUpdateLbl.Location = new Point(mainPanel.Location.X + cityTimeLbl.Size.Width, mainPanel.Location.Y + mainPanel.Size.Height + 2);
    }

    private void UpdateScreen()
    {
        mainPanel.BorderStyle = BorderStyle.FixedSingle;

        temperatureLbl.MaximumSize = new Size(190, 0);
        temperatureLbl.MinimumSize = new Size(190, 0);
        temperatureLbl.Location = new Point(temperatureLbl.Location.X, cityGeoLbl.Location.Y + cityGeoLbl.Size.Height + 5);

        weatherLbl.MaximumSize = new Size(190, 0);
        weatherLbl.MinimumSize = new Size(190, 0);
        weatherLbl.Location = new Point(weatherLbl.Location.X, temperatureLbl.Location.Y + temperatureLbl.Size.Height - 5);

        weatherIcon.BorderStyle = BorderStyle.FixedSingle;
        weatherIcon.BackColor = Color.FromArgb(116, 117, 116);
        weatherIcon.Location = new Point(temperatureLbl.Size.Width, cityGeoLbl.Location.Y + cityGeoLbl.Size.Height + 5);

        leftMiscDetailLbl.Location = new Point(leftMiscDetailLbl.Location.X, weatherLbl.Location.Y + weatherLbl.Size.Height + 8);

        rightMiscDetailLbl.Location = new Point(rightMiscDetailLbl.Location.X, weatherLbl.Location.Y + weatherLbl.Size.Height + 8);

        bottomMiscDetailLbl.Location = new Point(bottomMiscDetailLbl.Location.X, leftMiscDetailLbl.Location.Y + leftMiscDetailLbl.Size.Height + 8);

        sunSetRiseLbl.Location = new (sunSetRiseLbl.Location.X, bottomMiscDetailLbl.Location.Y + bottomMiscDetailLbl.Size.Height + 5);

        aqiLbl.Location = new Point(aqiLbl.Location.X, sunSetRiseLbl.Location.Y + sunSetRiseLbl.Size.Height + 5);

        cityTimeLbl.Location = new Point(mainPanel.Location.X, mainPanel.Location.Y + mainPanel.Size.Height + 2);

        lastUpdateLbl.Location = new Point(mainPanel.Location.X + cityTimeLbl.Size.Width, mainPanel.Location.Y + mainPanel.Size.Height + 2);
    }

    private string SimplifyClouds(float clouds)
    {
        if (clouds <= 10)
        {
            return "Clear sky";
        }
        else if (clouds <= 30)
        {
            return "Mostly clear";
        }
        else if (clouds <= 60)
        {
            return "Partly cloudy";
        }
        else if (clouds <= 85)
        {
            return "Mostly cloudy";
        }
        else
        {
            return "Overcast";
        }
    }

    private string SimplifyWindDirection(float deg)
    {
        switch (deg)
        {
            default:
                return "N";

            case float x when x > 22.5 && x <= 67.5:
                return "NE";

            case float x when x > 67.5 && x <= 112.5:
                return "E";

            case float x when x > 112.5 && x <= 157.5:
                return "SE";

            case float x when x > 157.5 && x <= 202.5:
                return "S";

            case float x when x > 202.5 && x <= 247.5:
                return "SW";

            case float x when x > 247.5 && x <= 292.5:
                return "W";

            case float x when x > 292.5 && x <= 337.5:
                return "NW";
        }
    }

    private string SimplyfyAqi(int aqi)
    {
        switch (aqi)
        {
            default:
                return "Unknown";

            case int x when x <= 50:
                aqiLbl.ForeColor = Color.Green; // DeepSkyBlue might also work
                return "Good";

            case int x when x <= 100:
                aqiLbl.ForeColor = Color.GreenYellow;
                return "Fair";

            case int x when x <= 150:
                aqiLbl.ForeColor = Color.Yellow;
                return "Moderate";

            case int x when x <= 200:
                aqiLbl.ForeColor = Color.Orange;
                return "Unhealthy";

            case int x when x <= 300:
                aqiLbl.ForeColor = Color.OrangeRed;
                return "Very unhealthy";

            case int x when x <= 500:
                aqiLbl.ForeColor = Color.Red;
                return "Hazardous";
        }
    }

    private int CalculateAqi(ComponentsInfo components)
    {
        float pm25 = components.pm2_5;
        double cHigh, cLow;
        int iHigh, iLow;

        if (pm25 <= 12)
        {
            (cHigh, cLow, iHigh, iLow) = (12, 0, 50, 0);
        }
        else if (pm25 <= 35.4)
        {
            (cHigh, cLow, iHigh, iLow) = (35.4, 12.1, 100, 51);
        }
        else if (pm25 <= 55.4)
        {
            (cHigh, cLow, iHigh, iLow) = (55.4, 35.5, 150, 101);
        }
        else if (pm25 <= 150.4)
        {
            (cHigh, cLow, iHigh, iLow) = (150.4, 55.5, 200, 151);
        }
        else if (pm25 <= 250.4)
        {
            (cHigh, cLow, iHigh, iLow) = (250.4, 150.5, 300, 201);
        }
        else
        {
            (cHigh, cLow, iHigh, iLow) = (500.4, 250.5, 500, 301);
        }

        return (int)Math.Round(((iHigh - iLow) / (cHigh - cLow)) * (pm25 - cLow) + iLow);
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
        ClearScreen();

        if (!NetworkInterface.GetIsNetworkAvailable())
        {
            cityGeoLbl.Text = "No connection...";
            return;
        }

        var city = await GetCity(cityInput.Text);

        if (city == null)
        {
            cityGeoLbl.Text = "Something went wrong...";
            return;
        }

        var weather = await GetWeather(city.lat, city.lon);
        var aqiVar = await GetAqi(city.lat, city.lon);

        cityGeoLbl.Text = city.name;
        cityGeoLbl.Text = city.state != null && city.state.Length > 0 ? cityGeoLbl.Text + ", " + city.state : cityGeoLbl.Text;
        cityGeoLbl.Text = city.country != null && city.country.Length > 0 ? cityGeoLbl.Text + ", " + city.country : cityGeoLbl.Text;

        cityGeoLbl.Text += $"\n{Math.Abs(city.lat)}°";
        cityGeoLbl.Text += city.lat > 0 ? " N" : city.lat < 0 ? " S" : "";

        cityGeoLbl.Text += $" {Math.Abs(city.lon)}°";
        cityGeoLbl.Text += city.lon > 0 ? " E" : city.lon < 0 ? " W" : "";

        if (weather == null)
        {
            temperatureLbl.Text = "Couldn't fetch weather details";
            return;
        }

        temperatureLbl.Text = Math.Round(weather.main.temp).ToString("F0") + "° C";

        weatherLbl.Text = $"{weather.weather[0].main}";

        weatherIcon.ImageLocation = $"https://openweathermap.org/img/wn/{weather.weather[0].icon}.png";

        cityTimeLbl.Text = DateTimeOffset.FromUnixTimeSeconds(weather.dt).ToOffset(TimeSpan.FromSeconds(weather.timezone)).ToString("hh:mm tt");

        lastUpdateLbl.Text = $"Last updated {(int)(DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(weather.dt)).TotalMinutes} minutes ago";

        leftMiscDetailLbl.Text = $"Feels: {Math.Round(weather.main.feels_like).ToString("F0") + "° C"}\nMax: {Math.Round(weather.main.temp_max).ToString("F0") + "° C"}\nMin: {Math.Round(weather.main.temp_min).ToString("F0") + "° C"}";

        rightMiscDetailLbl.Text = $"Pressure: {weather.main.pressure} hPa\nHumidity: {weather.main.humidity}%\nClouds: {SimplifyClouds(weather.clouds.all)}";

        bottomMiscDetailLbl.Text = $"Wind speed: {weather.wind.speed} Km/h {SimplifyWindDirection(weather.wind.deg)}\nVisibility: {weather.visibility / 100} Km";
        
        sunSetRiseLbl.Text = $"Sunrise: {DateTimeOffset.FromUnixTimeSeconds(weather.sys.sunrise).ToOffset(TimeSpan.FromSeconds(weather.timezone)).ToString("hh:mm tt")}\nSunset: {DateTimeOffset.FromUnixTimeSeconds(weather.sys.sunset).ToOffset(TimeSpan.FromSeconds(weather.timezone)).ToString("hh:mm tt")}";

        if (aqiVar == null)
        {
            aqiLbl.Text = "Couldn't fetch air quality details";
            UpdateScreen();
            return;
        }

        int calculatedAqi = CalculateAqi(aqiVar.components);
        aqiLbl.Text = $"AQI: {calculatedAqi} ({SimplyfyAqi(calculatedAqi)})";

        UpdateScreen();
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
            Location = new Point(50, 90),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 400),
            Padding = new Padding(0, 0, 0, 5),
            BorderStyle = BorderStyle.None,
            // BackColor = Color.Beige
        };
        this.Controls.Add(mainPanel);

        cityGeoLbl = new Label
        {
            Location = new Point(0, 5),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 0),
            Text = "Try searching a city..",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = Color.White,
            // BackColor = Color.AntiqueWhite
        };
        mainPanel.Controls.Add(cityGeoLbl);

        temperatureLbl = new Label
        {
            Location = new Point(0, cityGeoLbl.Location.Y + cityGeoLbl.Size.Height + 5),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            ForeColor = Color.White,
            // BackColor = Color.BlanchedAlmond
        };
        mainPanel.Controls.Add(temperatureLbl);

        weatherLbl = new Label
        {
            Location = new Point(0, temperatureLbl.Location.Y + temperatureLbl.Size.Height - 5),
            AutoSize = true,
            MinimumSize = new Size(300, 0),
            MaximumSize = new Size(300, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            ForeColor = Color.White,
            // BackColor = Color.BlanchedAlmond
        };
        mainPanel.Controls.Add(weatherLbl);

        weatherIcon = new PictureBox
        {
            Location = new Point(temperatureLbl.Size.Width, cityGeoLbl.Location.Y + cityGeoLbl.Size.Height + 5),
            Size = new Size(60, 60),
            SizeMode = PictureBoxSizeMode.StretchImage,
            BackColor = Color.FromArgb(0, 0, 0, 0),
            BorderStyle = BorderStyle.None
        };
        mainPanel.Controls.Add(weatherIcon);
        
        leftMiscDetailLbl = new Label
        {
            Location = new Point(0, weatherLbl.Location.Y + weatherLbl.Size.Height + 8),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width / 2, 0),
            MaximumSize = new Size(mainPanel.Size.Width / 2, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = Color.White,
            // BackColor = Color.DarkBlue
        };
        mainPanel.Controls.Add(leftMiscDetailLbl);

        rightMiscDetailLbl = new Label
        {
            Location = new Point(leftMiscDetailLbl.Location.X + leftMiscDetailLbl.Size.Width, weatherLbl.Location.Y + weatherLbl.Size.Height + 8),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width / 2, 0),
            MaximumSize = new Size(mainPanel.Size.Width / 2, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = Color.White,
            // BackColor = Color.DarkOrange
        };
        mainPanel.Controls.Add(rightMiscDetailLbl);

        bottomMiscDetailLbl = new Label
        {
            Location = new Point(0, leftMiscDetailLbl.Location.Y + leftMiscDetailLbl.Size.Height + 8),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width, 0),
            MaximumSize = new Size(mainPanel.Size.Width, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = Color.White,
            // BackColor = Color.DarkKhaki
        };
        mainPanel.Controls.Add(bottomMiscDetailLbl);

        sunSetRiseLbl = new Label
        {
            Location = new Point(0, bottomMiscDetailLbl.Location.Y + bottomMiscDetailLbl.Size.Height + 5),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width, 0),
            MaximumSize = new Size(mainPanel.Size.Width, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 8, FontStyle.Regular),
            ForeColor = Color.White,
            // BackColor = Color.DarkKhaki
        };
        mainPanel.Controls.Add(sunSetRiseLbl);

        aqiLbl = new Label
        {
            Location = new Point(0, sunSetRiseLbl.Location.Y + sunSetRiseLbl.Size.Height + 5),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width, 0),
            MaximumSize = new Size(mainPanel.Size.Width, 0),
            Text = "",
            TextAlign = ContentAlignment.TopCenter,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.White,
            // BackColor = Color.DarkGoldenrod
        };
        mainPanel.Controls.Add(aqiLbl);

        cityTimeLbl = new Label
        {
            Location = new Point(mainPanel.Location.X, mainPanel.Location.Y + mainPanel.Size.Height + 2),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width / 2, 0),
            MaximumSize = new Size(mainPanel.Size.Width / 2, 0),
            Text = "",
            TextAlign = ContentAlignment.TopLeft,
            Font = new Font("Segoe UI", 6, FontStyle.Bold),
            ForeColor = Color.White,
            // BackColor = Color.BlanchedAlmond
        };
        this.Controls.Add(cityTimeLbl);

        lastUpdateLbl = new Label
        {
            Location = new Point(mainPanel.Location.X + cityTimeLbl.Size.Width, mainPanel.Location.Y + mainPanel.Size.Height + 2),
            AutoSize = true,
            MinimumSize = new Size(mainPanel.Size.Width / 2, 0),
            MaximumSize = new Size(mainPanel.Size.Width / 2, 0),
            Text = "",
            TextAlign = ContentAlignment.TopRight,
            Font = new Font("Segoe UI", 6, FontStyle.Bold),
            ForeColor = Color.White,
            // BackColor = Color.BlanchedAlmond
        };
        this.Controls.Add(lastUpdateLbl);
    }
}
