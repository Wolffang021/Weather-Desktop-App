# Weather Desktop App

WinForms desktop app that shows current weather, air quality, and local time for a searched city.

**Features**
- City search with geocoding and country lookup
- Current temperature, feels-like, min/max, pressure, humidity, clouds, wind, visibility
- Air Quality Index (AQI) with category labels
- Sunrise and sunset times
- Weather icon from OpenWeatherMap
- Local city time and last-update indicator

**Tech Stack**
- .NET 8 (WinForms)
- C#
- OpenWeatherMap API (weather + air pollution)
- REST Countries API (country name resolution)

**Requirements**
- Windows
- .NET 8 SDK
- OpenWeatherMap API key

**Setup**
1. Create `APIKEY.txt` in the project root.
1. Put your OpenWeatherMap API key on the first line of `APIKEY.txt`.

**Run**
1. `dotnet build`
1. `dotnet run`

**Screenshot**


![App screenshot](Assets\Screenshots\1.png)

**Notes**
- The app reads the API key from `APIKEY.txt` at startup. Keep the file out of source control.
- If the API or network is unavailable, the app shows a friendly error in the UI.
