
# Running Web API 
1. build docker image
- docker build -t weatherapp .

2. run a container
- docker run -d -p 8080:80 --name weatherapp-container weatherapp

3. go to http://localhost:8080/swagger/index.html to see the detail 

# Using CLI 
1. Install python library 
- pip install requests click keyring pyyaml

2. go to weathercli/ directory 
- pip install -e. 

3. examples
- login --username admin --password password

- get-current-weather 02472 celsius
- get-current-weather 02472 celsius --output json
- get-current-weather 02472 celsius --output yaml

- get-average-weather 02472 fahrenheit 4 
- get-average-weather 02472 fahrenheit 4 --output json
- get-average-weather 02472 fahrenheit 4 --output yaml

# Authentication 
1. Adding JWT token to API call
- Authorization: Bearer <your-token>

2. Create token
-  http://localhost:8080/login
- Payload: { "Username":"admin", "Password":"password"}
 

# Unit tests 
1. unit test cases: WeatherAPITests.cs

2. run unit test, go to WeatherAPI.Tests directory
- dotnet test