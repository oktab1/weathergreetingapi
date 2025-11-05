# Simple DotNet App Weather Greeting API
Simple Weather Greeting Application for WEMA Training
Requires DotNet 9

# Build and Test Locally
- Clone the repo `git clone https://github.com/oktab1/weathergreetingapi.git`
- Navigate to the project folder: `cd ./weathergreetingapi`

## Restore dependencies
- `dotnet restore`

## Build the project
- `dotnet build -c Release`

## Run tests if you have any
- `dotnet test -c Release`

## Publish a self-contained deployment to a folder
- `dotnet publish -c Release -o ./publish`
- The folder `./publish` contains all files needed to run your app.

## To run on local development machine
- `dotnet WeatherGreetingApi.dll`
- Go to `http://<localhost-or-ip>:5000/weather?name=your-name&city=your-city`

# Deploy to Target Server

## Create folder on server
- `ssh xxx@remote-server-ip`
- `mkdir -p /var/www/weatherapp`

## Copy published files
`scp -r ./publish/* xxx@remote-server-ip:/var/www/weatherapp/`

# Run in the background with nohup
- `ssh xxx@remote-server-ip`
- `cd /var/www/weatherapp`

## Export your OpenWeatherMap API key
`export OPENWEATHER_API_KEY="your_api_key_here"`

## Run the app in the background
- `nohup dotnet WeatherGreetingApi.dll > app.log 2>&1 & disown`

## Terminate the running job
- `pkill -f WeatherGreetingApi.dll`