
# Running Web API 
1. build docker image
- docker build -t weatherapp .
2. run a container
- docker run -d -p 8080:80 --name weatherapp-container weatherapp
3. go to http://localhost:8080/swagger/index.html

# Using CLI 
1. go to weathercli/ directory 
- pip install -e. 
2. examples
- get-current-weather 02472 celsius
- get-current-weather 02472 celsius --output json
- get-current-weather 02472 celsius --output yaml

- get-average-weather 02472 fahrenheit 4 
- get-average-weather 02472 fahrenheit 4 --output json
- get-average-weather 02472 fahrenheit 4 --output yaml

# Unit tests 
1. unit test cases: WeatherAPITests.cs
2. run unit test, go to WeatherAPI.Tests folder
- dotnet test