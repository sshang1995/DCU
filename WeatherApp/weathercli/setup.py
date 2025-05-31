from setuptools import setup, find_packages

setup(
    name="weathercli",
    version="0.1",
    packages=find_packages(),
    install_requires=[
        "click",
        "requests",
        "pyyaml"
    ],
    entry_points={
        "console_scripts": [
            "get-current-weather = weathercli.cli:get_current_weather", 
            "get-average-weather = weathercli.cli:get_average_weather"

        ]
    },
)