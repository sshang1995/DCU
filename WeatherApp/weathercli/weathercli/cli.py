import click
import requests
import json
import yaml
import urllib3
import keyring

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
API_BASE = "http://localhost:8080/"
#API_BASE = "https://localhost:44310/"
SERVICE_NAME = "weathercli" # Used as keyring namespace
USERNAME = None
Login = "login"
Current_Weather = "Weather/Current/"
Average_Temp = "Weather/Average/"

@click.command()
@click.option("--username", required=True, help="Your username")
@click.option("--password", required=True, hide_input=True, prompt=True, help="Your password")
def login(username, password):
    """Log in and store JWT token securely."""
    try:
        
        response = requests.post(API_BASE+Login, json={"username": username, "password": password}, verify=False)
        if response.status_code == 200:
            token = response.json().get("token")
            if not token:
                click.echo("Token missing in response.")
                return

            # Store token securely
            keyring.set_password(SERVICE_NAME, username, token)
            USERNAME = username
            click.echo(f"Login successful. Token stored securely for user: {username}")
        else:
            click.echo(f"Login failed: {response.status_code} - {response.text}")
    except Exception as e:
        click.echo(f"Error: {e}")

def get_token(username): 
    token = keyring.get_password(SERVICE_NAME, username)
    if not token:
        raise Exception("No token found. Please log in first.")

    return token
    
@click.command()
@click.argument("zipcode")
@click.argument("units", type=click.Choice(["fahrenheit", "celsius"]))
@click.option("--output", type=click.Choice(["json", "yaml", "text"]), default="text", help="Output format")
def get_current_weather(zipcode, units, output):
    try: 
        token = get_token(USERNAME)
        headers = {
        "Authorization": f"Bearer {token}"
        }
        response = requests.get(f"{API_BASE}{Current_Weather}{zipcode}", params={"units": units}, headers=headers)
        if response.status_code == 400: 
            click.echo("Bad request: Please provide valid zip code and Unit must be Fahrenheit or Celsius")
            return
        elif response.status_code == 500: 
            click.echo("Server error")
            return 
        elif response.status_code == 404: 
            click.echo("Location cannot be found")
            return 

        data = response.json()

        if output == "json": 
            click.echo(json.dumps(data, indent=2))
        elif output == "yaml": 
            click.echo(yaml.dump(data))
        elif output == "text": 
            unit = ''
            res = ''
            if data["unit"] == "F":
                unit = "°F"
            else:
                unit = "°C"
            if data['rainPossibleToday']: 
                res = 'Yes'
            else:
                res = "No"

            click.echo(f"Location: {zipcode} \nCurrent Temperature: {data['currentTemperature']}{unit} \nRain Possible Today: {res}")


    except Exception as err: 
        click.echo(f"Exception: {err}")



@click.command()
@click.argument("zipcode")
@click.argument("units", type=click.Choice(["fahrenheit", "celsius"]))
@click.argument("timeperiod", type=int)
@click.option("--output", type=click.Choice(["json", "yaml", "text"]), default="text", help="Output format")
def get_average_weather(zipcode, units, timeperiod, output):
    try: 
        if timeperiod <2 or timeperiod > 5: 
            raise click.BadParameter("timeperiod must be number in 2-5.")
        
        token = get_token(USERNAME)
        headers = {
        "Authorization": f"Bearer {token}"
        }
        response = requests.get(f"{API_BASE}{Average_Temp}{zipcode}", params={"units": units, "timePeriod": timeperiod}, headers=headers)
        if response.status_code == 400: 
            click.echo("Bad request: Please provide valid zip code and Unit must be Fahrenheit or Celsius")
            return
        elif response.status_code == 500: 
            click.echo("Server error")
            return 
        elif response.status_code == 404: 
            click.echo("Location cannot be found")
            return 

        data = response.json()

        if output == "json": 
            click.echo(json.dumps(data, indent=2))
        elif output == "yaml": 
            click.echo(yaml.dump(data))
        elif output == "text": 
            unit = ''
            res = ''
            if data["unit"] == "F":
                unit = "°F"
            else:
                unit = "°C"
            if data['rainPossibleInPeriod']: 
                res = 'Yes'
            else:
                res = "No"

            click.echo(f"Location: {zipcode} \nAverage Temperature for next {timeperiod} days: {data['averageTemperature']}{unit} \nRain Possible In Period: {res}")


    except Exception as err: 
        click.echo(f"Exception: {err}")

# if __name__ == "__main__":
#     get_current_weather()