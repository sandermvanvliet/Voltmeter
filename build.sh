#!/usr/bin/env bash
# Define directories.
SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
CAKE_VERSION=0.31.0
CAKE_DLL=$TOOLS_DIR/Cake.CoreCLR.$CAKE_VERSION/Cake.dll

# Disable dotnet sdk telemetry
export DOTNET_CLI_TELEMETRY_OPTOUT=1

# Make sure the tools folder exist.
if [ ! -d "$TOOLS_DIR" ]; then
  mkdir "$TOOLS_DIR"
fi


###########################################################################
# INSTALL PREREQUISITES
###########################################################################

if [ `uname -s` = 'Linux' ]
then
    apt-get update

    if [ -z `which curl` ]
    then
        echo "curl is missing, installing..."
        apt-get install -y curl
    fi
    
    if [ -z `which unzip` ]
    then
        echo "unzip is missing, installing..."
        apt-get install -y unzip
    fi

    if [ -z `which git` ]
    then
        echo "git is missing, installing..."
        apt-get install -y git
    fi

    if [ -z `which dotnet` ]
    then
        curl -o /tmp/packages-microsoft-prod.deb https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb
        dpkg -i /tmp/packages-microsoft-prod.deb

        apt-get update
        apt-get install -y dotnet-sdk-2.1
    fi
fi

###########################################################################
# INSTALL CAKE
###########################################################################

if [ ! -f "$CAKE_DLL" ]; then
    curl -Lsfo Cake.CoreCLR.zip "https://www.nuget.org/api/v2/package/Cake.CoreCLR/$CAKE_VERSION" && unzip -q Cake.CoreCLR.zip -d "$TOOLS_DIR/Cake.CoreCLR.$CAKE_VERSION" && rm -f Cake.CoreCLR.zip
    if [ $? -ne 0 ]; then
        echo "An error occured while installing Cake."
        exit 1
    fi
fi

# Make sure that Cake has been installed.
if [ ! -f "$CAKE_DLL" ]; then
    echo "Could not find Cake.exe at '$CAKE_DLL'."
    exit 1
fi

###########################################################################
# RUN BUILD SCRIPT
###########################################################################

# Start Cake
exec dotnet "$CAKE_DLL" "$@"