import click
import requests
import json
import yaml
import urllib3

urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)
API_BASE = "http://localhost:8080/Weather/"
Current_Weather = "Current/"
Average_Temp = "Average/"

@click.command()
@click.argument("zipcode")
@click.argument("units", type=click.Choice(["fahrenheit", "celsius"]))
@click.option("--output", type=click.Choice(["json", "yaml", "text"]), default="text", help="Output format")
def get_current_weather(zipcode, units, output):
    try: 
        response = requests.get(f"{API_BASE}{Current_Weather}{zipcode}", params={"units": units}, verify=False)
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
        click.echo(f"X Exception: {err}")



@click.command()
@click.argument("zipcode")
@click.argument("units", type=click.Choice(["fahrenheit", "celsius"]))
@click.argument("timeperiod", type=int)
@click.option("--output", type=click.Choice(["json", "yaml", "text"]), default="text", help="Output format")
def get_average_weather(zipcode, units, timeperiod, output):
    try: 
        if timeperiod <2 or timeperiod > 5: 
            raise click.BadParameter("timeperiod must be number in 2-5.")
        
        response = requests.get(f"{API_BASE}{Average_Temp}{zipcode}", params={"units": units, "timePeriod": timeperiod}, verify=False)
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
        click.echo(f"X Exception: {err}")

# if __name__ == "__main__":
#     get_current_weather()